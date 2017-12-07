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
        
        private Vector targetPosition;
        private Vector currentPosition;
        public Vector QuadCenterOffset { get; }

        private bool useADRC = false;

        public Thruster(Vector QuadCenterOffset)
        {
            this.QuadCenterOffset = QuadCenterOffset;
            
            propellor = new Motor();
            primaryJoint = new Servo();
            secondaryJoint = new Servo();

            thrustKF = new VectorKalmanFilter(0.5, 1);//Increase memory to decrease response time

            //Console.WriteLine("QuadCenter: " + QuadCenterOffset.ToString());
        }

        public void SetTargetPosition(Vector position)
        {
            targetPosition = position;

            //Console.WriteLine("Thruster Target: " + position.ToString());
        }

        public void SetCurrentPosition(Vector position)
        {
            currentPosition = position;

            //Console.WriteLine("Thruster Position: " + position.ToString());
        }
        
        public Vector GetCurrentPosition()
        {
            return currentPosition;
        }

        public Vector GetTargetPosition()
        {
            return targetPosition;
        }

        public void SetOutputs(Vector outputs)
        {
            thrustKF.Filter(outputs);

            secondaryJoint.SetAngle(thrustKF.GetFilteredValue().X);
            propellor.SetOutput(thrustKF.GetFilteredValue().Y);
            primaryJoint.SetAngle(thrustKF.GetFilteredValue().Z);
        }
        
        /// <summary>
        /// For simulation only
        /// </summary>
        /// <param name="propellor"></param>
        /// <param name="primaryJoint"></param>
        /// <param name="secondaryJoint"></param>
        public Vector GetOutputs()
        {
            //X, Y, Z
            return new Vector(secondaryJoint.GetAngle(), propellor.GetOutput(), primaryJoint.GetAngle());
        }

        public Vector ReturnThrustVector()
        {//Relative to Quad Force in each direction
            // X = 
            // Z = 
            // Y(0 - 1 range) - X.Sin(thetaX) - Sin(thetaY)

            //Console.Write("Joints: " + secondaryJoint.GetAngle() + " " + primaryJoint.GetAngle() + " " + propellor.GetOutput() + " ");

            double X = Math.Sin(Misc.DegreesToRadians(secondaryJoint.GetAngle()));
            double Z = Math.Sin(Misc.DegreesToRadians(primaryJoint.GetAngle()));
            double Y = propellor.GetOutput() - X - Z;

            //Console.Write("Thrust Vector: " + X + " " + Y + " " + Z +" ");

            return new Vector(X, Y, Z);
        }

        public Vector ReturnRotationalForce()
        {
            Vector thrust = ReturnThrustVector();

            double XY = thrust.X + thrust.Y;
            double XZ = thrust.X + thrust.Z;
            double YZ = thrust.Y + thrust.Z;
            
            return new Vector(XY, XZ, YZ);
        }
    }
}
