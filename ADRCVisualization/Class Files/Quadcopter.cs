using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.QuadcopterSimulation;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualization.Class_Files
{
    class Quadcopter
    {
        /*
        B     C
         \   /
           A
         /   \
        E     D
        
        < -X- >
           
           ^
           Z
           v
        */
        private Thruster ThrusterB;
        private Thruster ThrusterC;
        private Thruster ThrusterD;
        private Thruster ThrusterE;
        private DateTime lastMeasurementTime;

        private Vector currentPosition;
        private Vector targetPosition;
        private Vector currentRotation;
        private Vector targetRotation;

        private Vector currentVelocity;
        private Vector currentAngularVelocity;
        private Vector currentAcceleration;
        //private IMU imu;

        public Quadcopter(double ArmLength, double ArmAngle)
        {
            CalculateArmPositions(ArmLength, ArmAngle);
            lastMeasurementTime = DateTime.Now;

            currentPosition = new Vector(0, 0, 0);
            targetPosition = new Vector(0, 0, 0);
            currentVelocity = new Vector(0, 0, 0);
        }

        private void CalculateArmPositions(double ArmLength, double ArmAngle)
        {
            double XLength = ArmLength * Math.Cos(Misc.DegreesToRadians(ArmAngle));
            double ZLength = ArmLength * Math.Sin(Misc.DegreesToRadians(ArmAngle));

            Console.WriteLine("X:" + XLength + " Y:" + ZLength);

            ThrusterB = new Thruster(new Vector(-XLength, 0,  ZLength));
            ThrusterC = new Thruster(new Vector( XLength, 0,  ZLength));
            ThrusterD = new Thruster(new Vector( XLength, 0, -ZLength));
            ThrusterE = new Thruster(new Vector(-XLength, 0, -ZLength));
        }
        
        public void Calculate()
        {
            ThrusterB.Calculate();
            ThrusterC.Calculate();
            ThrusterD.Calculate();
            ThrusterE.Calculate();
        }
        
        public void SetCurrent(Vector position, Vector rotation)
        {
            currentPosition = position;
            currentRotation = rotation;

            //calculate rotation matrix
            Matrix matrixB = new Matrix(ThrusterB.QuadCenterOffset);
            Matrix matrixC = new Matrix(ThrusterC.QuadCenterOffset);
            Matrix matrixD = new Matrix(ThrusterD.QuadCenterOffset);
            Matrix matrixE = new Matrix(ThrusterE.QuadCenterOffset);

            matrixB.Rotate(rotation);
            matrixC.Rotate(rotation);
            matrixD.Rotate(rotation);
            matrixE.Rotate(rotation);

            ThrusterB.SetCurrentPosition(matrixB.ConvertCoordinateToVector().Add(position));
            ThrusterC.SetCurrentPosition(matrixC.ConvertCoordinateToVector().Add(position));
            ThrusterD.SetCurrentPosition(matrixD.ConvertCoordinateToVector().Add(position));
            ThrusterE.SetCurrentPosition(matrixE.ConvertCoordinateToVector().Add(position));
        }

        public void SetTarget(Vector position, Vector rotation)
        {
            targetPosition = position;
            targetRotation = rotation;

            //calculate rotation matrix
            Matrix matrixB = new Matrix(ThrusterB.QuadCenterOffset);
            Matrix matrixC = new Matrix(ThrusterC.QuadCenterOffset);
            Matrix matrixD = new Matrix(ThrusterD.QuadCenterOffset);
            Matrix matrixE = new Matrix(ThrusterE.QuadCenterOffset);

            matrixB.Rotate(rotation);
            matrixC.Rotate(rotation);
            matrixD.Rotate(rotation);
            matrixE.Rotate(rotation);

            ThrusterB.SetTargetPosition(matrixB.ConvertCoordinateToVector().Add(position));
            ThrusterC.SetTargetPosition(matrixC.ConvertCoordinateToVector().Add(position));
            ThrusterD.SetTargetPosition(matrixD.ConvertCoordinateToVector().Add(position));
            ThrusterE.SetTargetPosition(matrixE.ConvertCoordinateToVector().Add(position));
        }

        public Vector EstimateAcceleration(Vector externalForce)
        {
            //ThrusterB.GetOutputs(out double propellorB, out double primaryJointB, out double secondaryJointB);
            //ThrusterC.GetOutputs(out double propellorC, out double primaryJointC, out double secondaryJointC);
            //ThrusterD.GetOutputs(out double propellorD, out double primaryJointD, out double secondaryJointD);
            //ThrusterE.GetOutputs(out double propellorE, out double primaryJointE, out double secondaryJointE);

            //determine reduction in thrust due to angle changes, the radius from the center point
            Vector TB = ThrusterB.ReturnThrustVector();
            Vector TC = ThrusterC.ReturnThrustVector();
            Vector TD = ThrusterD.ReturnThrustVector();
            Vector TE = ThrusterE.ReturnThrustVector();

            //Add components to find direction of quad

            currentAcceleration = TB.Add(TC).Add(TD).Add(TE).Add(externalForce);

            return currentAcceleration;
        }

        public Vector AccelerationNoGravity()
        {
            //ThrusterB.GetOutputs(out double propellorB, out double primaryJointB, out double secondaryJointB);
            //ThrusterC.GetOutputs(out double propellorC, out double primaryJointC, out double secondaryJointC);
            //ThrusterD.GetOutputs(out double propellorD, out double primaryJointD, out double secondaryJointD);
            //ThrusterE.GetOutputs(out double propellorE, out double primaryJointE, out double secondaryJointE);

            //determine reduction in thrust due to angle changes, the radius from the center point
            Vector TB = ThrusterB.ReturnThrustVector();
            Vector TC = ThrusterC.ReturnThrustVector();
            Vector TD = ThrusterD.ReturnThrustVector();
            Vector TE = ThrusterE.ReturnThrustVector();

            //Add components to find direction of quad
            
            return TB.Add(TC).Add(TD).Add(TE);
        }



        public Vector EstimatePosition()
        {
            //use force to estimate the position given dT
            
            double dT = DateTime.Now.Subtract(lastMeasurementTime).TotalSeconds;

            //velocity is initially 0
            //calculate velocity: finalVelocity(vf) = vi + at

            currentVelocity.X = currentVelocity.X + currentAcceleration.X * dT;
            currentVelocity.Y = currentVelocity.Y + currentAcceleration.Y * dT;
            currentVelocity.Z = currentVelocity.Z + currentAcceleration.Z * dT;

            //calculate position: displacement(s) = vi*t + 1/2 * at^2

            currentPosition.X = currentVelocity.X * dT + (1 / 2) * currentAcceleration.X * Math.Pow(dT, 2);
            currentPosition.Y = currentVelocity.Y * dT + (1 / 2) * currentAcceleration.Y * Math.Pow(dT, 2);
            currentPosition.Z = currentVelocity.Z * dT + (1 / 2) * currentAcceleration.Z * Math.Pow(dT, 2);

            lastMeasurementTime = DateTime.Now;

            return currentPosition;
        }

        public Vector EstimateRotation(Vector force)
        {
            throw new NotImplementedException();
        }
    }
}
