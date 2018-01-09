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
        private VectorFeedbackController PositionFeedbackController;

        private double armLength;
        private double armAngle;
        private double samplingPeriod;

        public Quadcopter(double armLength, double armAngle, double samplingPeriod)
        {
            this.armLength = armLength;
            this.armAngle = armAngle;
            this.samplingPeriod = samplingPeriod;

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

            PositionFeedbackController = new VectorFeedbackController()
            {
                X = new ADRC(50, 200, 4, 10, 30)
                {
                    PID = new PID(10, 0, 12.5, 1000)
                },
                Y = new ADRC(10, 10, 1.5, 0.05, 100)
                {
                    PID = new PID(1, 0, 0.2, 1000)
                },
                Z = new ADRC(50, 200, 4, 10, 30)
                {
                    PID = new PID(10, 0, 12.5, 1000)
                }
            };

            RotationFeedbackController = new VectorFeedbackController()
            {
                /*X = new ADRC(50, 200, 4, 10, 30)
                {
                    PID = new PID(0.05, 0, 0.75, 1000)
                },
                Y = new ADRC(10, 10, 1.5, 0.05, 100)
                {
                    PID = new PID(1, 0, 2.5, 1000)
                },
                Z = new ADRC(50, 200, 4, 10, 30)
                {
                    PID = new PID(0.05, 0, 0.75, 1000)
                }*/
                X = new PID(0.1, 0, 0.75, 30),
                Y = new PID(1, 0, 2.5, 30),
                Z = new PID(0.1, 0, 0.75, 30)
            };
        }

        private void CalculateArmPositions(double ArmLength, double ArmAngle)
        {
            double XLength = ArmLength * Math.Cos(Misc.DegreesToRadians(ArmAngle));
            double ZLength = ArmLength * Math.Sin(Misc.DegreesToRadians(ArmAngle));

            Console.WriteLine("Quad Arm Length X:" + XLength + " Z:" + ZLength + " Period:" + samplingPeriod);

            ThrusterB = new Thruster(new Vector(-XLength, 0,  ZLength), "TB", samplingPeriod);
            ThrusterC = new Thruster(new Vector( XLength, 0,  ZLength), "TC", samplingPeriod);
            ThrusterD = new Thruster(new Vector( XLength, 0, -ZLength), "TD", samplingPeriod);
            ThrusterE = new Thruster(new Vector(-XLength, 0, -ZLength), "TE", samplingPeriod);
        }

        public void CalculateCombinedThrustVector()
        {
            //string test =  ((ADRC)PositionFeedbackController.X).SetOffset(CurrentRotation.Z);
            //string test2 = ((ADRC)PositionFeedbackController.Z).SetOffset(-CurrentRotation.X);

            Vector rotationOutput = RotationFeedbackController.Calculate(new Vector(0, 0, 0), CurrentRotation.Subtract(TargetRotation), samplingPeriod).Multiply(-1);
            Vector positionOutput = PositionFeedbackController.Calculate(new Vector(0, 0, 0), CurrentPosition.Subtract(TargetPosition), samplingPeriod);

            positionOutput = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), positionOutput);//adjust thruster output to quad frame, only Y dimension

            Console.WriteLine(CurrentPosition.Subtract(TargetPosition) + " |" + positionOutput + " |");

            positionOutput.X = positionOutput.X + CurrentRotation.Z;
            positionOutput.Z = positionOutput.Z - CurrentRotation.X;

            Vector rotationOutputB = new Vector(-rotationOutput.Y, -rotationOutput.X + rotationOutput.Z, -rotationOutput.Y);//Thruster output relative to environment origin
            Vector rotationOutputC = new Vector(-rotationOutput.Y, -rotationOutput.X - rotationOutput.Z,  rotationOutput.Y);
            Vector rotationOutputD = new Vector( rotationOutput.Y,  rotationOutput.X - rotationOutput.Z,  rotationOutput.Y);
            Vector rotationOutputE = new Vector( rotationOutput.Y,  rotationOutput.X + rotationOutput.Z, -rotationOutput.Y);
            
            ThrusterB.Calculate(CurrentRotation, rotationOutputB.Add(positionOutput));
            ThrusterC.Calculate(CurrentRotation, rotationOutputC.Add(positionOutput));
            ThrusterD.Calculate(CurrentRotation, rotationOutputD.Add(positionOutput));
            ThrusterE.Calculate(CurrentRotation, rotationOutputE.Add(positionOutput));
        }
        
        public void CalculateCurrent()
        {
            EstimatePosition(samplingPeriod);
            EstimateRotation(samplingPeriod);
            
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

            TB = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 1, -1)), TB);//Thrust relative to environment origin
            TC = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 1, -1)), TC);
            TD = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 1, -1)), TD);
            TE = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 1, -1)), TE);

            TB = Matrix.RotateVector(new Vector(0, -CurrentRotation.Y, 0), TB);
            TC = Matrix.RotateVector(new Vector(0, -CurrentRotation.Y, 0), TC);
            TD = Matrix.RotateVector(new Vector(0, -CurrentRotation.Y, 0), TD);
            TE = Matrix.RotateVector(new Vector(0, -CurrentRotation.Y, 0), TE);

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

        private bool DetectAgitation()
        {
            //rapid rotation in any dimension
            //rapid movement or oscillation in the X or Z axes
            //rapid increase or decrease in height
            
            //angular velocity of certain speed

            //ESCs can be used to detect a dead motor/prop
            //

            throw new NotImplementedException();
        }
    }
}
