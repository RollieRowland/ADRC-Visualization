#pragma once

#include "MPU6050.h"
#include "MPU9150.h"
#include <stdio.h>
#include <iostream>
#include "PWMController.h"
#include "I2Cdev.h"
#include <sstream>
#include <unistd.h>
#include <chrono>

#include "../DTRQController/Rotation.h"
#include "../DTRQController/VectorKalmanFilter.h"


class I2CController {
public:
	enum Device {
		MainMPU,
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

	void SetBThrustVector(Vector3D);
	void SetCThrustVector(Vector3D);
	void SetDThrustVector(Vector3D);
	void SetEThrustVector(Vector3D);

private:
	u_int8_t address;
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

	PWMController *hPWM;

	Vector3D *MAO;
	Vector3D *TBAO;
	Vector3D *TCAO;
	Vector3D *TDAO;
	Vector3D *TEAO;

	Vector3D *MGO;
	Vector3D *TBGO;
	Vector3D *TCGO;
	Vector3D *TDGO;
	Vector3D *TEGO;

	VectorKalmanFilter *MAKF;
	VectorKalmanFilter *TBAKF;
	VectorKalmanFilter *TCAKF;
	VectorKalmanFilter *TDAKF;
	VectorKalmanFilter *TEAKF;

	VectorKalmanFilter *MGKF;
	VectorKalmanFilter *TBGKF;
	VectorKalmanFilter *TCGKF;
	VectorKalmanFilter *TDGKF;
	VectorKalmanFilter *TEGKF;

	void SelectDevice(Device mpu);
	void InitializeMPU(Device dev, MPU6050 *mpu);
	void InitializeMPU(Device dev, MPU9150 *mpu);
	void SetDefaultMPUOffsets();

	void CalibrateMPU(Device dev, MPU6050 *mpu, Vector3D *ao, Vector3D *go, VectorKalmanFilter *akf, VectorKalmanFilter *gkf);
	void CalibrateMPU(Device dev, MPU9150 *mpu, Vector3D *ao, Vector3D *go, VectorKalmanFilter *akf, VectorKalmanFilter *gkf);

	Quaternion GetRotation(Device dev, MPU6050 *mpu);
	Quaternion GetRotation(Device dev, MPU9150 *mpu);

	Vector3D GetLinearAcceleration(Device dev, MPU6050 *mpu);
	Vector3D GetLinearAcceleration(Device dev, MPU9150 *mpu);

};
