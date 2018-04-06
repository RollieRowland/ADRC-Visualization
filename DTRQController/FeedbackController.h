#pragma once

class FeedbackController {
public:
	FeedbackController() { }
	virtual ~FeedbackController() = 0;
	FeedbackController(const FeedbackController& feedbackController) { *this = feedbackController; }
	virtual double Calculate(double setpoint, double processVariable, double dT) = 0;
};
