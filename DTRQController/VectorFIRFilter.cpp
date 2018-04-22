#include "VectorFIRFilter.h"

VectorFIRFilter::VectorFIRFilter() {
	X = FiniteImpulseResponse();
	Y = FiniteImpulseResponse();
	Z = FiniteImpulseResponse();
}

VectorFIRFilter::VectorFIRFilter(FiniteImpulseResponse::Type type, int taps, double fs, double fx, double fxb) {
	X = FiniteImpulseResponse(type, taps, fs, fx, fxb);
	Y = FiniteImpulseResponse(type, taps, fs, fx, fxb);
	Z = FiniteImpulseResponse(type, taps, fs, fx, fxb);
}

VectorFIRFilter::VectorFIRFilter(FiniteImpulseResponse::Type type, Vector3D taps, Vector3D fs, Vector3D fx, Vector3D fxb){
	X = FiniteImpulseResponse(type, (int)taps.X, fs.X, fx.X, fxb.X);
	Y = FiniteImpulseResponse(type, (int)taps.Y, fs.Y, fx.Y, fxb.Y);
	Z = FiniteImpulseResponse(type, (int)taps.Z, fs.Z, fx.Z, fxb.Z);
}

Vector3D VectorFIRFilter::Filter(Vector3D input) {
	return Vector3D{
		X.Filter(input.X),
		Y.Filter(input.Y),
		Z.Filter(input.Z)
	};
}
