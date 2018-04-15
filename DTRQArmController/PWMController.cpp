#include "PWMController.h"

PWMController::PWMController(int frequency, uint8_t addr) {
	this->address = addr;
	this->frequency = frequency;

	std::cout << "Initializing PCA9685." << std::endl;

	Restart();
	Wake();

	SetAllPWM(0);

	WriteByte(MODE2, OUTDRV);
	WriteByte(MODE1, ALLCALL);

	delay(5);

	uint8_t m1;
	ReadByte(MODE1, &m1);
	m1 = m1 & ~SLEEP;//wake up
	WriteByte(MODE1, m1);

	delay(5);
	SetPWMFrequency(frequency);

	delay(20);

	//std::cout << "Not initializing ESCs." << std::endl;

	//InitializeESCs();
	std::cout << "Initialized PCA9685." << std::endl;
}

//No custom on time
void PWMController::SetPWMFrequency(int frequency) {
	std::cout << "   Setting PWM frequency." << std::endl;
	uint8_t prescaleValue = (int)((ClockFrequency / 4096.0 / (double)frequency) - 1.0);

	std::cout << "   Prescale value of: " << (int)prescaleValue << std::endl;

	uint8_t previousMode;
	uint8_t currentMode;
	ReadByte(MODE1, &previousMode);
	currentMode = (previousMode & 0x7F) | 0x10;//sleep

	WriteByte(MODE1, currentMode);//sleep
	WriteByte(PRESCALE, prescaleValue);//multiplier for PWM frequency
	WriteByte(MODE1, previousMode);//restart
	
	delay(5);

	WriteByte(MODE1, previousMode | 0x80);//totem pole
}

void PWMController::Sleep() {
	uint8_t m1;
	ReadByte(MODE1, &m1);

	m1 |= (1u << 4);//set 5th bit

	WriteByte(MODE1, m1);
	delay(10);
}

void PWMController::Wake() {
	uint8_t m1;
	ReadByte(MODE1, &m1);

	m1 |= (0u << 4);//set 5th bit

	WriteByte(MODE1, m1);
	delay(10);
}

void PWMController::Reset() {
	WriteByte(MODE1, 0x06);
	delay(10);
}

void PWMController::Restart() {
	uint8_t m1;
	ReadByte(MODE1, &m1);

	if ((m1 >> 7) & 1u) {
		m1 |= (0u << 4);//set 5th bit
		delay(10);//stabilize oscillator
	}

	m1 |= (1u << 7);//set 4th bit

	WriteByte(MODE1, m1);
	delay(10);
}

void PWMController::SetPWM(int device, int frequency) {
	WriteByte(device * 4 + PWM0_ON_L, 0 & 0xFF);//ON first 8
	WriteByte(device * 4 + PWM0_ON_H, 0 >> 8);//ON second 8

	WriteByte(device * 4 + PWM0_OFF_L, frequency & 0xFF);//OFF first 8
	WriteByte(device * 4 + PWM0_OFF_H, frequency >> 8);//Off second 8
}

void PWMController::SetAllPWM(int frequency) {
	WriteByte(ALL_PWM_ON_L, 0 & 0xFF);//ON first 8
	WriteByte(ALL_PWM_ON_H, 0 >> 8);//ON second 8

	WriteByte(ALL_PWM_OFF_L, frequency & 0xFF);//OFF first 8
	WriteByte(ALL_PWM_OFF_H, frequency >> 8);//Off second 8
}


void PWMController::WriteByte(uint8_t addr, uint8_t value) {
	I2Cdev::writeByte(this->address, addr, value);
}

void PWMController::ReadByte(uint8_t addr, uint8_t *value) {
	I2Cdev::readByte(this->address, addr, value);
}


int PWMController::CalculateServoFrequency(double angle) {
	angle = angle + 90;

	double ratio = angle / 180;

	if (ratio < 0) {
		ratio = 0;
	}
	else if (ratio > 1) {
		ratio = 1;
	}

	//servo range
	double cycleMS = 1000000.0 / this->frequency;
	double stepLengthMS = cycleMS / 4096;

	double pulseLengthMS = ratio * (MAXPWMMS - MINPWMMS) + MINPWMMS;//1 to 2 millis

	int pulseLengthSteps = (int)(pulseLengthMS / stepLengthMS) - 1;

	int off = pulseLengthSteps % 4096;

	return off;
}

int PWMController::CalculateRotorFrequency(double force) {
	//4g center -4g to +4g to 0 to 8g
	double ratio = force / 39.2;

	if (ratio < 0) {
		ratio = 0;
	}
	else if (ratio > 1) {
		ratio = 1;
	}

	//rotor range: ranges from min to max
	double cycleMS = 1000000.0 / this->frequency;
	double stepLengthMS = cycleMS / 4096;

	double pulseLengthMS = ratio * (MAXPWMMS - MINPWMMS) + MINPWMMS;//1 to 2 millis

	int pulseLengthSteps = (int)(pulseLengthMS / stepLengthMS) - 1;

	int off = pulseLengthSteps % 4096;

	return off;
}

void PWMController::InitializeESCs() {
	std::cout << "Disconnect power to ESCs." << std::endl;

	//cut power
	SetRotorBOutput(MAXROTOROUTPUT);//max
	SetRotorCOutput(MAXROTOROUTPUT);//max
	SetRotorDOutput(MAXROTOROUTPUT);//max
	SetRotorEOutput(MAXROTOROUTPUT);//max

	std::cout << "Press enter when power is connected." << std::endl;

	std::string temp;

	std::getline(std::cin, temp);

	std::cout << "Waiting for ESCs to configure." << std::endl;

	delay(5000);

	std::cout << "Done initializing ESCs." << std::endl;

	SetRotorBOutput(MINROTOROUTPUT);//min
	SetRotorCOutput(MINROTOROUTPUT);//min
	SetRotorDOutput(MINROTOROUTPUT);//min
	SetRotorEOutput(MINROTOROUTPUT);//min
}


//OUTER JOINT ANGLE
void PWMController::SetOuterBAngle(double angle) {
	SetPWM(OuterB, -CalculateServoFrequency(angle));
}

void PWMController::SetOuterCAngle(double angle) {
	SetPWM(OuterC, CalculateServoFrequency(angle));
}

void PWMController::SetOuterDAngle(double angle) {
	SetPWM(OuterD, CalculateServoFrequency(angle));
}

void PWMController::SetOuterEAngle(double angle) {
	SetPWM(OuterE, -CalculateServoFrequency(angle));
}


//INNER JOINT ANGLE
void PWMController::SetInnerBAngle(double angle) {
	SetPWM(InnerB, CalculateServoFrequency(angle));
}

void PWMController::SetInnerCAngle(double angle) {
	SetPWM(InnerC, CalculateServoFrequency(angle));
}

void PWMController::SetInnerDAngle(double angle) {
	SetPWM(InnerD, CalculateServoFrequency(angle));
}

void PWMController::SetInnerEAngle(double angle) {
	SetPWM(InnerE, CalculateServoFrequency(angle));
}


//ROTOR OUTPUTS
void PWMController::SetRotorBOutput(double output) {
	SetPWM(RotorB, CalculateRotorFrequency(output));
}

void PWMController::SetRotorCOutput(double output) {
	SetPWM(RotorC, CalculateRotorFrequency(output));
}

void PWMController::SetRotorDOutput(double output) {
	SetPWM(RotorD, CalculateRotorFrequency(output));
}

void PWMController::SetRotorEOutput(double output) {
	SetPWM(RotorE, CalculateRotorFrequency(output));
}
