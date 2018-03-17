#pragma once

typedef struct Motor {
private:
	double output = 0;//0->1 or -1->1

	void SetPWM();
	//DShot dShot;
public:
	Motor();
	bool CheckESC();
	void SetOutput(double value);
	double GetOutput();

} Motor;