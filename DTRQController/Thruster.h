#pragma once

#include <Vector.h>
#include <Servo.h>
#include <Motor.h>
#include <RotationMatrix.h>
#include <string>

class Thruster {
private:
	Servo outerJoint;
	Servo innerJoint;
	Motor rotor;
	std::string name;
	bool disable;

	bool CheckIfDisabled();
public:
	Vector3D TargetPosition;
	Vector3D CurrentPosition;
	Vector3D CurrentRotation;
	Vector3D ThrusterOffset;

	Thruster();
	Thruster(Vector3D ThrusterOffset, std::string name);
	void SetThrusterOutputs(Vector3D output);
	Vector3D ReturnThrustVector();
	Vector3D ReturnThrusterOutput();
	bool IsDisabled();
};