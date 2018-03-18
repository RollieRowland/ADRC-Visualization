#pragma once
#include "stdafx.h"

#include <SVector.h>

namespace DTRQCSInterface {
	public value class SThrust {
	public:
		SVector CurrentPosition;
		SVector CurrentRotation;
		SVector TargetPosition;

		SThrust(Vector3D CurrentPosition, Vector3D CurrentRotation, Vector3D TargetPosition) {
			this->CurrentPosition = SVector(CurrentPosition);
			this->CurrentRotation = SVector(CurrentRotation);
			this->TargetPosition = SVector(TargetPosition);
		}

		SThrust(SVector CurrentPosition, SVector CurrentRotation, SVector TargetPosition) {
			this->CurrentPosition = CurrentPosition;
			this->CurrentRotation = CurrentRotation;
			this->TargetPosition = TargetPosition;
		}

		SVector GetCurrentPosition() {
			return CurrentPosition;
		}

		SVector GetCurrentRotation() {
			return CurrentRotation;
		}

		SVector GetTargetPosition() {
			return TargetPosition;
		}
	};
}