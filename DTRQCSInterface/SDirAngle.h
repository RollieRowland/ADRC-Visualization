#pragma once
#include "stdafx.h"

#include <DirectionAngle.h>

#include <SVector.h>

namespace DTRQCSInterface {
	public value class SDirAngle {
	public:
		double Rotation;
		SVector Direction;

		SDirAngle(double r, double x, double y, double z) {
			this->Rotation = r;
			this->Direction = SVector(x, y, z);
		}

		SDirAngle(DirectionAngle dA) {
			this->Rotation = dA.Rotation;
			this->Direction = SVector(dA.Direction);
		}

		SDirAngle(double Rotation, SVector Direction) {
			this->Rotation = Rotation;
			this->Direction = Direction;
		}
	};
}
