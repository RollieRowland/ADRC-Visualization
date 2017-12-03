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
        //private IMU jointMotion;

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

            thrustADRC = new ADRC_PD(1, 1, 1, 1, 1, 1, 100);
            primaryJointADRC = new ADRC_PD(1, 1, 1, 1, 1, 1, 100);
            secondaryJointADRC = new ADRC_PD(1, 1, 1, 1, 1, 1, 100);

            thrustPID = new PID(40, 0, 0, 100);
            primaryJointPID = new PID(200, 0, 0, 100);
            secondaryJointPID = new PID(200, 0, 0, 100);

            propellor = new Motor();
            primaryJoint = new Servo();
            secondaryJoint = new Servo();

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

            //Console.Write("Control Output: " + propellorOutput + " " + primaryJointOutput + " " + secondaryJointOutput + " ");

            propellor.SetOutput(propellorOutput);
            primaryJoint.SetAngle(primaryJointOutput);
            secondaryJoint.SetAngle(secondaryJointOutput);

        }

        /// <summary>
        /// For simulation only
        /// </summary>
        /// <param name="propellor"></param>
        /// <param name="primaryJoint"></param>
        /// <param name="secondaryJoint"></param>
        public void GetOutputs(out double propellor, out double primaryJoint, out double secondaryJoint)
        {
            propellor = this.propellor.GetOutput();
            primaryJoint = this.primaryJoint.GetAngle();
            secondaryJoint = this.secondaryJoint.GetAngle();
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
    }
}
