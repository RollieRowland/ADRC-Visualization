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
QuaternionKalmanFilter quatKF = QuaternionKalmanFilter(0.1, 20);
VectorKalmanFilter accelKF = VectorKalmanFilter(0.2, 40);
Vector3D velocity;
Vector3D position;

Vector3D targetPosition;
Rotation targetRotation;

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
	delay(50);

	std::cout << "Starting MPU calibration procedure." << std::endl;
	i2cController->SetDThrustVector(Vector3D(-90, 0, 0));
	i2cController->SetEThrustVector(Vector3D(90, 0, 0));
	sleep(5);

	i2cController->InitializeMPUs();
	bcm2835_delay(250);
	
	i2cController->SetDThrustVector(Vector3D(0, 0, 0));
	i2cController->SetEThrustVector(Vector3D(0, 0, 0));
	sleep(5);

	std::cout << "Hardware initialization complete." << std::endl;

	/*
	std::cout << "Intializing servos and ESCs" << std::endl;
	for (int i = -90; i < 90; i++) {
		i2cController.SetDThrustVector(Vector3D(i, 0, i));
		i2cController.SetEThrustVector(Vector3D(i, 0, i));

		bcm2835_delay(50);
	}

	i2cController.SetDThrustVector(Vector3D(0, 0, 0));
	i2cController.SetEThrustVector(Vector3D(0, 0, 0));

	sleep(1);
	*/


	std::cout << "Beginning control loop..." << std::endl;
	while (true) {
		double dT = ((double)((std::chrono::system_clock::now() - previousTime).count()) / pow(10.0, 9.0));

		//Quaternion q = i2cController->GetTCRotation();
		//Vector3D v = i2cController.GetTBWorldAcceleration();

		rotation = quatKF.Filter(i2cController->GetTCRotation());
		rotation = quatKF.Filter(i2cController->GetTDRotation());
		rotation = quatKF.Filter(i2cController->GetTERotation());
		//worldAccel = Vector3D(accelKF.Filter(v));

		velocity = velocity + worldAccel * dT;
		position = position + velocity * dT;

		//std::cout << rotation.ToString() << " " << worldAccel.ToString() << std::endl;

		position = Vector3D(0, 0, 0);

		quad.SetTarget(targetPosition, targetRotation);
		quad.SetCurrent(position, Rotation(rotation));

		quad.CalculateCombinedThrustVector();//Secondary Solver

		YawPitchRoll ypr = Rotation(Quaternion(rotation)).GetYawPitchRoll();
		//EulerAngles ea = Rotation(Quaternion(rotation)).GetEulerAngles(EulerConstants::EulerOrderXYZR);

		std::cout << ypr.ToString() << " " << quad.TB->CurrentRotation.ToString() << std::endl;
		
		//SET OUTPUTS


		i2cController->SetDThrustVector(Vector3D(-quad.TB->CurrentRotation.X, 0, -quad.TB->CurrentRotation.Z));


		previousTime = std::chrono::system_clock::now();
		
		bcm2835_delay(5);
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