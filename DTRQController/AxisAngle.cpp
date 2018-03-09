#include <AxisAngle.h>


AxisAngle::AxisAngle(double rotation, double x, double y, double z) {
	Rotation = rotation;
	Axis = Vector3D(x, y, z);
}

AxisAngle::AxisAngle(double rotation, Vector3D axis) {
	Rotation = rotation;
	Axis = axis;
}

AxisAngle AxisAngle::QuaternionToAxisAngle(Quaternion quaternion) {
	AxisAngle axisAngle = AxisAngle(0, 0, 1, 0);

	quaternion = (abs(quaternion.W) > 1.0) ? quaternion.UnitQuaternion() : quaternion;

	axisAngle.Rotation = Math::RadiansToDegrees(2.0 * acos(quaternion.W));

	double quaternionCheck = sqrt(1.0 - pow(quaternion.W, 2.0));//Prevents rotation jumps, and division by zero

	if (quaternionCheck >= 0.001)//Prevents division by zero
	{
		//Normalizes axis
		axisAngle.Axis;
		axisAngle.Axis.X = quaternion.X / quaternionCheck;
		axisAngle.Axis.Y = quaternion.Y / quaternionCheck;
		axisAngle.Axis.Z = quaternion.Z / quaternionCheck;
	}
	else
	{
		//If X is close to zero the axis doesn't matter
		axisAngle.Axis;
		axisAngle.Axis.X = 0.0;
		axisAngle.Axis.Y = 1.0;
		axisAngle.Axis.Z = 0.0;
	}

	return axisAngle;
}

string AxisAngle::ToString() {
	string r = Math::DoubleToCleanString(Rotation);
	string x = Math::DoubleToCleanString(Axis.X);
	string y = Math::DoubleToCleanString(Axis.Y);
	string z = Math::DoubleToCleanString(Axis.Z);

	return r + ": [" + x + " " + y + " " + z + "]";
}