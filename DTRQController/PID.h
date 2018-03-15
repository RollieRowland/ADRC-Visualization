#pragma once

#include <Math.h>
#include <FeedbackController.h>

class PID : public FeedbackController {
private:
	double integral;
	double error;
	double previousError;
	double output;
	double kp;
	double ki;
	double kd;

public:
	PID();
	PID(double kp, double ki, double kd);
	virtual double Calculate(double setpoint, double processVariable, double dT);
};
