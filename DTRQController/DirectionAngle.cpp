#include <DirectionAngle.h>

DirectionAngle::DirectionAngle(double rotation, double x, double y, double z) {
	Rotation = rotation;
	Direction = Vector3D(x, y, z);
}

DirectionAngle::DirectionAngle(double rotation, Vector3D direction) {
	Rotation = rotation;
	Direction = direction;
}

std::string DirectionAngle::ToString() {
	std::string r = Math::DoubleToCleanString(Rotation);
	std::string x = Math::DoubleToCleanString(Direction.X);
	std::string y = Math::DoubleToCleanString(Direction.Y);
	std::string z = Math::DoubleToCleanString(Direction.Z);

	return r + ": [" + x + " " + y + " " + z + "]";
}