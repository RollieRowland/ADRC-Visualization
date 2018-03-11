#pragma once
#include <EulerAngles.h>
#include <EulerOrder.h>
#include <vector>

typedef struct HMatrix {
	vector<vector<double>> hierarchicalMatrix;

	HMatrix();
	HMatrix(vector<vector<double>> hMatrix);
	HMatrix(int x, int y) : hierarchicalMatrix(x, vector<double>(y, double())) {}

	double& operator() (int x, int y) { return hierarchicalMatrix[x][y]; }
} HMatrix;