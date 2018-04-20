#include "I2CController.h"

I2CController::I2CController(int addr) {
	this->address = addr;

	std::cout << "Initializing I2C device library." << std::endl;

	I2Cdev::initialize();

	mpuM  = new MPUController(new MPU6050(0x68));
	mpuF  = new MPUController(new MPU6050(0x68));
	mpuB  = new MPUController(new MPU6050(0x68));
	mpuTB = new MPUController(new MPU6050(0x68));
	mpuTC = new MPUController(new MPU6050(0x68));
	mpuTD = new MPUController(new MPU6050(0x68));
	mpuTE = new MPUController(new MPU6050(0x68));
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

	delete hPWM;
}

void I2CController::InitializeMPUs() {
	std::cout << "Initializing MPUs." << std::endl;

	SelectDevice(MainMPU);
	mpuM->Initialize();
	SelectDevice(MainFMPU);
	mpuF->Initialize();
	SelectDevice(MainBMPU);
	mpuB->Initialize();
	SelectDevice(ThrusterBMPU);
	mpuTB->Initialize();
	SelectDevice(ThrusterCMPU);
	mpuTC->Initialize();
	SelectDevice(ThrusterDMPU);
	mpuTD->Initialize();
	SelectDevice(ThrusterEMPU);
	mpuTE->Initialize();

	std::cout << "MPUs initialized." << std::endl;
}

void I2CController::InitializePCA() {
	SelectDevice(PWMManager);//PWMManager

	hPWM = new PWMController(200, 0x40);
}

void I2CController::SelectDevice(Device dev) {
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
	mpuM->SetDefaultMPUOffsets();

	SelectDevice(MainFMPU);
	mpuF->SetDefaultMPUOffsets();

	SelectDevice(MainBMPU);
	mpuB->SetDefaultMPUOffsets();

	SelectDevice(ThrusterBMPU);
	mpuTB->SetDefaultMPUOffsets();

	SelectDevice(ThrusterCMPU);
	mpuTC->SetDefaultMPUOffsets();

	SelectDevice(ThrusterDMPU);
	mpuTD->SetDefaultMPUOffsets();

	SelectDevice(ThrusterEMPU);
	mpuTE->SetDefaultMPUOffsets();
}

void I2CController::ClearMPUFIFOs() {
	SelectDevice(MainMPU);
	mpuM->ClearMPUFIFO();

	SelectDevice(MainFMPU);
	mpuF->ClearMPUFIFO();

	SelectDevice(MainBMPU);
	mpuB->ClearMPUFIFO();

	SelectDevice(ThrusterBMPU);
	mpuTB->ClearMPUFIFO();

	SelectDevice(ThrusterCMPU);
	mpuTC->ClearMPUFIFO();

	SelectDevice(ThrusterDMPU);
	mpuTD->ClearMPUFIFO();

	SelectDevice(ThrusterEMPU);
	mpuTE->ClearMPUFIFO();
}

void I2CController::CalibrateMPUs() {
	auto startTime = std::chrono::system_clock::now();
	double runTime = 0;

	std::cout << "Calibrating MPU offsets." << std::endl;

	int secondPrint = 0;
	int calTime = 10;
	bool first = true;

	while (runTime < calTime) {
		runTime = ((double)((std::chrono::system_clock::now() - startTime).count()) / pow(10.0, 9.0));

		int m, f, b, tb, tc, td, te;

		SelectDevice(MainMPU);
		m  = mpuM->CalibrateMPU(first);
		SelectDevice(MainFMPU);
		f  = mpuB->CalibrateMPU(first);
		SelectDevice(MainBMPU);
		b  = mpuF->CalibrateMPU(first);
		SelectDevice(ThrusterBMPU);
		tb = mpuTB->CalibrateMPU(first);
		SelectDevice(ThrusterCMPU);
		tc = mpuTC->CalibrateMPU(first);
		SelectDevice(ThrusterDMPU);
		td = mpuTD->CalibrateMPU(first);
		SelectDevice(ThrusterEMPU);
		te = mpuTE->CalibrateMPU(first);

		if (runTime > secondPrint) {
			std::cout << " MPU calibration step " << secondPrint << " of " << calTime << std::endl;
			std::cout << "   MPU AVG Offsets: " << m << ", " << f << ", " 
					  << b << ", " << tb << ", " << tc << ", " << td << ", " 
				      << te << std::endl;
			secondPrint++;
		}

		if (first) {
			first = false;
		}
	}

	std::cout << "MPU offsets calibrated." << std::endl;
}

Quaternion I2CController::GetMainRotation() {
	SelectDevice(MainMPU);
	Quaternion q = mpuM->GetRotation();

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(mpuM->GetPreviousRotation());
	}
	else {
		return q;
	}
}

Quaternion I2CController::GetMainFRotation() {
	SelectDevice(MainFMPU);
	Quaternion q = mpuF->GetRotation();

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(mpuF->GetPreviousRotation());
	}
	else {
		return q;
	}
}

Quaternion I2CController::GetMainBRotation() {
	SelectDevice(MainBMPU);
	Quaternion q = mpuB->GetRotation();

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(mpuB->GetPreviousRotation());
	}
	else {
		return q;
	}
}

Quaternion I2CController::GetTBRotation() {
	SelectDevice(ThrusterBMPU);
	Quaternion q = mpuTB->GetRotation();

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(mpuTB->GetPreviousRotation());
	}
	else {
		return q;
	}
}

Quaternion I2CController::GetTCRotation() {
	SelectDevice(ThrusterCMPU);
	Quaternion q = mpuTC->GetRotation();

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(mpuTC->GetPreviousRotation());
	}
	else {
		return q;
	}
}

Quaternion I2CController::GetTDRotation() {
	SelectDevice(ThrusterDMPU);
	Quaternion q = mpuTD->GetRotation();

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(mpuTD->GetPreviousRotation());
	}
	else {
		return q;
	}
}

Quaternion I2CController::GetTERotation() {
	SelectDevice(ThrusterEMPU);
	Quaternion q = mpuTE->GetRotation();

	if (q.W == -1 && q.X == -1 && q.Y == -1 && q.Z == -1) {
		return Quaternion(mpuTE->GetPreviousRotation());
	}
	else {
		return q;
	}
}

Vector3D I2CController::GetMainWorldAcceleration() {
	SelectDevice(MainMPU);
	Vector3D v = mpuM->GetLinearAcceleration();

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		return mpuM->GetPreviousAcceleration();
	}
	else {
		v = mpuM->GetPreviousRotation().RotateVector(v);

		return v;
	}
}

Vector3D I2CController::GetMainFWorldAcceleration() {
	SelectDevice(MainFMPU);
	Vector3D v = mpuF->GetLinearAcceleration();

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		return mpuF->GetPreviousAcceleration();
	}
	else {
		v = mpuF->GetPreviousRotation().RotateVector(v);

		return v;
	}
}

Vector3D I2CController::GetMainBWorldAcceleration() {
	SelectDevice(MainBMPU);
	Vector3D v = mpuB->GetLinearAcceleration();

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		return mpuB->GetPreviousAcceleration();
	}
	else {
		v = mpuB->GetPreviousRotation().RotateVector(v);

		return v;
	}
}

Vector3D I2CController::GetTBWorldAcceleration() {
	SelectDevice(ThrusterBMPU);
	Vector3D v = mpuTB->GetLinearAcceleration();

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		return mpuTB->GetPreviousAcceleration();
	}
	else {
		v = mpuTB->GetPreviousRotation().RotateVector(v);

		return v;
	}
}

Vector3D I2CController::GetTCWorldAcceleration() {
	SelectDevice(ThrusterCMPU);
	Vector3D v = mpuTC->GetLinearAcceleration();

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		return mpuTC->GetPreviousAcceleration();
	}
	else {
		v = mpuTC->GetPreviousRotation().RotateVector(v);

		return v;
	}
}

Vector3D I2CController::GetTDWorldAcceleration() {
	SelectDevice(ThrusterDMPU);
	Vector3D v = mpuTD->GetLinearAcceleration();

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		return mpuTD->GetPreviousAcceleration();
	}
	else {
		v = mpuTD->GetPreviousRotation().RotateVector(v);

		return v;
	}
}

Vector3D I2CController::GetTEWorldAcceleration() {
	SelectDevice(ThrusterEMPU);
	Vector3D v = mpuTE->GetLinearAcceleration();

	if (v.X == -1000 && v.Y == -1000 && v.Z == -1000) {
		return mpuTE->GetPreviousAcceleration();
	}
	else {
		v = mpuTE->GetPreviousRotation().RotateVector(v);

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
