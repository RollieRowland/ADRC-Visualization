#pragma once
#include "stdafx.h"

#include <Vector.h>

namespace DTRQCSInterface {
	public value class SVector {
	public:
		double X;
		double Y;
		double Z;

		SVector(Vector3D v) {
			this->X = v.X;
			this->Y = v.Y;
			this->Z = v.Z;
		}

		SVector(double X, double Y, double Z) {
			this->X = X;
			this->Y = Y;
			this->Z = Z;
		}
	};
}