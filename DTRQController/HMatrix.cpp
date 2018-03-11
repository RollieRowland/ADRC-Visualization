#include <HMatrix.h>
#include <math.h>
#include <Math.h>


HMatrix::HMatrix() {
	for (int i = 0; i < 3; i++)
	{
		for (int k = 0; k < 3; k++)
		{
			hierarchicalMatrix[i][k] = 0;
		}
	}
}

HMatrix::HMatrix(vector<vector<double>> hMatrix) {
	hierarchicalMatrix = hMatrix;
}