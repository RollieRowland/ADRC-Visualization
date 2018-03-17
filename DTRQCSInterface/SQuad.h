#pragma once
#include "stdafx.h"

#include <SVector.h>
#include <SDirAngle.h>
#include <SThrust.h>

namespace DTRQCSInterface {
	public ref class SQuad {
	private:
		SVector CurrentPosition;
		SDirAngle CurrentRotation;
		SVector TargetPosition;
		SThrust TB, TC, TD, TE;

	public:
		SQuad() {}

		SQuad(const SQuad% sQ) {
			this->CurrentPosition = sQ.CurrentPosition;
			this->CurrentRotation = sQ.CurrentRotation;
			this->TargetPosition = sQ.TargetPosition;

			this->TB = sQ.TB;
			this->TC = sQ.TC;
			this->TD = sQ.TD;
			this->TE = sQ.TE;
		}

		SQuad(SVector CurrentPosition, SDirAngle CurrentRotation, SVector TargetPosition, SThrust TB, SThrust TC, SThrust TD, SThrust TE) {
			this->CurrentPosition = CurrentPosition;
			this->CurrentRotation = CurrentRotation;
			this->TargetPosition = TargetPosition;
			this->TB = TB;
			this->TC = TC;
			this->TD = TD;
			this->TE = TE;
		}

		SVector GetCurrentPosition() {
			return CurrentPosition;
		}

		SDirAngle GetCurrentRotation() {
			return CurrentRotation;
		}

		SVector GetTargetPosition() {
			return TargetPosition;
		}

		SThrust GetTB() {
			return TB;
		}

		SThrust GetTC() {
			return TC;
		}

		SThrust GetTD() {
			return TD;
		}

		SThrust GetTE() {
			return TE;
		}
	};
}