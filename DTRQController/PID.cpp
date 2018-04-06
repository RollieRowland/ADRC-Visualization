#include "PID.h"

PID::PID() {
	this->kp = 1;
	this->ki = 0;
	this->kd = 0;
}

PID::~PID() {

}

PID::PID(double kp, double ki, double kd) {
	this->kp = kp;
	this->ki = ki;
	this->kd = kd;
}

double PID::Calculate(double setpoint, double processVariable, double dT) {
	double POut, IOut, DOut;

	error = setpoint - processVariable;
	integral += error * dT;

	POut = kp * error;
	IOut = ki * integral;
	DOut = kd * ((error - previousError) / dT);

	output = POut + IOut + DOut;
	previousError = error;

	return output;
}
