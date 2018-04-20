#include "I2CController.h"

I2CController::I2CController(int addr) {
	this->address = addr;

	std::cout << "Initializing I2C device library." << std::endl;

	I2Cdev::initialize();

	MQ  = new Quaternion();
	MFQ = new Quaternion();
	MBQ = new Quaternion();
	TBQ = new Quaternion();
	TCQ = new Quaternion();
	TDQ = new Quaternion();
	TEQ = new Quaternion();

	MV  = new Vector3D();
	MFV = new Vector3D();
	MBV = new Vector3D();
	TBV = new Vector3D();
	TCV = new Vector3D();
	TDV = new Vector3D();
	TEV = new Vector3D();

	MAO  = new Vector3D();
	MFAO = new Vector3D();
	MBAO = new Vector3D();
	TBAO = new Vector3D();
	TCAO = new Vector3D();
	TDAO = new Vector3D();
	TEAO = new Vector3D();

	MGO  = new Vector3D();
	MFGO = new Vector3D();
	MBGO = new Vector3D();
	TBGO = new Vector3D();
	TCGO = new Vector3D();
	TDGO = new Vector3D();
	TEGO = new Vector3D();

	MALS  = new VectorLeastSquares(500);
	MFALS = new VectorLeastSquares(500);
	MBALS = new VectorLeastSquares(500);
	TBALS = new VectorLeastSquares(500);
	TCALS = new VectorLeastSquares(500);
	TDALS = new VectorLeastSquares(500);
	TEALS = new VectorLeastSquares(500);
	
	MGLS  = new VectorLeastSquares(20);
	MFGLS = new VectorLeastSquares(20);
	MBGLS = new VectorLeastSquares(20);
	TBGLS = new VectorLeastSquares(20);
	TCGLS = new VectorLeastSquares(20);
	TDGLS = new VectorLeastSquares(20);
	TEGLS = new VectorLeastSquares(20);
}

I2CController::~I2CController() {
	std::cout << "Resetting hardware and clearing memory." << std::endl;

	SelectDevice(PWMManager);//PWMManager

	SetBThrustVector(Vector3D(0, 0, 0));
	SetCThrustVector(Vector3D(0, 0, 0));
	SetDThrustVector(Vector3D(0, 0, 0));
	SetEThrustVector(Vector3D(0, 0, 0));

	bcm2835_delay(500);

	hPWM->Restart();

	bcm2835_delay(10);

	hPWM->Sleep();

	bcm2835_delay(10);

	delete mpuMain;
	delete mpuFMain;
	delete mpuBMain;
	delete mpuB;
	delete mpuC;
	delete mpuD;
	delete mpuE;

	delete MQ;
	delete MFQ;
	delete MBQ;
	delete TBQ;
	delete TCQ;
	delete TDQ;
	delete TEQ;

	delete MV;
	delete MFV;
	delete MBV;
	delete TBV;
	delete TCV;
	delete TDV;
	delete TEV;

	delete MALS;
	delete MFALS;
	delete MBALS;
	delete TBALS;
	delete TCALS;
	delete TDALS;
	delete TEALS;

	delete MGLS;
	delete MFGLS;
	delete MBGLS;
	delete TBGLS;
	delete TCGLS;
	delete TDGLS;
	delete TEGLS;

	delete hPWM;
}

void I2CController::InitializeMPUs() {
	std::cout << "Creating MPU objects." << std::endl;

	mpuMain  = new MPU6050(0x68);
	mpuFMain = new MPU6050(0x68);
	mpuBMain = new MPU6050(0x68);
	mpuB     = new MPU6050(0x68);
	mpuC     = new MPU6050(0x68);
	mpuD     = new MPU6050(0x68);
	mpuE     = new MPU6050(0x68);

	std::cout << "Initializing MPUs." << std::endl;

	packetSizeM  = InitializeMPU(MainMPU,   mpuMain);
	packetSizeF  = InitializeMPU(MainFMPU, mpuFMain);
	packetSizeB  = InitializeMPU(MainBMPU, mpuBMain);
	packetSizeTB = InitializeMPU(ThrusterBMPU, mpuB);
	packetSizeTC = InitializeMPU(ThrusterCMPU, mpuC);
	packetSizeTD = InitializeMPU(ThrusterDMPU, mpuD);
	packetSizeTE = InitializeMPU(ThrusterEMPU, mpuE);

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
	if (dev == MainFMPU) {
		addr = 1 << 7;//1 * 2^i
	}
	if (dev == MainBMPU) {
		addr = 1 << 6;//1 * 2^i
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
}

void I2CController::SetDefaultMPUOffsets() {
	std::cout << "Setting default calibration offsets." << std::endl;

	SelectDevice(MainMPU);
	mpuMain->setXAccelOffset(0); mpuMain->setYAccelOffset(0); mpuMain->setZAccelOffset(0);
	mpuMain->setXGyroOffset(0);  mpuMain->setYGyroOffset(0);  mpuMain->setZGyroOffset(0);

	SelectDevice(MainFMPU);
	mpuFMain->setXAccelOffset(0); mpuFMain->setYAccelOffset(0); mpuFMain->setZAccelOffset(0);
	mpuFMain->setXGyroOffset(0);  mpuFMain->setYGyroOffset(0);  mpuFMain->setZGyroOffset(0);

	SelectDevice(MainBMPU);
	mpuBMain->setXAccelOffset(0); mpuBMain->setYAccelOffset(0); mpuBMain->setZAccelOffset(0);
	mpuBMain->setXGyroOffset(0);  mpuBMain->setYGyroOffset(0);  mpuBMain->setZGyroOffset(0);

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

void I2CController::ClearMPUFIFOs() {
	SelectDevice(MainMPU);
	mpuMain->resetFIFO();

	SelectDevice(MainFMPU);
	mpuFMain->resetFIFO();

	SelectDevice(MainBMPU);
	mpuBMain->resetFIFO();

	SelectDevice(ThrusterBMPU);
	mpuB->resetFIFO();

	SelectDevice(ThrusterCMPU);
	mpuC->resetFIFO();

	SelectDevice(ThrusterDMPU);
	mpuD->resetFIFO();

	SelectDevice(ThrusterEMPU);
	mpuE->resetFIFO();
}

void I2CController::CalibrateMPUs() {
	auto startTime = std::chrono::system_clock::now();
	double runTime = 0;

	std::cout << "Calibrating MPU offsets." << std::endl;

	int secondPrint = 0;
	int calTime = 5;

	while (runTime < calTime) {
		runTime = ((double)((std::chrono::system_clock::now() - startTime).count()) / pow(10.0, 9.0));

		double m, f, b, tb, tc, td, te;

		m = CalibrateMPU(MainMPU,   mpuMain, MAO,  MGO,  MALS,  MGLS );
		f = CalibrateMPU(MainFMPU, mpuFMain, MFAO, MFGO, MFALS, MFGLS);
		b = CalibrateMPU(MainBMPU, mpuBMain, MBAO, MBGO, MBALS, MBGLS);
		tb = CalibrateMPU(ThrusterBMPU, mpuB, TBAO, TBGO, TBALS, TBGLS);
		tc = CalibrateMPU(ThrusterCMPU, mpuC, TCAO, TCGO, TCALS, TCGLS);
		td = CalibrateMPU(ThrusterDMPU, mpuD, TDAO, TDGO, TDALS, TDGLS);
		te = CalibrateMPU(ThrusterEMPU, mpuE, TEAO, TEGO, TEALS, TEGLS);

		if (runTime > secondPrint) {
			std::cout << " MPU calibration step " << secondPrint << " of " << calTime << std::endl;
			std::cout << "   MPU AVG Offsets: " << (int)std::abs(m) << ", " << (int)std::abs(f) << ", " 
					  << (int)std::abs(b) << ", " << (int)std::abs(tb) << ", " 
				      << (int)std::abs(tc) << ", " << (int)std::abs(td) << ", " 
				      << (int)std::abs(te) << std::endl;
			secondPrint++;
		}
	}

	std::cout << "MPU offsets calibrated." << std::endl;
}

double I2CController::CalibrateMPU(Device dev, MPU6050 *mpu, Vector3D *ao, Vector3D *go, VectorLeastSquares *als, VectorLeastSquares *gls) {
	SelectDevice(dev);

	double avgOffset;

	int16_t ax, ay, az, gx, gy, gz;

	mpu->getMotion6(&ax, &ay, &az, &gx, &gy, &gz);

	avgOffset = (ax + ay + (az - 16384) + gx + gy + gz) / 6;

	ao = new Vector3D(mpu->getXAccelOffset(), mpu->getYAccelOffset(), mpu->getZAccelOffset());
	go = new Vector3D(mpu->getXGyroOffset(),  mpu->getYGyroOffset(),  mpu->getZGyroOffset());

	als->Calculate(Vector3D(ax, ay, az), *ao, Vector3D(0, 0, 16384));
	gls->Calculate(Vector3D(gx, gy, gz), *go, Vector3D(0, 0, 0));

	//linear, wastes time
	if (ax > 0)      ao->X = ao->X - 1; else if (ax < 0)      ao->X = ao->X + 1;
	if (ay > 0)      ao->Y = ao->Y - 1; else if (ay < 0)      ao->Y = ao->Y + 1;
	if (az > 16384)  ao->Z = ao->Z - 1; else if (az < 16384)  ao->Z = ao->Z + 1;

	if (gx > 0)     go->X = go->X - 1; else if (gx < 0)     go->X = go->X + 1;
	if (gy > 0)     go->Y = go->Y - 1; else if (gy < 0)     go->Y = go->Y + 1;
	if (gz > 0)     go->Z = go->Z - 1; else if (gz < 0)     go->Z = go->Z + 1;

	mpu->setXAccelOffset(ao->X);
	mpu->setYAccelOffset(ao->Y);
	mpu->setZAccelOffset(ao->Z);
	mpu->setXGyroOffset(go->X);
	mpu->setYGyroOffset(go->Y);
	mpu->setZGyroOffset(go->Z);

	bcm2835_delay(1);

	mpu->getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
	ao = new Vector3D(mpu->getXAccelOffset(), mpu->getYAccelOffset(), mpu->getZAccelOffset());
	go = new Vector3D(mpu->getXGyroOffset(), mpu->getYGyroOffset(), mpu->getZGyroOffset());

	Vector3D alsOffset = als->Calculate(Vector3D(ax, ay, az), *ao, Vector3D(0, 0, 16384));
	Vector3D glsOffset = gls->Calculate(Vector3D(gx, gy, gz), *go, Vector3D(0, 0, 0));

	mpu->setXAccelOffset(alsOffset.X);
	mpu->setYAccelOffset(alsOffset.Y);
	mpu->setZAccelOffset(alsOffset.Z);
	mpu->setXGyroOffset( glsOffset.X);
	mpu->setYGyroOffset( glsOffset.Y);
	mpu->setZGyroOffset( glsOffset.Z);

	//std::cout << "Gyro:" << Vector3D(gx, gy, gz).ToString() << " Mod:" << glsOffset.ToString() << std::endl;
	//std::cout << "Accel:" << Vector3D(ax, ay, az).ToString() << " Mod:" << alsOffset.ToString() << std::endl;

	return avgOffset;
}

double I2CController::CalibrateMPU(Device dev, MPU9150 *mpu, Vector3D *ao, Vector3D *go, VectorLeastSquares *als, VectorLeastSquares *gls) {
	/*
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
	*/
}

uint16_t I2CController::InitializeMPU(Device dev, MPU6050 *mpu) {
	int dmpStatus;

	std::cout << "Initializing MPU6050 device: " << dev << std::endl;

	SelectDevice(dev);
	std::cout << "   Resetting MPU6050 before initialization." << std::endl;
	mpu->reset();

	bcm2835_delay(50);

	mpu->resetDMP();
	mpu->resetSensors();

	bcm2835_delay(50);

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
	
	if (dmpStatus == 0) {
		std::cout << "      Enabling DMP of MPU6050" << std::endl;

		mpu->setDMPEnabled(true);

		uint16_t val = mpu->dmpGetFIFOPacketSize();
		std::cout << "      DMP Packet Size:" << val << std::endl;

		return val;
	}
	else {
		std::cout << "      DMP Initialization Failed on " << dev << std::endl;

		return 0;
	}
}

uint16_t I2CController::InitializeMPU(Device dev, MPU9150 *mpu) {
	int dmpStatus;

	std::cout << "Initializing MPU9150 device: " << dev << std::endl;

	SelectDevice(dev);
	std::cout << "   Resetting MPU9150 before initialization." << std::endl;
	mpu->reset();

	bcm2835_delay(50);

	mpu->resetDMP();
	mpu->resetSensors();

	bcm2835_delay(50);

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

	if (dmpStatus == 0) {
		std::cout << "      Enabling DMP of MPU9150" << std::endl;

		mpu->setDMPEnabled(true);

		return mpu->dmpGetFIFOPacketSize();
	}
	else {
		std::cout << "      DMP Initialization Failed on " << dev << std::endl;

		return 0;
	}
}

Quaternion I2CController::GetRotation(Device dev, MPU6050 *mpu, uint16_t packetSize) {
	SelectDevice(dev);

	//std::cout << "Getting mpu6050 status." << std::endl;
	int mpuIntStatus = mpu->getIntStatus();

	//std::cout << "Getting mpu6050 fifo count." << std::endl;
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	//std::cout << "FIFO count: " << fifoCount << " Packet:" << packetSize << std::endl;

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		std::cout << "MPU6050 FIFO reset on device:" << dev << std::endl;

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

Quaternion I2CController::GetRotation(Device dev, MPU9150 *mpu, uint16_t packetSize) {
	SelectDevice(dev);

	//std::cout << "Getting mpu9150 status." << std::endl;
	int mpuIntStatus = mpu->getIntStatus();

	//std::cout << "Getting mpu9150 fifo count." << std::endl;
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	//std::cout << "FIFO count: " << fifoCount << std::endl;

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		std::cout << "MPU9150 FIFO reset on device:" << dev << std::endl;

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
	Quaternion q = GetRotation(MainMPU, mpuMain, packetSizeM);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		//std::cout << "Main R fetch failed, using previous values." << std::endl;
		return Quaternion(MQ->W, MQ->X, MQ->Y, MQ->Z);
	}
	else {
		MQ = new Quaternion(q);

		return q;
	}
}

Quaternion I2CController::GetMainFRotation() {
	Quaternion q = GetRotation(MainFMPU, mpuFMain, packetSizeF);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		//std::cout << "Main R fetch failed, using previous values." << std::endl;
		return Quaternion(MFQ->W, MFQ->X, MFQ->Y, MFQ->Z);
	}
	else {
		MFQ = new Quaternion(q);

		return q;
	}
}

Quaternion I2CController::GetMainBRotation() {
	Quaternion q = GetRotation(MainBMPU, mpuBMain, packetSizeB);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		//std::cout << "Main R fetch failed, using previous values." << std::endl;
		return Quaternion(MBQ->W, MBQ->X, MBQ->Y, MBQ->Z);
	}
	else {
		MBQ = new Quaternion(q);

		return q;
	}
}

Quaternion I2CController::GetTBRotation() {
	Quaternion q = GetRotation(ThrusterBMPU, mpuB, packetSizeTB);

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
	Quaternion q = GetRotation(ThrusterCMPU, mpuC, packetSizeTC);

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
	Quaternion q = GetRotation(ThrusterDMPU, mpuD, packetSizeTD);

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
	Quaternion q = GetRotation(ThrusterEMPU, mpuE, packetSizeTE);

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		//std::cout << "TE R fetch failed, using previous values." << std::endl;
		return Quaternion(TEQ->W, TEQ->X, TEQ->Y, TEQ->Z);
	}
	else {
		TEQ = new Quaternion(q);

		return q;
	}
}

VectorInt16 I2CController::GetGyro(Device dev, MPU6050 *mpu, uint16_t packetSize) {
	SelectDevice(dev);

	int mpuIntStatus = mpu->getIntStatus();
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		std::cout << "MPU6050 FIFO reset on device:" << dev << std::endl;

		return VectorInt16(-1, -1, -1);
	}
	else if (mpuIntStatus & 0x02) {
		while (fifoCount < packetSize) {
			fifoCount = mpu->getFIFOCount();
		}

		mpu->getFIFOBytes(fifoBuffer, packetSize);

		fifoCount -= packetSize;

		VectorInt16 v = VectorInt16();

		mpu->dmpGetGyro(&v, fifoBuffer);

		return v;
	}
	else {
		return VectorInt16(-1, -1, -1);
	}
}

VectorInt16 I2CController::GetAccel(Device dev, MPU6050 *mpu, uint16_t packetSize) {
	SelectDevice(dev);

	int mpuIntStatus = mpu->getIntStatus();
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		std::cout << "MPU6050 FIFO reset on device:" << dev << std::endl;

		return VectorInt16(-1, -1, -1);
	}
	else if (mpuIntStatus & 0x02) {
		while (fifoCount < packetSize) {
			fifoCount = mpu->getFIFOCount();
		}

		mpu->getFIFOBytes(fifoBuffer, packetSize);

		fifoCount -= packetSize;

		VectorInt16 v = VectorInt16();

		mpu->dmpGetAccel(&v, fifoBuffer);

		return v;
	}
	else {
		return VectorInt16(-1, -1, -1);
	}
}

Vector3D I2CController::GetLinearAcceleration(Device dev, MPU6050 *mpu, uint16_t packetSize) {
	SelectDevice(dev);

	int mpuIntStatus = mpu->getIntStatus();
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; //FIFO storage buffer

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		std::cout << "MPU6050 FIFO reset on device:" << dev << std::endl;

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
		if (dev == MainFMPU) {
			q = QuaternionFloat(MFQ->W, MFQ->X, MFQ->Y, MFQ->Z);
		}
		if (dev == MainBMPU) {
			q = QuaternionFloat(MBQ->W, MBQ->X, MBQ->Y, MBQ->Z);
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
		gAccel = gAccel / 16384;//Scaling factor from 8192+/- to 

		return gAccel;
	}
	else {
		return Vector3D(-1000, -1000, -1000);
	}
}

Vector3D I2CController::GetLinearAcceleration(Device dev, MPU9150 *mpu, uint16_t packetSize) {
	SelectDevice(dev);

	int mpuIntStatus = mpu->getIntStatus();
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		std::cout << "MPU9150 FIFO reset on device:" << dev << std::endl;

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
		if (dev == MainFMPU) {
			q = QuaternionFloat(MFQ->W, MFQ->X, MFQ->Y, MFQ->Z);
		}
		if (dev == MainBMPU) {
			q = QuaternionFloat(MBQ->W, MBQ->X, MBQ->Y, MBQ->Z);
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
		gAccel = gAccel / 16384;//Scaling factor from 8192+/- to 

		return gAccel;
	}
	else {
		return Vector3D(-1000, -1000, -1000);
	}
}

Vector3D I2CController::GetMainWorldAcceleration() {
	Vector3D v = GetLinearAcceleration(MainMPU, mpuMain, packetSizeM);

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

Vector3D I2CController::GetMainFWorldAcceleration() {
	Vector3D v = GetLinearAcceleration(MainFMPU, mpuFMain, packetSizeF);

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		//std::cout << "Main WA fetch failed, using previous values." << std::endl;
		return Vector3D(MFV->X, MFV->Y, MFV->Z);
	}
	else {
		v = MFQ->RotateVector(v);

		MFV = new Vector3D(v);

		return v;
	}
}

Vector3D I2CController::GetMainBWorldAcceleration() {
	Vector3D v = GetLinearAcceleration(MainBMPU, mpuBMain, packetSizeB);

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		//std::cout << "Main WA fetch failed, using previous values." << std::endl;
		return Vector3D(MBV->X, MBV->Y, MBV->Z);
	}
	else {
		v = MBQ->RotateVector(v);

		MBV = new Vector3D(v);

		return v;
	}
}

Vector3D I2CController::GetTBWorldAcceleration() {
	Vector3D v = GetLinearAcceleration(ThrusterBMPU, mpuB, packetSizeTB);

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
	Vector3D v = GetLinearAcceleration(ThrusterCMPU, mpuC, packetSizeTC);

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
	Vector3D v = GetLinearAcceleration(ThrusterDMPU, mpuD, packetSizeTD);

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
	Vector3D v = GetLinearAcceleration(ThrusterEMPU, mpuE, packetSizeTE);

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

double I2CController::GetTemperature(Device dev) {
	SelectDevice(dev);

	uint16_t t = mpuMain->getTemperature();

	return ((double)t) / 340.0 + 36.53;
}

