#pragma once
#include "stdafx.h"

#include <SVector.h>
#include <SDirAngle.h>
#include <SThrust.h>

namespace DTRQCSInterface {
	public value class SQuad {
	public:
		SVector CurrentPosition;
		SDirAngle CurrentRotation;
		SVector TargetPosition;
		SThrust ThrusterB, ThrusterC, ThrusterD, ThrusterE;
		double dT;

		SQuad(SVector CurrentPosition, SDirAngle CurrentRotation, SVector TargetPosition, SThrust TB, SThrust TC, SThrust TD, SThrust TE, double dT) {
			this->CurrentPosition = CurrentPosition;
			this->CurrentRotation = CurrentRotation;
			this->TargetPosition = TargetPosition;
			this->ThrusterB = TB;
			this->ThrusterC = TC;
			this->ThrusterD = TD;
			this->ThrusterE = TE;
			this->dT = dT;
		}
	};
}