#include <TriangleWaveFader.h>

TriangleWaveFader::TriangleWaveFader(double curvature, double amplitude) {
	this->curvature = curvature;
	this->amplitude = amplitude;
}

double TriangleWaveFader::CalculateRatio(double value) {
	return (1.0 / amplitude) * pow((amplitude - abs(fmod(value,(amplitude * 2.0)) - amplitude)), curvature) / pow(amplitude, curvature - 1.0);

}

double TriangleWaveFader::CalculateInverseRatio(double value) {
	return 1 - CalculateRatio(value);
}
