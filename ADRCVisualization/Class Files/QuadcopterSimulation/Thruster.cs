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
                X = new ADRC(50, 200, 4, 10, 30)
                {
                    PID = new PID(10, 0, 12.5, 1000)
                },
                Y = new ADRC(10, 10, 1.5, 0.05, 100)
                {
                    PID = new PID(1, 0, 0.2, 1000)
                },
                Z = new ADRC(50, 200, 4, 10, 30)
                {
                    PID = new PID(10, 0, 12.5, 1000)
                }
            };

            CurrentRotation = new Vector(0, 0, 0);
            TargetRotation = new Vector(0, 0, 0);
            CurrentPosition = new Vector(0, 0, 0);
            TargetPosition = new Vector(0, 0, 0);
            Disable = false;
        }

        public Vector Calculate(Vector rotation, Vector offset)
        {
            /*
            //dynamic control of the maximum output, dependent on the current angle of the quad
            string test = ((ADRC)ThrustController.X).SetOffset(rotation.Z);//Adjusts max outputs of thrusters to allow more 
            string test2 = ((ADRC)ThrustController.Z).SetOffset(-rotation.X);//than 45 degree rotation

            //Thrust compensated for the change required when rotating in the xyz dimensions
            Vector thrust = ThrustController.Calculate(new Vector(0, 0, 0), CurrentPosition.Subtract(TargetPosition), samplingPeriod);//thrust output in XYZ dimension

            thrust.X = thrust.X + rotation.Z;
            thrust.Z = thrust.Z - rotation.X;
            
            //if (name == "TB") Console.WriteLine(thrust + " " + test2);

            //AdjustThrust(thrust, rotation);

            //Use offset angle to compensate for Y output, add to Y position
            thrust = Matrix.RotateVector(new Vector(0, rotation.Y, 0), thrust);//adjust thruster output to quad frame, only Y dimension

            //Normalize secondary rotation control
            offset.Y = offset.Y < 0 ? 0 : offset.Y;
            */
            //Combine quad rotation output with individual thruster output
            //thrust = thrust.Add(offset);
            Vector thrust = offset;

            //Normalize thrust output
            thrust.Y = thrust.Y < 0 ? 0 : thrust.Y;

            thrust.X = Disable ? 0 : thrust.X;
            thrust.Y = Disable ? 0 : thrust.Y;
            thrust.Z = Disable ? 0 : thrust.Z;

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
