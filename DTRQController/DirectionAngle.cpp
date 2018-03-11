#include <DirectionAngle.h>

DirectionAngle::DirectionAngle(double rotation, double x, double y, double z) {
	Rotation = rotation;
	Direction = Vector3D(x, y, z);
}

DirectionAngle::DirectionAngle(double rotation, Vector3D direction) {
	Rotation = rotation;
	Direction = direction;
}

string DirectionAngle::ToString() {
	string r = Math::DoubleToCleanString(Rotation);
	string x = Math::DoubleToCleanString(Direction.X);
	string y = Math::DoubleToCleanString(Direction.Y);
	string z = Math::DoubleToCleanString(Direction.Z);

	return r + ": [" + x + " " + y + " " + z + "]";

}