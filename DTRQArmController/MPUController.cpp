#include "MPUController.h"

MPUController::MPUController(MPU *mpu) {
	this->mpu = mpu;

	rotation = new Quaternion();
	acceleration = new Vector3D();
	accelerationOffset = new Vector3D();
	gyroscopeOffset = new Vector3D();

	acceKF = new VectorKalmanFilter(0.9, 4);
	gyroKF = new VectorKalmanFilter(0.9, 4);
	acceOKF = new VectorKalmanFilter(0.9, 4);
	gyroOKF = new VectorKalmanFilter(0.9, 4);
	acceLS = new VectorLeastSquares(500);
	gyroLS = new VectorLeastSquares(20);
}

void MPUController::Initialize() {
	int dmpStatus;

	std::cout << "   Resetting MPU before initialization." << std::endl;
	mpu->reset();

	bcm2835_delay(50);

	mpu->resetDMP();
	mpu->resetSensors();

	bcm2835_delay(50);

	std::cout << "   Testing I2C connection to MPU." << std::endl;

	int id = (int)mpu->getDeviceID();

	std::stringstream sstream;
	sstream << std::hex << id;
	std::string result = sstream.str();
	std::string name = "MPU6050";

	int addrExp = 0x34;

	if (id == 0x39) {
		name = "MPU9150";
		addrExp = 0x34;
	}

	std::cout << "   Device ID: " << std::hex << result << " Expected: " << addrExp << std::dec << std::endl;

	if (mpu->testConnection()) {
		std::cout << "      " << name << " connection test successful." << std::endl;
	}
	else {
		std::cout << "      " << name << " connection test failed." << std::endl;
	}

	mpu->initialize();

	std::cout << "   Initializing DMP." << std::endl;

	dmpStatus = mpu->dmpInitialize();

	if (dmpStatus == 0) {
		std::cout << "      Enabling DMP of " << name << std::endl;

		mpu->setDMPEnabled(true);

		packetSize = mpu->dmpGetFIFOPacketSize();
	}
	else {
		std::cout << "      DMP Initialization Failed." << std::endl;

		packetSize = 42;
	}
}

void MPUController::ClearMPUFIFO() {
	mpu->resetFIFO();
}

double MPUController::GetTemperature() {
	uint16_t t = mpu->getTemperature();

	return ((double)t) / 340.0 + 36.53;
}

int MPUController::CalibrateMPU(bool first) {
	int avgOffset;
	int16_t ax, ay, az, gx, gy, gz;
	Vector3D a;
	Vector3D g;

	if (first) {

		mpu->getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
		a = Vector3D(acceKF->Filter(Vector3D(ax, ay, az)));
		g = Vector3D(gyroKF->Filter(Vector3D(gx, gy, gz)));

		//get mpu offsets
		accelerationOffset = new Vector3D(acceOKF->Filter( Vector3D(mpu->getXAccelOffset(), mpu->getYAccelOffset(), mpu->getZAccelOffset()) ));
		gyroscopeOffset = new Vector3D(gyroOKF->Filter( Vector3D(mpu->getXGyroOffset(), mpu->getYGyroOffset(), mpu->getZGyroOffset()) ));

		//calculate least squares of target offset
		acceLS->Calculate(a, *accelerationOffset, Vector3D(0, 0, 16384));
		gyroLS->Calculate(g, *gyroscopeOffset, Vector3D(0, 0, 0));

		//linear, wastes time - used to initialize offset 
		if (a.X > 0)      accelerationOffset->X = accelerationOffset->X - 5; else if (a.X < 0)      accelerationOffset->X = accelerationOffset->X + 5;
		if (a.Y > 0)      accelerationOffset->Y = accelerationOffset->Y - 5; else if (a.Y < 0)      accelerationOffset->Y = accelerationOffset->Y + 5;
		if (a.Z > 16384)  accelerationOffset->Z = accelerationOffset->Z - 5; else if (a.Z < 16384)  accelerationOffset->Z = accelerationOffset->Z + 5;

		if (g.X > 0)     gyroscopeOffset->X = gyroscopeOffset->X - 5; else if (g.X < 0)     gyroscopeOffset->X = gyroscopeOffset->X + 5;
		if (g.Y > 0)     gyroscopeOffset->Y = gyroscopeOffset->Y - 5; else if (g.Y < 0)     gyroscopeOffset->Y = gyroscopeOffset->Y + 5;
		if (g.Z > 0)     gyroscopeOffset->Z = gyroscopeOffset->Z - 5; else if (g.Z < 0)     gyroscopeOffset->Z = gyroscopeOffset->Z + 5;

		mpu->setXAccelOffset(accelerationOffset->X);
		mpu->setYAccelOffset(accelerationOffset->Y);
		mpu->setZAccelOffset(accelerationOffset->Z);
		mpu->setXGyroOffset(gyroscopeOffset->X);
		mpu->setYGyroOffset(gyroscopeOffset->Y);
		mpu->setZGyroOffset(gyroscopeOffset->Z);

		bcm2835_delay(1);
	}

	mpu->getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
	a = Vector3D(acceKF->Filter(Vector3D(ax, ay, az)));
	g = Vector3D(gyroKF->Filter(Vector3D(gx, gy, gz)));

	//get mpu offsets
	accelerationOffset = new Vector3D(acceOKF->Filter(Vector3D(mpu->getXAccelOffset(), mpu->getYAccelOffset(), mpu->getZAccelOffset())));
	gyroscopeOffset = new Vector3D(gyroOKF->Filter(Vector3D(mpu->getXGyroOffset(), mpu->getYGyroOffset(), mpu->getZGyroOffset())));

	//linear, wastes time - used to initialize offset 
	if (a.X > 0)      accelerationOffset->X = accelerationOffset->X - 5; else if (a.X < 0)      accelerationOffset->X = accelerationOffset->X + 5;
	if (a.Y > 0)      accelerationOffset->Y = accelerationOffset->Y - 5; else if (a.Y < 0)      accelerationOffset->Y = accelerationOffset->Y + 5;
	if (a.Z > 16384)  accelerationOffset->Z = accelerationOffset->Z - 5; else if (a.Z < 16384)  accelerationOffset->Z = accelerationOffset->Z + 5;

	if (g.X > 0)     gyroscopeOffset->X = gyroscopeOffset->X - 5; else if (g.X < 0)     gyroscopeOffset->X = gyroscopeOffset->X + 5;
	if (g.Y > 0)     gyroscopeOffset->Y = gyroscopeOffset->Y - 5; else if (g.Y < 0)     gyroscopeOffset->Y = gyroscopeOffset->Y + 5;
	if (g.Z > 0)     gyroscopeOffset->Z = gyroscopeOffset->Z - 5; else if (g.Z < 0)     gyroscopeOffset->Z = gyroscopeOffset->Z + 5;

	//calculate least squares of target offset on each sensor axis
	Vector3D alsOffset = acceLS->Calculate(a, *accelerationOffset, Vector3D(0, 0, 16384));
	Vector3D glsOffset = gyroLS->Calculate(g, *gyroscopeOffset, Vector3D(0, 0, 0));

	mpu->setXAccelOffset(alsOffset.X);
	mpu->setYAccelOffset(alsOffset.Y);
	mpu->setZAccelOffset(alsOffset.Z);
	mpu->setXGyroOffset(glsOffset.X);
	mpu->setYGyroOffset(glsOffset.Y);
	mpu->setZGyroOffset(glsOffset.Z);

	//std::cout << "Gyro:" << Vector3D(gx, gy, gz).ToString() << " Mod:" << glsOffset.ToString() << std::endl;
	//std::cout << "Accel:" << Vector3D(ax, ay, az).ToString() << " Mod:" << alsOffset.ToString() << std::endl;

	avgOffset = (std::abs(a.X) + std::abs(a.Y) + std::abs((a.Z - 16384)) + std::abs(g.X) + std::abs(g.Y) + std::abs(g.Z)) / 6;

	return avgOffset;
}

void MPUController::SetDefaultMPUOffsets() {
	mpu->setXAccelOffset(0); mpu->setYAccelOffset(0); mpu->setZAccelOffset(0);
	mpu->setXGyroOffset(0);  mpu->setYGyroOffset(0);  mpu->setZGyroOffset(0);
}

Quaternion MPUController::GetRotation() {
	int mpuIntStatus = mpu->getIntStatus();
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		std::cout << "MPU FIFO reset on device" << std::endl;

		return Quaternion(-1, -1, -1, -1);
	}
	else {//if (mpuIntStatus & 0x02)
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
}

Vector3D MPUController::GetLinearAcceleration() {
	int mpuIntStatus = mpu->getIntStatus();
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; //FIFO storage buffer

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		std::cout << "MPU FIFO reset on device" << std::endl;

		return Vector3D(-1000, -1000, -1000);
	}
	else {
		while (fifoCount < packetSize) {
			fifoCount = mpu->getFIFOCount();
		}

		mpu->getFIFOBytes(fifoBuffer, packetSize);

		fifoCount -= packetSize;

		QuaternionFloat q = QuaternionFloat();

	    q = QuaternionFloat(rotation->W, rotation->X, rotation->Y, rotation->Z);

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
}

VectorInt16 MPUController::GetGyro() {
	int mpuIntStatus = mpu->getIntStatus();
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		std::cout << "MPU FIFO reset on device" << std::endl;

		return VectorInt16(-1, -1, -1);
	}
	else {
		while (fifoCount < packetSize) {
			fifoCount = mpu->getFIFOCount();
		}

		mpu->getFIFOBytes(fifoBuffer, packetSize);

		fifoCount -= packetSize;

		VectorInt16 v = VectorInt16();

		mpu->dmpGetGyro(&v, fifoBuffer);

		return v;
	}
}

VectorInt16 MPUController::GetAccel() {
	int mpuIntStatus = mpu->getIntStatus();
	int fifoCount = mpu->getFIFOCount();
	uint8_t fifoBuffer[64]; // FIFO storage buffer

	if (mpuIntStatus & 0x10 || fifoCount == 1024) {
		mpu->resetFIFO();

		std::cout << "MPU FIFO reset on device" << std::endl;

		return VectorInt16(-1, -1, -1);
	}
	else {
		while (fifoCount < packetSize) {
			fifoCount = mpu->getFIFOCount();
		}

		mpu->getFIFOBytes(fifoBuffer, packetSize);

		fifoCount -= packetSize;

		VectorInt16 v = VectorInt16();

		mpu->dmpGetAccel(&v, fifoBuffer);

		return v;
	}
}

Quaternion MPUController::GetPreviousRotation() {
	return *rotation;
}

Vector3D MPUController::GetPreviousAcceleration() {
	return *acceleration;
}
