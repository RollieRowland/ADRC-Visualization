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
        B     C      v    ^      >       >      ^       ^
         \   /
           A            Z            Y              X
         /   \
        E     D      v    ^      <       <      v       v
        
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

            accelerationKF = new VectorKalmanFilter(0.5, 10);
            velocityKF = new VectorKalmanFilter(0.5, 10);

            angularAccelerationKF = new VectorKalmanFilter(0.5, 1);
            angularVelocityKF = new VectorKalmanFilter(0.5, 1);

            xPositionPID = new PID(120, 0, 20, 90);
            yPositionPID = new PID(3,   0, 0, 100);
            zPositionPID = new PID(120, 0, 20, 90);

            xRotationPID = new PID(0.01, 0, 0.01, 45);
            yRotationPID = new PID(0.01, 0, 0.01, 45);
            zRotationPID = new PID(0.01, 0, 0.01, 45);

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

        public void CalculateCombinedThrustVector()
        {
            Vector positionOutput = new Vector(0, 0, 0);
            Vector rotationOutput = new Vector(0, 0, 0);

            positionOutput.X = xPositionPID.Calculate(targetPosition.X, currentPosition.X);
            positionOutput.Y = yPositionPID.Calculate(targetPosition.Y, currentPosition.Y);
            positionOutput.Z = zPositionPID.Calculate(targetPosition.Z, currentPosition.Z);
            
            rotationOutput.X = -xRotationPID.Calculate(targetRotation.X, currentRotation.X);//in the YZ
            rotationOutput.Y = -yRotationPID.Calculate(targetRotation.Y, currentRotation.Y);//in the XZ
            rotationOutput.Z = -zRotationPID.Calculate(targetRotation.Z, currentRotation.Z);//in the XY

            //Console.WriteLine(currentRotation.X + " " + targetRotation.X + " " + xRotationOutput);
            
            Vector positionOutputBX =   positionOutput.Subtract(new Vector(0, rotationOutput.X, 0));//about X Axis, in the YZ dimensions
            Vector positionOutputCX =   positionOutput.Subtract(new Vector(0, rotationOutput.X, 0));//x, 0, 0
            Vector positionOutputDX =   positionOutput.Add(     new Vector(0, rotationOutput.X, 0));
            Vector positionOutputEX =   positionOutput.Add(     new Vector(0, rotationOutput.X, 0));
            
            Vector positionOutputBY = positionOutputBX.Subtract(new Vector(rotationOutput.Y, 0, 0));//about Y Axis, in the XZ dimensions
            Vector positionOutputCY = positionOutputCX.Subtract(new Vector(rotationOutput.Y, 0, 0));//0, x, 0
            Vector positionOutputDY = positionOutputDX.Add(     new Vector(rotationOutput.Y, 0, 0));
            Vector positionOutputEY = positionOutputEX.Add(     new Vector(rotationOutput.Y, 0, 0));

            Vector positionOutputBZ = positionOutputBY.Add(     new Vector(0, rotationOutput.Z, 0));//about X Axis, in the YZ dimensions
            Vector positionOutputCZ = positionOutputCY.Subtract(new Vector(0, rotationOutput.Z, 0));//0, 0, x
            Vector positionOutputDZ = positionOutputDY.Subtract(new Vector(0, rotationOutput.Z, 0));
            Vector positionOutputEZ = positionOutputEY.Add(     new Vector(0, rotationOutput.Z, 0));
            
            Console.WriteLine(rotationOutput.Y + " " + currentRotation.Y + " " + targetRotation.Y);

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

            currentAcceleration = accelerationKF.Filter(TB.Add(TC).Add(TD).Add(TE).Add(externalForce));

            return currentAcceleration;
        }

        public Vector AccelerationNoGravity()
        {
            Vector TB = ThrusterB.ReturnThrustVector();
            Vector TC = ThrusterC.ReturnThrustVector();
            Vector TD = ThrusterD.ReturnThrustVector();
            Vector TE = ThrusterE.ReturnThrustVector();

            currentAcceleration = TB.Add(TC).Add(TD).Add(TE);
            
            return currentAcceleration;
        }
        
        public Vector EstimatePosition(double dT)
        {
            if (dT > 0)
            {
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
            Vector TB = ThrusterB.GetOutputs();
            Vector TC = ThrusterC.GetOutputs();
            Vector TD = ThrusterD.GetOutputs();
            Vector TE = ThrusterE.GetOutputs();
            
            double torque  = armLength * Math.Sin(Misc.DegreesToRadians(180 - armAngle));

            double xYRotationalForce = (TB.X + TC.X - TD.X - TE.X);//rotate around deep axis
            double yXRotationalForce = (TB.Y + TC.Y - TD.Y - TE.Y);//rotate around horizontal axis
            double yZRotationalForce = (-TB.Y + TC.Y + TD.Y - TE.Y);//rotate around vertical axis

            currentAngularVelocity.X = (currentAngularVelocity.X + yXRotationalForce * dT);//resistance factor
            currentAngularVelocity.Y = (currentAngularVelocity.Y + xYRotationalForce * dT);
            currentAngularVelocity.Z = (currentAngularVelocity.Z + yZRotationalForce * dT);
            
            currentAngularVelocity = angularVelocityKF.Filter(currentAngularVelocity);

            //calculate angular position: theta = wt + 1/2 * a * t^2
            currentRotation.X = currentRotation.X + currentAngularVelocity.X * dT;
            currentRotation.Y = currentRotation.Y + currentAngularVelocity.Y * dT;
            currentRotation.Z = currentRotation.Z + currentAngularVelocity.Z * dT;
            
            return currentRotation;
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
