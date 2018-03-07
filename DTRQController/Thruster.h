#pragma once

#include <string>
#include <Vector.h>
#include <Servo.h>

using namespace std;

class Thruster
{
	public:
		Vector TargetPosition{ get; set; }
		Vector CurrentPosition{ get; set; }
		Vector TargetRotation{ get; set; }
		Vector CurrentRotation{ get; set; }
		Vector QuadCenterOffset{ get; }
		bool Disable{ get; set; }

		Thruster(Vector positionOffset, string name, double samplingPeriod);
		void SetCartesianForces(Vector forces);
		Vector GetThrustVector();
		Vector GetCartesionForces();

	private:
		Servo innerServo;//X
		Motor propellorMotor;//Y
		Servo outerServo;//Z 
		double samplingPeriod;
		string name;

};