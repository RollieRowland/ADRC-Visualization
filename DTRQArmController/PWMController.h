#pragma once

#include "bcm2835.h"
#include <stdio.h>

class PWMController {
private:
	uint8_t Mode1 = 0x00;//mode 1
	uint8_t Mode2 = 0x01;//mode 2
	uint8_t PreScale = 0xFE;
	double ClockFrequency = 25000000.0;

	uint8_t OuterB = 0;
	uint8_t OuterC = 1;
	uint8_t OuterD = 2;
	uint8_t OuterE = 3;

	uint8_t InnerB = 4;
	uint8_t InnerC = 5;
	uint8_t InnerD = 6;
	uint8_t InnerE = 7;

	uint8_t RotorB = 8;
	uint8_t RotorC = 9;
	uint8_t RotorD = 10;
	uint8_t RotorE = 11;

	uint8_t StartInt = 0x6;

	void SetPWMFrequency(int frequency);
	void Reset();

	void SetPWM(int device, int frequency);

	int CalculateServoFrequency(double angle);
	int CalculateRotorFrequency(double force);

	void WriteByte(uint8_t addr, uint8_t value);

public:
	PWMController(int frequency);//24 -> 1526Hz

	void SetOuterBAngle(double angle);
	void SetOuterCAngle(double angle);
	void SetOuterDAngle(double angle);
	void SetOuterEAngle(double angle);

	void SetInnerBAngle(double angle);
	void SetInnerCAngle(double angle);
	void SetInnerDAngle(double angle);
	void SetInnerEAngle(double angle);

	void SetRotorBOutput(double output);//0 -> 9.8, * 4 = 39.2
	void SetRotorCOutput(double output);
	void SetRotorDOutput(double output);
	void SetRotorEOutput(double output);

};