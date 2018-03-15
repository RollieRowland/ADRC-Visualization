#pragma once
#include <PID.h>
#include <ExtendedStateObserver.h>
#include <NonlinearCombiner.h>
#include <FeedbackController.h>

using namespace std;

class ADRC : public FeedbackController {
private:
	double amplification;
	double damping;
	double precision;
	double plant;
	double precisionModifier;
	NonlinearCombiner::Output output;
	PID pid;
	ExtendedStateObserver eso;
	NonlinearCombiner nlc;

public:
	ADRC(double amplification, double damping, double plant, double precisionModifier, PID pid);
	virtual double Calculate(double setpoint, double processVariable, double dT);
};
