using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.Mathematics;
using ADRCVisualization.Class_Files.FeedbackControl;

namespace ADRCVisualization.Class_Files.QuadcopterSimulation
{
    public class Thruster
    {
        private Servo secondaryJoint;//X
        private Motor propellor;//Y
        private Servo primaryJoint;//Z
        
        public Vector TargetPosition { get; set; }
        public Vector CurrentPosition { get; set; }

        public Vector TargetRotation { get; set; }
        public Vector CurrentRotation { get; set; }
        public Vector QuadCenterOffset { get; }
        public bool Disable { get; set; }
        
        private string name;
        private double samplingPeriod;
        
        public Thruster(Vector QuadCenterOffset, string name, double samplingPeriod)
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
            Vector thrust = new Vector (offset);

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

            thrustVector = Matrix.RotateVector(rotationVector, thrustVector);

            return thrustVector;
        }

        public Vector ReturnThrusterOutput()
        {
            return new Vector(primaryJoint.GetAngle(), propellor.GetOutput(), secondaryJoint.GetAngle());
        }

        public Vector AdjustThrust(Vector thrust, Vector rotation)
        {
            thrust = new Vector(thrust);
            rotation = new Vector(rotation);

            //Calculate angle offset of combined X and Z vector, given both output angles
            double xThrustAngle = thrust.X - rotation.Z;
            double zThrustAngle = thrust.Z + rotation.X;
            double combinedThrustAngle = Math.Sqrt(Math.Pow(xThrustAngle, 2) + Math.Pow(zThrustAngle, 2));//Combined angle of X and Z dimensions

            double adjustedThrustOutput  = 0;//Adjusted vertical output, if the X and Z components are changed
            double xAdjustedThrustOutput = 0;
            double zAdjustedThrustOutput = 0;

            if (combinedThrustAngle > 0)
            {
                adjustedThrustOutput  = thrust.Y / Math.Sin(MathE.DegreesToRadians(combinedThrustAngle));

                xAdjustedThrustOutput = Math.Sin(MathE.DegreesToRadians(thrust.Y * Math.Asin(MathE.DegreesToRadians(xThrustAngle))) / thrust.Y);
                zAdjustedThrustOutput = Math.Sin(MathE.DegreesToRadians(thrust.Y * Math.Asin(MathE.DegreesToRadians(zThrustAngle))) / thrust.Y);
            }
            else
            {
                adjustedThrustOutput  = thrust.Y;
                xAdjustedThrustOutput = xThrustAngle;
                zAdjustedThrustOutput = zThrustAngle;
            }
            
            if (adjustedThrustOutput != 0 && adjustedThrustOutput != thrust.Y)
            {
                thrust.Y = adjustedThrustOutput;
                thrust.X = MathE.RadiansToDegrees(MathE.RadiansToDegrees(xAdjustedThrustOutput));
                thrust.Z = MathE.RadiansToDegrees(MathE.RadiansToDegrees(zAdjustedThrustOutput));

                thrust.X += rotation.Z;
                thrust.Z -= rotation.X;
            }

            return thrust;
        }
    }
}
