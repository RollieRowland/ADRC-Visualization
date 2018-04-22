#pragma once

#include "Mathematics.h"

class FiniteImpulseResponse {
public:
	enum Type{
		Low,
		High,
		Band
	};

	FiniteImpulseResponse();
	FiniteImpulseResponse(Type, int numberTaps, double fs, double fx, double fxb);

	double Filter(double sample);

private:
	int numberTaps;
	double fs;//sampling frequency
	double fx;//cutoff frequency
	double fxb;//upper bandpass cutoff
	double lambda;
	double phi;
	std::vector<double> taps;
	std::vector<double> sr;

	void SetupLowPassTaps();
	void SetupHighPassTaps();
	void SetupBandPassTaps();

};
