#include "I2CController.h"

I2CController::I2CController() {
	SelectDevice(Main);
	mpuMain = new MPU6050(0x70);
	mpuMain->testConnection();
	mpuMain->initialize();
	mpuMain->dmpin

	mpuB = new MPU6050(0x70);
	mpuC = new MPU6050(0x70);
	mpuD = new MPU6050(0x70);
	mpuE = new MPU6050(0x70);
}

void I2CController::SelectDevice(MPU mpu) {
	if (mpu == Main) {
		int addr = 1 << 0;//1 * 2^i
		char buf[4];

		sprintf(buf, "%d", addr);

		bcm2835_i2c_write(buf, 4);
	}
	else if (mpu = ThrusterB) {

	}
	else if (mpu = ThrusterC) {

	}
	else if (mpu = ThrusterD) {

	}
	else if (mpu = ThrusterE) {

	}
}
