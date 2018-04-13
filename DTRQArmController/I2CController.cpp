#include "I2CController.h"

I2CController::I2CController(int addr) {
	this->address = addr;

	std::cout << "Initializing I2C device library." << std::endl;

	I2Cdev::initialize();

	std::cout << "Creating MPU objects." << std::endl;

	mpuMain = new MPU9150(address);
	mpuB = new MPU6050(address);
	mpuC = new MPU6050(address);
	mpuD = new MPU6050(address);
	mpuE = new MPU6050(address);

	std::cout << "Initializing MPUs." << std::endl;

	InitializeMPU(MainMPU, mpuMain);
	InitializeMPU(ThrusterBMPU, mpuB);
	InitializeMPU(ThrusterCMPU, mpuC);
	InitializeMPU(ThrusterDMPU, mpuD);
	InitializeMPU(ThrusterEMPU, mpuE);

	//hPWM = new PWMController(500);

	std::cout << "MPUs initialized." << std::endl;
}

I2CController::~I2CController() {
	delete mpuMain;
	delete mpuB;
	delete mpuC;
	delete mpuD;
	delete mpuE;

	delete MQ;
	delete TBQ;
	delete TCQ;
	delete TDQ;
	delete TEQ;

	delete MV;
	delete TBV;
	delete TCV;
	delete TDV;
	delete TEV;

	delete hPWM;
}

void I2CController::SelectDevice(Device mpu) {
	std::cout << "Selecting " << mpu << std::endl;

	int addr;

	if (mpu == MainMPU) {
		addr = 1 << 1;//1 * 2^i
	}
	else if (mpu == ThrusterBMPU) {
		addr = 1 << 2;//1 * 2^i
	}
	else if (mpu == ThrusterCMPU) {
		addr = 1 << 3;//1 * 2^i
	}
	else if (mpu == ThrusterDMPU) {
		addr = 1 << 4;//1 * 2^i
	}
	else if (mpu == ThrusterEMPU) {
		addr = 1 << 5;//1 * 2^i
	}

	I2Cdev::writeByte(address, 0x00, addr);
}

void I2CController::InitializeMPU(Device dev, MPU9150 *mpu) {
	int dmpStatus;

	std::cout << "Initializing MPU9150." << std::endl;

	SelectDevice(dev);

	std::cout << "Testing I2C connection to MPU9150." << std::endl;

	mpu->testConnection();
	mpu->initialize();

	std::cout << "Initializing DMP of " << dev << std::endl;

	dmpStatus = mpu->dmpInitialize();

	mpu->setXGyroOffset(220);
	mpu->setYGyroOffset(76);
	mpu->setZGyroOffset(-85);
	mpu->setZAccelOffset(1788);

	if (dmpStatus == 0) {
		std::cout << "Enabling DMP" << std::endl;

		mpu->setDMPEnabled(true);

		//interupt setting

		//dmpReady

		packetSize = mpu->dmpGetFIFOPacketSize();
	}
	else {
		std::cout << "DMP Initialization Failed on " << dev << std::endl;
	}
}

void I2CController::InitializeMPU(Device dev, MPU6050 *mpu) {
	int dmpStatus;

	std::cout << "Initializing MPU6050." << std::endl;

	SelectDevice(dev);

	std::cout << "Testing I2C connection to MPU6050." << std::endl;

	mpu->testConnection();
	mpu->initialize();

	std::cout << "Initializing DMP of " << dev << std::endl;
	
	dmpStatus = mpu->dmpInitialize(address);

	mpu->setXGyroOffset(220);
	mpu->setYGyroOffset(76);
	mpu->setZGyroOffset(-85);
	mpu->setZAccelOffset(1788);

	if (dmpStatus == 0) {
		std::cout << "Enabling DMP" << std::endl;

		mpu->setDMPEnabled(true);

		//interupt setting

		//dmpReady

		packetSize = mpu->dmpGetFIFOPacketSize();
	}
	else {
		std::cout << "DMP Initialization Failed on " << dev << std::endl;
	}
}

Quaternion I2CController::GetRotation(Device dev, MPU6050 *mpu) {
	SelectDevice(dev);

	int mpuIntStatus = mpu->getIntStatus();
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		return Quaternion(-1, -1, -1, -1);
	}
	else if (mpuIntStatus & 0x02) {
		while (fifoCount < packetSize) fifoCount = mpu->getFIFOCount();

		mpu->getFIFOBytes(fifoBuffer, packetSize);

		fifoCount -= packetSize;

		QuaternionFloat q = QuaternionFloat();

		mpu->dmpGetQuaternion(&q, fifoBuffer);

		return Quaternion(q.w, q.x, q.y, q.z);
	}
}

Quaternion I2CController::GetRotation(Device dev, MPU9150 *mpu) {
	SelectDevice(dev);

	int mpuIntStatus = mpu->getIntStatus();
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		return Quaternion(-1, -1, -1, -1);
	}
	else if (mpuIntStatus & 0x02) {
		while (fifoCount < packetSize) fifoCount = mpu->getFIFOCount();

		mpu->getFIFOBytes(fifoBuffer, packetSize);

		fifoCount -= packetSize;

		QuaternionFloat q = QuaternionFloat();

		mpu->dmpGetQuaternion(&q, fifoBuffer);

		return Quaternion(q.w, q.x, q.y, q.z);
	}
}

Quaternion I2CController::GetMainRotation() {
	Quaternion q = GetRotation(MainMPU, mpuMain);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(MQ->W, MQ->X, MQ->Y, MQ->Z);
	}
	else {
		MQ = new Quaternion(q);

		return q;
	}
}

Quaternion I2CController::GetTBRotation() {
	Quaternion q = GetRotation(ThrusterBMPU, mpuB);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(TBQ->W, TBQ->X, TBQ->Y, TBQ->Z);
	}
	else {
		TBQ = new Quaternion(q);

		return q;
	}
}

Quaternion I2CController::GetTCRotation() {
	Quaternion q = GetRotation(ThrusterCMPU, mpuC);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(TCQ->W, TCQ->X, TCQ->Y, TCQ->Z);
	}
	else {
		TCQ = new Quaternion(q);

		return q;
	}
}

Quaternion I2CController::GetTDRotation() {
	Quaternion q = GetRotation(ThrusterDMPU, mpuD);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(TDQ->W, TDQ->X, TDQ->Y, TDQ->Z);
	}
	else {
		TDQ = new Quaternion(q);

		return q;
	}
}

Quaternion I2CController::GetTERotation() {
	Quaternion q = GetRotation(ThrusterEMPU, mpuE);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(TEQ->W, TEQ->X, TEQ->Y, TEQ->Z);
	}
	else {
		TEQ = new Quaternion(q);

		return q;
	}
}


Vector3D I2CController::GetLinearAcceleration(Device dev, MPU6050 *mpu) {
	SelectDevice(dev);

	int mpuIntStatus = mpu->getIntStatus();
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		return Vector3D(-1000, -1000, -1000);
	}
	else if (mpuIntStatus & 0x02) {
		while (fifoCount < packetSize) fifoCount = mpu->getFIFOCount();

		mpu->getFIFOBytes(fifoBuffer, packetSize);

		fifoCount -= packetSize;

		QuaternionFloat q = QuaternionFloat();

		if (dev == MainMPU) {
			q = QuaternionFloat(MQ->W, MQ->X, MQ->Y, MQ->Z);
		}
		else if (dev == ThrusterBMPU) {
			q = QuaternionFloat(TBQ->W, TBQ->X, TBQ->Y, TBQ->Z);
		}
		else if (dev == ThrusterCMPU) {
			q = QuaternionFloat(TCQ->W, TCQ->X, TCQ->Y, TCQ->Z);
		}
		else if (dev == ThrusterDMPU) {
			q = QuaternionFloat(TDQ->W, TDQ->X, TDQ->Y, TDQ->Z);
		}
		else if (dev == ThrusterEMPU) {
			q = QuaternionFloat(TEQ->W, TEQ->X, TEQ->Y, TEQ->Z);
		}

		VectorInt16 a = VectorInt16();
		VectorFloat g = VectorFloat();
		VectorInt16 lA = VectorInt16();

		mpu->dmpGetAccel(&a, fifoBuffer);
		mpu->dmpGetGravity(&g, &q);
		mpu->dmpGetLinearAccel(&lA, &a, &g);

		Vector3D gAccel = Vector3D(lA.x, lA.y, lA.z);

		//ax = raw / sensitivity SET TO 4 CURRENTLY
		//2g = sensitivity of 16384
		//4g = sensitivity of 8192
		//8g = sensitivity of 4096
		//16g = sensitivity of 2048
		gAccel = gAccel / 8192;//Scaling factor from 8192+/- to 

		return gAccel;
	}
	else {
		return Vector3D(-1000, -1000, -1000);
	}
}

Vector3D I2CController::GetLinearAcceleration(Device dev, MPU9150 *mpu) {
	SelectDevice(dev);

	int mpuIntStatus = mpu->getIntStatus();
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		return Vector3D(-1000, -1000, -1000);
	}
	else if (mpuIntStatus & 0x02) {
		while (fifoCount < packetSize) fifoCount = mpu->getFIFOCount();

		mpu->getFIFOBytes(fifoBuffer, packetSize);

		fifoCount -= packetSize;

		QuaternionFloat q = QuaternionFloat();

		if (dev == MainMPU) {
			q = QuaternionFloat(MQ->W, MQ->X, MQ->Y, MQ->Z);
		}
		else if (dev == ThrusterBMPU) {
			q = QuaternionFloat(TBQ->W, TBQ->X, TBQ->Y, TBQ->Z);
		}
		else if (dev == ThrusterCMPU) {
			q = QuaternionFloat(TCQ->W, TCQ->X, TCQ->Y, TCQ->Z);
		}
		else if (dev == ThrusterDMPU) {
			q = QuaternionFloat(TDQ->W, TDQ->X, TDQ->Y, TDQ->Z);
		}
		else if (dev == ThrusterEMPU) {
			q = QuaternionFloat(TEQ->W, TEQ->X, TEQ->Y, TEQ->Z);
		}

		VectorInt16 a = VectorInt16();
		VectorFloat g = VectorFloat();
		VectorInt16 lA = VectorInt16();

		mpu->dmpGetAccel(&a, fifoBuffer);
		mpu->dmpGetGravity(&g, &q);
		mpu->dmpGetLinearAccel(&lA, &a, &g);

		Vector3D gAccel = Vector3D(lA.x, lA.y, lA.z);

		//ax = raw / sensitivity SET TO 4 CURRENTLY
		//2g = sensitivity of 16384
		//4g = sensitivity of 8192
		//8g = sensitivity of 4096
		//16g = sensitivity of 2048
		gAccel = gAccel / 8192;//Scaling factor from 8192+/- to 

		return gAccel;
	}
	else {
		return Vector3D(-1000, -1000, -1000);
	}
}

Vector3D I2CController::GetMainWorldAcceleration() {
	Vector3D v = GetLinearAcceleration(MainMPU, mpuMain);

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		return Vector3D(MV->X, MV->Y, MV->Z);
	}
	else {
		v = MQ->RotateVector(v);

		MV = new Vector3D(v);

		return v;
	}
}

Vector3D I2CController::GetTBWorldAcceleration() {
	Vector3D v = GetLinearAcceleration(ThrusterBMPU, mpuB);

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		return Vector3D(TBV->X, TBV->Y, TBV->Z);
	}
	else {
		v = TBQ->RotateVector(v);

		TBV = new Vector3D(v);

		return v;
	}
}

Vector3D I2CController::GetTCWorldAcceleration() {
	Vector3D v = GetLinearAcceleration(ThrusterCMPU, mpuC);

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		return Vector3D(TCV->X, TCV->Y, TCV->Z);
	}
	else {
		v = TCQ->RotateVector(v);

		TCV = new Vector3D(v);

		return v;
	}
}

Vector3D I2CController::GetTDWorldAcceleration() {
	Vector3D v = GetLinearAcceleration(ThrusterDMPU, mpuD);

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		return Vector3D(TDV->X, TDV->Y, TDV->Z);
	}
	else {
		v = TDQ->RotateVector(v);

		TDV = new Vector3D(v);

		return v;
	}
}

Vector3D I2CController::GetTEWorldAcceleration() {
	Vector3D v = GetLinearAcceleration(ThrusterEMPU, mpuE);

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		return Vector3D(TEV->X, TEV->Y, TEV->Z);
	}
	else {
		v = TEQ->RotateVector(v);

		TEV = new Vector3D(v);

		return v;
	}
}

void I2CController::SetBThrustVector(Vector3D tV) {
	hPWM->SetInnerBAngle(tV.X);
	hPWM->SetRotorBOutput(tV.Y);
	hPWM->SetOuterBAngle(tV.Z);
}

void I2CController::SetCThrustVector(Vector3D tV) {
	hPWM->SetInnerCAngle(tV.X);
	hPWM->SetRotorCOutput(tV.Y);
	hPWM->SetOuterCAngle(tV.Z);
}

void I2CController::SetDThrustVector(Vector3D tV) {
	hPWM->SetInnerDAngle(tV.X);
	hPWM->SetRotorDOutput(tV.Y);
	hPWM->SetOuterDAngle(tV.Z);
}

void I2CController::SetEThrustVector(Vector3D tV) {
	hPWM->SetInnerEAngle(tV.X);
	hPWM->SetRotorEOutput(tV.Y);
	hPWM->SetOuterEAngle(tV.Z);
}
