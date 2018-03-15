#pragma once

#include <string>
#include <iomanip>
#include <sstream>
//#include <math.h>
#include <Vector.h>
#include <stdarg.h>
#include <iostream>
#include <vector>
//#include <cmath>

class Math {
public:
	static double PI;

	static double Constrain(double value, double minimum, double maximum);
	static double DegreesToRadians(double degrees);
	static double RadiansToDegrees(double radians);
	static Vector3D DegreesToRadians(Vector3D degrees);
	static Vector3D RadiansToDegrees(Vector3D radians);
	static std::string DoubleToCleanString(double value);
	static void CleanPrint(int values, ...);
	static bool IsNaN(double);
	static bool IsInfinite(double);
	static bool IsFinite(double);
	static int Sign(double);
};