#include <HMatrix.h>

HMatrix::HMatrix() {
	for (int i = 0; i < 3; i++)
	{
		for (int k = 0; k < 3; k++)
		{
			hierarchicalMatrix[i][k] = 0;
		}
	}
}

HMatrix::HMatrix(std::vector<std::vector<double>> hMatrix) {
	hierarchicalMatrix = hMatrix;
}