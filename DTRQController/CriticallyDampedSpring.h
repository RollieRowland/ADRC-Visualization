#pragma once

#include "Mathematics.h"

typedef struct CriticallyDampedSpring {
private:
	double dT;
	double currentVelocity = 0.0;
	double currentPosition = 0.0;
	double springConstant;
	std::string name;

public:
	CriticallyDampedSpring(double dT, double springConstant, std::string name);

	double Calculate(double target);
	
} CriticallyDampedSpring;
