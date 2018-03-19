#include "stdafx.h"
#include <iostream>
#include <windows.h>
#include <DTRQCSInterface.cpp>

using namespace DTRQCSInterface;

int main()
{
	std::cout << "Creating DTRQCS Object." << std::endl;

	DTRQQuadcopter^ q = gcnew DTRQQuadcopter(true, 0.3, 55, 0.05);

	q->SimulateCurrent(SVector(0, -9.81, 0));

	SVector targetPosition = SVector(0, 0, 0);
	SDirAngle targetRotation = SDirAngle(0, 0, 1, 0);

	q->SetTarget(targetPosition, targetRotation);

	while (true) {

		q->CalculateCombinedThrustVector();//Secondary Solver

		q->SetTarget(targetPosition, targetRotation);
		q->SimulateCurrent(SVector(0, -9.81, 0));

		std::cout << q->GetQuadcopter().CurrentPosition.X << " "
					<< q->GetQuadcopter().CurrentPosition.Y << " "
					<< q->GetQuadcopter().CurrentPosition.Z << std::endl;

		Sleep(5);
	}

	system("PAUSE");
}