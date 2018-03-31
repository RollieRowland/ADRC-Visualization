#include "VectorFeedbackController.h"

VectorFeedbackController::~VectorFeedbackController() {
	delete X;
	delete Y;
	delete Z;
}

VectorFeedbackController::VectorFeedbackController(const VectorFeedbackController& vectorFeedbackController) : X(vectorFeedbackController.X), 
																											   Y(vectorFeedbackController.Y), 
																											   Z(vectorFeedbackController.Z) {}

VectorFeedbackController::VectorFeedbackController(FeedbackController *X, FeedbackController *Y, FeedbackController *Z) : X(X), Y(Y), Z(Z) {
	this->X = X;
	this->Y = Y;
	this->Z = Z;
}

Vector3D VectorFeedbackController::Calculate(Vector3D setpoint, Vector3D processVariable, double dT) {
	output.X = X->Calculate(setpoint.X, processVariable.X, dT);
	output.Y = Y->Calculate(setpoint.Y, processVariable.Y, dT);
	output.Z = Z->Calculate(setpoint.Z, processVariable.Z, dT);

	return output;
}

VectorFeedbackController& VectorFeedbackController::operator =(const VectorFeedbackController& vectorFeedbackController) {
	this->X = vectorFeedbackController.X;
	this->Y = vectorFeedbackController.Y;
	this->Z = vectorFeedbackController.Z;

	return *this;
}
