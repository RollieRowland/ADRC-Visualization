#pragma once

#include "bcm2835.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include <stdio.h>

class I2CController {
private:
	int address;
	int baudrate;
	MPU6050 *mpuMain;
	MPU6050 *mpuB;
	MPU6050 *mpuC;
	MPU6050 *mpuD;
	MPU6050 *mpuE;

public:
	enum MPU{
		Main,
		ThrusterB,
		ThrusterC,
		ThrusterD,
		ThrusterE
	};

	//valid inputs: 0 -> 7
	//open connection, write, end connection
	I2CController();

	void SelectDevice(MPU mpu);


	

};