using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.QuadcopterSimulation;
using ADRCVisualization.Class_Files.Mathematics;
using ADRCVisualization.Class_Files.FeedbackControl;

namespace ADRCVisualization.Class_Files
{
    public class Quadcopter
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
        public Thruster ThrusterB { get; private set; }
        public Thruster ThrusterC { get; private set; }
        public Thruster ThrusterD { get; private set; }
        public Thruster ThrusterE { get; private set; }
        private DateTime lastMeasurementTime;

        public Vector CurrentPosition { get; private set; }
        public Vector TargetPosition { get; private set; }
        public Vector CurrentRotation { get; private set; }
        public Vector TargetRotation { get; private set; }

        private Vector CurrentVelocity;
        private Vector currentAngularVelocity;
        private Vector CurrentAcceleration;
        private Vector currentAngularAcceleration;

        private Vector externalAcceleration;

        private VectorKalmanFilter positionMomentumKF;
        private VectorKalmanFilter angularMomentumKF;
        
        private VectorFeedbackController RotationFeedbackController;

        private double armLength;
        private double armAngle;

        private bool useADRC = true;

        public Quadcopter(double armLength, double armAngle)
        {
            CalculateArmPositions(armLength, armAngle);
            lastMeasurementTime = DateTime.Now;

            CurrentPosition = new Vector(0, 0, 0);
            TargetPosition = new Vector(0, 0, 0);
            CurrentRotation = new Vector(0, 0, 0);
            TargetRotation = new Vector(0, 0, 0);
            CurrentVelocity = new Vector(0, 0, 0);
            currentAngularVelocity = new Vector(0, 0, 0);
            CurrentAcceleration = new Vector(0, 0, 0);
            externalAcceleration = new Vector(0, 0, 0);
            
            RotationFeedbackController = new VectorFeedbackController()
            {
                X = new ADRC(2000, 2.5, 1, 0.01, 45)
                {
                    PID = new PID(10, 0, 20, 45)
                },
                Y = new ADRC(2000, 2.5, 0.01, 0.01, 45)
                {
                    PID = new PID(10, 0, 20, 45)
                },
                Z = new ADRC(2000, 2.5, 1, 0.01, 45)
                {
                    PID = new PID(10, 0, 20, 45)
                }
            };

            positionMomentumKF = new VectorKalmanFilter(1, 0);
            angularMomentumKF = new VectorKalmanFilter(1, 0);

            this.armLength = armLength;
            this.armAngle = armAngle;
        }

        private void CalculateArmPositions(double ArmLength, double ArmAngle)
        {
            double XLength = ArmLength * Math.Cos(Misc.DegreesToRadians(ArmAngle));
            double ZLength = ArmLength * Math.Sin(Misc.DegreesToRadians(ArmAngle));

            Console.WriteLine("Quad Arm Length X:" + XLength + " Z:" + ZLength);

            ThrusterB = new Thruster(new Vector(-XLength, 0,  ZLength));
            ThrusterC = new Thruster(new Vector( XLength, 0,  ZLength));
            ThrusterD = new Thruster(new Vector( XLength, 0, -ZLength));
            ThrusterE = new Thruster(new Vector(-XLength, 0, -ZLength));
        }

        public void CalculateCombinedThrustVector()
        {
            //Vector positionOutput = PositionFeedbackController.Calculate(TargetPosition, CurrentPosition);//.Multiply(new Vector(-1, 1, -1));
            Vector rotationOutput = RotationFeedbackController.Calculate(TargetRotation, CurrentRotation).Multiply(-1);

            //rotationOutput.X = 0;

            Vector rotationOutputB = new Vector(-rotationOutput.Y / 2, -rotationOutput.X + rotationOutput.Z, -rotationOutput.Y / 2);//Thruster output relative to environment origin
            Vector rotationOutputC = new Vector(-rotationOutput.Y / 2, -rotationOutput.X - rotationOutput.Z,  rotationOutput.Y / 2);
            Vector rotationOutputD = new Vector( rotationOutput.Y / 2,  rotationOutput.X - rotationOutput.Z,  rotationOutput.Y / 2);
            Vector rotationOutputE = new Vector( rotationOutput.Y / 2,  rotationOutput.X + rotationOutput.Z, -rotationOutput.Y / 2);
            
            //Motors need to be rotated relative to the ground
            //rotationOutputB = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), rotationOutputB);//Thruster output relative to quad origin
            //rotationOutputC = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), rotationOutputC);
            //rotationOutputD = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), rotationOutputD);
            //rotationOutputE = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), rotationOutputE);

            rotationOutputB = Matrix.RotateVector(CurrentRotation, rotationOutputB);//Thruster output relative to quad origin
            rotationOutputC = Matrix.RotateVector(CurrentRotation, rotationOutputC);
            rotationOutputD = Matrix.RotateVector(CurrentRotation, rotationOutputD);
            rotationOutputE = Matrix.RotateVector(CurrentRotation, rotationOutputE);
            
            if (rotationOutputB.Y < 0)
            {
                rotationOutputD.Y -= rotationOutputB.Y;
                rotationOutputB.Y = 0;

                if (rotationOutputD.Y < 0)
                {
                    rotationOutputD.Y = 0;
                }
            }
            
            if (rotationOutputC.Y < 0)
            {
                rotationOutputE.Y -= rotationOutputC.Y;
                rotationOutputC.Y = 0;
                
                if (rotationOutputE.Y < 0)
                {
                    rotationOutputE.Y = 0;
                }
            }

            if (rotationOutputD.Y < 0)
            {
                rotationOutputB.Y -= rotationOutputD.Y;
                rotationOutputD.Y = 0;

                if (rotationOutputB.Y < 0)
                {
                    rotationOutputB.Y = 0;
                }
            }

            if (rotationOutputE.Y < 0)
            {
                rotationOutputC.Y -= rotationOutputE.Y;
                rotationOutputE.Y = 0;

                if (rotationOutputC.Y < 0)
                {
                    rotationOutputC.Y = 0;
                }
            }

            ThrusterB.Calculate(CurrentRotation, rotationOutputB);
            ThrusterC.Calculate(CurrentRotation, rotationOutputC);
            ThrusterD.Calculate(CurrentRotation, rotationOutputD);
            ThrusterE.Calculate(CurrentRotation, rotationOutputE);

            /*
            ThrusterB.SetOutputs(rotationOutputB.Add(ThrusterB.Calculate(CurrentRotation)));
            ThrusterC.SetOutputs(rotationOutputC.Add(ThrusterC.Calculate(CurrentRotation)));
            ThrusterD.SetOutputs(rotationOutputD.Add(ThrusterD.Calculate(CurrentRotation)));
            ThrusterE.SetOutputs(rotationOutputE.Add(ThrusterE.Calculate(CurrentRotation)));
            */
        }
        
        public void CalculateCurrent()
        {
            EstimatePosition(0.05);
            EstimateRotation(0.05);
            
            //calculate rotation matrix
            ThrusterB.CurrentPosition = Matrix.RotateVector(CurrentRotation, ThrusterB.QuadCenterOffset).Add(CurrentPosition);
            ThrusterC.CurrentPosition = Matrix.RotateVector(CurrentRotation, ThrusterC.QuadCenterOffset).Add(CurrentPosition);
            ThrusterD.CurrentPosition = Matrix.RotateVector(CurrentRotation, ThrusterD.QuadCenterOffset).Add(CurrentPosition);
            ThrusterE.CurrentPosition = Matrix.RotateVector(CurrentRotation, ThrusterE.QuadCenterOffset).Add(CurrentPosition);

            //Vector test = Matrix.RotateVector(CurrentRotation, new Vector(1, 1, 1));
        }

        public void SetTarget(Vector position, Vector rotation)
        {
            TargetPosition = position;
            TargetRotation = rotation;

            //calculate rotation matrix
            ThrusterB.TargetPosition = Matrix.RotateVector(TargetRotation, ThrusterB.QuadCenterOffset).Add(TargetPosition);
            ThrusterC.TargetPosition = Matrix.RotateVector(TargetRotation, ThrusterC.QuadCenterOffset).Add(TargetPosition);
            ThrusterD.TargetPosition = Matrix.RotateVector(TargetRotation, ThrusterD.QuadCenterOffset).Add(TargetPosition);
            ThrusterE.TargetPosition = Matrix.RotateVector(TargetRotation, ThrusterE.QuadCenterOffset).Add(TargetPosition);
        }

        public void ApplyForce(Vector externalAcceleration)
        {
            this.externalAcceleration = externalAcceleration;
        }
        
        public void EstimatePosition(double dT)
        {
            if (dT > 0)
            {
                Vector TB = ThrusterB.ReturnThrustVector();
                Vector TC = ThrusterC.ReturnThrustVector();
                Vector TD = ThrusterD.ReturnThrustVector();
                Vector TE = ThrusterE.ReturnThrustVector();

                Vector thrustSum = TB.Add(TC).Add(TD).Add(TE);

                thrustSum = Matrix.RotateVector(CurrentRotation.Multiply(-1), thrustSum);

                CurrentAcceleration = positionMomentumKF.Filter(thrustSum).Add(externalAcceleration);

                //calculate velocity: finalVelocity(vf) = vi + at
                CurrentVelocity = CurrentVelocity.Add(CurrentAcceleration.Multiply(dT));

                //calculate position: displacement(s) = vi*t + 1/2 * at^2
                //CurrentPosition = currentVelocity.Multiply(dT).Add(currentAcceleration.Multiply(1 / 2).Multiply(Math.Pow(dT, 2)));

                CurrentPosition = CurrentPosition.Add(CurrentVelocity.Multiply(dT));

                lastMeasurementTime = DateTime.Now;
            }
        }
        
        public void EstimateRotation(double dT)
        {
            Vector TB = ThrusterB.ReturnThrustVector();//Thrust relative to quad origin
            Vector TC = ThrusterC.ReturnThrustVector();
            Vector TD = ThrusterD.ReturnThrustVector();
            Vector TE = ThrusterE.ReturnThrustVector();

            TB = Matrix.RotateVector(CurrentRotation.Multiply(-1), TB);//Thrust relative to environment origin
            TC = Matrix.RotateVector(CurrentRotation.Multiply(-1), TC);
            TD = Matrix.RotateVector(CurrentRotation.Multiply(-1), TD);
            TE = Matrix.RotateVector(CurrentRotation.Multiply(-1), TE);

            //TC = new Vector(0, 0, 0);
            //TD = new Vector(0, 0, 0);
            //TE = new Vector(0, 0, 0);

            //Console.WriteLine(ThrusterB.CurrentPosition + " " + ThrusterB.TargetPosition);

            //Console.WriteLine(Vector.CalculateEuclideanDistance(ThrusterB.CurrentPosition, ThrusterB.TargetPosition));

            //Console.WriteLine(TB.X + " " + AdjustThrustVector(currentRotation, TB).X);

            double torque  = armLength * Math.Sin(Misc.DegreesToRadians(180 - armAngle)) * 1.5;

            //Console.WriteLine(TB.X + " " +  TC.X + " " + TD.X + " " + TE.X);

            currentAngularAcceleration = new Vector(0, 0, 0)
            {
                X = (TB.Y + TC.Y - TD.Y - TE.Y) * torque,
                Y = (TB.X + TC.X - TD.X - TE.X) * torque + (TB.Z - TC.Z - TD.Z + TE.Z) * torque,
                Z = (-TB.Y + TC.Y + TD.Y - TE.Y) * torque
            };

            //currentAngularAcceleration = Matrix.RotateVector(CurrentRotation, currentAngularAcceleration);
            
            //currentAngularAcceleration.Y = 0;
            //currentAngularAcceleration.X = 0;

            currentAngularAcceleration = angularMomentumKF.Filter(currentAngularAcceleration);

            //current angular velocity: w = wi + at
            currentAngularVelocity = currentAngularVelocity.Add(currentAngularAcceleration.Multiply(dT));

            //calculate angular position: theta = wt + 1/2 * a * t^2
            CurrentRotation = CurrentRotation.Add(currentAngularVelocity.Multiply(dT));
            //CurrentRotation = CurrentRotation.Add(currentAngularVelocity.Multiply(dT).Add(currentAngularAcceleration.Multiply((1 / 2) * Math.Pow(dT, 2))));
        }
    }
}
