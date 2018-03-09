#pragma once

#include <string>
#include <iomanip>
#include <sstream>
#include <math.h>
#include <Vector.h>
#include <stdarg.h>

using namespace std;

typedef struct Math {
	static double PI;

	static double Constrain(double value, double minimum, double maximum);
	static double DegreesToRadians(double degrees);
	static double RadiansToDegrees(double radians);
	static Vector3D DegreesToRadians(Vector3D degrees);
	static Vector3D RadiansToDegrees(Vector3D radians);
	static string DoubleToCleanString(double value);
	static void CleanPrint(int values, ...);
} Math;