#include <Vector.h>
#include <Thruster.h>




	Thruster::Thruster(Vector QuadCenterOffset, string name, double samplingPeriod)
	{
		this.QuadCenterOffset = new Vector(QuadCenterOffset);
		this.name = name;
		this.samplingPeriod = samplingPeriod;

		Console.WriteLine("Thruster " + name + " initialized at point: " + QuadCenterOffset);

		secondaryJoint = new Servo(samplingPeriod, 150, 1);//X
		propellor = new Motor(samplingPeriod, 250, 1);//Y
		primaryJoint = new Servo(samplingPeriod, 150, 1);//Z

		CurrentRotation = new Vector(0, 0, 0);
		TargetRotation = new Vector(0, 0, 0);
		CurrentPosition = new Vector(0, 0, 0);
		TargetPosition = new Vector(0, 0, 0);
		Disable = false;
	}

	public void Calculate(Vector offset)
	{
		//Combine quad rotation output with individual thruster output
		Vector thrust = new Vector(offset);

		//Disable negative thrust output
		//thrust.Y = thrust.Y < 0 ? 0 : thrust.Y;

		thrust.X = Disable ? 0 : thrust.X;
		thrust.Y = Disable ? 0 : thrust.Y;
		thrust.Z = Disable ? 0 : thrust.Z;

		//Sets current rotation of thruster for use in the visualization of the quad
		CurrentRotation = new Vector(-primaryJoint.GetAngle(), 0, -secondaryJoint.GetAngle());

		//Sets the outputs of the thrusters
		secondaryJoint.SetAngle(thrust.X);
		propellor.SetOutput(thrust.Y);
		primaryJoint.SetAngle(thrust.Z);
	}

	public Vector ReturnThrustVector()
	{
		Vector thrustVector = new Vector(0, propellor.GetOutput(), 0);
		Vector rotationVector = new Vector(-primaryJoint.GetAngle(), 0, secondaryJoint.GetAngle());

		thrustVector = RotationMatrix.RotateVector(rotationVector, thrustVector);

		return thrustVector;
	}

	public Vector ReturnThrusterOutput()
	{
		return new Vector(primaryJoint.GetAngle(), propellor.GetOutput(), secondaryJoint.GetAngle());
	}