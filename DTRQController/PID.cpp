#include <PID.h>


PID::PID(double kp, double ki, double kd) {
	this->kp = kp;
	this->ki = ki;
	this->kd = kd;
}

double PID::Calculate(double setpoint, double processVariable, double dT) {
	double POut, IOut, DOut;

	error = setpoint - processVariable;

	POut = kp * error;

	integral += error * dT;
	IOut = ki * integral;

	DOut = kd * ((error - previousError) / dT);

	previousError = error;

	return output;
}
