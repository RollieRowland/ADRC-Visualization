#pragma once

#include "FiniteImpulseResponse.h"
#include "Vector.h"

class VectorFIRFilter {
private:
	FiniteImpulseResponse X;
	FiniteImpulseResponse Y;
	FiniteImpulseResponse Z;

public:
	VectorFIRFilter();
	VectorFIRFilter(FiniteImpulseResponse::Type type, int taps, double fs, double fx, double fxb);
	VectorFIRFilter(FiniteImpulseResponse::Type type, Vector3D taps, Vector3D fs, Vector3D fx, Vector3D fxb);

	Vector3D Filter(Vector3D input);

};