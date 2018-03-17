#pragma once
#include "stdafx.h"

#include <DirectionAngle.h>

#include <SVector.h>

namespace DTRQCSInterface {
	public ref class SDirAngle {
	public:
		double Rotation;
		SVector Direction;

		SDirAngle() {
			this->Rotation = 0;
		}

		SDirAngle(double r, double x, double y, double z) {
			this->Rotation = r;
			this->Direction = SVector(x, y, z);
		}

		SDirAngle(const SDirAngle% sDA) {
			this->Rotation = sDA.Rotation;
			this->Direction = sDA.Direction;
		}

		SDirAngle(DirectionAngle dA) {
			this->Rotation = dA.Rotation;
			this->Direction = SVector(dA.Direction);
		}

		SDirAngle(double Rotation, SVector Direction) {
			this->Rotation = Rotation;
			this->Direction = Direction;
		}

		SDirAngle operator =(SDirAngle sDA) {
			this->Rotation = sDA.Rotation;
			this->Direction = sDA.Direction;

			return *this;
		}
	};
}
