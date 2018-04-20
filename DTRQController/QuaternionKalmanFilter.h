#pragma once

#include "Quaternion.h"

class QuaternionKalmanFilter {
private:
	double gain;
	int memory;
	std::vector<Quaternion> values;

public:
	QuaternionKalmanFilter();
	QuaternionKalmanFilter(double gain, int memory);

	Quaternion Filter(Quaternion input);

};
