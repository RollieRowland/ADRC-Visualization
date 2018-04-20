#pragma once

#include "LeastSquares.h"
#include "Vector.h"

class VectorLeastSquares {
private:
	LeastSquares *X;
	LeastSquares *Y;
	LeastSquares *Z;

public:
	VectorLeastSquares();
	~VectorLeastSquares();
	VectorLeastSquares(int memory);
	VectorLeastSquares(Vector3D memory);
	Vector3D Calculate(Vector3D x, Vector3D y, Vector3D target);
};
