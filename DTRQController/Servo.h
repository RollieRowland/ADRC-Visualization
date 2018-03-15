#pragma once

typedef struct Servo {
private:
	double angle;//-90->90

	void SetPWM();
	//DShot dShot;
public:
	void SetAngle(double value);
	double GetAngle();
} Servo;