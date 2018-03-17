#pragma once
#include "stdafx.h"

#include <Vector.h>

namespace DTRQCSInterface {
	public ref class SVector {
	public:
		double X;
		double Y;
		double Z;

		SVector() {}

		SVector(Vector3D v) {
			this->X = v.X;
			this->Y = v.Y;
			this->Z = v.Z;
		}

		SVector(const SVector% v) {
			this->X = v.X;
			this->Y = v.Y;
			this->Z = v.Z;
		}

		SVector(const SVector ^v) {
			this->X = v->X;
			this->Y = v->Y;
			this->Z = v->Z;
		}

		SVector(double X, double Y, double Z) {
			this->X = X;
			this->Y = Y;
			this->Z = Z;
		}

		SVector operator =(SVector sV) {
			this->X = sV.X;
			this->Y = sV.Y;
			this->Z = sV.Z;

			return *this;
		}
	};
}