#pragma once

#include <ADRC.h>
#include <Mathematics.h>
#include <MPU6050.h>
#include <Rotation.h>
#include <Thruster.h>
#include <TriangleWaveFader.h>
#include <Vector.h>
#include <VectorFeedbackController.h>

class Quadcopter {
private:
	MPU6050 mpu;
	TriangleWaveFader gimbalLockFader;
	Vector3D externalAcceleration;
	Vector3D currentVelocity;
	Vector3D currentAngularVelocity;
	Vector3D currentAngularAcceleration;
	Vector3D currentAcceleration;
	VectorFeedbackController positionController;
	VectorFeedbackController rotationController;
	double armLength;
	double armAngle;
	double dT;

	void CalculateArmPositions(double armLength, double armAngle);
	void CalculateGimbalLockedMotion(Vector3D &positionControl, Vector3D &thrusterOutputB,
							         Vector3D &thrusterOutputC, Vector3D &thrusterOutputD,
									 Vector3D &thrusterOutputE);
	void EstimatePosition();
	void EstimateRotation();

public:
	Rotation CurrentRotation;
	Rotation TargetRotation;
	Vector3D CurrentPosition;
	Vector3D TargetPosition;
	Thruster TB;
	Thruster TC;
	Thruster TD;
	Thruster TE;

	Quadcopter(double armLength, double armAngle, double dT);
	void CalculateCombinedThrustVector();
	void SetTarget(Vector3D position, Rotation rotation);
	void GetCurrent();
	void SimulateCurrent(Vector3D externalAcceleration);
	Vector3D RotationQuaternionToHoverAngles(Rotation rotation);
};