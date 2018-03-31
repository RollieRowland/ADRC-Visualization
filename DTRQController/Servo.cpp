#include "Servo.h"

Servo::Servo() {
	angle = 0;
}

void Servo::SetAngle(double value) {
	this->angle = value;
}

double Servo::GetAngle() {
	return angle;
}