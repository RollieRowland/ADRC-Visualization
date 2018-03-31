#pragma once

typedef struct Servo {
private:
	double angle = 0;//-90->90

	//DShot dShot;
public:
	Servo();
	void SetAngle(double value);
	double GetAngle();
} Servo;