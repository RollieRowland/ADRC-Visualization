#include <Thruster.h>

Thruster::Thruster() {
	this->ThrusterOffset = Vector3D(0, 0, 0);
	this->name = "null";

	this->CurrentPosition = Vector3D(0, 0, 0);
	this->TargetPosition = Vector3D(0, 0, 0);
	this->CurrentRotation = Vector3D(0, 0, 0);
	this->Disable = false;
}

Thruster::Thruster(Vector3D thrusterOffset, std::string name) {
	this->ThrusterOffset = thrusterOffset;
	this->name = name;

	this->CurrentPosition = Vector3D(0, 0, 0);
	this->TargetPosition = Vector3D(0, 0, 0);
	this->CurrentRotation = Vector3D(0, 0, 0);
	this->Disable = false;
}

Vector3D Thruster::ReturnThrustVector() {
	Vector3D thrustVector = Vector3D(0, rotor.GetOutput(), 0);
	Vector3D rotationVector = Vector3D(-outerJoint.GetAngle(), 0, innerJoint.GetAngle());

	thrustVector = RotationMatrix::RotateVector(rotationVector, thrustVector);

	return thrustVector;
}

Vector3D Thruster::ReturnThrusterOutput() {
	return Vector3D(outerJoint.GetAngle(), rotor.GetOutput(), innerJoint.GetAngle());
}

void Thruster::SetThrusterOutputs(Vector3D output) {
	//Disable negative thrust output
	CheckIfDisabled();

	output.X = Disable ? 0 : output.X;
	output.Y = Disable ? 0 : output.Y;
	output.Z = Disable ? 0 : output.Z;

	//Sets current rotation of thruster for use in the visualization of the quad
	CurrentRotation = Vector3D(-outerJoint.GetAngle(), 0, -innerJoint.GetAngle());

	//Sets the outputs of the thrusters
	innerJoint.SetAngle(output.X);
	rotor.SetOutput(output.Y);
	outerJoint.SetAngle(output.Z);
}

bool Thruster::CheckIfDisabled() {
	//setDisabled if dshot
	rotor.CheckESC();
}

bool Thruster::IsDisabled() {
	return IsDisabled;
}