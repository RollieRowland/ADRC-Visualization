#pragma once

typedef struct FeedbackController {
	virtual double Calculate(double setpoint, double processVariable, double dT);
} FeedbackController;
