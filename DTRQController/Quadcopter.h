#pragma once

#include "ADRC.h"
#include "Mathematics.h"
#include "Rotation.h"
#include "Thruster.h"
#include "TriangleWaveFader.h"
#include "Vector.h"
#include "VectorFeedbackController.h"

class Quadcopter {
private:
	TriangleWaveFader gimbalLockFader;
	Vector3D externalAcceleration;
	Vector3D currentVelocity;
	Vector3D currentAngularVelocity;
	Vector3D currentAngularAcceleration;
	Vector3D currentAcceleration;
	double armLength;
	double armAngle;
	double dT;
	bool simulation;

	void CalculateArmPositions(double armLength, double armAngle);
	void CalculateGimbalLockedMotion(Vector3D &positionControl, Vector3D &thrusterOutputB,
							         Vector3D &thrusterOutputC, Vector3D &thrusterOutputD,
									 Vector3D &thrusterOutputE);
	Quaternion CalculateRotationOffset();
	void EstimatePosition();
	void EstimateRotation();

	VectorFeedbackController *positionController;
	VectorFeedbackController *rotationController;
	
	Vector3D RotationToHoverAngles(Rotation rotation);
public:
	Rotation CurrentRotation;
	Rotation TargetRotation;
	Vector3D CurrentPosition;
	Vector3D TargetPosition;
	Thruster *TB;
	Thruster *TC;
	Thruster *TD;
	Thruster *TE;

	Quadcopter(bool simulation, double armLength, double armAngle, double dT, VectorFeedbackController *pos, VectorFeedbackController *rot);
	~Quadcopter();
	void CalculateCombinedThrustVector();
	void SetTarget(Vector3D position, Rotation rotation);
	void SetCurrent(Vector3D position, Rotation rotation);
	void SimulateCurrent(Vector3D externalAcceleration);
};
