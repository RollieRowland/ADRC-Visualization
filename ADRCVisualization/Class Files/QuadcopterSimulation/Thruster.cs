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

        private ADRC_PD thrustADRC;
        private ADRC_PD primaryJointADRC;
        private ADRC_PD secondaryJointADRC;

        private PID thrustPID;
        private PID primaryJointPID;
        private PID secondaryJointPID;

        private Vector targetPosition;
        private Vector currentPosition;
        public Vector QuadCenterOffset { get; }

        private bool useADRC = false;

        public Thruster(Vector QuadCenterOffset)
        {
            this.QuadCenterOffset = QuadCenterOffset;

            thrustADRC = new ADRC_PD(20, 2000, 0.00085, 4, 3, 0.5, 100);
            primaryJointADRC = new ADRC_PD(1, 1, 1, 1, 200, 0, 100);
            secondaryJointADRC = new ADRC_PD(1, 1, 1, 1, 200, 0, 100);

            thrustPID = new PID(3, 0.5, 0.5, 100);
            primaryJointPID = new PID(1000, 0, 600, 90);
            secondaryJointPID = new PID(1000, 0, 600, 90);

            propellor = new Motor();
            primaryJoint = new Servo();
            secondaryJoint = new Servo();

            thrustKF = new VectorKalmanFilter(0.5, 10);//Increase memory to decrease response time

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

        public void SetOutputs(double primaryJointOutput, double secondaryJointOutput, double propellorOutput)
        {
            thrustKF.Filter(new Vector(primaryJointOutput, propellorOutput, secondaryJointOutput));

            primaryJoint.SetAngle(thrustKF.GetFilteredValue().X);
            propellor.SetOutput(thrustKF.GetFilteredValue().Y);
            secondaryJoint.SetAngle(thrustKF.GetFilteredValue().Z);
        }

        public void Calculate()
        {
            double propellorOutput = 0;
            double primaryJointOutput = 0;
            double secondaryJointOutput = 0;

            //Calculate X offset, Y offset, Z offset
            if (useADRC)
            {
                propellorOutput = thrustADRC.Calculate(targetPosition.Y, currentPosition.Y);
                primaryJointOutput = primaryJointADRC.Calculate(targetPosition.Z, currentPosition.Z);
                secondaryJointOutput = secondaryJointADRC.Calculate(targetPosition.X, currentPosition.X);
            }
            else
            {
                propellorOutput = thrustPID.Calculate(targetPosition.Y, currentPosition.Y);
                primaryJointOutput = primaryJointPID.Calculate(targetPosition.Z, currentPosition.Z);
                secondaryJointOutput = secondaryJointPID.Calculate(targetPosition.X, currentPosition.X);
            }

            thrustKF.Filter(new Vector(primaryJointOutput, propellorOutput, secondaryJointOutput));

            primaryJoint.SetAngle(thrustKF.GetFilteredValue().X);
            propellor.SetOutput(thrustKF.GetFilteredValue().Y);
            secondaryJoint.SetAngle(thrustKF.GetFilteredValue().Z);
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
            return new Vector(secondaryJoint.GetAngle(), primaryJoint.GetAngle(), propellor.GetOutput());
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

            double XY = 0;// thrust.X + thrust.Y;
            double XZ = thrust.X + thrust.Z;
            double YZ = 0;// thrust.Y + thrust.Z;
            
            return new Vector(XY, XZ, YZ);
        }
    }
}
