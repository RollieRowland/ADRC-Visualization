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
        
        public Thruster(Vector QuadCenterOffset)
        {
            this.QuadCenterOffset = QuadCenterOffset;
            
            propellor = new Motor();
            primaryJoint = new Servo();
            secondaryJoint = new Servo();

            
            ThrustController = new VectorFeedbackController()
            {
                X = new ADRC(100, 0.001, 0.25, 0.01, 90)
                {
                    PID = new PID(5, 0, 5.5, 180)
                },
                Y = new ADRC(1000, 0.001, 0.75, 0.01, 100)
                {
                    PID = new PID(10, 0, 5, 100)
                },
                Z = new ADRC(100, 0.001, 0.25, 0.01, 90)
                {
                    PID = new PID(5, 0, 5.5, 180)
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
            rotation = rotation.Multiply(new Vector(1, 1, 1));

            //Thrust compensated for the change required when rotating in the xyz dimensions
            Vector thrust = ThrustController.Calculate(TargetPosition, CurrentPosition);//thrust output in XYZ dimension

            thrust = Matrix.RotateVector(new Vector(0, rotation.Y, 0), thrust);//adjust thruster output to quad frame, only Y dimension

            offset.Y = offset.Y < 0 ? 0 : offset.Y;

            thrust = thrust.Add(offset);

            thrust.Y = thrust.Y < 0 ? 0 : thrust.Y;

            thrustKF.Filter(thrust);

            CurrentRotation = new Vector(thrustKF.GetFilteredValue().Z, 0, -thrustKF.GetFilteredValue().X);//for display purposes

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
