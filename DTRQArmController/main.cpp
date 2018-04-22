#include "I2CController.h"
#include "../DTRQController/Rotation.h"
#include "../DTRQController/Quadcopter.h"
#include "../DTRQController/VectorFIRFilter.h"
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
QuaternionKalmanFilter quatKF = QuaternionKalmanFilter(0.75, 10);
VectorKalmanFilter accelKF = VectorKalmanFilter(0.5, 50);
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
	bcm2835_delay(1000);

	std::cout << "Setting thruster rotations to calibration mode." << std::endl;
	i2cController->SetBThrustVector(Vector3D(-90, 0, 0));
	i2cController->SetCThrustVector(Vector3D(90, 0, 0));
	i2cController->SetDThrustVector(Vector3D(-90, 0, 0));
	i2cController->SetEThrustVector(Vector3D(90, 0, 0));
	bcm2835_delay(1000);

	i2cController->InitializeMPUs();
	bcm2835_delay(1000);
	
	std::cout << "Temperature: " << i2cController->GetAvgTemperature() << std::endl;

	i2cController->CalibrateMPUs();

	i2cController->SetBThrustVector(Vector3D(0, 0, 0));
	i2cController->SetCThrustVector(Vector3D(0, 0, 0));
	i2cController->SetDThrustVector(Vector3D(0, 0, 0));
	i2cController->SetEThrustVector(Vector3D(0, 0, 0));
	bcm2835_delay(1000);

	std::cout << "Hardware initialization complete." << std::endl;
	
	//////////////////////////

	std::cout << "Waiting for DMPs to stabilize for setting default offsets." << std::endl;

	QuaternionKalmanFilter gmkf = QuaternionKalmanFilter(0.075, 50);
	QuaternionKalmanFilter gfkf = QuaternionKalmanFilter(0.075, 50);
	QuaternionKalmanFilter gbkf = QuaternionKalmanFilter(0.075, 50);

	VectorKalmanFilter amkf = VectorKalmanFilter(0.075, 50);
	VectorKalmanFilter afkf = VectorKalmanFilter(0.075, 50);
	VectorKalmanFilter abkf = VectorKalmanFilter(0.075, 50);

	Quaternion maingOffset;
	Quaternion forwgOffset;
	Quaternion backgOffset;

	Vector3D mainaOffset;
	Vector3D forwaOffset;
	Vector3D backaOffset;

	double calTime = 0;

	previousTime = std::chrono::system_clock::now();

	VectorFIRFilter acceMHP = VectorFIRFilter(FiniteImpulseResponse::High, 100, 1000, 15, 0);
	VectorFIRFilter acceFHP = VectorFIRFilter(FiniteImpulseResponse::High, 100, 1000, 15, 0);
	VectorFIRFilter acceBHP = VectorFIRFilter(FiniteImpulseResponse::High, 100, 1000, 15, 0);

	while (calTime < 3) {
		calTime = ((double)((std::chrono::system_clock::now() - previousTime).count()) / pow(10.0, 9.0));
		Quaternion gm = i2cController->GetMainRotation();
		Quaternion gf = i2cController->GetMainFRotation();
		Quaternion gb = i2cController->GetMainBRotation();

		Vector3D  am = i2cController->GetMainWorldAcceleration();
		Vector3D  af = i2cController->GetMainFWorldAcceleration();
		Vector3D  ab = i2cController->GetMainBWorldAcceleration();

		gm = Quaternion(1, 0, 0, 0).Multiply(gm.Conjugate());
		gf = Quaternion(1, 0, 0, 0).Multiply(gf.Conjugate());
		gb = Quaternion(1, 0, 0, 0).Multiply(gb.Conjugate());

		maingOffset = gmkf.Filter(gm);
		forwgOffset = gfkf.Filter(gf);
		backgOffset = gbkf.Filter(gb);

		mainaOffset = amkf.Filter(am.Multiply(-1));
		forwaOffset = afkf.Filter(af.Multiply(-1));
		backaOffset = abkf.Filter(ab.Multiply(-1));
	}

	std::cout << "Main offset: " << maingOffset.ToString() << std::endl;

	std::cout << "Offsets Captured." << std::endl;
	////////////////////////////////////

	previousTime = std::chrono::system_clock::now();
	std::cout << "Beginning control loop..." << std::endl;
	while (true) {
		double dT = ((double)((std::chrono::system_clock::now() - previousTime).count()) / pow(10.0, 9.0));
		previousTime = std::chrono::system_clock::now();

		Vector3D am, af, ab;
		af = i2cController->GetMainFWorldAcceleration().Add(forwaOffset);
		ab = i2cController->GetMainBWorldAcceleration().Add(backaOffset);

		Quaternion qm, qf, qb;
		//qm = i2cController->GetMainRotation();
		qf = i2cController->GetMainFRotation().UnitQuaternion();// .Multiply(forwgOffset);//correct initial offset
		qb = i2cController->GetMainBRotation().UnitQuaternion();// .Multiply(backgOffset);

		//quatKF.Filter(qm);
		quatKF.Filter(qb);
		rotation = quatKF.Filter(qf).UnitQuaternion();

		// -1000 / af

		//worldAccel = af;//(af.Add(ab)).Divide(2);
		//acceFHP.Filter(accelKF.Filter(af));
		worldAccel = acceBHP.Filter(ab).Add(acceFHP.Filter(af)).Divide(2.0);//accelkf

		//std::cout << rotation.ToString() << std::endl;

		velocity = velocity.Add(worldAccel.Multiply(9.81).Multiply(dT));//g-force to m/s^2
		position = position.Add(velocity.Multiply(dT));

		std::cout << worldAccel.ToString() << " " << position.ToString() << std::endl;

		quad.SetTarget(position, rotation);
		quad.SetCurrent(position, rotation);

		quad.CalculateCombinedThrustVector();//Secondary Solver

		EulerAngles eaypr = Rotation(Quaternion(rotation)).GetEulerAngles(EulerConstants::EulerOrderYXZS);
		
		//std::cout << eaypr.Angles.ToString() << std::endl;
		//std::cout  << position.ToString() << " " << velocity.ToString() << " " << worldAccel.ToString() << std::endl;
		
		//set outputs
		i2cController->SetBThrustVector(Vector3D(-quad.TB->CurrentRotation.X, 0, -quad.TB->CurrentRotation.Z));
		i2cController->SetCThrustVector(Vector3D(-quad.TC->CurrentRotation.X, 0,  quad.TC->CurrentRotation.Z));
		i2cController->SetDThrustVector(Vector3D( quad.TD->CurrentRotation.X, 0,  quad.TD->CurrentRotation.Z));
		i2cController->SetEThrustVector(Vector3D( quad.TE->CurrentRotation.X, 0, -quad.TE->CurrentRotation.Z));
		
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