#pragma once
#include <string>
#include <math.h>
#include <Math.h>
#include <Vector.h>
#include <EulerAngles.h>
#include <AxisAngle.h>
#include <DirectionAngle.h>
#include <RotationMatrix.h>

using namespace std;

typedef struct Quaternion {
	double W = 1.0;
	double X = 0.0;
	double Y = 0.0;
	double Z = 0.0;

	Quaternion(const Quaternion& quaternion);
	Quaternion(Vector3D vector);
	Quaternion(double W, double X, double Y, double Z);

	Vector3D RotateVector(Vector3D coordinate);
	Vector3D UnrotateVector(Vector3D coordinate);
	Vector3D GetBiVector();

	Quaternion Add(Quaternion quaternion);
	Quaternion Subtract(Quaternion quaternion);
	Quaternion Multiply(Quaternion quaternion);
	Quaternion Divide(Quaternion quaternion);
	Quaternion Power(Quaternion quaternion);
	Quaternion DotProduct(Quaternion quaternion);

	Quaternion Power(double exponent);
	Quaternion Permutate(Vector3D permutation);

	Quaternion Absolute();
	Quaternion AdditiveInverse();
	Quaternion MultiplicativeInverse();
	Quaternion Conjugate();
	Quaternion UnitQuaternion();
	Quaternion Magnitude();

	double Normal();

	bool IsNan();
	bool IsFinite();
	bool IsInfinite();
	bool IsNonZero();
	bool IsEqual(Quaternion quaternion);

	string ToString();

	//Static functions
	static Quaternion EulerToQuaternion(EulerAngles eulerAngles);
	static Quaternion AxisAngleToQuaternion(AxisAngle axisAngle);
	static Quaternion DirectionAngleToQuaternion(DirectionAngle directionAngle);
	static Quaternion RotationMatrixToQuaternion(RotationMatrix rotationMatrix);
	static Quaternion QuaternionFromDirectionVectors(Vector3D initial, Vector3D final);
	static Quaternion SphericalInterpolation(Quaternion quaternion, double ratio);

	static Quaternion Add(Quaternion q1, Quaternion q2) {
		return q1.Add(q2);
	}

	static Quaternion Subtract(Quaternion q1, Quaternion q2) {
		return q1.Subtract(q2);
	}

	static Quaternion Multiply(Quaternion q1, Quaternion q2) {
		return q1.Multiply(q2);
	}

	static Quaternion Divide(Quaternion q1, Quaternion q2) {
		return q1.Divide(q2);
	}

	static Quaternion Power(Quaternion q1, Quaternion q2) {
		return q1.Power(q2);
	}

	static Quaternion DotProduct(Quaternion q1, Quaternion q2) {
		return q1.DotProduct(q2);
	}


	static Quaternion Power(Quaternion quaternion, double exponent) {
		return quaternion.Power(exponent);
	}

	static Quaternion Permutate(Quaternion quaternion, Vector3D vector) {
		return quaternion.Permutate(vector);
	}

	static Quaternion Absolute(Quaternion quaternion) {
		return quaternion.Absolute();
	}

	static Quaternion AdditiveInverse(Quaternion quaternion) {
		return quaternion.AdditiveInverse();
	}

	static Quaternion MultiplicativeInverse(Quaternion quaternion) {
		return quaternion.MultiplicativeInverse();
	}

	static Quaternion Conjugate(Quaternion quaternion) {
		return quaternion.Conjugate();
	}

	static Quaternion UnitQuaternion(Quaternion quaternion) {
		return quaternion.UnitQuaternion();
	}

	static Quaternion Magnitude(Quaternion quaternion) {
		return quaternion.Magnitude();
	}

	static double Normal(Quaternion quaternion) {
		return quaternion.Normal();
	}

	//Operator overloads
	bool operator ==(Quaternion quaternion) {
		return this->IsEqual(quaternion);
	}

	bool operator !=(Quaternion quaternion) {
		return !(this->IsEqual(quaternion));
	}

	Quaternion operator  +(Quaternion quaternion) {
		return this->Add(quaternion);
	}

	Quaternion operator  -(Quaternion quaternion) {
		return this->Subtract(quaternion);
	}

	Quaternion operator  *(Quaternion quaternion) {
		return this->Multiply(quaternion);
	}

	Quaternion operator  /(Quaternion quaternion) {
		return this->Divide(quaternion);
	}

} Quaternion;
