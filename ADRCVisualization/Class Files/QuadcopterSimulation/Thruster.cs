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
        private Motor propellor;
        private Servo primaryJoint;
        private Servo secondaryJoint;

        private VectorKalmanFilter thrustKF;
        
        public Vector TargetPosition { get; set; }
        public Vector CurrentPosition { get; set; }

        public Vector TargetRotation { get; set; }
        public Vector CurrentRotation { get; set; }
        public Vector QuadCenterOffset { get; }

        private VectorFeedbackController ThrustController;
        private string name;
        
        public Thruster(Vector QuadCenterOffset, string name)
        {
            this.QuadCenterOffset = QuadCenterOffset;
            this.name = name;
            
            propellor = new Motor();
            primaryJoint = new Servo();
            secondaryJoint = new Servo();
            
            ThrustController = new VectorFeedbackController()
            {
                X = new ADRC(100, 0.001, 0.25, 0.01, 30)
                {
                    PID = new PID(30, 0, 30, 1000)
                },
                Y = new ADRC(1000, 0.001, 0.75, 0.01, 100)
                {
                    PID = new PID(10, 0, 5, 1000)
                },
                Z = new ADRC(100, 0.001, 0.25, 0.01, 30)
                {
                    PID = new PID(30, 0, 30, 1000)
                }
            };

            thrustKF = new VectorKalmanFilter(new Vector(0.2, 0.2, 0.2), new Vector(25, 25, 25));//Increase memory to decrease response time

            CurrentRotation = new Vector(0, 0, 0);
            TargetRotation = new Vector(0, 0, 0);
            CurrentPosition = new Vector(0, 0, 0);
            TargetPosition = new Vector(0, 0, 0);
        }

        public Vector Calculate(Vector rotation, Vector offset)
        {
            //dynamic control of the maximum output, dependent on the current angle of the quad
            ((ADRC)ThrustController.Z).SetOffset(-rotation.X);//Adjusts max outputs of thrusters to allow more 
            ((ADRC)ThrustController.X).SetOffset(rotation.Z);//than 45 degree rotation

            //Thrust compensated for the change required when rotating in the xyz dimensions
            Vector thrust = ThrustController.Calculate(TargetPosition, CurrentPosition);//thrust output in XYZ dimension

            //Calculate angle offset of combined X and Z vector, given both output angles

            double xThrustAngle = thrust.X - rotation.Z;
            double zThrustAngle = thrust.Z + rotation.X;
            double combinedThrustAngle = Math.Sqrt(Math.Pow(xThrustAngle, 2) + Math.Pow(zThrustAngle, 2));//Combined angle of X and Z dimensions

            double adjustedThrustOutput  = 0;//Adjusted vertical output, if the X and Z components are changed
            double xAdjustedThrustOutput = 0;
            double zAdjustedThrustOutput = 0;

            if (combinedThrustAngle > 1)
            {
                adjustedThrustOutput  = thrust.Y / Math.Sin(Misc.DegreesToRadians(combinedThrustAngle));

                xAdjustedThrustOutput = Math.Sin(Misc.DegreesToRadians(thrust.Y * Math.Asin(Misc.DegreesToRadians(xThrustAngle))) / thrust.Y);
                zAdjustedThrustOutput = Math.Sin(Misc.DegreesToRadians(thrust.Y * Math.Asin(Misc.DegreesToRadians(zThrustAngle))) / thrust.Y);
            }
            else
            {
                adjustedThrustOutput  = thrust.Y;
                xAdjustedThrustOutput = xThrustAngle;
                zAdjustedThrustOutput = zThrustAngle;
            }

            if (adjustedThrustOutput != 0 && adjustedThrustOutput != thrust.Y)
            {
                thrust.X = xAdjustedThrustOutput * 58 * 58;
                thrust.Z = zAdjustedThrustOutput * 58 * 58;

                thrust.X += rotation.Z;
                thrust.Z -= rotation.X;

                //if (name == "TB") Console.WriteLine(thrust.X.ToString().PadRight(20) + " " + xAdjustedThrustOutput.ToString().PadRight(20));
            }

            thrust.Y = adjustedThrustOutput;
            
            //Use offset angle to compensate for Y output, add to Y position
            thrust = Matrix.RotateVector(new Vector(0, rotation.Y, 0), thrust);//adjust thruster output to quad frame, only Y dimension
            
            offset.Y = offset.Y < 0 ? 0 : offset.Y;

            thrust = thrust.Add(offset);

            thrust.Y = thrust.Y < 0 ? 0 : thrust.Y;

            thrustKF.Filter(thrust);

            CurrentRotation = new Vector(-thrustKF.GetFilteredValue().Z, 0, -thrustKF.GetFilteredValue().X);//for display purposes

            secondaryJoint.SetAngle(thrustKF.GetFilteredValue().X);
            propellor.SetOutput(thrustKF.GetFilteredValue().Y);
            primaryJoint.SetAngle(thrustKF.GetFilteredValue().Z);

            return thrust;
        }

        public Vector ReturnThrustVector()
        {
            Vector thrustVector = new Vector(0, propellor.GetOutput(), 0);
            Vector rotationVector = new Vector(-primaryJoint.GetAngle(), 0, secondaryJoint.GetAngle());

            thrustVector = Matrix.RotateVector(rotationVector, thrustVector);

            //thrustVector = new Vector(secondaryJoint.GetAngle() * 0.1, propellor.GetOutput(), primaryJoint.GetAngle() * 0.1);//old method assumes no thrust loss in Y dimension, calibrate pids

            return thrustVector;
        }
    }
}
