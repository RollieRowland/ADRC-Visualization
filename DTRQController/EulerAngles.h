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

} EulerAngles;