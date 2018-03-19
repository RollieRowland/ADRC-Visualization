#pragma once
#include "stdafx.h"

#include <SVector.h>

namespace DTRQCSInterface {
	public ref class SThrust {
	public:
		SVector^ CurrentPosition;
		SVector^ CurrentRotation;
		SVector^ TargetPosition;

		SThrust(Vector3D CurrentPosition, Vector3D CurrentRotation, Vector3D TargetPosition) {
			this->CurrentPosition = gcnew SVector(CurrentPosition);
			this->CurrentRotation = gcnew SVector(CurrentRotation);
			this->TargetPosition = gcnew SVector(TargetPosition);
		}

		SThrust(SVector^ CurrentPosition, SVector^ CurrentRotation, SVector^ TargetPosition) {
			this->CurrentPosition = CurrentPosition;
			this->CurrentRotation = CurrentRotation;
			this->TargetPosition = TargetPosition;
		}

		SThrust(const SThrust% sThrust) {
			this->CurrentPosition = sThrust.CurrentPosition;
			this->CurrentRotation = sThrust.CurrentRotation;
			this->TargetPosition = sThrust.TargetPosition;
		}
	};
}