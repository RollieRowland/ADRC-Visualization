#pragma once
#include "stdafx.h"

#include <DirectionAngle.h>

#include <SVector.h>

namespace DTRQCSInterface {
	public ref class SQuaternion {
	public:
		double W;
		double X;
		double Y;
		double Z;

		SQuaternion(double w, double x, double y, double z) {
			this->W = w;
			this->X = x;
			this->Y = y;
			this->Z = z;
		}

		SQuaternion(Quaternion q) {
			this->W = q.W;
			this->X = q.X;
			this->Y = q.Y;
			this->Z = q.Z;
		}

		SQuaternion(const SQuaternion% q) {
			this->W = q.W;
			this->X = q.X;
			this->Y = q.Y;
			this->Z = q.Z;
		}
	};
}
