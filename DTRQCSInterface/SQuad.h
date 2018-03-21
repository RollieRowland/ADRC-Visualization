#pragma once
#include "stdafx.h"

#include <SVector.h>
#include <SDirAngle.h>
#include <SThrust.h>

namespace DTRQCSInterface {
	public ref class SQuad {
	public:
		SVector^ CurrentPosition;
		SQuaternion^ CurrentRotation;
		SVector^ TargetPosition;
		SThrust^ ThrusterB;
		SThrust^ ThrusterC;
		SThrust^ ThrusterD;
		SThrust^ ThrusterE;
		double dT;

		SQuad(const SQuad% sQ) {
			this->CurrentPosition = sQ.CurrentPosition;
			this->CurrentRotation = sQ.CurrentRotation;
			this->TargetPosition = sQ.TargetPosition;
			this->ThrusterB = sQ.ThrusterB;
			this->ThrusterC = sQ.ThrusterC;
			this->ThrusterD = sQ.ThrusterD;
			this->ThrusterE = sQ.ThrusterE;
			this->dT = sQ.dT;
		}

		SQuad(SVector^ CurrentPosition, SQuaternion^ CurrentRotation, SVector^ TargetPosition, SThrust^ TB, SThrust^ TC, SThrust^ TD, SThrust^ TE, double dT) {
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