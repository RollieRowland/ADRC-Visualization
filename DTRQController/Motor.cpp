#include "Motor.h"

Motor::Motor() {
	output = 0;
}

void Motor::SetOutput(double value) {
	this->output = value;
}

double Motor::GetOutput() {
	return output;
}