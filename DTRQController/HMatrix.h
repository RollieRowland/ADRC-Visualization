#pragma once

#include <EulerAngles.h>
#include <EulerOrder.h>
#include <Math.h>

typedef struct HMatrix {
public:
	std::vector<std::vector<double>> hierarchicalMatrix;

	HMatrix();
	HMatrix(std::vector<std::vector<double>> hMatrix);
	HMatrix(int x, int y) : hierarchicalMatrix(x, std::vector<double>(y, double())) {}

	double& operator() (int x, int y) { return hierarchicalMatrix[x][y]; }
} HMatrix;