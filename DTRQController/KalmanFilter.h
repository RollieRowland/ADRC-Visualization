#pragma once

#include "Mathematics.h"

class KalmanFilter {
private:
	double gain;
	int memory;
	std::vector<double> values;

public:
	KalmanFilter();
	KalmanFilter(double gain, int memory);
	double Filter(double value);

};
