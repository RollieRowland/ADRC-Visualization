#include "VectorLeastSquares.h"

VectorLeastSquares::VectorLeastSquares() {
	X = new LeastSquares();
	Y = new LeastSquares();
	Z = new LeastSquares();
}

VectorLeastSquares::~VectorLeastSquares() {
	delete X;
	delete Y;
	delete Z;
}

VectorLeastSquares::VectorLeastSquares(int memory) {
	X = new LeastSquares(memory);
	Y = new LeastSquares(memory);
	Z = new LeastSquares(memory);
}

VectorLeastSquares::VectorLeastSquares(Vector3D memory) {
	X = new LeastSquares((int)memory.X);
	Y = new LeastSquares((int)memory.Y);
	Z = new LeastSquares((int)memory.Z);
}

Vector3D VectorLeastSquares::Calculate(Vector3D x, Vector3D y, Vector3D target) {
	return Vector3D{
		X->Calculate(x.X, y.X, target.X),
		Y->Calculate(x.Y, y.Y, target.Y),
		Z->Calculate(x.Z, y.Z, target.Z)
	};
}
