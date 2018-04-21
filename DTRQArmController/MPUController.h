#pragma once

#include "MPU.h"
#include "MPU6050.h"
#include "MPU9150.h"
#include <stdio.h>
#include <iostream>
#include <unistd.h>

#include "../DTRQController/Rotation.h"
#include "../DTRQController/VectorKalmanFilter.h"
#include "../DTRQController/VectorLeastSquares.h"

class MPUController {
public:
	MPUController(MPU *);

	void Initialize();
	int CalibrateMPU(bool);
	void ClearMPUFIFO();
	double GetTemperature();

	void SetDefaultMPUOffsets();

	Quaternion GetRotation();
	VectorInt16 GetGyro();
	VectorInt16 GetAccel();
	uint8_t GetLowPass();
	void SetLowPass(uint8_t);
	uint8_t GetHighPass();
	void SetHighPass(uint8_t);

	Quaternion GetPreviousRotation();
	Vector3D GetPreviousAcceleration();

	Vector3D GetLinearAcceleration();


private:
	uint16_t packetSize;

	MPU *mpu;

	Quaternion *rotation;
	Vector3D *acceleration;

	Vector3D *accelerationOffset;
	Vector3D *gyroscopeOffset;

	VectorKalmanFilter *gyroKF;
	VectorKalmanFilter *acceKF;
	VectorKalmanFilter *gyroOKF;
	VectorKalmanFilter *acceOKF;

	VectorLeastSquares *gyroLS;
	VectorLeastSquares *acceLS;

};
