#include <Math.h>
#include <iostream>

double Math::PI = atan(1) * 4;

double Math::Constrain(double value, double minimum, double maximum) {
	if (value > maximum)
	{
		value = maximum;
	}
	else if (value < minimum)
	{
		value = minimum;
	}

	return value;
}

double Math::DegreesToRadians(double degrees) {
	return degrees / (180.0 / PI);
}

double Math::RadiansToDegrees(double radians) {
	return radians * (180.0 / PI);
}

Vector3D Math::DegreesToRadians(Vector3D degrees) {
	return degrees / (180.0 / PI);
}

Vector3D Math::RadiansToDegrees(Vector3D radians) {
	return radians * (180.0 / PI);
}

string Math::DoubleToCleanString(double value) {
	stringstream stream;

	stream << fixed << setprecision(2) << setw(10) << value;

	return stream.str();
}

void Math::CleanPrint(int values, ...) {
	va_list valueList;
	string printOut = "";

	va_start(valueList, values);

	for (int i = 0; i <= values; i++) {
		double value = va_arg(valueList, double);
		printOut += Math::DoubleToCleanString(value);
	}

	va_end(valueList);

	cout << printOut << endl;
}