#include <Math.h>

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

std::string Math::DoubleToCleanString(double value) {
	std::stringstream stream;

	stream << std::fixed << std::setprecision(2) << std::setw(10) << value;

	return stream.str();
}

void Math::CleanPrint(int values, ...) {
	va_list valueList;
	std::string printOut = "";

	va_start(valueList, values);

	for (int i = 0; i <= values; i++) {
		double value = va_arg(valueList, double);
		printOut += Math::DoubleToCleanString(value);
	}

	va_end(valueList);

	std::cout << printOut << std::endl;
}

bool Math::IsNaN(double value) {
	return value != value;
}

bool Math::IsInfinite(double value) {
	return value == std::numeric_limits<double>::infinity();
}

bool Math::IsFinite(double value) {
	return value != std::numeric_limits<double>::infinity();
}

int Math::Sign(double value) {
		return (0 < value) - (value < 0);
}