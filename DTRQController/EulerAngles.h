#pragma once

#include <Vector.h>
#include <EulerOrder.h>
#include <Quaternion.h>
#include <HMatrix.h>
#include <EulerConstants.h>
#include <Math.h>

typedef struct EulerAngles {
public:
	Vector3D Angles;
	EulerOrder Order;

	EulerAngles();
	EulerAngles(Vector3D angles, EulerOrder order);

	std::string ToString();
} EulerAngles;