#pragma once

#include "I2Cdev.h"
#include <stdio.h>
#include <iostream>
#include <string>

class PWMController {
private:
	//Registers/etc:
	const int PCA9685_ADDRESS = 0x40;
	const int MODE1 = 0x00;
	const int MODE2 = 0x01;
	const int SUBADR1 = 0x02;
	const int SUBADR2 = 0x03;
	const int SUBADR3 = 0x04;
	const int PRESCALE = 0xFE;
	const int PWM0_ON_L = 0x06;
	const int PWM0_ON_H = 0x07;;
	const int PWM0_OFF_L = 0x08;
	const int PWM0_OFF_H = 0x09;
	const int ALL_PWM_ON_L = 0xFA;
	const int ALL_PWM_ON_H = 0xFB;
	const int ALL_PWM_OFF_L = 0xFC;
	const int ALL_PWM_OFF_H = 0xFD;

	//Bits:
	const int RESTART = 0x80;
	const int SLEEP = 0x10;
	const int ALLCALL = 0x01;
	const int INVRT = 0x10;
	const int OUTDRV = 0x04;

	const double MINPWMMS = 750;//1ms - 1500
	const double MAXPWMMS = 2500;//2ms - 1900

	double ClockFrequency = 25000000.0;

	uint8_t RotorB = 0;
	uint8_t RotorC = 1;
	uint8_t RotorD = 2;
	uint8_t RotorE = 3;

	uint8_t OuterB = 4;
	uint8_t OuterC = 5;
	uint8_t OuterD = 12;
	uint8_t OuterE = 13;

	uint8_t InnerB = 8;
	uint8_t InnerC = 9;
	uint8_t InnerD = 10;
	uint8_t InnerE = 11;


	uint8_t StartInt = 0x6;

	uint8_t address;
	int frequency;

	void SetPWMFrequency(int frequency);

	void SetPWM(int device, int frequency); 
	void SetAllPWM(int frequency);

	int CalculateServoFrequency(double angle);
	int CalculateRotorFrequency(double force);

	void WriteByte(uint8_t addr, uint8_t value);
	void ReadByte(uint8_t addr, uint8_t *value);

	void InitializeESCs();

public:
	const double MAXROTOROUTPUT = 39.2;
	const double MINROTOROUTPUT = 0.0;
	const double MAXSERVOOUTPUT = 90.0;
	const double MINSERVOOUTPUT = -90.0;

	PWMController(int frequency, uint8_t addr);//24 -> 1526Hz

	void Wake();
	void Sleep();
	void Reset();
	void Restart();

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
