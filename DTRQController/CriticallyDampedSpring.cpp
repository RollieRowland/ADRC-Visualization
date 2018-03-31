#include "CriticallyDampedSpring.h"

CriticallyDampedSpring::CriticallyDampedSpring(double dT, double springConstant, std::string name) {
	this->dT = dT;
	this->springConstant = springConstant;

	std::cout << "    CDS for " << name << " initializing with: dT:" << dT << " K:" << springConstant << std::endl;
}

double CriticallyDampedSpring::Calculate(double target) {
	double currentToTarget = target - currentPosition;
	double springForce = currentToTarget * springConstant;
	double dampingForce = -currentVelocity * 2 * sqrt(springConstant);
	double force = springForce + dampingForce;

	currentVelocity += force * dT;
	currentPosition += currentVelocity * dT;

	return currentPosition;
}