#include <Motor.h>


void Motor::SetPWM() {
	
}

bool Motor::CheckESC() {
	return true;
}

void Motor::SetOutput(double value) {
	this->output = value;
}

double Motor::GetOutput() {
	return output;
}