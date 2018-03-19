#pragma once
#include "stdafx.h"

#include <DirectionAngle.h>

#include <SVector.h>

namespace DTRQCSInterface {
	public ref class SDirAngle {
	public:
		double Rotation;
		SVector^ Direction;

		SDirAngle(double r, double x, double y, double z) {
			this->Rotation = r;
			this->Direction = gcnew SVector(x, y, z);
		}

		SDirAngle(DirectionAngle dA) {
			this->Rotation = dA.Rotation;
			this->Direction = gcnew SVector(dA.Direction);
		}

		SDirAngle(const SDirAngle% sDA) {
			this->Rotation = sDA.Rotation;
			this->Direction = sDA.Direction;
		}

		SDirAngle(double Rotation, SVector^ Direction) {
			this->Rotation = Rotation;
			this->Direction = Direction;
		}
	};
}
