﻿using System;
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
                X = new ADRC(20, 200, 4, 10, 30)
                {
                    PID = new PID(0.5, 0, 0.75, 1000)
                },
                Y = new ADRC(10, 10, 1.5, 64, 30)
                {
                    PID = new PID(1, 0, 2.5, 1000)
                },
                Z = new ADRC(20, 200, 4, 10, 30)
                {
                    PID = new PID(0.5, 0, 0.75, 1000)
                }
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
            Vector rotationOutput = RotationFeedbackController.Calculate(new Vector(0, 0, 0), CurrentRotation.Subtract(TargetRotation), samplingPeriod).Multiply(-1);
            Vector positionOutput = PositionFeedbackController.Calculate(new Vector(0, 0, 0), CurrentPosition.Subtract(TargetPosition), samplingPeriod);

            positionOutput.X = positionOutput.X + CurrentRotation.Z;

            positionOutput = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), positionOutput);//adjust thruster output to quad frame, only Y dimension

            positionOutput.Z = positionOutput.Z - CurrentRotation.X;
            
            Vector thrusterOutputB = new Vector(-rotationOutput.Y, -rotationOutput.X + rotationOutput.Z, -rotationOutput.Y);//Thruster output relative to environment origin
            Vector thrusterOutputC = new Vector(-rotationOutput.Y, -rotationOutput.X - rotationOutput.Z,  rotationOutput.Y);
            Vector thrusterOutputD = new Vector( rotationOutput.Y,  rotationOutput.X - rotationOutput.Z,  rotationOutput.Y);
            Vector thrusterOutputE = new Vector( rotationOutput.Y,  rotationOutput.X + rotationOutput.Z, -rotationOutput.Y);

            //rotate position output to compensate for change in thrust when manipulating B and D thrusters
            //when in -90/90 Z position:
            //two thrusters are allocated to translation in the X dimension
            //the other two are allocated to translation in the Z dimension

            //Fade the two rotation/translation controls together
            /*
            thrustSum.X = outputXZ.X;
            thrustSum.Z = rotationZ.Z + outputXZ.Z;
            */

            Vector rotationZ = Matrix.RotateVector(new Vector(CurrentRotation.Z, 0, 0), new Vector(0, 0, positionOutput.Z));
            Vector outputXZ  = Matrix.RotateVector(new Vector(0, positionOutput.Z, 0),  new Vector(positionOutput.X, 0, 0));

            //X and Z outputs known
            //need to find angle for primary joint rotation

            //Console.WriteLine(rotationZ + " " + outputXZ);

            //positionOutput.X = positionOutput.X;
            //positionOutput.Z = rotationZ.Z + outputXZ.Z;//gimbal lock
            
            thrusterOutputB = thrusterOutputB.Add(positionOutput);
            thrusterOutputC = thrusterOutputC.Add(positionOutput);//.Add(new Vector(0, 0, CurrentRotation.Z));
            thrusterOutputD = thrusterOutputD.Add(positionOutput);
            thrusterOutputE = thrusterOutputE.Add(positionOutput);//.Add(new Vector(0, 0, CurrentRotation.Z));

            ThrusterB.Calculate(thrusterOutputB);
            ThrusterC.Calculate(thrusterOutputC);
            ThrusterD.Calculate(thrusterOutputD);
            ThrusterE.Calculate(thrusterOutputE);//.Add(new Vector(0, 0, CurrentRotation.Z))
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

            //Adjust thrust output so that it is relative to origin
            thrustSum = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, -1, -1)), thrustSum);

            //X nothing changes
            //Y nothing changes
            //Z rotation changes, gimbal lock

            //Z rotation calculation
            //uses X as primary joint

            //Adjusts rotation output for Z when rotating about Z - simulated gimbal lock
            Vector rotationZ = Matrix.RotateVector(new Vector(CurrentRotation.Z, 0, 0), new Vector(0, 0, thrustSum.Z));

            //Rotate the Inner joint about the outer joint
            Vector outputXZ = Matrix.RotateVector(new Vector(0, thrustSum.Z, 0), new Vector(thrustSum.X, 0, 0));
            
            thrustSum.X = outputXZ.X;
            thrustSum.Z = rotationZ.Z + outputXZ.Z;

            //Rotation output negates when over 90 due to rotation matrix, this solution should be expanded to allow multiple increments
            if (CurrentRotation.X >= 90 || CurrentRotation.Z <= -90)
            {
                thrustSum.X *= -1;
            }

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

            TB = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 0, 0)), TB);//Thrust relative to environment origin
            TC = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 0, 0)), TC);
            TD = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 0, 0)), TD);
            TE = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 0, 0)), TE);

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
