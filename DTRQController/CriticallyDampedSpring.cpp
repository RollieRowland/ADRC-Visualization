#include <CriticallyDampedSpring.h>

CriticallyDampedSpring::CriticallyDampedSpring() {
	this->dT = 1;
	this->springConstant = 1;
	this->mass = 1;
}

CriticallyDampedSpring::CriticallyDampedSpring(double dT, double springConstant, double mass) {
	this->dT = dT;
	this->springConstant = springConstant;
	this->mass = mass;

	std::cout << "CDS: " << dT << " " << springConstant << " " << mass << std::endl;
}

double CriticallyDampedSpring::Calculate(double target) {
	double currentToTarget = target - currentPosition;
	double springForce = currentToTarget * springConstant;
	double dampingForce = -currentVelocity * mass * sqrt(springConstant);
	double force = springForce + dampingForce;

	currentVelocity = currentVelocity + force * dT;
	currentPosition = currentPosition + currentVelocity * dT;

	return currentPosition;
}