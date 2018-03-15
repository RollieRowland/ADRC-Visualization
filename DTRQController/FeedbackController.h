#pragma once

class FeedbackController {
public:
	virtual double Calculate(double setpoint, double processVariable, double dT);
};
