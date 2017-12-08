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
        public Thruster ThrusterB { get; private set; }
        public Thruster ThrusterC { get; private set; }
        public Thruster ThrusterD { get; private set; }
        public Thruster ThrusterE { get; private set; }
        private DateTime lastMeasurementTime;

        public Vector CurrentPosition { get; private set; }
        public Vector TargetPosition { get; private set; }
        private Vector currentRotation;
        private Vector targetRotation;

        private Vector currentVelocity;
        private Vector currentAngularVelocity;
        private Vector currentAcceleration;
        private Vector currentAngularAcceleration;

        private Vector externalAcceleration;
        

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
            currentRotation = new Vector(0, 0, 0);
            targetRotation = new Vector(0, 0, 0);
            currentVelocity = new Vector(0, 0, 0);
            currentAngularVelocity = new Vector(0, 0, 0);

            externalAcceleration = new Vector(0, 0, 0);

            PositionPID = new VectorPID(new Vector(150, 15, 150), new Vector(0, 0, 0), new Vector(20, 0.025, 20), new Vector(90, 100, 90));
            RotationPID = new VectorPID(new Vector(0.02, 20, 0.02), new Vector(0, 0, 0), new Vector(0.02, 10, 0.02), new Vector(45, 45, 45));
            
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
            Vector positionOutput = new Vector(0, 0, 0);
            Vector rotationOutput = new Vector(0, 0, 0);

            positionOutput = PositionPID.Calculate(TargetPosition, CurrentPosition);
            rotationOutput = RotationPID.Calculate(targetRotation, currentRotation).Multiply(-1);

            Vector rotationOutputB = new Vector(-rotationOutput.Y / 2, -rotationOutput.X + rotationOutput.Z,  rotationOutput.Y / 2);//Thruster output relative to environment origin
            Vector rotationOutputC = new Vector(-rotationOutput.Y / 2, -rotationOutput.X - rotationOutput.Z,  rotationOutput.Y / 2);
            Vector rotationOutputD = new Vector( rotationOutput.Y / 2,  rotationOutput.X - rotationOutput.Z, -rotationOutput.Y / 2);
            Vector rotationOutputE = new Vector( rotationOutput.Y / 2,  rotationOutput.X + rotationOutput.Z, -rotationOutput.Y / 2);

            //Motors need to be rotated relative to the ground
            //Vector rotatedPositionOutputB = Matrix.RotateVector(currentRotation, rotationOutputB);//Thruster output relative to quad origin
            //Vector rotatedPositionOutputC = Matrix.RotateVector(currentRotation, rotationOutputC);
            //Vector rotatedPositionOutputD = Matrix.RotateVector(currentRotation, rotationOutputD);
            //Vector rotatedPositionOutputE = Matrix.RotateVector(currentRotation, rotationOutputE);

            ThrusterB.SetOutputs(rotationOutputB.Add(positionOutput));
            ThrusterC.SetOutputs(rotationOutputC.Add(positionOutput));
            ThrusterD.SetOutputs(rotationOutputD.Add(positionOutput));
            ThrusterE.SetOutputs(rotationOutputE.Add(positionOutput));
        }
        
        public void CalculateCurrent()
        {
            EstimatePosition(0.1);
            EstimateRotation(0.6);
            
            //calculate rotation matrix
            ThrusterB.CurrentPosition = Matrix.RotateVector(currentRotation, ThrusterB.QuadCenterOffset).Add(CurrentPosition);
            ThrusterC.CurrentPosition = Matrix.RotateVector(currentRotation, ThrusterC.QuadCenterOffset).Add(CurrentPosition);
            ThrusterD.CurrentPosition = Matrix.RotateVector(currentRotation, ThrusterD.QuadCenterOffset).Add(CurrentPosition);
            ThrusterE.CurrentPosition = Matrix.RotateVector(currentRotation, ThrusterE.QuadCenterOffset).Add(CurrentPosition);
        }

        public void SetTarget(Vector position, Vector rotation)
        {
            TargetPosition = position;
            targetRotation = rotation;

            //calculate rotation matrix
            ThrusterB.TargetPosition = Matrix.RotateVector(targetRotation, ThrusterB.QuadCenterOffset).Add(TargetPosition);
            ThrusterC.TargetPosition = Matrix.RotateVector(targetRotation, ThrusterC.QuadCenterOffset).Add(TargetPosition);
            ThrusterD.TargetPosition = Matrix.RotateVector(targetRotation, ThrusterD.QuadCenterOffset).Add(TargetPosition);
            ThrusterE.TargetPosition = Matrix.RotateVector(targetRotation, ThrusterE.QuadCenterOffset).Add(TargetPosition);
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

                //thrustSum = RotateVector(currentRotation, thrustSum);

                currentAcceleration = thrustSum.Add(externalAcceleration);

                //currentAcceleration = AdjustThrustVector(currentRotation, TB);

                //calculate velocity: finalVelocity(vf) = vi + at
                currentVelocity = currentVelocity.Add(currentAcceleration.Multiply(dT));

                //calculate position: displacement(s) = vi*t + 1/2 * at^2
                CurrentPosition = currentVelocity.Multiply(dT).Add(currentAcceleration.Multiply(1 / 2).Multiply(Math.Pow(dT, 2)));
                
                lastMeasurementTime = DateTime.Now;
            }
        }
        
        public void EstimateRotation(double dT)
        {
            Vector TB = ThrusterB.ReturnThrustVector();//Thrust relative to quad origin
            Vector TC = ThrusterC.ReturnThrustVector();
            Vector TD = ThrusterD.ReturnThrustVector();
            Vector TE = ThrusterE.ReturnThrustVector();

            //TB = AdjustThrustVector(currentRotation, TB);//Thrust relative to environment origin
            //TC = AdjustThrustVector(currentRotation, TC);
            //TD = AdjustThrustVector(currentRotation, TD);
            //TE = AdjustThrustVector(currentRotation, TE);

            //Console.WriteLine(TB.X + " " + AdjustThrustVector(currentRotation, TB).X);

            double torque  = armLength * Math.Sin(Misc.DegreesToRadians(180 - armAngle));

            Vector rotationalForce = new Vector(0, 0, 0)
            {
                X = (TB.Y + TC.Y - TD.Y - TE.Y) * torque,
                Y = (TB.X + TC.X - TD.X - TE.X) * torque,
                Z = (-TB.Y + TC.Y + TD.Y - TE.Y) * torque
            };

            //current angular velocity: w = wi + at
            currentAngularVelocity = currentAngularVelocity.Add(rotationalForce.Multiply(dT));

            //calculate angular position: theta = wt + 1/2 * a * t^2
            currentRotation = currentRotation.Add(currentAngularVelocity.Multiply(dT));
        }
    }
}
