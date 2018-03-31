#pragma once

#include "Mathematics.h"

class ExtendedStateObserver {
public:
	typedef struct State {
		double Z1;
		double Z2;
		double Z3;

		State() {
			Z1 = 0.0;
			Z2 = 0.0;
			Z3 = 0.0;
		}

		State(double Z1, double Z2, double Z3) {
			this->Z1 = Z1;
			this->Z2 = Z2;
			this->Z3 = Z3;
		}
	} State;

	ExtendedStateObserver();
	ExtendedStateObserver(bool linear);
	State ObserveState(double samplingPeriod, double u, double b0, double processVariable);


private:
	State state;
	State gain;
	bool linear;
	double NonlinearFunction(double eta, double alpha, double delta);

};