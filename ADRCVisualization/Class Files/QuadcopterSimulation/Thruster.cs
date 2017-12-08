using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualization.Class_Files.QuadcopterSimulation
{
    class Thruster
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

        private VectorPID RotationPID;
        private VectorKalmanFilter RotationVectorKalmanFilter;

        public Thruster(Vector QuadCenterOffset)
        {
            this.QuadCenterOffset = QuadCenterOffset;
            
            propellor = new Motor();
            primaryJoint = new Servo();
            secondaryJoint = new Servo();

            thrustKF = new VectorKalmanFilter(0.5, 1);//Increase memory to decrease response time

            RotationPID = new VectorPID(1, 0, 0.1, 180);
            RotationVectorKalmanFilter = new VectorKalmanFilter(0.5, 5);

            CurrentRotation = new Vector(0, 0, 0);

            //Console.WriteLine("QuadCenter: " + QuadCenterOffset.ToString());
        }

        public void CalculateRotation()
        {
            //Kalman Filter to simulate acceleration / delay in movement of servos
            //CurrentRotation = RotationVectorKalmanFilter.Filter(RotationPID.Calculate(TargetRotation, CurrentRotation));
            TargetRotation.Y = 0;
            CurrentRotation.Y = 0;

            CurrentRotation = RotationPID.Calculate(TargetRotation, RotationVectorKalmanFilter.Filter(CurrentRotation));//Assumes there are three rotational axes, when there are two

            Console.WriteLine(CurrentRotation.X + " " + secondaryJoint.GetAngle());
        }
        
        
        public void SetOutputs(Vector outputs)
        {
            thrustKF.Filter(outputs);

            secondaryJoint.SetAngle(thrustKF.GetFilteredValue().X);
            propellor.SetOutput(thrustKF.GetFilteredValue().Y);
            primaryJoint.SetAngle(thrustKF.GetFilteredValue().Z);
        }

        public Vector ReturnThrustVector()
        {
            //Relative to Quad Force in each direction
            double X = Math.Sin(Misc.DegreesToRadians(secondaryJoint.GetAngle()));
            double Z = Math.Sin(Misc.DegreesToRadians(primaryJoint.GetAngle()));
            double Y = propellor.GetOutput() - X - Z;

            return new Vector(X, Y, Z);
        }
    }
}
