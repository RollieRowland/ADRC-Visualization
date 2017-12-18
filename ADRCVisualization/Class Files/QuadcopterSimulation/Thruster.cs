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
                X = new ADRC(2000, 2.5, 0.75, 0.005, 45)
                {
                    PID = new PID(20, 0, 0, 90)
                },
                Y = new ADRC(2000, 2.5, 5, 0.01, 100)
                {
                    PID = new PID(25, 0, 0.75, 100)
                },
                Z = new ADRC(2000, 2.5, 0.75, 0.005, 45)
                {
                    PID = new PID(20, 0, 0, 90)
                }
            };
            
            ThrustController = new VectorFeedbackController()
            {
                X = new PID(20, 0, 0, 45),
                Y = new PID(5, 0, 0.75, 100),
                Z = new PID(20, 0, 0, 45)
            };

            ThrustController = new VectorFeedbackController()
            {
                X = new PID(20, 0, 5, 45),
                Y = new PID(5, 0, 0.75, 100),
                Z = new PID(20, 0, 5, 45)
            };

            thrustKF = new VectorKalmanFilter(new Vector(0.125, 0.875, 0.125), new Vector(20, 3, 20));//Increase memory to decrease response time

            CurrentRotation = new Vector(0, 0, 0);
            TargetRotation = new Vector(0, 0, 0);
            CurrentPosition = new Vector(0, 0, 0);
            TargetPosition = new Vector(0, 0, 0);
        }

        public Vector Calculate()
        {
            Vector thrust = ThrustController.Calculate(TargetPosition, CurrentPosition);

            thrust.Y = thrust.Y < 0 ? 0 : thrust.Y;

            //thrustKF.Filter(thrust);

            return thrust;

            //secondaryJoint.SetAngle(thrustKF.GetFilteredValue().X);
            //propellor.SetOutput(thrustKF.GetFilteredValue().Y);
            //primaryJoint.SetAngle(thrustKF.GetFilteredValue().Z);
        }

        public void SetOutputs(Vector outputs)
        {
            thrustKF.Filter(outputs);

            thrustKF.GetFilteredValue();

            secondaryJoint.SetAngle(thrustKF.GetFilteredValue().X);
            propellor.SetOutput(thrustKF.GetFilteredValue().Y);
            primaryJoint.SetAngle(thrustKF.GetFilteredValue().Z);
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
