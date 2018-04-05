#include "I2CController.h"

I2CController::I2CController(int addr) {
	this->address = addr;

	mpuMain = new MPU6050(address);
	mpuB = new MPU6050(address);
	mpuC = new MPU6050(address);
	mpuD = new MPU6050(address);
	mpuE = new MPU6050(address);

	InitializeMPU(Main, mpuMain);
	InitializeMPU(ThrusterB, mpuMain);
	InitializeMPU(ThrusterC, mpuMain);
	InitializeMPU(ThrusterD, mpuMain);
	InitializeMPU(ThrusterE, mpuMain);
}

void I2CController::SelectDevice(MPU mpu) {
	if (mpu == Main) {
		int addr = 1 << 0;//1 * 2^i
		char buf[4];

		sprintf(buf, "%d", addr);

		bcm2835_i2c_write(buf, 4);
	}
	else if (mpu == ThrusterB) {
		int addr = 1 << 1;//1 * 2^i
		char buf[4];

		sprintf(buf, "%d", addr);

		bcm2835_i2c_write(buf, 4);
	}
	else if (mpu == ThrusterC) {
		int addr = 1 << 2;//1 * 2^i
		char buf[4];

		sprintf(buf, "%d", addr);

		bcm2835_i2c_write(buf, 4);
	}
	else if (mpu == ThrusterD) {
		int addr = 1 << 3;//1 * 2^i
		char buf[4];

		sprintf(buf, "%d", addr);

		bcm2835_i2c_write(buf, 4);
	}
	else if (mpu == ThrusterE) {
		int addr = 1 << 4;//1 * 2^i
		char buf[4];

		sprintf(buf, "%d", addr);

		bcm2835_i2c_write(buf, 4);
	}
}

void I2CController::InitializeMPU(MPU dev, MPU6050 *mpu) {
	int dmpStatus;

	SelectDevice(dev);
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

Quaternion I2CController::GetRotation(MPU dev, MPU6050 *mpu) {
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
	}
}

Quaternion I2CController::GetMainRotation() {
	Quaternion q = GetRotation(Main, mpuMain);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(MQ->W, MQ->X, MQ->Y, MQ->Z);
	}
	else {
		MQ = new Quaternion(q);

		return q;
	}
}

Quaternion I2CController::GetTBRotation() {
	Quaternion q = GetRotation(ThrusterB, mpuB);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(TBQ->W, TBQ->X, TBQ->Y, TBQ->Z);
	}
	else {
		TBQ = new Quaternion(q);

		return q;
	}
}

Quaternion I2CController::GetTCRotation() {
	Quaternion q = GetRotation(ThrusterC, mpuC);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(TCQ->W, TCQ->X, TCQ->Y, TCQ->Z);
	}
	else {
		TCQ = new Quaternion(q);

		return q;
	}
}

Quaternion I2CController::GetTDRotation() {
	Quaternion q = GetRotation(ThrusterD, mpuD);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(TDQ->W, TDQ->X, TDQ->Y, TDQ->Z);
	}
	else {
		TDQ = new Quaternion(q);

		return q;
	}
}

Quaternion I2CController::GetTERotation() {
	Quaternion q = GetRotation(ThrusterE, mpuE);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(TEQ->W, TEQ->X, TEQ->Y, TEQ->Z);
	}
	else {
		TEQ = new Quaternion(q);

		return q;
	}
}


Vector3D I2CController::GetLinearAcceleration(MPU dev, MPU6050 *mpu) {
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

		if (dev == Main) {
			q = QuaternionFloat(MQ->W, MQ->X, MQ->Y, MQ->Z);
		}
		else if (dev == ThrusterB) {
			q = QuaternionFloat(TBQ->W, TBQ->X, TBQ->Y, TBQ->Z);
		}
		else if (dev == ThrusterC) {
			q = QuaternionFloat(TCQ->W, TCQ->X, TCQ->Y, TCQ->Z);
		}
		else if (dev == ThrusterD) {
			q = QuaternionFloat(TDQ->W, TDQ->X, TDQ->Y, TDQ->Z);
		}
		else if (dev == ThrusterE) {
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
}

Vector3D I2CController::GetMainWorldAcceleration() {
	Vector3D v = GetLinearAcceleration(Main, mpuMain);

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
	Vector3D v = GetLinearAcceleration(ThrusterB, mpuB);

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
	Vector3D v = GetLinearAcceleration(ThrusterC, mpuC);

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
	Vector3D v = GetLinearAcceleration(ThrusterD, mpuD);

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
	Vector3D v = GetLinearAcceleration(ThrusterE, mpuE);

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		return Vector3D(TEV->X, TEV->Y, TEV->Z);
	}
	else {
		v = TEQ->RotateVector(v);

		TEV = new Vector3D(v);

		return v;
	}
}
