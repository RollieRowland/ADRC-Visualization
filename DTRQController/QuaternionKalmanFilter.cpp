#include "QuaternionKalmanFilter.h"

QuaternionKalmanFilter::QuaternionKalmanFilter() {
	X = KalmanFilter();
	Y = KalmanFilter();
	Z = KalmanFilter();
}

QuaternionKalmanFilter::QuaternionKalmanFilter(double gain, int memory) {
	X = KalmanFilter(gain, memory);
	Y = KalmanFilter(gain, memory);
	Z = KalmanFilter(gain, memory);
}

QuaternionKalmanFilter::QuaternionKalmanFilter(Quaternion gain, Quaternion memory) {
	W = KalmanFilter(gain.W, (int)memory.W);
	X = KalmanFilter(gain.X, (int)memory.X);
	Y = KalmanFilter(gain.Y, (int)memory.Y);
	Z = KalmanFilter(gain.Z, (int)memory.Z);
}

Quaternion QuaternionKalmanFilter::Filter(Quaternion input) {
	return Quaternion{
		W.Filter(input.W),
		X.Filter(input.X),
		Y.Filter(input.Y),
		Z.Filter(input.Z)
	};
}
