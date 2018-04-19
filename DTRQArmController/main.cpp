#include "I2CController.h"
#include "../DTRQController/Rotation.h"
#include "../DTRQController/Quadcopter.h"
#include "../DTRQController/VectorKalmanFilter.h"
#include "../DTRQController/QuaternionKalmanFilter.h"
#include <chrono>
#include <signal.h>
#include <bcm2835.h>
#include <unistd.h>

VectorFeedbackController pos = VectorFeedbackController{
	new PID{ 10, 0, 12.5 },
	new PID{ 1, 0, 0.2 },
	new PID{ 10, 0, 12.5 }
};

VectorFeedbackController rot = VectorFeedbackController{
	new PID{ 0.05, 0, 0.325 },
	new PID{ 0.05, 0, 0.325 },
	new PID{ 0.05, 0, 0.325 }
};

I2CController *i2cController;
Quadcopter quad = Quadcopter(false, 0.3, 55, 0.05, &pos, &rot);
QuaternionKalmanFilter quatKF = QuaternionKalmanFilter(0.1, 50);
VectorKalmanFilter accelKF = VectorKalmanFilter(0.1, 50);
Vector3D velocity = Vector3D(0, 0, 0);
Vector3D position = Vector3D(0, 0, 0);

Vector3D targetPosition = Vector3D(0, 0, 0);
Rotation targetRotation = Rotation(Quaternion(1, 0, 0, 0));

//catches the interupt
void sighandler(int signal) {
	std::cout << "Caught signal interupt: " << signal << std::endl;

	i2cController->~I2CController();
	quad.~Quadcopter();

	std::cout << "Shutting down quadcopter..." << std::endl;

	exit(1);
}

int main() {
	signal(SIGINT, &sighandler);

	std::cout << "Starting quadcopter..." << std::endl;
	auto previousTime = std::chrono::system_clock::now();

	Quaternion rotation;
	Vector3D worldAccel;

	i2cController = new I2CController(0x70);

	i2cController->InitializePCA();
	bcm2835_delay(50);

	std::cout << "Starting MPU calibration procedure..." << std::endl;
	std::cout << "Setting thruster rotations to default." << std::endl;
	i2cController->SetBThrustVector(Vector3D(0, 0, 0));
	i2cController->SetCThrustVector(Vector3D(0, 0, 0));
	i2cController->SetDThrustVector(Vector3D(0, 0, 0));
	i2cController->SetEThrustVector(Vector3D(0, 0, 0));
	bcm2835_delay(3000);

	std::cout << "Setting thruster rotations to calibration mode." << std::endl;
	i2cController->SetBThrustVector(Vector3D(-90, 0, 0));
	i2cController->SetCThrustVector(Vector3D(90, 0, 0));
	i2cController->SetDThrustVector(Vector3D(-90, 0, 0));
	i2cController->SetEThrustVector(Vector3D(90, 0, 0));
	bcm2835_delay(4000);

	i2cController->InitializeMPUs();
	bcm2835_delay(1000);

	i2cController->CalibrateMPUs();

	i2cController->SetBThrustVector(Vector3D(0, 0, 0));
	i2cController->SetCThrustVector(Vector3D(0, 0, 0));
	i2cController->SetDThrustVector(Vector3D(0, 0, 0));
	i2cController->SetEThrustVector(Vector3D(0, 0, 0));
	bcm2835_delay(1000);

	std::cout << "Waiting for MPU DMP initialization..." << std::endl;

	//i2cController->CalibrateMPUDMPs();
	//bcm2835_delay(250);

	std::cout << "Hardware initialization complete." << std::endl;

	std::cout << "Beginning control loop..." << std::endl;
	while (true) {
		double dT = ((double)((std::chrono::system_clock::now() - previousTime).count()) / pow(10.0, 9.0));

		Quaternion q = i2cController->GetMainRotation();
		Vector3D  vm = i2cController->GetMainWorldAcceleration();
		Vector3D  vb = i2cController->GetTBWorldAcceleration();
		Vector3D  vc = i2cController->GetTCWorldAcceleration();
		Vector3D  vd = i2cController->GetTDWorldAcceleration();
		Vector3D  ve = i2cController->GetTEWorldAcceleration();

		rotation   = quatKF.Filter(q);
		worldAccel = accelKF.Filter((vm + vb + vc + vd + ve) / 5);

		velocity = velocity.Add(worldAccel.Multiply(dT).Multiply(9.81));//g-force to m/s^2
		position = position.Add(velocity.Multiply(dT));

		//std::cout << rotation.ToString() << " " << worldAccel.ToString() << std::endl;

		//position = Vector3D(0, 0, 0);

		quad.SetTarget(targetPosition, targetRotation);
		quad.SetCurrent(position, Rotation(rotation));

		quad.CalculateCombinedThrustVector();//Secondary Solver

		EulerAngles eaypr = Rotation(Quaternion(rotation)).GetEulerAngles(EulerConstants::EulerOrderYXZS);
		
		//std::cout << eaypr.Angles.ToString() << " ";
		std::cout  << position.ToString() << " " << worldAccel.ToString() << std::endl;
		
		//set outputs
		i2cController->SetBThrustVector(Vector3D( quad.TB->CurrentRotation.X, 0, -quad.TB->CurrentRotation.Z));
		i2cController->SetCThrustVector(Vector3D( quad.TC->CurrentRotation.X, 0, -quad.TC->CurrentRotation.Z));
		i2cController->SetDThrustVector(Vector3D(-quad.TD->CurrentRotation.X, 0, -quad.TD->CurrentRotation.Z));
		i2cController->SetEThrustVector(Vector3D(-quad.TE->CurrentRotation.X, 0, -quad.TE->CurrentRotation.Z));

		previousTime = std::chrono::system_clock::now();
		
		bcm2835_delay(1);
	}
	
	//ROTATION
	//calculate quat from previous servo positions rotation matrix, convert 
	//   to quaternion and subtract generated quaternion from arm MPUs
	//complementary filter of all individually KFed arm sensors
	//complementary filter arm with KFed center MPU 50/50
	
	//POSITION
	//calculate current velocity from complementary of all individually KFed accelerometers
	//calculate current position from velocity
	
	//calculate in quadcopter "library"
	//write outputs
	//END LOOP
	//////////

	std::cout << "Removing objects from memory." << std::endl;

	i2cController->~I2CController();
	quad.~Quadcopter();

	std::cout << "End of control." << std::endl;

	return 0;
}