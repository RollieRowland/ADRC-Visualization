#pragma once

#include "Mathematics.h"
#include "FastFourierTransform.h"

class HighPassFilter {
private:
	double samplingFrequency;
	double cutoffFrequency;
	int memory;
	std::vector<double> samples;
	double* previousTransform;

public:
	HighPassFilter();
	~HighPassFilter();
	HighPassFilter(double samplingFrequency, double cutoffFrequency, int memory);

	double Filter(double value);
	double* GetSamples();

};
