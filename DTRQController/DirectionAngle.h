#pragma once
#include <Vector.h>
#include <Quaternion.h>

typedef struct DirectionAngle {
	double Rotation;
	Vector3D Direction;

	DirectionAngle(double rotation, double x, double y, double z);
	DirectionAngle(double rotation, Vector3D direction);

	static DirectionAngle QuaternionToDirectionAngle(Quaternion quaternion);

	string ToString();
} DirectionAngle;