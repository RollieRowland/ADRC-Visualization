#pragma once

#include "Mathematics.h"
#include "FeedbackController.h"

class PID : virtual public FeedbackController {
private:
	double integral = 0;
	double error = 0;
	double previousError = 0;
	double output = 0;
	double kp;
	double ki;
	double kd;

public:
	PID();
	~PID();
	PID(double kp, double ki, double kd);
	double Calculate(double setpoint, double processVariable, double dT);
};
