#pragma once

#include <FeedbackController.h>
#include <Vector.h>

typedef struct VectorFeedbackController {
	FeedbackController X;
	FeedbackController Y;
	FeedbackController Z;
	Vector3D output;

	VectorFeedbackController(FeedbackController X, FeedbackController Y, FeedbackController Z);
	Vector3D Calculate(Vector3D setpoint, Vector3D processVariable, double dT);
} VectorFeedbackController;
