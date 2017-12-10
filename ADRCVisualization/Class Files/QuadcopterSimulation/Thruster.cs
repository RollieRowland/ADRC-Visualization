using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.Mathematics;

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
        public Vector CurrentRotation { get; private set; }

        public Vector QuadCenterOffset { get; }

        private VectorPID RotationPID;

        public Thruster(Vector QuadCenterOffset)
        {
            this.QuadCenterOffset = QuadCenterOffset;
            
            propellor = new Motor();
            primaryJoint = new Servo();
            secondaryJoint = new Servo();

            thrustKF = new VectorKalmanFilter(new Vector(0.5, 0.5, 0.5), new Vector(2, 1, 2));//Increase memory to decrease response time

            RotationPID = new VectorPID(1, 0, 0.1, 180);

            CurrentRotation = new Vector(0, 0, 0);
        }
        
        public void SetOutputs(Vector outputs)
        {
            //outputs.X = outputs.X < 0 ? 0 : outputs.X;
            //outputs.Y = outputs.Y < 0 ? 0 : outputs.Y;
            //outputs.Z = outputs.Z < 0 ? 0 : outputs.Z;

            thrustKF.Filter(outputs);

            CurrentRotation = thrustKF.GetFilteredValue();

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
