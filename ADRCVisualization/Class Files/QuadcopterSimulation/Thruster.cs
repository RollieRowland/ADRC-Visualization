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
                X = new PID(5, 0, 5.5, 45),
                Y = new PID(0.5, 0, 2.75, 100),
                Z = new PID(5, 0, 5.5, 45)
            };
            
            ThrustController = new VectorFeedbackController()
            {
                X = new ADRC(400, 1, 2, 0.01, 45)
                {
                    PID = new PID(5, 0, 5, 45)
                },
                Y = new ADRC(400, 1, 2, 0.01, 100)
                {
                    PID = new PID(5, 0, 5, 100)
                },
                Z = new ADRC(400, 1, 2, 0.01, 45)
                {
                    PID = new PID(5, 0, 5, 45)
                }
            };

            thrustKF = new VectorKalmanFilter(new Vector(0.5, 0.5, 0.5), new Vector(10, 10, 10));//Increase memory to decrease response time

            CurrentRotation = new Vector(0, 0, 0);
            TargetRotation = new Vector(0, 0, 0);
            CurrentPosition = new Vector(0, 0, 0);
            TargetPosition = new Vector(0, 0, 0);
        }

        public Vector Calculate(Vector Rotation, Vector Offset)
        {
            Rotation = Rotation.Multiply(new Vector(1, 1, 1));

            //Thrust compensated for the change required when rotating in the xyz dimensions
            Vector thrust = ThrustController.Calculate(Matrix.RotateVector(Rotation, TargetPosition), Matrix.RotateVector(Rotation, CurrentPosition));
            //Vector thrust = ThrustController.Calculate(TargetPosition, CurrentPosition);

            thrust = thrust.Add(Offset);

            thrust.Y = thrust.Y < 0 ? 0 : thrust.Y;

            thrustKF.Filter(thrust);

            CurrentRotation = new Vector(thrustKF.GetFilteredValue().Z, 0, thrustKF.GetFilteredValue().X);

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

            return thrustVector;
        }
    }
}
