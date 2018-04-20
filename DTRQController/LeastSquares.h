#pragma once

#include "Mathematics.h"

class LeastSquares {
private:
	int memory;
	std::vector<double> X;
	std::vector<double> Y;
	double intercept;
	double slope;
	double correlation;

public:
	LeastSquares();
	LeastSquares(int memory);
	double Calculate(double x, double y, double target);

};
