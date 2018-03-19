#pragma once

#include <Mathematics.h>

typedef struct CriticallyDampedSpring {
private:
	double dT;
	double currentPosition = 0;
	double currentVelocity = 0;
	double springConstant;
	double mass;
	std::string name;

public:
	CriticallyDampedSpring();
	CriticallyDampedSpring(double dT, double springConstant, double mass, std::string name);

	double Calculate(double target);
	
} CriticallyDampedSpring;
