#pragma once

#include "HighPassFilter.h"
#include "Vector.h"

class VectorHighPassFilter {
private:
	HighPassFilter X;
	HighPassFilter Y;
	HighPassFilter Z;

public:
	VectorHighPassFilter();
	VectorHighPassFilter(double samplingFrequency, double cutoffFrequency, int memory);
	VectorHighPassFilter(Vector3D samplingFrequency, Vector3D cutoffFrequency, Vector3D memory);

	Vector3D Filter(Vector3D input);

};