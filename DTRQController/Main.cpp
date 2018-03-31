#include <iostream>
#include <cstdio>
#include "Quadcopter.h"
#include "Vector.h"
#include <windows.h>

int main()
{
	std::cout << "Creating Quadcopter Object." << std::endl;

	VectorFeedbackController *pos = new VectorFeedbackController{
		new PID{ 10, 0, 12.5 },
		new PID{ 1, 0, 0.2 },
		new PID{ 10, 0, 12.5 }
	};

	VectorFeedbackController *rot = new VectorFeedbackController{
		new PID{ 0.05, 0, 0.325 },
		new PID{ 0.05, 0, 0.325 },
		new PID{ 0.05, 0, 0.325 }
	};

	Quadcopter q = Quadcopter(true, 0.3, 55, 0.05, pos, rot);

	while (true) {
		q.SetTarget(Vector3D(0, 0, 0), Rotation(DirectionAngle(0, Vector3D(0, 1, 0))));
		q.SimulateCurrent(Vector3D(0, -9.81, 0));
		q.CalculateCombinedThrustVector();

		//std::cout << q.TB.ReturnThrusterOutput().ToString() << std::endl;

		//std::cout << q.CurrentPosition.ToString() << "  " << q.CurrentRotation.GetDirectionAngle().ToString() << std::endl;
		Sleep(5);
	}


	system("PAUSE");
}