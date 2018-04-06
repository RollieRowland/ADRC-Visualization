#include "I2CController.h"
#include "Quadcopter.h"
#include "VectorKalmanFilter.h"
#include "QuaternionKalmanFilter.h"
#include <chrono>

I2CController i2cController;
Quadcopter quad;
QuaternionKalmanFilter quatKF = QuaternionKalmanFilter(0.1, 20);
VectorKalmanFilter accelKF = VectorKalmanFilter(0.2, 40);
Vector3D velocity;
Vector3D position;

Vector3D targetPosition;
Rotation targetRotation;

VectorFeedbackController *pos = new VectorFeedbackController{
	new PID{ 10, 0, 12.5 },
	new PID{ 1, 0, 0.2 },
	new PID{ 10, 0, 12.5 }
};

VectorFeedbackController *rot = new VectorFeedbackController{
	new PID{ 0.05, 0, 0.325 },
	new PID{ 0.05, 0, 0.325 },
	new PID{ 0.05, 0, 0.325 }
};

int main(void) {
	i2cController = I2CController(0x70);
	quad = Quadcopter(false, 0.3, 55, 0.05, pos, rot);

	auto previousTime = std::chrono::system_clock::now();

	Quaternion rotation;
	Vector3D worldAccel;

	while (true) {
		Quaternion q = i2cController.GetMainRotation();
		Vector3D v = i2cController.GetMainWorldAcceleration();

		rotation = quatKF.Filter(q);
		worldAccel = Vector3D(accelKF.Filter(v));

		double dT = ((std::chrono::system_clock::now() - previousTime).count() / pow(1, -9));

		velocity = velocity + worldAccel * dT;
		position = position + velocity * dT;

		quad.SetTarget(targetPosition, targetRotation);
		quad.SetCurrent(position, Rotation(rotation));

		quad.CalculateCombinedThrustVector();//Secondary Solver

		previousTime = std::chrono::system_clock::now();
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

	return 0;
}