#pragma once

#include <Quaternion.h>
#include <Vector.h>

class MPU6050 {
private:
	Vector3D   CurrentPosition;
	Vector3D   CurrentVelocity;
	Vector3D   CurrentAcceleration;
	Quaternion CurrentAngularPosition;
	Vector3D   CurrentAngularVelocity;

public:
	MPU6050();
	MPU6050(int pin);
	void Calculate();

	Vector3D GetCurrentPosition();
	Vector3D GetCurrentVelocity();
	Vector3D GetCurrentAcceleration();
	Quaternion GetCurrentAngularPosition();
	Vector3D GetCurrentAngularVelocity();
};
