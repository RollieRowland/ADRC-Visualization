#pragma once

#include <Math.h>
#include <FeedbackController.h>

typedef struct PID : FeedbackController {
private:
	double integral;
	double error;
	double previousError;
	double output;
	double kp;
	double ki;
	double kd;

public:
	PID(double kp, double ki, double kd);
	virtual double Calculate(double setpoint, double processVariable, double dT);
} PID;
