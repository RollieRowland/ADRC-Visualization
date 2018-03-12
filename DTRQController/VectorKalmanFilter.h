#pragma once

#include <KalmanFilter.h>
#include <Vector.h>

typedef struct VectorKalmanFilter {
	KalmanFilter X;
	KalmanFilter Y;
	KalmanFilter Z;

	VectorKalmanFilter();
	VectorKalmanFilter(double gain, int memory);
	VectorKalmanFilter(Vector3D gain, Vector3D memory);

	Vector3D Filter(Vector3D input);

} VectorKalmanFilter;