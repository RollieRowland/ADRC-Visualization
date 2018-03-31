#include <wiringPi.h>

#define	LED	17

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

	wiringPiSetupSys();

	pinMode(LED, OUTPUT);

	while (true)
	{
		digitalWrite(LED, HIGH);  // On
		delay(500); // ms
		digitalWrite(LED, LOW);	  // Off
		delay(500);
	}
	return 0;
}