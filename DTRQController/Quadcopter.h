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
	double armLength;
	double armAngle;
	double dT;
	bool simulation;

	void CalculateArmPositions(double armLength, double armAngle);
	void CalculateGimbalLockedMotion(Vector3D &positionControl, Vector3D &thrusterOutputB,
							         Vector3D &thrusterOutputC, Vector3D &thrusterOutputD,
									 Vector3D &thrusterOutputE);
	void EstimatePosition();
	void EstimateRotation();
	
	/*VectorFeedbackController positionController = VectorFeedbackController {
		new ADRC { 50.0, 200.0, 4.0, 10.0,
			PID { 10, 0, 12.5 }
		},
		new ADRC { 10.0, 10.0, 1.5, 0.05,
			PID { 1, 0, 0.2 }
		},
		new ADRC { 50.0, 200.0, 4.0, 10.0,
			PID { 10, 0, 12.5 }
		}
	};
	*/
	VectorFeedbackController rotationController = VectorFeedbackController {
		new ADRC { 20.0, 200.0, 4.0, 10.0,
			PID { 5, 0, 7.5 }
		},
		new ADRC { 10.0, 10.0, 1.5, 64,
			PID { 10, 0, 25 }
		},
		new ADRC { 20.0, 200.0, 4.0, 10.0,
			PID { 5, 0, 7.5 }
		}
	};
	
	VectorFeedbackController positionController = VectorFeedbackController {
		new PID { 10, 0, 12.5 },
		new PID { 1, 0, 0.2 },
		new PID { 10, 0, 12.5 }
	};
	/*
	VectorFeedbackController rotationController = VectorFeedbackController {
		new PID { 0.5, 0, 0.75 },
		new PID { 1, 0, 2.5 },
		new PID { 0.5, 0, 0.75 }
	};
	*/
	Vector3D RotationQuaternionToHoverAngles(Rotation rotation);
public:
	Rotation CurrentRotation;
	Rotation TargetRotation;
	Vector3D CurrentPosition;
	Vector3D TargetPosition;
	Thruster *TB;
	Thruster *TC;
	Thruster *TD;
	Thruster *TE;

	Quadcopter(bool simulation, double armLength, double armAngle, double dT);
	void CalculateCombinedThrustVector();
	void SetTarget(Vector3D position, Rotation rotation);
	void GetCurrent();
	void SimulateCurrent(Vector3D externalAcceleration);
};