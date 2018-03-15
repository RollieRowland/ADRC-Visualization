#include <AxisAngle.h>

AxisAngle::AxisAngle(double rotation, double x, double y, double z) {
	Rotation = rotation;
	Axis = Vector3D(x, y, z);
}

AxisAngle::AxisAngle(double rotation, Vector3D axis) {
	Rotation = rotation;
	Axis = axis;
}

std::string AxisAngle::ToString() {
	std::string r = Math::DoubleToCleanString(Rotation);
	std::string x = Math::DoubleToCleanString(Axis.X);
	std::string y = Math::DoubleToCleanString(Axis.Y);
	std::string z = Math::DoubleToCleanString(Axis.Z);

	return r + ": [" + x + " " + y + " " + z + "]";
}