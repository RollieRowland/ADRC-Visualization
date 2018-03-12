#pragma once

#include <vector>

typedef struct KalmanFilter {
private:
	double gain;
	int memory;
	vector<double> values;

public:
	KalmanFilter();
	KalmanFilter(double gain, int memory);
	double Filter(double value);

} KalmanFilter;