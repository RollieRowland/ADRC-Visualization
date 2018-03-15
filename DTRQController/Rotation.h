#pragma once

#include <Quaternion.h>
#include <RotationMatrix.h>
#include <DirectionAngle.h>
#include <AxisAngle.h>
#include <EulerAngles.h>
#include <Vector.h>
#include <HMatrix.h>
#include <EulerConstants.h>

class Rotation {
private:
	Quaternion QuaternionRotation;

	Quaternion AxisAngleToQuaternion(AxisAngle axisAngle);
	Quaternion DirectionAngleToQuaternion(DirectionAngle directionAngle);
	Quaternion RotationMatrixToQuaternion(RotationMatrix rotationMatrix);
	Quaternion EulerAnglesToQuaternion(EulerAngles eulerAngles);
	Quaternion HierarchicalMatrixToQuaternion(HMatrix hMatrix);
	EulerAngles HierarchicalMatrixToEulerAngles(HMatrix hM, EulerOrder order);
	HMatrix EulerAnglesToHierarchicalMatrix(EulerAngles eulerAngles);
public:
	Rotation();
	Rotation(Quaternion quaternion);
	Rotation(AxisAngle axisAngle);
	Rotation(DirectionAngle directionAngle);
	Rotation(RotationMatrix rotationMatrix);
	Rotation(EulerAngles eulerAngles);
	Rotation(HMatrix hMatrix);

	Quaternion GetQuaternion();
	AxisAngle GetAxisAngle();
	DirectionAngle GetDirectionAngle();
	RotationMatrix GetRotationMatrix();
	EulerAngles GetEulerAngles(EulerOrder order);
	HMatrix GetHierarchicalMatrix();
};
