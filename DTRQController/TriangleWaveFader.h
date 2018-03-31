#pragma once

#include "Mathematics.h"

class TriangleWaveFader {
private:
	double curvature;
	double amplitude;
public:
	TriangleWaveFader();
	TriangleWaveFader(double curvature, double amplitude);
	double CalculateRatio(double value);
	double CalculateInverseRatio(double value);
};