#pragma once

#include <Vector.h>

class MPU6050 {
private:
	Vector3D CurrentPosition;
	Vector3D CurrentVelocity;
	Vector3D CurrentAcceleration;
	Quaternion CurrentAngularPosition;
	Vector3D CurrentAngularVelocity;
	Vector3D CurrentAngularAcceleration;

public:
	MPU6050();
	MPU6050(int pin);
	void Calculate();
	Vector3D GetCurrentVelocity();
	Vector3D GetCurrentAngularVelocity();
	Vector3D GetCurrentAcceleration();

};