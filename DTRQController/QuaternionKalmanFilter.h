#pragma once

#include "KalmanFilter.h"
#include "Quaternion.h"

class QuaternionKalmanFilter {
private:
	KalmanFilter W;
	KalmanFilter X;
	KalmanFilter Y;
	KalmanFilter Z;

public:
	QuaternionKalmanFilter();
	QuaternionKalmanFilter(double gain, int memory);
	QuaternionKalmanFilter(Quaternion gain, Quaternion memory);

	Quaternion Filter(Quaternion input);

};