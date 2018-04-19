#include "I2CController.h"

I2CController::I2CController(int addr) {
	this->address = addr;

	std::cout << "Initializing I2C device library." << std::endl;

	I2Cdev::initialize();

	MQ  = new Quaternion();
	TBQ = new Quaternion();
	TCQ = new Quaternion();
	TDQ = new Quaternion();
	TEQ = new Quaternion();

	MV  = new Vector3D();
	TBV = new Vector3D();
	TCV = new Vector3D();
	TDV = new Vector3D();
	TEV = new Vector3D();

	MAO  = new Vector3D();
	TBAO = new Vector3D();
	TCAO = new Vector3D();
	TDAO = new Vector3D();
	TEAO = new Vector3D();

	MGO  = new Vector3D();
	TBGO = new Vector3D();
	TCGO = new Vector3D();
	TDGO = new Vector3D();
	TEGO = new Vector3D();

	MAKF  = new VectorKalmanFilter(0.001, 250);
	TBAKF = new VectorKalmanFilter(0.001, 250);
	TCAKF = new VectorKalmanFilter(0.001, 250);
	TDAKF = new VectorKalmanFilter(0.001, 250);
	TEAKF = new VectorKalmanFilter(0.001, 250);

	MGKF  = new VectorKalmanFilter(0.2, 50);
	TBGKF = new VectorKalmanFilter(0.2, 50);
	TCGKF = new VectorKalmanFilter(0.2, 50);
	TDGKF = new VectorKalmanFilter(0.2, 50);
	TEGKF = new VectorKalmanFilter(0.2, 50);
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
	mpuB    = new MPU6050(0x68);
	mpuC    = new MPU6050(0x68);
	mpuD    = new MPU6050(0x68);
	mpuE    = new MPU6050(0x68);

	std::cout << "Initializing MPUs." << std::endl;

	InitializeMPU(MainMPU,   mpuMain);
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

void I2CController::SetDefaultMPUOffsets() {
	std::cout << "Setting default calibration offsets." << std::endl;

	SelectDevice(MainMPU);
	mpuMain->setXAccelOffset(0); mpuMain->setYAccelOffset(0); mpuMain->setZAccelOffset(0);
	mpuMain->setXGyroOffset(0);  mpuMain->setYGyroOffset(0);  mpuMain->setZGyroOffset(0);

	SelectDevice(ThrusterBMPU);
	mpuB->setXAccelOffset(0); mpuB->setYAccelOffset(0); mpuB->setZAccelOffset(0);
	mpuB->setXGyroOffset(0);  mpuB->setYGyroOffset(0);  mpuB->setZGyroOffset(0);

	SelectDevice(ThrusterCMPU);
	mpuC->setXAccelOffset(0); mpuC->setYAccelOffset(0); mpuC->setZAccelOffset(0);
	mpuC->setXGyroOffset(0);  mpuC->setYGyroOffset(0);  mpuC->setZGyroOffset(0);

	SelectDevice(ThrusterDMPU);
	mpuD->setXAccelOffset(0); mpuD->setYAccelOffset(0); mpuD->setZAccelOffset(0);
	mpuD->setXGyroOffset(0);  mpuD->setYGyroOffset(0);  mpuD->setZGyroOffset(0);

	SelectDevice(ThrusterEMPU);
	mpuE->setXAccelOffset(0); mpuE->setYAccelOffset(0); mpuE->setZAccelOffset(0);
	mpuE->setXGyroOffset(0);  mpuE->setYGyroOffset(0);  mpuE->setZGyroOffset(0);
}

void I2CController::CalibrateMPUs() {
	auto startTime = std::chrono::system_clock::now();
	double runTime = 0;

	std::cout << "Calibrating MPU offsets." << std::endl;

	int secondPrint = 0;
	int calTime = 20;

	while (runTime < calTime) {
		runTime = ((double)((std::chrono::system_clock::now() - startTime).count()) / pow(10.0, 9.0));

		CalibrateMPU(MainMPU,   mpuMain, MAO,  MGO,  MAKF,  MGKF );
		CalibrateMPU(ThrusterBMPU, mpuB, TBAO, TBGO, TBAKF, TBGKF);
		CalibrateMPU(ThrusterCMPU, mpuC, TCAO, TCGO, TCAKF, TCGKF);
		CalibrateMPU(ThrusterDMPU, mpuD, TDAO, TDGO, TDAKF, TDGKF);
		CalibrateMPU(ThrusterEMPU, mpuE, TEAO, TEGO, TEAKF, TEGKF);

		if (runTime > secondPrint) {
			std::cout << "   MPU calibration step " << secondPrint << " of " << calTime << std::endl;
			secondPrint++;
		}

		bcm2835_delay(200);
	}

	std::cout << "MPU offsets calibrated." << std::endl;
}

void I2CController::CalibrateMPU(Device dev, MPU6050 *mpu, Vector3D *ao, Vector3D *go, VectorKalmanFilter *akf, VectorKalmanFilter *gkf) {
	SelectDevice(dev);

	int16_t ax, ay, az, gx, gy, gz;

	mpu->getMotion6(&ax, &ay, &az, &gx, &gy, &gz);

	ao = new Vector3D(mpu->getXAccelOffset(), mpu->getYAccelOffset(), mpu->getZAccelOffset());
	go = new Vector3D(mpu->getXGyroOffset(),  mpu->getYGyroOffset(),  mpu->getZGyroOffset() );

	Vector3D mod = akf->Filter(Vector3D(ax, ay, az - 8192)).Absolute().Divide(10);//subtract gravity

	//linear, wastes time
	if (ax > 0)     ao->X = ao->X - 1 * mod.X; else if (ax < 0)     ao->X = ao->X + 1 * mod.X;
	if (ay > 0)     ao->Y = ao->Y - 1 * mod.Y; else if (ay < 0)     ao->Y = ao->Y + 1 * mod.Y;
	if (az > 8192)  ao->Z = ao->Z - 1 * mod.Z; else if (az < 8192)  ao->Z = ao->Z + 1 * mod.Z;

	/*
	if (gx > 0)     go->X = go->X - 1; else if (gx < 0)     go->X = go->X + 1;
	if (gy > 0)     go->Y = go->Y - 1; else if (gy < 0)     go->Y = go->Y + 1;
	if (gz > 0)     go->Z = go->Z - 1; else if (gz < 0)     go->Z = go->Z + 1;
	*/
	
	//filter offset between value and zero, add this to the offset
	//Vector3D aOff = akf->Filter(Vector3D(ax, ay, az));//subtract gravity
	Vector3D gOff = gkf->Filter(Vector3D(gx, gy, gz));

	//ao = new Vector3D(ao->Subtract(aOff));
	go = new Vector3D(go->Subtract(gOff));

	//mpu->setXAccelOffset(ao->X);
	//mpu->setYAccelOffset(ao->Y);
	//mpu->setZAccelOffset(ao->Z);
	mpu->setXGyroOffset( go->X);
	mpu->setYGyroOffset( go->Y);
	mpu->setZGyroOffset( go->Z);

	//std::cout << "Accel:" << Vector3D(ax, ay, az).ToString() << " Mod:" << mod.ToString() << std::endl;
	//std::cout << "Accel:" << Vector3D(ax, ay, az).ToString() << " Gyro:" << Vector3D(gx, gy, gz).ToString() << std::endl;
	//std::cout << "Accel:" << Vector3D(ao->X, ao->Y, ao->Z).ToString() << " Gyro:" << Vector3D(go->X, go->Y, go->Z).ToString() << std::endl;
	//std::cout << "Accel:" << aOff.ToString() << " Gyro:" << gOff.ToString() << std::endl;
}

void I2CController::CalibrateMPU(Device dev, MPU9150 *mpu, Vector3D *ao, Vector3D *go, VectorKalmanFilter *akf, VectorKalmanFilter *gkf) {
	SelectDevice(dev);

	int16_t ax, ay, az, gx, gy, gz;

	mpu->getMotion6(&ax, &ay, &az, &gx, &gy, &gz);

	//filter offset between value and zero, add this to the offset
	Vector3D aOff = akf->Filter(Vector3D(ax, ay, az - 16384));//subtract gravity
	Vector3D gOff = gkf->Filter(Vector3D(gx, gy, gz));

	ao = new Vector3D(ao->Subtract(aOff));
	go = new Vector3D(go->Subtract(gOff));

	mpu->setXAccelOffset(ao->X);
	mpu->setYAccelOffset(ao->Y);
	mpu->setZAccelOffset(ao->Z);
	mpu->setXGyroOffset(go->X);
	mpu->setYGyroOffset(go->Y);
	mpu->setZGyroOffset(go->Z);

	std::cout << "Accel:" << Vector3D(ax, ay, az - 16384).ToString()
		<< " Gyro:" << Vector3D(gx, gy, gz).ToString() << std::endl;
}

void I2CController::CalibrateMPUDMPs() {
	auto startTime = std::chrono::system_clock::now();
	double runTime = 0;

	std::cout << "Calibrating DMP Modules." << std::endl;

	int secondPrint = 0;
	int calTime = 20;

	while (runTime < calTime) {
		runTime = ((double)((std::chrono::system_clock::now() - startTime).count()) / pow(10.0, 9.0));

		GetMainRotation();
		GetMainWorldAcceleration();
		GetTBRotation();
		GetTBWorldAcceleration();
		GetTCRotation();
		GetTCWorldAcceleration();
		GetTDRotation();
		GetTDWorldAcceleration();
		GetTERotation();
		GetTEWorldAcceleration();

		if (runTime > secondPrint) {
			std::cout << "   DMP calibration step " << secondPrint << " of " << calTime << std::endl;
			secondPrint++;
		}

		bcm2835_delay(50);
	}

	MV  = new Vector3D();
	TBV = new Vector3D();
	TCV = new Vector3D();
	TDV = new Vector3D();
	TEV = new Vector3D();

	std::cout << "MPU DMPs calibrated." << std::endl;
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

	std::cout << "   Initializing DMP of " << std::dec << dev << std::endl;

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

	std::cout << "   Initializing DMP of " << std::dec << dev << std::endl;

	dmpStatus = mpu->dmpInitialize();
	/*
	mpu->setXGyroOffset(220);
	mpu->setYGyroOffset(76);
	mpu->setZGyroOffset(-85);
	mpu->setZAccelOffset(1788);
	*/
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
