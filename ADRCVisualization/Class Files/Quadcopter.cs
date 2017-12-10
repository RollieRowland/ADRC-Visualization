using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.QuadcopterSimulation;
using ADRCVisualization.Class_Files.Mathematics;

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

        private Vector currentVelocity;
        private Vector currentAngularVelocity;
        private Vector currentAcceleration;
        private Vector currentAngularAcceleration;

        private Vector externalAcceleration;

        private VectorKalmanFilter positionMomentumKF;

        private VectorPID PositionPID;
        private VectorPID RotationPID;

        private double armLength;
        private double armAngle;

        public Quadcopter(double armLength, double armAngle)
        {
            CalculateArmPositions(armLength, armAngle);
            lastMeasurementTime = DateTime.Now;

            CurrentPosition = new Vector(0, 0, 0);
            TargetPosition = new Vector(0, 0, 0);
            CurrentRotation = new Vector(0, 0, 0);
            TargetRotation = new Vector(0, 0, 0);
            currentVelocity = new Vector(0, 0, 0);
            currentAngularVelocity = new Vector(0, 0, 0);

            externalAcceleration = new Vector(0, 0, 0);

            PositionPID = new VectorPID(new Vector(150, 15, 150), new Vector(0, 0, 0), new Vector(20, 10, 20), new Vector(45, 20, 45));
            RotationPID = new VectorPID(new Vector(0.01, 20, 0.01), new Vector(0, 0, 0), new Vector(0.01, 10, 0.01), new Vector(90, 90, 90));

            positionMomentumKF = new VectorKalmanFilter(0.5, 1);
            
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

        public void CalculateIndividualThrustVectors()
        {
            ThrusterB.Calculate();
            ThrusterC.Calculate();
            ThrusterD.Calculate();
            ThrusterE.Calculate();
        }

        public void CalculateCombinedThrustVector()
        {
            Vector positionOutput = PositionPID.Calculate(TargetPosition, CurrentPosition);//.Multiply(new Vector(-1, 1, -1));
            Vector rotationOutput = RotationPID.Calculate(TargetRotation, CurrentRotation).Multiply(-1);

            Vector rotationOutputB = new Vector(-rotationOutput.Y, -rotationOutput.X + rotationOutput.Z, 0);//Thruster output relative to environment origin
            Vector rotationOutputC = new Vector(-rotationOutput.Y, -rotationOutput.X - rotationOutput.Z, 0);
            Vector rotationOutputD = new Vector( rotationOutput.Y,  rotationOutput.X - rotationOutput.Z, 0);
            Vector rotationOutputE = new Vector( rotationOutput.Y,  rotationOutput.X + rotationOutput.Z, 0);
            
            //Motors need to be rotated relative to the ground
            //rotationOutputB = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), rotationOutputB);//Thruster output relative to quad origin
            //rotationOutputC = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), rotationOutputC);
            //rotationOutputD = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), rotationOutputD);
            //rotationOutputE = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), rotationOutputE);

            //rotationOutputB = Matrix.RotateVector(new Vector(0, 0, 0), rotationOutputB);//Thruster output relative to quad origin
            //rotationOutputC = Matrix.RotateVector(new Vector(0, 0, 0), rotationOutputC);
            //rotationOutputD = Matrix.RotateVector(new Vector(0, 0, 0), rotationOutputD);
            //rotationOutputE = Matrix.RotateVector(new Vector(0, 0, 0), rotationOutputE);

            //Console.WriteLine(ThrusterB.CurrentRotation);

            if (rotationOutputB.Y < 0)
            {
                rotationOutputD.Y -= rotationOutputB.Y;
                rotationOutputB.Y = 0;
            }
            
            if (rotationOutputC.Y < 0)
            {
                rotationOutputE.Y -= rotationOutputC.Y;
                rotationOutputC.Y = 0;
            }

            if (rotationOutputD.Y < 0)
            {
                rotationOutputB.Y -= rotationOutputD.Y;
                rotationOutputD.Y = 0;
            }

            if (rotationOutputE.Y < 0)
            {
                rotationOutputC.Y -= rotationOutputE.Y;
                rotationOutputE.Y = 0;
            }

            //Console.WriteLine(rotationOutputB.Y + " " + rotationOutputC.Y + " " + rotationOutputD.Y + " " + rotationOutputE.Y);

            ThrusterB.SetOutputs(rotationOutputB.Add(positionOutput));
            ThrusterC.SetOutputs(rotationOutputC.Add(positionOutput));
            ThrusterD.SetOutputs(rotationOutputD.Add(positionOutput));
            ThrusterE.SetOutputs(rotationOutputE.Add(positionOutput));

            //Console.WriteLine(ThrusterB.CurrentRotation + " ");
        }
        
        public void CalculateCurrent()
        {
            EstimatePosition(0.1);
            EstimateRotation(0.6);
            
            //calculate rotation matrix
            ThrusterB.CurrentPosition = Matrix.RotateVector(CurrentRotation, ThrusterB.QuadCenterOffset).Add(CurrentPosition);
            ThrusterC.CurrentPosition = Matrix.RotateVector(CurrentRotation, ThrusterC.QuadCenterOffset).Add(CurrentPosition);
            ThrusterD.CurrentPosition = Matrix.RotateVector(CurrentRotation, ThrusterD.QuadCenterOffset).Add(CurrentPosition);
            ThrusterE.CurrentPosition = Matrix.RotateVector(CurrentRotation, ThrusterE.QuadCenterOffset).Add(CurrentPosition);
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

                //thrustSum = Matrix.RotateVector(CurrentRotation, thrustSum);

                currentAcceleration = thrustSum.Add(externalAcceleration);//.Add(Matrix.RotateVector(CurrentRotation, externalAcceleration.Multiply(-1)));
                
                //calculate velocity: finalVelocity(vf) = vi + at
                currentVelocity = currentVelocity.Add(currentAcceleration.Multiply(dT));

                //calculate position: displacement(s) = vi*t + 1/2 * at^2
                CurrentPosition = positionMomentumKF.Filter(currentVelocity.Multiply(dT).Add(currentAcceleration.Multiply(1 / 2).Multiply(Math.Pow(dT, 2))));
                
                lastMeasurementTime = DateTime.Now;
            }
        }
        
        public void EstimateRotation(double dT)
        {
            Vector TB = ThrusterB.ReturnThrustVector().Add(externalAcceleration);//Thrust relative to quad origin
            Vector TC = ThrusterC.ReturnThrustVector().Add(externalAcceleration);
            Vector TD = ThrusterD.ReturnThrustVector().Add(externalAcceleration);
            Vector TE = ThrusterE.ReturnThrustVector().Add(externalAcceleration);

            TB = Matrix.RotateVector(CurrentRotation, TB);//Thrust relative to environment origin
            TC = Matrix.RotateVector(CurrentRotation, TC);
            TD = Matrix.RotateVector(CurrentRotation, TD);
            TE = Matrix.RotateVector(CurrentRotation, TE);

            //Console.WriteLine(TB.X + " " + AdjustThrustVector(currentRotation, TB).X);

            double torque  = armLength * Math.Sin(Misc.DegreesToRadians(180 - armAngle));

            currentAngularAcceleration = new Vector(0, 0, 0)
            {
                X = (TB.Y + TC.Y - TD.Y - TE.Y) * torque,
                Y = (TB.X + TC.X - TD.X - TE.X) * torque,
                Z = (-TB.Y + TC.Y + TD.Y - TE.Y) * torque
            };

            //current angular velocity: w = wi + at
            currentAngularVelocity = currentAngularVelocity.Add(currentAngularAcceleration.Multiply(dT));

            //calculate angular position: theta = wt + 1/2 * a * t^2
            CurrentRotation = CurrentRotation.Add(currentAngularVelocity.Multiply(dT));
        }
    }
}
