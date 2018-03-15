#pragma once

#include <Mathematics.h>
#include <FeedbackController.h>

class PID : virtual public FeedbackController {
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
	double Calculate(double setpoint, double processVariable, double dT);
};
