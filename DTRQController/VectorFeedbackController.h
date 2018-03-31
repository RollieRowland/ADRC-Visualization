#pragma once

#include "FeedbackController.h"
#include "Vector.h"

class VectorFeedbackController {
public:
	FeedbackController *X;
	FeedbackController *Y;
	FeedbackController *Z;
	Vector3D output;

	~VectorFeedbackController();
	VectorFeedbackController(const VectorFeedbackController& vectorFeedbackController);
	VectorFeedbackController(FeedbackController *X, FeedbackController *Y, FeedbackController *Z);
	Vector3D Calculate(Vector3D setpoint, Vector3D processVariable, double dT);

	VectorFeedbackController& operator =(const VectorFeedbackController& vectorFeedbackController);
};
