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
        private Vector currentAngularAcceleration;

        private VectorKalmanFilter accelerationKF;
        private VectorKalmanFilter velocityKF;
        //private IMU imu;

        private VectorKalmanFilter angularAccelerationKF;
        private VectorKalmanFilter angularVelocityKF;
        
        private PID xPositionPID;
        private PID yPositionPID;
        private PID zPositionPID;

        private PID xRotationPID;
        private PID yRotationPID;
        private PID zRotationPID;

        private double armLength;
        private double armAngle;

        public Quadcopter(double armLength, double armAngle)
        {
            CalculateArmPositions(armLength, armAngle);
            lastMeasurementTime = DateTime.Now;

            currentPosition = new Vector(0, 0, 0);
            targetPosition = new Vector(0, 0, 0);
            currentVelocity = new Vector(0, 0, 0);

            currentAngularVelocity = new Vector(0, 0, 0);

            accelerationKF = new VectorKalmanFilter(0.5, 1);
            velocityKF = new VectorKalmanFilter(0.5, 1);

            angularAccelerationKF = new VectorKalmanFilter(0.5, 1);
            angularVelocityKF = new VectorKalmanFilter(0.5, 1);

            xPositionPID = new PID(150, 0, 0, 90);
            yPositionPID = new PID(3, 0, 0, 100);
            zPositionPID = new PID(150, 0, 0, 90);

            xRotationPID = new PID(500, 0, 250, 45);
            yRotationPID = new PID(500, 0, 250, 45);
            zRotationPID = new PID(500, 0, 250, 45);

            this.armLength = armLength;
            this.armAngle = armAngle;
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
        
        public void CalculateIndividualThrustVectors()
        {
            ThrusterB.Calculate();
            ThrusterC.Calculate();
            ThrusterD.Calculate();
            ThrusterE.Calculate();

            //Console.WriteLine(ThrusterB.GetOutputs());
        }

        public void CalculateCombinedThrustVector()
        {
            Vector positionOutput = new Vector(0, 0, 0);
            Vector rotationOutput = new Vector(0, 0, 0);

            double xPositionOutput = 0;
            double yPositionOutput = 0;
            double zPositionOutput = 0;
            double xRotationOutput = 0;
            double yRotationOutput = 0;
            double zRotationOutput = 0;

            xPositionOutput = xPositionPID.Calculate(targetPosition.X, currentPosition.X);
            yPositionOutput = yPositionPID.Calculate(targetPosition.Y, currentPosition.Y);
            zPositionOutput = zPositionPID.Calculate(targetPosition.Z, currentPosition.Z);

            xRotationOutput = xRotationPID.Calculate(targetRotation.X, currentRotation.X);//in the YZ
            yRotationOutput = yRotationPID.Calculate(targetRotation.Y, currentRotation.Y);//in the XZ
            zRotationOutput = -zRotationPID.Calculate(targetRotation.Z, currentRotation.Z);//in the XY
            
            positionOutput.X = xPositionOutput;
            positionOutput.Y = yPositionOutput;
            positionOutput.Z = zPositionOutput;

            rotationOutput.X = xRotationOutput;
            rotationOutput.Y = yRotationOutput;
            rotationOutput.Z = zRotationOutput;

            //Console.WriteLine(targetRotation.X + " " + currentRotation.X + " " + xRotationOutput);

            //Console.WriteLine(positionOutput.Subtract(new Vector(yRotationOutput, 0, 0)) + "   " + positionOutput.Add(new Vector(yRotationOutput, 0, 0)));

            /*
            double xRotationalForce = torqueXZ * TB.X + torqueXZ * TC.X + torqueXZ * TD.X + torqueXZ * TE.X;
            double yRotationalForce = torqueY * TB.Y + torqueY * TC.Y + torqueY * TD.Y + torqueY * TE.Y;
            double zRotationalForce = torqueXZ * TB.Z + torqueXZ * TC.Z + torqueXZ * TD.Z + torqueXZ * TE.Z;

            Rotate about YZ (xRotationalForce) ^
            
            T = rFsin(theta)

            */

            Vector positionOutputBY = positionOutput.Subtract(new Vector(yRotationOutput, 0, 0));//about Y Axis, in the XZ dimensions
            Vector positionOutputCY = positionOutput.Subtract(new Vector(yRotationOutput, 0, 0));
            Vector positionOutputDY = positionOutput.Add(new Vector(yRotationOutput, 0, 0));
            Vector positionOutputEY = positionOutput.Add(new Vector(yRotationOutput, 0, 0));

            Vector positionOutputBX = positionOutput.Subtract(new Vector(0, xRotationOutput, 0));//about X Axis, in the YZ dimensions
            Vector positionOutputCX = positionOutput.Subtract(new Vector(0, xRotationOutput, 0));
            Vector positionOutputDX = positionOutput.Add(new Vector(0, xRotationOutput, 0));
            Vector positionOutputEX = positionOutput.Add(new Vector(0, xRotationOutput, 0));

            Vector positionOutputBZ = positionOutputBY.Subtract(new Vector(0, 0, zRotationOutput));//about X Axis, in the YZ dimensions
            Vector positionOutputCZ = positionOutputCY.Subtract(new Vector(0, 0, zRotationOutput));
            Vector positionOutputDZ = positionOutputDY.Add(new Vector(0, 0, zRotationOutput));
            Vector positionOutputEZ = positionOutputEY.Add(new Vector(0, 0, zRotationOutput));
            
            ThrusterB.SetOutputs(positionOutputBZ);//about y axis, about x axis, about z axis
            ThrusterC.SetOutputs(positionOutputCZ);
            ThrusterD.SetOutputs(positionOutputDZ);
            ThrusterE.SetOutputs(positionOutputEZ);
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
            //determine reduction in thrust due to angle changes, the radius from the center point
            Vector TB = ThrusterB.ReturnThrustVector();
            Vector TC = ThrusterC.ReturnThrustVector();
            Vector TD = ThrusterD.ReturnThrustVector();
            Vector TE = ThrusterE.ReturnThrustVector();

            //Add components to find direction of quad

            currentAcceleration = accelerationKF.Filter(TB.Add(TC).Add(TD).Add(TE).Add(externalForce));

            return currentAcceleration;
        }

        public Vector AccelerationNoGravity()
        {
            Vector TB = ThrusterB.ReturnThrustVector();
            Vector TC = ThrusterC.ReturnThrustVector();
            Vector TD = ThrusterD.ReturnThrustVector();
            Vector TE = ThrusterE.ReturnThrustVector();

            //Add components to find direction of quad

            currentAcceleration = TB.Add(TC).Add(TD).Add(TE);
            
            return currentAcceleration;
        }
        
        public Vector EstimatePosition(double dT)
        {
            //use force to estimate the position given dT

            //dT = DateTime.Now.Subtract(lastMeasurementTime).TotalSeconds;
            if (dT > 0)
            {
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
            }

            return currentPosition;
        }
        
        public Vector EstimateRotation(double dT)
        {
            //relative to the object, which direction will it rotate?
            //double XLength = armLength * Math.Cos(Misc.DegreesToRadians(armAngle));

            //calculate angular acceleration: add components

            //use position and rotation to determine force at an angle
            Vector TB = ThrusterB.ReturnThrustVector();
            Vector TC = ThrusterC.ReturnThrustVector();
            Vector TD = ThrusterD.ReturnThrustVector();
            Vector TE = ThrusterE.ReturnThrustVector();
            //Vector TB = Vector.CrossProduct(ThrusterB.QuadCenterOffset, ThrusterB.ReturnRotationalForce());
            //Vector TC = Vector.CrossProduct(ThrusterC.QuadCenterOffset, ThrusterC.ReturnRotationalForce());
            //Vector TD = Vector.CrossProduct(ThrusterD.QuadCenterOffset, ThrusterD.ReturnRotationalForce());
            //Vector TE = Vector.CrossProduct(ThrusterE.QuadCenterOffset, ThrusterE.ReturnRotationalForce());

            //currentAngularAcceleration = angularAccelerationKF.Filter(TB.Add(TC).Add(TD).Add(TE));

            //TB = AdjustThrustVector(currentRotation, TB);
            //TC = AdjustThrustVector(currentRotation, TC);
            //TD = AdjustThrustVector(currentRotation, TD);
            //TE = AdjustThrustVector(currentRotation, TE);

            double torqueXZ = armLength * Math.Sin(Misc.DegreesToRadians(180 - armAngle));
            double torqueY  = armLength * Math.Sin(Misc.DegreesToRadians(180 - armAngle));

            double xRotationalForce = torqueXZ * TB.X + torqueXZ * TC.X - torqueXZ * TD.X - torqueXZ * TE.X;//rotate around horizontal axis
            double yRotationalForce = torqueY  * TB.Y + torqueY  * TC.Y - torqueY  * TD.Y - torqueY  * TE.Y;//rotate around deep axis
            double zRotationalForce = torqueXZ * TB.Z + torqueXZ * TC.Z - torqueXZ * TD.Z - torqueXZ * TE.Z;//rotate around vertical

            Console.WriteLine(xRotationalForce + " " + yRotationalForce + " " + zRotationalForce);

            currentAngularVelocity.X = 0;// (currentAngularVelocity.X + xRotationalForce * dT);// * 0.98;//resistance factor
            currentAngularVelocity.Y = (currentAngularVelocity.Y + yRotationalForce * dT);// * 0.98;
            currentAngularVelocity.Z = (currentAngularVelocity.Z + zRotationalForce * dT);// * 0.98;
            
            //currentAngularAcceleration = TB.Add(TC).Add(TD).Add(TE);

            //Console.WriteLine(currentAngularAcceleration.ToString());

            //calculate angular velocity: w = w0 + at or w = dTheta / dT
            //currentAngularVelocity.X = (currentAngularVelocity.X + currentAngularAcceleration.X * dT) * 0.98;//resistance factor
            //currentAngularVelocity.Y = (currentAngularVelocity.Y + currentAngularAcceleration.Y * dT) * 0.98;
            //currentAngularVelocity.Z = (currentAngularVelocity.Z + currentAngularAcceleration.Z * dT) * 0.98;

            //calculate angular position: theta = wt + 1/2 * a * t^2
            currentRotation.X = currentRotation.X + currentAngularVelocity.X * dT;
            currentRotation.Y = currentRotation.Y + currentAngularVelocity.Y * dT;
            currentRotation.Z = currentRotation.Z + currentAngularVelocity.Z * dT;

            //Console.WriteLine(currentRotation.ToString());

            //currentRotation.X = currentRotation.X + currentAngularVelocity.X * dT + (1 / 2) * currentAngularAcceleration.X * dT;
            //currentRotation.Y = currentRotation.Y + currentAngularVelocity.Y * dT + (1 / 2) * currentAngularAcceleration.Y * dT;
            //currentRotation.Z = currentRotation.Z + currentAngularVelocity.Z * dT + (1 / 2) * currentAngularAcceleration.Z * dT;

            return currentRotation;
        }

        private Vector AdjustThrustVector(Vector Rotate, Vector ThrusterDirection)
        {
            //calculate rotation matrix
            Matrix matrix = new Matrix(ThrusterDirection);

            matrix.Rotate(Rotate);

            return matrix.ConvertCoordinateToVector();
        }

        public Tuple<Vector, Vector, Vector, Vector> GetMotorPositions()
        {
            Vector TB = ThrusterB.GetCurrentPosition();
            Vector TC = ThrusterC.GetCurrentPosition();
            Vector TD = ThrusterD.GetCurrentPosition();
            Vector TE = ThrusterE.GetCurrentPosition();

            return new Tuple<Vector, Vector, Vector, Vector>(TB, TC, TD, TE);
        }

        public Tuple<Vector, Vector, Vector, Vector> GetMotorTargetPositions()
        {
            Vector TB = ThrusterB.GetTargetPosition();
            Vector TC = ThrusterC.GetTargetPosition();
            Vector TD = ThrusterD.GetTargetPosition();
            Vector TE = ThrusterE.GetTargetPosition();

            return new Tuple<Vector, Vector, Vector, Vector>(TB, TC, TD, TE);
        }

        public Tuple<Vector, Vector, Vector, Vector> GetMotorOrientations()
        {
            Vector TB = ThrusterB.GetOutputs();
            Vector TC = ThrusterC.GetOutputs();
            Vector TD = ThrusterD.GetOutputs();
            Vector TE = ThrusterE.GetOutputs();

            return new Tuple<Vector, Vector, Vector, Vector>(TB, TC, TD, TE);
        }
    }
}
