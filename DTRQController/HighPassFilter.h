#pragma once

#include "Mathematics.h"
#include "FastFourierTransform.h"

class HighPassFilter {
private:
	double frequency;
	int memory;
	std::vector<double> samples;

public:
	HighPassFilter();
	HighPassFilter(double frequency, int memory);

	double Filter(double value);

};
