#include "ExtendedStateObserver.h"

ExtendedStateObserver::ExtendedStateObserver() {
	this->linear = true;
}

ExtendedStateObserver::ExtendedStateObserver(bool linear) {
	this->linear = linear;
}

ExtendedStateObserver::State ExtendedStateObserver::ObserveState(double samplingPeriod, double u, double b0, double processVariable) {
	State gain;

	if (linear)
	{
		gain = State{
			1,
			1 / (3 * samplingPeriod),
			2 / (pow(8, 2) * pow(samplingPeriod, 2))
		};
	}
	else
	{
		gain = State{
			1,
			1 / (2 * pow(samplingPeriod, 0.5)),
			2 / (pow(5, 2) * pow(samplingPeriod, 1.2))
		};
	}

	double e, fe, fe1;

	e = state.Z1 - processVariable;//pv = y
	fe = NonlinearFunction(e, 0.5, samplingPeriod);//3rd parameter as sampling period as shown in 
	fe1 = NonlinearFunction(e, 0.25, samplingPeriod);//From PID to Active Disturbance Rejection Control by Jingqing Han

	state.Z1 = state.Z1 + (samplingPeriod * state.Z2) - (gain.Z1 * e);
	state.Z2 = state.Z2 + (samplingPeriod * (state.Z3 + (b0 * u))) - (gain.Z2 * fe);
	state.Z3 = state.Z3 - (gain.Z3 * fe1);

	return state;
}

double ExtendedStateObserver::NonlinearFunction(double eta, double alpha, double delta) {
	if (std::abs(eta) <= delta)
	{
		return eta / (pow(delta, 1 - alpha));
	}
	else
	{
		return pow(std::abs(eta), alpha) * Mathematics::Sign(eta);
	}
}