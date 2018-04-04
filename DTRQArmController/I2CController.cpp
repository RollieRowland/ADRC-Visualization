#include "I2CController.h"

I2CController::I2CController() {

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
