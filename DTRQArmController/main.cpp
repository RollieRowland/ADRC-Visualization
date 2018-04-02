

int main(void)
{
	//calculate dT estimate to be used as a constant - average

	//initialize sensors and wait

	//create quadcopter

	////////////
	//BEGIN LOOP
	//read sensors

	//ROTATION
	//calculate quat from previous servo positions rotation matrix, convert 
	//   to quaternion and subtract generated quaternion from arm MPUs
	//complementary filter of all individually KFed arm sensors
	//complementary filter arm with KFed center MPU 50/50
	
	//POSITION
	//calculate current velocity from complementary of all individually KFed accelerometers
	//calculate current position from velocity
	
	//calculate in quadcopter "library"
	//write outputs
	//END LOOP
	//////////

	//abstract i2c device
	//mpu 6050 derived from i2c device
	//mpu 9    derived from i2c device
	//magnetometer derived from i2c device
	//multiplexer contains multiple abstract i2c device instances (up to 8)
	//
	//

	return 0;
}