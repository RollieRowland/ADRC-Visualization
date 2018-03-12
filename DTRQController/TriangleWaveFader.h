#pragma once
#include <math.h>

typedef struct TriangleWaveFader {
private:
	double curvature;
	double amplitude;
public:
	TriangleWaveFader(double curvature, double amplitude);
	double CalculateRatio(double value);
	double CalculateInverseRatio(double value);
} TriangleWaveFader;