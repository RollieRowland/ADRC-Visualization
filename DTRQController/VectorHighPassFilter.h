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
	VectorHighPassFilter(double frequency, int memory);
	VectorHighPassFilter(Vector3D frequency, Vector3D memory);

	Vector3D Filter(Vector3D input);

};