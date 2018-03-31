#pragma once

#include "KalmanFilter.h"
#include "Vector.h"

class VectorKalmanFilter {
private:
	KalmanFilter X;
	KalmanFilter Y;
	KalmanFilter Z;

public:
	VectorKalmanFilter();
	VectorKalmanFilter(double gain, int memory);
	VectorKalmanFilter(Vector3D gain, Vector3D memory);

	Vector3D Filter(Vector3D input);

};