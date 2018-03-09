#include <Math.h>

string Math::DoubleToCleanString(double value) {
	stringstream stream;

	stream << fixed << setprecision(2) << setw(10) << value;

	return stream.str();
}