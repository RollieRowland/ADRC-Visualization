#include <iostream>
#include <cstdio>
#include <Quadcopter.h>
#include <Vector.h>

int main()
{
	std::cout << "Creating Quadcopter Object." << std::endl;

	Quadcopter q = Quadcopter(1, 40, 0.05);

	while (true) {
		q.SimulateCurrent(Vector3D(0, -9.81, 0));
		q.CalculateCombinedThrustVector();
		std::cout << q.CurrentPosition.ToString() << "  " << q.CurrentRotation.GetDirectionAngle().ToString() << std::endl;
	}


	system("PAUSE");
}