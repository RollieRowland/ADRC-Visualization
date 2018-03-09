#pragma once
#include <Vector.h>
#include <Quaternion.h>
#include <Math.h>

typedef struct AxisAngle {
	double Rotation;
	Vector3D Axis;

	AxisAngle(double rotation, double x, double y, double z);
	AxisAngle(double rotation, Vector3D axis);

	static AxisAngle QuaternionToAxisAngle(Quaternion quaternion);

	string ToString();
} AxisAngle;
