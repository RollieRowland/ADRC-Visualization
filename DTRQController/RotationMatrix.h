#pragma once
#include <Vector.h>
#include <math.h>
#include <Math.h>
#include <Quaternion.h>

typedef struct RotationMatrix {
private:
	Vector3D InitialVector;
	bool didRotate;

public:
	Vector3D XAxis;
	Vector3D YAxis;
	Vector3D ZAxis;

	RotationMatrix(Vector3D axes);
	
	Vector3D ConvertCoordinateToVector();
	void ReadjustMatrix();
	void Rotate(Vector3D rotation);
	void RotateX(double theta);
	void RotateY(double theta);
	void RotateZ(double theta);
	void Multiply(double d);
	void Multiply(RotationMatrix rM);
	void RotateRelative(RotationMatrix rM);
	void Normalize();
	void Transpose();
	void Inverse();

	bool IsEqual(RotationMatrix rM);
	double Determinant();

	static RotationMatrix QuaternionToRotationMatrix(Quaternion quaternion);
	static Vector3D RotateVector(Vector3D rotate, Vector3D coordinates);

	string ToString();
} RotationMatrix;