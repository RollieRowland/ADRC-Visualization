#pragma once
#include <Vector.h>
#include <EulerOrder.h>
#include <Quaternion.h>
#include <HMatrix.h>

typedef struct EulerAngles {
	Vector3D Angles;
	EulerOrder Order;

	EulerAngles();
	EulerAngles(Vector3D angles, EulerOrder order);

	static EulerAngles HMatrixToEuler(HMatrix hM, EulerOrder order);
	static EulerAngles QuaternionToEuler(Quaternion q, EulerOrder order);

} EulerAngles;