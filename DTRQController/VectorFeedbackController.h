#pragma once

#include <FeedbackController.h>
#include <Vector.h>

class VectorFeedbackController {
public:
	FeedbackController X;
	FeedbackController Y;
	FeedbackController Z;
	Vector3D output;

	VectorFeedbackController();
	VectorFeedbackController(FeedbackController X, FeedbackController Y, FeedbackController Z);
	Vector3D Calculate(Vector3D setpoint, Vector3D processVariable, double dT);
};
