#include "PWMController.h"

PWMController::PWMController(int frequency) {
	Reset();
	SetPWMFrequency(frequency);
}

//No custom on time
void PWMController::SetPWMFrequency(int frequency) {
	uint8_t prescaleValue = (ClockFrequency / 4096 / frequency) - 1;

	WriteByte(Mode1, 0x10);//sleep
	WriteByte(PreScale, prescaleValue); //multiplier for PWM frequency
	WriteByte(Mode1, 0x80);//restart
	WriteByte(Mode2, 0x04);//totem pole
}

void PWMController::Reset() {
	WriteByte(Mode1, 0x00);
	WriteByte(Mode2, 0x04);
}

void PWMController::SetPWM(int device, int frequency) {
	WriteByte(device * 4 + 0x6, 0 & 0xFF);//ON first 8
	WriteByte(device * 4 + 0x7, 0 >> 8);//ON second 8

	WriteByte(device * 4 + 0x8, frequency & 0xFF);//OFF first 8
	WriteByte(device * 4 + 0x9, frequency >> 8);//Off second 8
}

void PWMController::WriteByte(uint8_t addr, uint8_t value) {
	char *val;

	sprintf(val, "%d", value);

	bcm2835_i2c_setSlaveAddress(addr);
	bcm2835_i2c_write(val , 1);
}


int PWMController::CalculateServoFrequency(double angle) {
	angle = angle + 90;

	double ratio = angle / 180;

	if (ratio < 0) {
		ratio = 0;
	}
	else if (ratio > 1) {
		ratio = 1;
	}

	int output = ratio * 4096.0;

	return output;
}

int PWMController::CalculateRotorFrequency(double force) {
	force = force + 39.2;//4g center -4g to +4g to 0 to 8g

	double ratio = force / 78.4;

	if (ratio < 0) {
		ratio = 0;
	}
	else if (ratio > 1) {
		ratio = 1;
	}

	int output = ratio * 4096.0;

	return output;
}


//OUTER JOINT ANGLE
void PWMController::SetOuterBAngle(double angle) {
	SetPWM(OuterB, CalculateServoFrequency(angle));
}

void PWMController::SetOuterCAngle(double angle) {
	SetPWM(OuterC, CalculateServoFrequency(angle));
}

void PWMController::SetOuterDAngle(double angle) {
	SetPWM(OuterD, CalculateServoFrequency(angle));
}

void PWMController::SetOuterEAngle(double angle) {
	SetPWM(OuterE, CalculateServoFrequency(angle));
}


//INNER JOINT ANGLE
void PWMController::SetInnerBAngle(double angle) {
	SetPWM(InnerB, CalculateServoFrequency(angle));
}

void PWMController::SetInnerCAngle(double angle) {
	SetPWM(InnerC, CalculateServoFrequency(angle));
}

void PWMController::SetInnerDAngle(double angle) {
	SetPWM(InnerD, CalculateServoFrequency(angle));
}

void PWMController::SetInnerEAngle(double angle) {
	SetPWM(InnerE, CalculateServoFrequency(angle));
}


//ROTOR OUTPUTS
void PWMController::SetRotorBOutput(double output) {
	SetPWM(RotorB, CalculateRotorFrequency(output));
}

void PWMController::SetRotorCOutput(double output) {
	SetPWM(RotorC, CalculateRotorFrequency(output));
}

void PWMController::SetRotorDOutput(double output) {
	SetPWM(RotorD, CalculateRotorFrequency(output));
}

void PWMController::SetRotorEOutput(double output) {
	SetPWM(RotorE, CalculateRotorFrequency(output));
}
