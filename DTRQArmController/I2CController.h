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
	double GetTemperature(Device dev);

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

	void SetBThrustVector(Vector3D);
	void SetCThrustVector(Vector3D);
	void SetDThrustVector(Vector3D);
	void SetEThrustVector(Vector3D);

private:
	u_int8_t address;
	uint16_t packetSizeM;
	uint16_t packetSizeF;
	uint16_t packetSizeB;
	uint16_t packetSizeTB;
	uint16_t packetSizeTC;
	uint16_t packetSizeTD;
	uint16_t packetSizeTE;

	MPU6050 *mpuMain;
	MPU6050 *mpuFMain;
	MPU6050 *mpuBMain;
	MPU6050 *mpuB;
	MPU6050 *mpuC;
	MPU6050 *mpuD;
	MPU6050 *mpuE;

	Quaternion *MQ;
	Quaternion *MFQ;
	Quaternion *MBQ;
	Quaternion *TBQ;
	Quaternion *TCQ;
	Quaternion *TDQ;
	Quaternion *TEQ;

	Vector3D *MV;
	Vector3D *MFV;
	Vector3D *MBV;
	Vector3D *TBV;
	Vector3D *TCV;
	Vector3D *TDV;
	Vector3D *TEV;

	PWMController *hPWM;

	Vector3D *MAO;
	Vector3D *MFAO;
	Vector3D *MBAO;
	Vector3D *TBAO;
	Vector3D *TCAO;
	Vector3D *TDAO;
	Vector3D *TEAO;

	Vector3D *MGO;
	Vector3D *MFGO;
	Vector3D *MBGO;
	Vector3D *TBGO;
	Vector3D *TCGO;
	Vector3D *TDGO;
	Vector3D *TEGO;

	VectorLeastSquares *MGLS;
	VectorLeastSquares *MFGLS;
	VectorLeastSquares *MBGLS;
	VectorLeastSquares *TBGLS;
	VectorLeastSquares *TCGLS;
	VectorLeastSquares *TDGLS;
	VectorLeastSquares *TEGLS;

	VectorLeastSquares *MALS;
	VectorLeastSquares *MFALS;
	VectorLeastSquares *MBALS;
	VectorLeastSquares *TBALS;
	VectorLeastSquares *TCALS;
	VectorLeastSquares *TDALS;
	VectorLeastSquares *TEALS;

	void SelectDevice(Device mpu);
	uint16_t InitializeMPU(Device dev, MPU6050 *mpu);
	uint16_t InitializeMPU(Device dev, MPU9150 *mpu);
	void SetDefaultMPUOffsets();

	double CalibrateMPU(Device dev, MPU6050 *mpu, Vector3D *ao, Vector3D *go, VectorLeastSquares *als, VectorLeastSquares *gls);
	double CalibrateMPU(Device dev, MPU9150 *mpu, Vector3D *ao, Vector3D *go, VectorLeastSquares *als, VectorLeastSquares *gls);

	Quaternion GetRotation(Device dev, MPU6050 *mpu, uint16_t packetSize);
	Quaternion GetRotation(Device dev, MPU9150 *mpu, uint16_t packetSize);

	VectorInt16 GetGyro(Device dev, MPU6050 *mpu, uint16_t packetSize);
	VectorInt16 GetAccel(Device dev, MPU6050 *mpu, uint16_t packetSize);

	Vector3D GetLinearAcceleration(Device dev, MPU6050 *mpu, uint16_t packetSize);
	Vector3D GetLinearAcceleration(Device dev, MPU9150 *mpu, uint16_t packetSize);

};
