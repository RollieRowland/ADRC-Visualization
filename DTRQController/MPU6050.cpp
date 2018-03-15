#include <MPU6050.h>

MPU6050::MPU6050() {

}

MPU6050::MPU6050(int pin) {

}

void MPU6050::Calculate() {

}

Vector3D MPU6050::GetCurrentPosition() {
	return CurrentPosition;
}

Vector3D MPU6050::GetCurrentVelocity() {
	return CurrentVelocity;
}

Vector3D MPU6050::GetCurrentAcceleration() {
	return CurrentAcceleration;
}

Quaternion MPU6050::GetCurrentAngularPosition() {
	return CurrentAngularPosition;
}

Vector3D MPU6050::GetCurrentAngularVelocity() {
	return CurrentAngularVelocity;
}
