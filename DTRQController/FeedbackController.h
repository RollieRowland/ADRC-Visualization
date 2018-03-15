#pragma once

class FeedbackController {
public:
	FeedbackController() { }
	FeedbackController(const FeedbackController& feedbackController) { *this = feedbackController; }
	virtual double Calculate(double setpoint, double processVariable, double dT) = 0;
};
