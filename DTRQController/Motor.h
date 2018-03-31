#pragma once

typedef struct Motor {
private:
	double output = 0;//0->1 or -1->1

	//DShot dShot;
public:
	Motor();
	void SetOutput(double value);
	double GetOutput();

} Motor;