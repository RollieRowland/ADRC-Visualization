#include "I2CController.h"

I2CController::I2CController(int addr) {
	this->address = addr;

	std::cout << "Initializing I2C device library." << std::endl;

	I2Cdev::initialize();

	MQ = new Quaternion();
	TBQ = new Quaternion();
	TCQ = new Quaternion();
	TDQ = new Quaternion();
	TEQ = new Quaternion();

	MV = new Vector3D();
	TBV = new Vector3D();
	TCV = new Vector3D();
	TDV = new Vector3D();
	TEV = new Vector3D();
}

I2CController::~I2CController() {
	std::cout << "Resetting hardware and clearing memory." << std::endl;

	SelectDevice(PWMManager);//PWMManager
	hPWM->Restart();

	delay(10);

	hPWM->Sleep();

	delay(10);

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

void I2CController::InitializeMPUs() {
	std::cout << "Creating MPU objects." << std::endl;

	mpuMain = new MPU6050(0x68);
	mpuB = new MPU6050(0x68);
	mpuC = new MPU6050(0x68);
	mpuD = new MPU6050(0x68);
	mpuE = new MPU6050(0x68);

	std::cout << "Initializing MPUs." << std::endl;

	InitializeMPU(MainMPU, mpuMain);
	InitializeMPU(ThrusterBMPU, mpuB);
	InitializeMPU(ThrusterCMPU, mpuC);
	InitializeMPU(ThrusterDMPU, mpuD);
	InitializeMPU(ThrusterEMPU, mpuE);

	std::cout << "MPUs initialized." << std::endl;
}

void I2CController::InitializePCA() {
	SelectDevice(PWMManager);//PWMManager

	hPWM = new PWMController(200, 0x40);

}

void I2CController::SelectDevice(Device dev) {
	//std::cout << "Selecting " << mpu << std::endl;

	int addr;

	if (dev == MainMPU) {
		addr = 1 << 4;//1 * 2^i
	}
	else if (dev == ThrusterBMPU) {
		addr = 1 << 3;//1 * 2^i
	}
	else if (dev == ThrusterCMPU) {
		addr = 1 << 2;//1 * 2^i
	}
	else if (dev == ThrusterDMPU) {
		addr = 1 << 1;//1 * 2^i
	}
	else if (dev == ThrusterEMPU) {
		addr = 1 << 0;//1 * 2^i
	}
	else if (dev == PWMManager) {
		addr = 1 << 5;//1 * 2^i
	}

	I2Cdev::writeByte(address, 0x00, addr);

	//std::cout << "Selected " << mpu << std::endl;
}

void I2CController::InitializeMPU(Device dev, MPU6050 *mpu) {
	int dmpStatus;

	std::cout << "Initializing MPU6050 device: " << dev << std::endl;

	SelectDevice(dev);
	std::cout << "   Resetting MPU6050 before initialization." << std::endl;
	mpu->reset();

	delay(100);

	std::cout << "   Testing I2C connection to MPU6050." << std::endl;

	std::stringstream sstream;
	sstream << std::hex << (int)mpu->getDeviceID();
	std::string result = sstream.str();

	std::cout << "   Device ID: " << std::hex << result << " Expected: " << 0x34 << std::endl;

	if (mpu->testConnection()) {
		std::cout << "      MPU6050 connection test successful." << std::endl;
	}
	else {
		std::cout << "      MPU6050 connection test failed." << std::endl;
	}

	mpu->initialize();

	std::cout << "   Initializing DMP of " << dev << std::endl;

	dmpStatus = mpu->dmpInitialize(address);

	mpu->setXGyroOffset(220);
	mpu->setYGyroOffset(76);
	mpu->setZGyroOffset(-85);
	mpu->setZAccelOffset(1788);

	if (dmpStatus == 0) {
		std::cout << "      Enabling DMP of MPU6050" << std::endl;

		mpu->setDMPEnabled(true);

		packetSize = mpu->dmpGetFIFOPacketSize();
	}
	else {
		std::cout << "      DMP Initialization Failed on " << dev << std::endl;
	}
}

void I2CController::InitializeMPU(Device dev, MPU9150 *mpu) {
	int dmpStatus;

	std::cout << "Initializing MPU9150 device: " << dev << std::endl;

	SelectDevice(dev);
	std::cout << "   Resetting MPU9150 before initialization." << std::endl;
	mpu->reset();

	delay(100);

	std::cout << "   Testing I2C connection to MPU9150." << std::endl;

	std::stringstream sstream;
	sstream << std::hex << (int)mpu->getDeviceID();
	std::string result = sstream.str();

	std::cout << "   Device ID: " << std::hex << result << " Expected: " << 0x39 << std::endl;

	if (mpu->testConnection()) {
		std::cout << "      MPU9150 connection test successful." << std::endl;
	}
	else {
		std::cout << "      MPU9150 connection test failed." << std::endl;
	}

	mpu->initialize();

	std::cout << "   Initializing DMP of " << dev << std::endl;

	dmpStatus = mpu->dmpInitialize();

	mpu->setXGyroOffset(220);
	mpu->setYGyroOffset(76);
	mpu->setZGyroOffset(-85);
	mpu->setZAccelOffset(1788);

	if (dmpStatus == 0) {
		std::cout << "      Enabling DMP of MPU9150" << std::endl;

		mpu->setDMPEnabled(true);

		packetSize = mpu->dmpGetFIFOPacketSize();
	}
	else {
		std::cout << "      DMP Initialization Failed on " << dev << std::endl;
	}
}

Quaternion I2CController::GetRotation(Device dev, MPU6050 *mpu) {
	SelectDevice(dev);

	//std::cout << "Getting mpu6050 status." << std::endl;
	int mpuIntStatus = mpu->getIntStatus();

	//std::cout << "Getting mpu6050 fifo count." << std::endl;
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	//std::cout << "FIFO count: " << fifoCount << std::endl;

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		std::cout << "Resetting mpu6050 fifo." << std::endl;
		mpu->resetFIFO();

		std::cout << "MPU6050 FIFO reset." << std::endl;

		return Quaternion(-1, -1, -1, -1);
	}
	else if (mpuIntStatus & 0x02) {
		while (fifoCount < packetSize) {
			fifoCount = mpu->getFIFOCount();
		}

		mpu->getFIFOBytes(fifoBuffer, packetSize);

		fifoCount -= packetSize;

		QuaternionFloat q = QuaternionFloat();

		mpu->dmpGetQuaternion(&q, fifoBuffer);

		//flip chirality and negate
		return Quaternion(q.w, q.y, q.z, q.x);
	}
	else {
		return Quaternion(-1, -1, -1, -1);
	}
}

Quaternion I2CController::GetRotation(Device dev, MPU9150 *mpu) {
	SelectDevice(dev);

	//std::cout << "Getting mpu9150 status." << std::endl;
	int mpuIntStatus = mpu->getIntStatus();

	//std::cout << "Getting mpu9150 fifo count." << std::endl;
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	//std::cout << "FIFO count: " << fifoCount << std::endl;

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		std::cout << "Resetting MPU9150 fifo." << std::endl;
		mpu->resetFIFO();

		std::cout << "MPU9150 FIFO reset." << std::endl;

		return Quaternion(-1, -1, -1, -1);
	}
	else if (mpuIntStatus & 0x02) {
		while (fifoCount < packetSize) {
			fifoCount = mpu->getFIFOCount();
		}

		mpu->getFIFOBytes(fifoBuffer, packetSize);

		fifoCount -= packetSize;

		QuaternionFloat q = QuaternionFloat();

		mpu->dmpGetQuaternion(&q, fifoBuffer);

		/*
		+1 x w if (1,2,3), (3,1,2), or (2,3,1)
		-1 x w if (1,3,2), (3,2,1), or (2,1,3)
		0 if i=j || j=k || k=i
		-> antisymmetric tensor(Levi-Civita symbol)
		*/
		//flip chirality and negate
		return Quaternion(q.w, q.y, q.z, q.x);
	}
	else {
		return Quaternion(-1, -1, -1, -1);
	}
}

Quaternion I2CController::GetMainRotation() {
	Quaternion q = GetRotation(MainMPU, mpuMain);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		//std::cout << "Main R fetch failed, using previous values." << std::endl;
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
		//std::cout << "TB R fetch failed, using previous values." << std::endl;
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
		//std::cout << "TC R fetch failed, using previous values." << std::endl;
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
		//std::cout << "TD R fetch failed, using previous values." << std::endl;
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
		//std::cout << "TE R fetch failed, using previous values." << std::endl;
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
	uint8_t fifoBuffer[64]; //FIFO storage buffer

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		std::cout << "Resetting MPU6050 fifo." << std::endl;
		mpu->resetFIFO();

		std::cout << "MPU6050 FIFO reset." << std::endl;

		return Vector3D(-1000, -1000, -1000);
	}
	else if (mpuIntStatus & 0x02) {
		while (fifoCount < packetSize) {
			fifoCount = mpu->getFIFOCount();
		}

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

		Vector3D gAccel = Vector3D(lA.x, lA.z, lA.y);

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
		std::cout << "Resetting MPU9150 fifo." << std::endl;
		mpu->resetFIFO();

		std::cout << "MPU9150 FIFO reset." << std::endl;

		return Vector3D(-1000, -1000, -1000);
	}
	else if (mpuIntStatus & 0x02) {
		while (fifoCount < packetSize) {
			fifoCount = mpu->getFIFOCount();
		}

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

		Vector3D gAccel = Vector3D(lA.x, lA.z, lA.y);

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
		//std::cout << "Main WA fetch failed, using previous values." << std::endl;
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
		//std::cout << "TB WA fetch failed, using previous values." << std::endl;
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
		//std::cout << "TC WA fetch failed, using previous values." << std::endl;
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
		//std::cout << "TD WA fetch failed, using previous values." << std::endl;
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
		//std::cout << "TE WA fetch failed, using previous values." << std::endl;
		return Vector3D(TEV->X, TEV->Y, TEV->Z);
	}
	else {
		v = TEQ->RotateVector(v);

		TEV = new Vector3D(v);

		return v;
	}
}

void I2CController::SetBThrustVector(Vector3D tV) {
	SelectDevice(PWMManager);//PWMManager

	hPWM->SetInnerBAngle(tV.X);
	hPWM->SetRotorBOutput(tV.Y);
	hPWM->SetOuterBAngle(tV.Z);
}

void I2CController::SetCThrustVector(Vector3D tV) {
	SelectDevice(PWMManager);//PWMManager

	hPWM->SetInnerCAngle(tV.X);
	hPWM->SetRotorCOutput(tV.Y);
	hPWM->SetOuterCAngle(tV.Z);
}

void I2CController::SetDThrustVector(Vector3D tV) {
	SelectDevice(PWMManager);//PWMManager

	hPWM->SetInnerDAngle(tV.X);
	hPWM->SetRotorDOutput(tV.Y);
	hPWM->SetOuterDAngle(tV.Z);
}

void I2CController::SetEThrustVector(Vector3D tV) {
	SelectDevice(PWMManager);//PWMManager

	hPWM->SetInnerEAngle(tV.X);
	hPWM->SetRotorEOutput(tV.Y);
	hPWM->SetOuterEAngle(tV.Z);
}
