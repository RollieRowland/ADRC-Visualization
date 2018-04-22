#include "VectorHighPassFilter.h"

VectorHighPassFilter::VectorHighPassFilter() {
	X = HighPassFilter();
	Y = HighPassFilter();
	Z = HighPassFilter();
}

VectorHighPassFilter::VectorHighPassFilter(double samplingFrequency, double cutoffFrequency, int memory) {
	X = HighPassFilter(samplingFrequency, cutoffFrequency, memory);
	Y = HighPassFilter(samplingFrequency, cutoffFrequency, memory);
	Z = HighPassFilter(samplingFrequency, cutoffFrequency, memory);
}

VectorHighPassFilter::VectorHighPassFilter(Vector3D samplingFrequency, Vector3D cutoffFrequency, Vector3D memory) {
	X = HighPassFilter(samplingFrequency.X, cutoffFrequency.X, (int)memory.X);
	Y = HighPassFilter(samplingFrequency.Y, cutoffFrequency.Y, (int)memory.Y);
	Z = HighPassFilter(samplingFrequency.Z, cutoffFrequency.Z, (int)memory.Z);
}

Vector3D VectorHighPassFilter::Filter(Vector3D input) {
	return Vector3D{
		X.Filter(input.X),
		Y.Filter(input.Y),
		Z.Filter(input.Z)
	};
}
