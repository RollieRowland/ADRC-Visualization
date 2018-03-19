#include <Quadcopter.h>

Quadcopter::Quadcopter(bool simulation, double armLength, double armAngle, double dT) {
	std::cout << "DTRQ Controller Initializing." << std::endl;

	this->simulation = simulation;
	this->armLength = armLength;
	this->armAngle = armAngle;
	this->dT = dT;
	this->gimbalLockFader = TriangleWaveFader(8, 90);

	this->externalAcceleration = Vector3D(0, -9.81, 0);
	this->currentVelocity = Vector3D(0, 0, 0);
	this->currentAngularVelocity = Vector3D(0, 0, 0);
	this->currentAngularAcceleration = Vector3D(0, 0, 0);
	this->currentAcceleration = Vector3D(0, 0, 0);

	this->CurrentPosition = Vector3D(0, 0, 0);
	this->TargetPosition = Vector3D(0, 0, 0);
	this->CurrentRotation = Rotation(Quaternion(1, 0, 0, 0));
	this->TargetRotation = Rotation(Quaternion(1, 0, 0, 0));

	std::cout << "Calculating Quadcopter Arm Positions." << std::endl;

	CalculateArmPositions(armLength, armAngle);

	std::cout << "DTRQ Initialized, ready for commands." << std::endl;
}

void Quadcopter::CalculateArmPositions(double armLength, double armAngle) {
	double XLength = armLength * cos(Mathematics::DegreesToRadians(armAngle));
	double ZLength = armLength * sin(Mathematics::DegreesToRadians(armAngle));

	std::cout << "Quadcopter Thruster Offset: X:" + Mathematics::DoubleToCleanString(XLength) +
		" Z:" + Mathematics::DoubleToCleanString(ZLength) +
		" dT:" + Mathematics::DoubleToCleanString(dT) << std::endl;

	TB = Thruster(Vector3D(-XLength, 0, ZLength), "TB", simulation, dT);
	TC = Thruster(Vector3D(XLength, 0, ZLength), "TC", simulation, dT);
	TD = Thruster(Vector3D(XLength, 0, -ZLength), "TD", simulation, dT);
	TE = Thruster(Vector3D(-XLength, 0, -ZLength), "TE", simulation, dT);
}

void Quadcopter::CalculateCombinedThrustVector() {
	//Omega = 2 * (qt - qc) * qc^-1 / dt -> only bivector quantity, real value is disregarded
	Vector3D change = (2 * (TargetRotation.GetQuaternion() - CurrentRotation.GetQuaternion()) * CurrentRotation.GetQuaternion().Conjugate() / dT).GetBiVector();

	Vector3D rotationOutput = rotationController.Calculate(Vector3D(0, 0, 0), change, dT);
	Vector3D positionOutput = positionController.Calculate(Vector3D(0, 0, 0), CurrentPosition.Subtract(TargetPosition), dT);

	positionOutput = positionOutput.Constrain(-30, 30);
	rotationOutput = rotationOutput.Constrain(-30, 30);

	//std::cout << positionOutput.ToString() + " " + CurrentPosition.Subtract(TargetPosition).ToString() << std::endl;

	//Thruster output relative to environment origin
	Vector3D thrusterOutputB = Vector3D(-rotationOutput.Y, -rotationOutput.X + rotationOutput.Z, -rotationOutput.Y);
	Vector3D thrusterOutputC = Vector3D(-rotationOutput.Y, -rotationOutput.X - rotationOutput.Z, rotationOutput.Y);
	Vector3D thrusterOutputD = Vector3D(rotationOutput.Y, rotationOutput.X - rotationOutput.Z, rotationOutput.Y);
	Vector3D thrusterOutputE = Vector3D(rotationOutput.Y, rotationOutput.X + rotationOutput.Z, -rotationOutput.Y);

	Vector3D hoverAngles = RotationQuaternionToHoverAngles(CurrentRotation);

	positionOutput = CurrentRotation.GetQuaternion().RotateVector(positionOutput);

	//Due to XYZ permutation order of Euler angle
	positionOutput.X = positionOutput.X + hoverAngles.Z;//Adjust main joint to rotation
	positionOutput.Z = positionOutput.Z - hoverAngles.X;//Adjust secondary joint to rotation

	TB.SetThrusterOutputs(thrusterOutputB.Add(positionOutput));
	TC.SetThrusterOutputs(thrusterOutputC.Add(positionOutput));
	TD.SetThrusterOutputs(thrusterOutputD.Add(positionOutput));
	TE.SetThrusterOutputs(thrusterOutputE.Add(positionOutput));
}

void Quadcopter::SetTarget(Vector3D position, Rotation rotation) {
	TargetPosition = position;
	TargetRotation = rotation;

	TB.TargetPosition = TargetRotation.GetQuaternion().RotateVector(TB.ThrusterOffset).Add(TargetPosition);
	TC.TargetPosition = TargetRotation.GetQuaternion().RotateVector(TC.ThrusterOffset).Add(TargetPosition);
	TD.TargetPosition = TargetRotation.GetQuaternion().RotateVector(TD.ThrusterOffset).Add(TargetPosition);
	TE.TargetPosition = TargetRotation.GetQuaternion().RotateVector(TE.ThrusterOffset).Add(TargetPosition);
}

void Quadcopter::SimulateCurrent(Vector3D externalAcceleration) {
	this->externalAcceleration = externalAcceleration;
	EstimatePosition();
	EstimateRotation();

	TB.CurrentPosition = CurrentRotation.GetQuaternion().RotateVector(TB.ThrusterOffset).Add(CurrentPosition);
	TC.CurrentPosition = CurrentRotation.GetQuaternion().RotateVector(TC.ThrusterOffset).Add(CurrentPosition);
	TD.CurrentPosition = CurrentRotation.GetQuaternion().RotateVector(TD.ThrusterOffset).Add(CurrentPosition);
	TE.CurrentPosition = CurrentRotation.GetQuaternion().RotateVector(TE.ThrusterOffset).Add(CurrentPosition);
}

void Quadcopter::EstimatePosition() {
	Vector3D TBO = TB.ReturnThrusterOutput();
	Vector3D TCO = TC.ReturnThrusterOutput();
	Vector3D TDO = TD.ReturnThrusterOutput();
	Vector3D TEO = TE.ReturnThrusterOutput();

	Vector3D TBThrust = Vector3D(0, TBO.Y, 0);
	Vector3D TCThrust = Vector3D(0, TCO.Y, 0);
	Vector3D TDThrust = Vector3D(0, TDO.Y, 0);
	Vector3D TEThrust = Vector3D(0, TEO.Y, 0);

	Quaternion TBR = Rotation(EulerAngles(Vector3D(TBO.X, 0, -TBO.Z), EulerConstants::EulerOrderZYXS)).GetQuaternion();
	Quaternion TCR = Rotation(EulerAngles(Vector3D(TCO.X, 0, -TCO.Z), EulerConstants::EulerOrderZYXS)).GetQuaternion();
	Quaternion TDR = Rotation(EulerAngles(Vector3D(TDO.X, 0, -TDO.Z), EulerConstants::EulerOrderZYXS)).GetQuaternion();
	Quaternion TER = Rotation(EulerAngles(Vector3D(TEO.X, 0, -TEO.Z), EulerConstants::EulerOrderZYXS)).GetQuaternion();

	TBThrust = TBR.RotateVector(TBThrust);
	TCThrust = TCR.RotateVector(TCThrust);
	TDThrust = TDR.RotateVector(TDThrust);
	TEThrust = TER.RotateVector(TEThrust);

	Vector3D thrustSum = TBThrust.Add(TBThrust).Add(TBThrust).Add(TBThrust);

	thrustSum = CurrentRotation.GetQuaternion().RotateVector(thrustSum);

	currentAcceleration = thrustSum.Add(externalAcceleration);
	currentVelocity = currentVelocity.Add(currentAcceleration.Multiply(dT));

	CurrentPosition = CurrentPosition.Add(currentVelocity.Multiply(dT));
}

void Quadcopter::EstimateRotation() {
	Vector3D TBO = TB.ReturnThrustVector();//Thrust relative to quad origin
	Vector3D TCO = TC.ReturnThrustVector();
	Vector3D TDO = TD.ReturnThrustVector();
	Vector3D TEO = TE.ReturnThrustVector();

	//Rotate Thrust Vector about current quaternion rotation
	TBO = CurrentRotation.GetQuaternion().RotateVector(TBO);//Thrust relative to world origin
	TCO = CurrentRotation.GetQuaternion().RotateVector(TCO);
	TDO = CurrentRotation.GetQuaternion().RotateVector(TDO);
	TEO = CurrentRotation.GetQuaternion().RotateVector(TEO);

	double torque = armLength * sin(Mathematics::DegreesToRadians(180 - armAngle)) * 5;

	//calculate current inertia tensor
	currentAngularAcceleration = Vector3D {
		( TBO.Y + TCO.Y - TDO.Y - TEO.Y) * torque,
		( TBO.X + TCO.X - TDO.X - TEO.X) * torque + 
		( TBO.Z - TCO.Z - TDO.Z + TEO.Z) * torque,
		(-TBO.Y + TCO.Y + TDO.Y - TEO.Y) * torque
	};

	//TB + TD - (TC + TE)
	Vector3D differentialThrustRotation = TBO.Add(TDO).Add(TCO.Add(TEO).Multiply(-1)).Multiply(0.15);
	currentAngularAcceleration = Vector3D::DegreesToRadians(currentAngularAcceleration);
	currentAngularVelocity = currentAngularVelocity + currentAngularAcceleration * dT;

	Quaternion angularRotation = Quaternion(currentAngularVelocity * dT * 0.5);

	CurrentRotation = CurrentRotation.GetQuaternion() + angularRotation * CurrentRotation.GetQuaternion();
	CurrentRotation = Rotation(CurrentRotation.GetQuaternion().UnitQuaternion());
}

Vector3D Quadcopter::RotationQuaternionToHoverAngles(Rotation rotation) {
	double innerJoint = 0;
	double outerJoint = 0;
	DirectionAngle directionAngle = rotation.GetDirectionAngle();

	directionAngle.Direction = RotationMatrix::RotateVector(Vector3D(0, -90, 0), directionAngle.Direction);

	Vector3D directionVector = Vector3D(directionAngle.Direction);

	directionVector = RotationMatrix::RotateVector(Vector3D(0, directionAngle.Rotation, 0), directionVector);

	//These are cartesian coordinates, convert them to the angle from 1, 0 to the point it is at
	outerJoint = Mathematics::RadiansToDegrees(asin(directionVector.Z));
	innerJoint = Mathematics::RadiansToDegrees(atan2(directionVector.X, directionVector.Y));

	return Vector3D(outerJoint, 0, innerJoint);
}

void Quadcopter::CalculateGimbalLockedMotion(Vector3D &positionControl, Vector3D &thrusterOutputB,
											 Vector3D &thrusterOutputC, Vector3D &thrusterOutputD,
											 Vector3D &thrusterOutputE) {
	Vector3D hoverAngles = RotationQuaternionToHoverAngles(CurrentRotation);

	double fadeIn = gimbalLockFader.CalculateRatio(hoverAngles.Z);//0 -> 1, New position control faders, approaches 1 when z rot is -90/90
	double fadeOut = 1 - fadeIn;//1 -> 0, Rotation magnitude faders, approaches 0 when Z rot is -90/90 degrees

	double rotation = 40 * fadeIn;

	double magnitude = sqrt(pow(positionControl.X, 2) + pow(positionControl.Z, 2));//Give hypotenuse for origin rotation, magnitude
	double angle = Mathematics::RadiansToDegrees(Mathematics::Sign(hoverAngles.Z) * atan2(magnitude, 0) - atan2(positionControl.Z, positionControl.X));//Determine angle of output, -180 -> 180
																													  //Rotation matrix on position control copy
	//Vector3D RotatedControl = RotationMatrix::RotateVector(Vector3D(0, CurrentEulerRotation.Y, 0), Vector3D(positionControl.X, 0, positionControl.Z));
	Vector3D rotatedControl = CurrentRotation.GetQuaternion().RotateVector(positionControl);

	//---- (X-), ++++ (X+), +-+- (Z+), -+-+ (Z-)
	thrusterOutputB.X = thrusterOutputB.X * fadeOut + (rotatedControl.X * fadeIn) + (rotatedControl.Z * fadeIn);
	thrusterOutputC.X = thrusterOutputC.X * fadeOut + (rotatedControl.X * fadeIn) - (rotatedControl.Z * fadeIn);
	thrusterOutputD.X = thrusterOutputD.X * fadeOut + (rotatedControl.X * fadeIn) + (rotatedControl.Z * fadeIn);
	thrusterOutputE.X = thrusterOutputE.X * fadeOut + (rotatedControl.X * fadeIn) - (rotatedControl.Z * fadeIn);

	//Alternate Z rotation in individual thrusters, preventing rotation bias
	thrusterOutputB.Z = thrusterOutputB.Z * fadeOut + rotation;
	thrusterOutputC.Z = thrusterOutputC.Z * fadeOut - rotation;
	thrusterOutputD.Z = thrusterOutputD.Z * fadeOut + rotation;
	thrusterOutputE.Z = thrusterOutputE.Z * fadeOut - rotation;

	positionControl.X *= fadeOut;
	positionControl.Z *= fadeOut;
}

void Quadcopter::GetCurrent() {
	//CurrentPosition = mpu.GetCurrentPosition();
	//CurrentRotation = mpu.GetCurrentRotation();

	TB.CurrentPosition = CurrentRotation.GetQuaternion().RotateVector(TB.ThrusterOffset).Add(CurrentPosition);
	TC.CurrentPosition = CurrentRotation.GetQuaternion().RotateVector(TC.ThrusterOffset).Add(CurrentPosition);
	TD.CurrentPosition = CurrentRotation.GetQuaternion().RotateVector(TD.ThrusterOffset).Add(CurrentPosition);
	TE.CurrentPosition = CurrentRotation.GetQuaternion().RotateVector(TE.ThrusterOffset).Add(CurrentPosition);
}