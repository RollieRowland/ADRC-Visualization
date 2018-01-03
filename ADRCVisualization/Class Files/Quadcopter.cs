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
        
        private VectorFeedbackController RotationFeedbackController;

        private double armLength;
        private double armAngle;

        private double airResistance = 0.95;

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
            currentAngularAcceleration = new Vector(0, 0, 0);
            CurrentAcceleration = new Vector(0, 0, 0);
            externalAcceleration = new Vector(0, 0, 0);
            
            RotationFeedbackController = new VectorFeedbackController()
            {
                X = new ADRC(10000, 0.001, 0.01, 0.01, 30)
                {
                    PID = new PID(15, 0, 15, 1000)
                },
                Y = new ADRC(10000, 0.001, 0.01, 0.01, 30)
                {
                    PID = new PID(15, 0, 15, 1000)
                },
                Z = new ADRC(10000, 0.001, 0.01, 0.01, 30)
                {
                    PID = new PID(15, 0, 15, 1000)
                }
            };
            
            this.armLength = armLength;
            this.armAngle = armAngle;
        }

        private void CalculateArmPositions(double ArmLength, double ArmAngle)
        {
            double XLength = ArmLength * Math.Cos(Misc.DegreesToRadians(ArmAngle));
            double ZLength = ArmLength * Math.Sin(Misc.DegreesToRadians(ArmAngle));

            Console.WriteLine("Quad Arm Length X:" + XLength + " Z:" + ZLength);

            ThrusterB = new Thruster(new Vector(-XLength, 0,  ZLength), "TB");
            ThrusterC = new Thruster(new Vector( XLength, 0,  ZLength), "TC");
            ThrusterD = new Thruster(new Vector( XLength, 0, -ZLength), "TD");
            ThrusterE = new Thruster(new Vector(-XLength, 0, -ZLength), "TE");
        }

        public void CalculateCombinedThrustVector()
        {
            //Vector positionOutput = PositionFeedbackController.Calculate(TargetPosition, CurrentPosition);//.Multiply(new Vector(-1, 1, -1));
            Vector rotationOutput = RotationFeedbackController.Calculate(TargetRotation, CurrentRotation).Multiply(-1);

            //rotationOutput = rotationFCOutput.Filter(rotationOutput);

            //rotationOutput = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), rotationOutput);

            Vector rotationOutputB = new Vector(-rotationOutput.Y, -rotationOutput.X + rotationOutput.Z, -rotationOutput.Y);//Thruster output relative to environment origin
            Vector rotationOutputC = new Vector(-rotationOutput.Y, -rotationOutput.X - rotationOutput.Z,  rotationOutput.Y);
            Vector rotationOutputD = new Vector( rotationOutput.Y,  rotationOutput.X - rotationOutput.Z,  rotationOutput.Y);
            Vector rotationOutputE = new Vector( rotationOutput.Y,  rotationOutput.X + rotationOutput.Z, -rotationOutput.Y);

            //rotationOutputB = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), rotationOutputB);//Thruster output relative to quad origin
            //rotationOutputC = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), rotationOutputC);
            //rotationOutputD = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), rotationOutputD);
            //rotationOutputE = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), rotationOutputE);
            
            ThrusterB.Calculate(CurrentRotation, rotationOutputB);
            ThrusterC.Calculate(CurrentRotation, rotationOutputC);
            ThrusterD.Calculate(CurrentRotation, rotationOutputD);
            ThrusterE.Calculate(CurrentRotation, rotationOutputE);
        }
        
        public void CalculateCurrent()
        {
            EstimatePosition(0.05);
            //EstimateRotation(0.05);
            
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
            Vector TB = ThrusterB.ReturnThrustVector();
            Vector TC = ThrusterC.ReturnThrustVector();
            Vector TD = ThrusterD.ReturnThrustVector();
            Vector TE = ThrusterE.ReturnThrustVector();

            Vector thrustSum = TB.Add(TC).Add(TD).Add(TE);

            thrustSum = Matrix.RotateVector(CurrentRotation.Multiply(-1), thrustSum);

            CurrentAcceleration = thrustSum.Add(externalAcceleration);

            //calculate velocity: finalVelocity(vf) = vi + at
            CurrentVelocity = CurrentVelocity.Add(CurrentAcceleration.Multiply(dT));//.Multiply(airResistance);

            //calculate position: displacement(s) = si + vt
            CurrentPosition = CurrentPosition.Add(CurrentVelocity.Multiply(dT));
        }
        
        public void EstimateRotation(double dT)
        {
            Vector TB = ThrusterB.ReturnThrustVector();//Thrust relative to quad origin
            Vector TC = ThrusterC.ReturnThrustVector();
            Vector TD = ThrusterD.ReturnThrustVector();
            Vector TE = ThrusterE.ReturnThrustVector();

            //Console.WriteLine(TB + " " + CurrentRotation);

            TB = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 1, -1)), TB);//Thrust relative to environment origin
            TC = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 1, -1)), TC);
            TD = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 1, -1)), TD);
            TE = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 1, -1)), TE);

            double torque  = armLength * Math.Sin(Misc.DegreesToRadians(180 - armAngle)) * 5;

            currentAngularAcceleration = new Vector(0, 0, 0)
            {
                X = (TB.Y + TC.Y - TD.Y - TE.Y) * torque,
                Y = (TB.X + TC.X - TD.X - TE.X) * torque + (TB.Z - TC.Z - TD.Z + TE.Z) * torque,
                Z = (-TB.Y + TC.Y + TD.Y - TE.Y) * torque
            };
            
            //current angular velocity: w = wi + at
            currentAngularVelocity = currentAngularVelocity.Add(currentAngularAcceleration.Multiply(dT));

            //calculate angular position: theta = thetai + wt
            CurrentRotation = CurrentRotation.Add(currentAngularVelocity.Multiply(dT));
        }
    }
}
