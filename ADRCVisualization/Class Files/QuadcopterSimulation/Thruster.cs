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

        private VectorFeedbackController ThrustController;
        private string name;
        private double samplingPeriod;
        
        public Thruster(Vector QuadCenterOffset, string name, double samplingPeriod)
        {
            this.QuadCenterOffset = QuadCenterOffset;
            this.name = name;
            this.samplingPeriod = samplingPeriod;

            secondaryJoint = new Servo(samplingPeriod, 150, 1);//X
            propellor = new Motor(samplingPeriod, 250, 1);//Y
            primaryJoint = new Servo(samplingPeriod, 150, 1);//Z
            
            ThrustController = new VectorFeedbackController()
            {
                X = new ADRC(650, 12500, 1, 2, 45)
                {
                    PID = new PID(15, 0, 7.5, 1000)
                },
                Y = new ADRC(1500, 0.001, 0.75, 0.01, 100)
                {
                    PID = new PID(10, 0, 6, 1000)
                },
                Z = new ADRC(650, 12500, 1, 2, 45)
                {
                    PID = new PID(15, 0, 7.5, 1000)
                }
            };

            CurrentRotation = new Vector(0, 0, 0);
            TargetRotation = new Vector(0, 0, 0);
            CurrentPosition = new Vector(0, 0, 0);
            TargetPosition = new Vector(0, 0, 0);
        }

        public Vector Calculate(Vector rotation, Vector offset)
        {
            ADRC X = (ADRC)ThrustController.X;
            ADRC Y = (ADRC)ThrustController.Y;
            ADRC Z = (ADRC)ThrustController.Z;

            //dynamic control of the maximum output, dependent on the current angle of the quad
            Z.SetOffset(-rotation.X);//Adjusts max outputs of thrusters to allow more 
            X.SetOffset(rotation.Z);//than 45 degree rotation
            
            ThrustController.X = X;
            ThrustController.Y = Y;
            ThrustController.Z = Z;
            
            //Base on previous offset
            //increase output if error is stationary
            //decrease output if error is decreasing

            //Thrust compensated for the change required when rotating in the xyz dimensions
            Vector thrust = ThrustController.Calculate(TargetPosition, CurrentPosition, samplingPeriod);//thrust output in XYZ dimension

            //AdjustThrust(thrust, rotation);

            //Use offset angle to compensate for Y output, add to Y position
            thrust = Matrix.RotateVector(new Vector(0, rotation.Y, 0), thrust);//adjust thruster output to quad frame, only Y dimension

            //Normalize secondary rotation control
            offset.Y = offset.Y < 0 ? 0 : offset.Y;

            //Combine quad rotation output with individual thruster output
            thrust = thrust.Add(offset);
            //thrust = offset;

            //Normalize thrust output
            thrust.Y = thrust.Y < 0 ? 0 : thrust.Y;
            
            //Sets current rotation of thruster for use in the visualization of the quad
            CurrentRotation = new Vector(-primaryJoint.GetAngle(), 0, -secondaryJoint.GetAngle());

            //Sets the outputs of the thrusters
            secondaryJoint.SetAngle(thrust.X);
            propellor.SetOutput(thrust.Y);
            primaryJoint.SetAngle(thrust.Z);
            
            return thrust;
        }

        public Vector ReturnThrustVector()
        {
            Vector thrustVector = new Vector(0, propellor.GetOutput(), 0);
            Vector rotationVector = new Vector(-primaryJoint.GetAngle(), 0, secondaryJoint.GetAngle());

            thrustVector = Matrix.RotateVector(rotationVector, thrustVector);

            return thrustVector;
        }

        public Vector AdjustThrust(Vector thrust, Vector rotation)
        {
            //Calculate angle offset of combined X and Z vector, given both output angles
            double xThrustAngle = thrust.X - rotation.Z;
            double zThrustAngle = thrust.Z + rotation.X;
            double combinedThrustAngle = Math.Sqrt(Math.Pow(xThrustAngle, 2) + Math.Pow(zThrustAngle, 2));//Combined angle of X and Z dimensions

            double adjustedThrustOutput  = 0;//Adjusted vertical output, if the X and Z components are changed
            double xAdjustedThrustOutput = 0;
            double zAdjustedThrustOutput = 0;

            if (combinedThrustAngle > 0)
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
                thrust.Y = adjustedThrustOutput;
                thrust.X = Misc.RadiansToDegrees(Misc.RadiansToDegrees(xAdjustedThrustOutput));
                thrust.Z = Misc.RadiansToDegrees(Misc.RadiansToDegrees(zAdjustedThrustOutput));

                thrust.X += rotation.Z;
                thrust.Z -= rotation.X;
            }

            return thrust;
        }
    }
}
