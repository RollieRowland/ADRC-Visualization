#include <DirectionAngle.h>

DirectionAngle::DirectionAngle(double rotation, double x, double y, double z) {
	Rotation = rotation;
	Direction = Vector3D(x, y, z);
}

DirectionAngle::DirectionAngle(double rotation, Vector3D direction) {
	Rotation = rotation;
	Direction = direction;
}

DirectionAngle DirectionAngle::QuaternionToDirectionAngle(Quaternion quaternion) {
	Vector3D up = Vector3D(0, 1, 0);//up vector
	Vector3D right = Vector3D(1, 0, 0);
	Vector3D rotatedUp = quaternion.RotateVector(up);//new direction vector
	Vector3D rotatedRight = quaternion.RotateVector(right);
	Quaternion rotationChange = Quaternion::QuaternionFromDirectionVectors(up, rotatedUp);

	//rotate forward vector by direction vector rotation
	Vector3D rightXZCompensated = rotationChange.UnrotateVector(rotatedRight);//should only be two points on circle, compare against right

																			//define angles that define the forward vector, and the rotated then compensated forward vector
	double rightAngle = Math::RadiansToDegrees(atan2(right.Z, right.X));//forward as zero
	double rightRotatedAngle = Math::RadiansToDegrees(atan2(rightXZCompensated.Z, rightXZCompensated.X));//forward as zero

																											  //angle about the axis defined by the direction of the object
	double angle = rightAngle - rightRotatedAngle;

	//returns the angle rotated about the rotated up vector as an axis
	return DirectionAngle(angle, rotatedUp);
}

string DirectionAngle::ToString() {
	string r = Math::DoubleToCleanString(Rotation);
	string x = Math::DoubleToCleanString(Direction.X);
	string y = Math::DoubleToCleanString(Direction.Y);
	string z = Math::DoubleToCleanString(Direction.Z);

	return r + ": [" + x + " " + y + " " + z + "]";

}