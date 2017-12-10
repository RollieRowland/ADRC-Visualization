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

        private ADRC_PD thrustADRC;
        private ADRC_PD primaryJointADRC;
        private ADRC_PD secondaryJointADRC;

        private PID thrustPID;
        private PID primaryJointPID;
        private PID secondaryJointPID;

        private bool useADRC = false;

        public Thruster(Vector QuadCenterOffset)
        {
            this.QuadCenterOffset = QuadCenterOffset;
            
            propellor = new Motor();
            primaryJoint = new Servo();
            secondaryJoint = new Servo();

            thrustADRC = new ADRC_PD(1, 1, 1, 1, 1, 1, 100);
            primaryJointADRC = new ADRC_PD(1, 1, 1, 1, 1, 1, 100);
            secondaryJointADRC = new ADRC_PD(1, 1, 1, 1, 1, 1, 100);

            thrustPID = new PID(4, 0, 0.75, 100);
            primaryJointPID = new PID(200, 0, 0, 90);
            secondaryJointPID = new PID(200, 0, 0, 90);

            thrustKF = new VectorKalmanFilter(new Vector(0.5, 0.5, 0.5), new Vector(1, 2, 1));//Increase memory to decrease response time

            CurrentRotation = new Vector(0, 0, 0);
        }

        public void Calculate()
        {
            double propellorOutput = 0;
            double primaryJointOutput = 0;
            double secondaryJointOutput = 0;

            CurrentRotation = thrustKF.GetFilteredValue();

            //Calculate X offset, Y offset, Z offset
            if (useADRC)
            {
                propellorOutput = thrustADRC.Calculate(TargetPosition.Y, CurrentPosition.Y);
                primaryJointOutput = primaryJointADRC.Calculate(TargetPosition.Z, CurrentPosition.Z);
                secondaryJointOutput = secondaryJointADRC.Calculate(TargetPosition.X, CurrentPosition.X);
            }
            else
            {
                propellorOutput = thrustPID.Calculate(TargetPosition.Y, CurrentPosition.Y);
                primaryJointOutput = primaryJointPID.Calculate(TargetPosition.Z, CurrentPosition.Z);
                secondaryJointOutput = secondaryJointPID.Calculate(TargetPosition.X, CurrentPosition.X);
            }

            if (propellorOutput < 0)
            {
                propellorOutput = 0;
            }

            thrustKF.Filter(new Vector(primaryJointOutput, propellorOutput, secondaryJointOutput));

            primaryJoint.SetAngle(thrustKF.GetFilteredValue().X);
            propellor.SetOutput(thrustKF.GetFilteredValue().Y);
            secondaryJoint.SetAngle(thrustKF.GetFilteredValue().Z);
        }

        public void SetOutputs(Vector outputs)
        {
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
            double Y = propellor.GetOutput() - Math.Abs(X) - Math.Abs(Z);

            return new Vector(X, Y, Z);
        }
    }
}
