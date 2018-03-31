#pragma once

#include "Mathematics.h"
#include "ExtendedStateObserver.h"

class NonlinearCombiner {
private:
	double amplificationCoefficient;
	double dampingCoefficient;

	double SetPointJumpPrevention(double target, double targetDerivative, double r0, double h);

public:
	typedef struct Output{
		double Current = 0;
		double Previous = 0;

		Output() {
			Current = 0.0;
			Previous = 0.0;
		}

		Output(double Current, double Previous) {
			this->Current = Current;
			this->Previous = Previous;
		}
	} Output;

	NonlinearCombiner();
	NonlinearCombiner(double amplification, double damping);
	double Combine(Output output, double b0, ExtendedStateObserver::State state, double precisionCoefficient);
};
