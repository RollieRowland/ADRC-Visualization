#pragma once

#include "bcm2835.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include <stdio.h>
#include <iostream>
#include "Rotation.h"

class I2CController {
public:
	enum MPU {
		Main,
		ThrusterB,
		ThrusterC,
		ThrusterD,
		ThrusterE
	};

	//valid inputs: 0 -> 7
	//open connection, write, end connection
	I2CController(int addr);

	Quaternion GetMainRotation();
	Quaternion GetTBRotation();
	Quaternion GetTCRotation();
	Quaternion GetTDRotation();
	Quaternion GetTERotation();

	Vector3D GetMainWorldAcceleration();
	Vector3D GetTBWorldAcceleration();
	Vector3D GetTCWorldAcceleration();
	Vector3D GetTDWorldAcceleration();
	Vector3D GetTEWorldAcceleration();

private:
	int address;
	int baudrate;
	int packetSize;

	MPU6050 *mpuMain;
	MPU6050 *mpuB;
	MPU6050 *mpuC;
	MPU6050 *mpuD;
	MPU6050 *mpuE;

	Quaternion *MQ;
	Quaternion *TBQ;
	Quaternion *TCQ;
	Quaternion *TDQ;
	Quaternion *TEQ;

	Vector3D *MV;
	Vector3D *TBV;
	Vector3D *TCV;
	Vector3D *TDV;
	Vector3D *TEV;

	void SelectDevice(MPU mpu);
	void InitializeMPU(MPU dev, MPU6050 *mpu);

	Quaternion GetRotation(MPU dev, MPU6050 *mpu);
	Vector3D GetLinearAcceleration(MPU dev, MPU6050 *mpu);

};
