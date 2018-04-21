#include "VectorHighPassFilter.h"

VectorHighPassFilter::VectorHighPassFilter() {
	X = HighPassFilter();
	Y = HighPassFilter();
	Z = HighPassFilter();
}

VectorHighPassFilter::VectorHighPassFilter(double frequency, int memory) {
	X = HighPassFilter(frequency, memory);
	Y = HighPassFilter(frequency, memory);
	Z = HighPassFilter(frequency, memory);
}

VectorHighPassFilter::VectorHighPassFilter(Vector3D frequency, Vector3D memory) {
	X = HighPassFilter(frequency.X, (int)memory.X);
	Y = HighPassFilter(frequency.Y, (int)memory.Y);
	Z = HighPassFilter(frequency.Z, (int)memory.Z);
}

Vector3D VectorHighPassFilter::Filter(Vector3D input) {
	return Vector3D{
		X.Filter(input.X),
		Y.Filter(input.Y),
		Z.Filter(input.Z)
	};
}
