#include "FiniteImpulseResponse.h"

//Discrete convolution
FiniteImpulseResponse::FiniteImpulseResponse() {
	this->numberTaps = 50;
	this->fs = 1000;
	this->fx = 100;
	this->fxb = 0;

	this->lambda = Mathematics::PI * fx / (fs / 2.0);
	this->phi = Mathematics::PI * fxb / (fs / 2.0);

	SetupHighPassTaps();
}

FiniteImpulseResponse::FiniteImpulseResponse(Type filter, int numberTaps, double fs, double fx, double fxb) {
	this->numberTaps = numberTaps;
	this->fs = fs;
	this->fx = fx;
	this->fxb = fxb;

	if (numberTaps > 1000) {
		std::cout << "FIR taps limited to 1000." << std::endl;
		numberTaps = 1000;
	}

	this->lambda = Mathematics::PI * fx / (fs / 2.0);
	this->phi = Mathematics::PI * fxb / (fs / 2.0);

	if (filter == Low) {
		SetupLowPassTaps();
	}
	else if (filter == High) {
		SetupHighPassTaps();
	}
	else {
		SetupBandPassTaps();
	}
}

double FiniteImpulseResponse::Filter(double sample) {
	double output = 0.0;

	sr.insert(sr.begin(), sample);
	sr.pop_back();

	sr.resize(numberTaps);

	for (int i = 0; i < numberTaps; i++) {
		output += (double)sr.at(i) * (double)taps.at(i);
	}

	return output;
}

void FiniteImpulseResponse::SetupLowPassTaps() {
	double mm;

	taps.resize(numberTaps);

	for (int i = 0; i < numberTaps; i++) {
		mm = i - ((double)numberTaps - 1.0) / 2.0;

		if (mm == 0.0) {
			taps.at(i) = lambda / Mathematics::PI;
		}
		else {
			taps.at(i) = sin(mm * lambda) / (mm * Mathematics::PI);
		}
	}
}

void FiniteImpulseResponse::SetupHighPassTaps() {
	double mm;

	taps.resize(numberTaps);

	for (int i = 0; i < numberTaps; i++) {
		mm = i - ((double)numberTaps - 1.0) / 2.0;

		if (mm == 0.0) {
			taps.at(i) = 1.0 - (lambda / Mathematics::PI);
		}
		else {
			taps.at(i) = -sin(mm * lambda) / (mm * Mathematics::PI);
		}
	}
}

void FiniteImpulseResponse::SetupBandPassTaps() {
	double mm;

	taps.resize(numberTaps);

	for (int i = 0; i < numberTaps; i++) {
		mm = i - ((double)numberTaps - 1.0) / 2.0;

		if (mm == 0.0) {
			taps.at(i) = (phi - lambda) / Mathematics::PI;
		}
		else {
			taps.at(i) = (sin(mm * phi) - sin(mm * lambda)) / (mm * Mathematics::PI);
		}
	}
}
