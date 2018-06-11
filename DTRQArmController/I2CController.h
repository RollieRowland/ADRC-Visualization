#pragma once

#include "MPU6050.h"
#include "MPU9150.h"
#include <stdio.h>
#include <iostream>
#include "PWMController.h"
#include "MPUController.h"
#include "I2Cdev.h"
#include <sstream>
#include <unistd.h>
#include <chrono>

#include "../DTRQController/Rotation.h"
#include "../DTRQController/VectorKalmanFilter.h"
#include "../DTRQController/VectorLeastSquares.h"

class I2CController {
public:
	enum Device {
		MainMPU,
		MainFMPU,
		MainBMPU,
		ThrusterBMPU,
		ThrusterCMPU,
		ThrusterDMPU,
		ThrusterEMPU,
		PWMManager
	};

	//valid inputs: 0 -> 7
	//open connection, write, end connection
	I2CController(int addr);
	~I2CController();

	void InitializeMPUs();
	void InitializePCA();
	void CalibrateMPUs();
	void ClearMPUFIFOs();
	void SetDefaultMPUOffsets();

	Quaternion GetMainRotation();
	Quaternion GetMainFRotation();
	Quaternion GetMainBRotation();
	Quaternion GetTBRotation();
	Quaternion GetTCRotation();
	Quaternion GetTDRotation();
	Quaternion GetTERotation();

	Vector3D GetMainWorldAcceleration();
	Vector3D GetMainFWorldAcceleration();
	Vector3D GetMainBWorldAcceleration();
	Vector3D GetTBWorldAcceleration();
	Vector3D GetTCWorldAcceleration();
	Vector3D GetTDWorldAcceleration();
	Vector3D GetTEWorldAcceleration();

	double GetAvgTemperature();

	void SetBThrustVector(Vector3D);
	void SetCThrustVector(Vector3D);
	void SetDThrustVector(Vector3D);
	void SetEThrustVector(Vector3D);

private:
	u_int8_t address;
	MPUController *mpuM;
	MPUController *mpuF;
	MPUController *mpuB;
	MPUController *mpuTB;
	MPUController *mpuTC;
	MPUController *mpuTD;
	MPUController *mpuTE;

	PWMController *hPWM;

	void SelectDevice(Device mpu);

};
