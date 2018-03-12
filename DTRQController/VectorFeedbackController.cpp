#include <VectorFeedbackController.h>

VectorFeedbackController::VectorFeedbackController(FeedbackController X, FeedbackController Y, FeedbackController Z) {
	this->X = X;
	this->Y = Y;
	this->Z = Z;
}

Vector3D VectorFeedbackController::Calculate(Vector3D setpoint, Vector3D processVariable, double dT) {

	output.X = X.Calculate(setpoint.X, processVariable.X, dT);
	output.Y = Y.Calculate(setpoint.Y, processVariable.Y, dT);
	output.Z = Z.Calculate(setpoint.Z, processVariable.Z, dT);

	return output;
}
