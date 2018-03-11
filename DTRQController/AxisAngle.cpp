#include <AxisAngle.h>


AxisAngle::AxisAngle(double rotation, double x, double y, double z) {
	Rotation = rotation;
	Axis = Vector3D(x, y, z);
}

AxisAngle::AxisAngle(double rotation, Vector3D axis) {
	Rotation = rotation;
	Axis = axis;
}

string AxisAngle::ToString() {
	string r = Math::DoubleToCleanString(Rotation);
	string x = Math::DoubleToCleanString(Axis.X);
	string y = Math::DoubleToCleanString(Axis.Y);
	string z = Math::DoubleToCleanString(Axis.Z);

	return r + ": [" + x + " " + y + " " + z + "]";
}