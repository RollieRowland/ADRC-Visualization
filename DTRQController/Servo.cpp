#include <Servo.h>

void Servo::SetPWM() {

}

void Servo::SetAngle(double value) {
	this->angle = value;
}

double Servo::GetAngle() {
	return angle;
}