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
            
            Vector thrusterOutputB = new Vector(0, -rotationOutput.X + rotationOutput.Z - rotationOutput.Y, 0);//Thruster output relative to environment origin
            Vector thrusterOutputC = new Vector(0, -rotationOutput.X - rotationOutput.Z + rotationOutput.Y, 0);
            Vector thrusterOutputD = new Vector(0,  rotationOutput.X - rotationOutput.Z - rotationOutput.Y, 0);
            Vector thrusterOutputE = new Vector(0,  rotationOutput.X + rotationOutput.Z + rotationOutput.Y, 0);

            CalculateGimbalLockedMotion(ref positionOutput, ref thrusterOutputB, ref thrusterOutputC, ref thrusterOutputD, ref thrusterOutputE);
            //CalculateGimbalLockedRotation(ref rotationOutput, ref thrusterOutputB, ref thrusterOutputC, ref thrusterOutputD, ref thrusterOutputE);

            positionOutput.X = positionOutput.X + CurrentRotation.Z;//Adjust main thruster to rotation
            positionOutput = Matrix.RotateVector(new Vector(0, CurrentRotation.Y, 0), positionOutput);//adjust thruster output to quad frame, only Y dimension
            positionOutput.Z = positionOutput.Z - CurrentRotation.X;//Adjust secondary thruster to rotation

            ThrusterB.Calculate(thrusterOutputB.Add(positionOutput));
            ThrusterC.Calculate(thrusterOutputC.Add(positionOutput));
            ThrusterD.Calculate(thrusterOutputD.Add(positionOutput));
            ThrusterE.Calculate(thrusterOutputE.Add(positionOutput));
        }

        private void CalculateGimbalLockedMotion(ref Vector positionControl, ref Vector thrusterOutputB, ref Vector thrusterOutputC, ref Vector thrusterOutputD, ref Vector thrusterOutputE)
        {
            double curvature = 8;
            double fadeIn = (1.0 / 90.0) * Math.Pow((90.0 - Math.Abs(CurrentRotation.Z % (90.0 * 2) - 90.0)), curvature) / Math.Pow(90, curvature - 1);//0 -> 1, New position control faders, approaches 1 when z rot is -90/90
            double fadeOut = 1 - fadeIn;//1 -> 0, Rotation magnitude faders, approaches 0 when Z rot is -90/90 degrees

            double rotation = 30 * fadeIn;
            
            double magnitude = Math.Sqrt(Math.Pow(positionControl.X, 2) + Math.Pow(positionControl.Z, 2));//Give hypotenuse for origin rotation, magnitude
            double angle = Misc.RadiansToDegrees(Math.Sign(CurrentRotation.Z) * Math.Atan2(magnitude, 0) - Math.Atan2(positionControl.Z, positionControl.X));//Determine angle of output, -180 -> 180

            //Rotation matrix on position control copy
            Vector RotatedControl = Matrix.RotateVector(new Vector(0, -CurrentRotation.Y, 0), new Vector(positionControl.X, 0, positionControl.Z));

            //---- (X-), ++++ (X+), +-+- (Z+), -+-+ (Z-)
            thrusterOutputB.X = thrusterOutputB.X * fadeOut + (RotatedControl.X * fadeIn) + (RotatedControl.Z * fadeIn);
            thrusterOutputC.X = thrusterOutputC.X * fadeOut + (RotatedControl.X * fadeIn) - (RotatedControl.Z * fadeIn);
            thrusterOutputD.X = thrusterOutputD.X * fadeOut + (RotatedControl.X * fadeIn) + (RotatedControl.Z * fadeIn);
            thrusterOutputE.X = thrusterOutputE.X * fadeOut + (RotatedControl.X * fadeIn) - (RotatedControl.Z * fadeIn);

            //Alternate Z rotation in individual thrusters, preventing rotation bias
            thrusterOutputB.Z = thrusterOutputB.Z * fadeOut + rotation;
            thrusterOutputC.Z = thrusterOutputC.Z * fadeOut - rotation;
            thrusterOutputD.Z = thrusterOutputD.Z * fadeOut + rotation;
            thrusterOutputE.Z = thrusterOutputE.Z * fadeOut - rotation;

            positionControl.X *= fadeOut;
            positionControl.Z *= fadeOut;

            //Console.WriteLine(Misc.DoubleToString(thrusterOutputB.Y));

            //Console.WriteLine(Misc.DoubleToString(CurrentRotation.Z) + " " + Misc.DoubleToString(fadeIn) + " " + Misc.DoubleToString(fadeOut));
        }

        private void CalculateGimbalLockedRotation(ref Vector rotationControl, ref Vector thrusterOutputB, ref Vector thrusterOutputC, ref Vector thrusterOutputD, ref Vector thrusterOutputE)
        {
            //Create new 
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
            
            //Adjust thrust output so that it is relative to origin
            TB = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 0, -1)), TB);
            TB = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(0, -1, 0)), TB);
            TC = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 0, -1)), TC);
            TC = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(0, -1, 0)), TC);
            TD = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 0, -1)), TD);
            TD = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(0, -1, 0)), TD);
            TE = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 0, -1)), TE);
            TE = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(0, -1, 0)), TE);

            //Adjusts rotation output for Z when rotating about Z - simulated gimbal lock
            Vector rotationTBX = Matrix.RotateVector(new Vector(0, 0, -CurrentRotation.Z), new Vector(TB.X, 0, 0));
            Vector rotationTBZ = Matrix.RotateVector(new Vector(-CurrentRotation.Z, 0, 0), new Vector(0, 0, TB.Z));
            Vector rotationTCX = Matrix.RotateVector(new Vector(0, 0, CurrentRotation.Z), new Vector(TC.X, 0, 0));
            Vector rotationTCZ = Matrix.RotateVector(new Vector(CurrentRotation.Z, 0, 0), new Vector(0, 0, TC.Z));
            Vector rotationTDX = Matrix.RotateVector(new Vector(0, 0, CurrentRotation.Z), new Vector(TD.X, 0, 0));
            Vector rotationTDZ = Matrix.RotateVector(new Vector(CurrentRotation.Z, 0, 0), new Vector(0, 0, TD.Z));
            Vector rotationTEX = Matrix.RotateVector(new Vector(0, 0, -CurrentRotation.Z), new Vector(TE.X, 0, 0));
            Vector rotationTEZ = Matrix.RotateVector(new Vector(-CurrentRotation.Z, 0, 0), new Vector(0, 0, TE.Z));

            //Rotate the Inner joint about the outer joint
            Vector outputTBZ = Matrix.RotateVector(new Vector(0, TB.Z, 0), new Vector(TB.X, 0, 0));
            Vector outputTCZ = Matrix.RotateVector(new Vector(0, TC.Z, 0), new Vector(TC.X, 0, 0));
            Vector outputTDZ = Matrix.RotateVector(new Vector(0, TD.Z, 0), new Vector(TD.X, 0, 0));
            Vector outputTEZ = Matrix.RotateVector(new Vector(0, TE.Z, 0), new Vector(TE.X, 0, 0));

            TB.X = rotationTBX.X + outputTBZ.X;//Reduced when rotated
            TB.Z = rotationTBZ.Z + outputTBZ.Z;//Added to when rotated
            TC.X = rotationTCX.X + outputTCZ.X;//Reduced when rotated
            TC.Z = rotationTCZ.Z + outputTCZ.Z;//Added to when rotated
            TD.X = rotationTDX.X + outputTDZ.X;//Reduced when rotated
            TD.Z = rotationTDZ.Z + outputTDZ.Z;//Added to when rotated
            TE.X = rotationTEX.X + outputTEZ.X;//Reduced when rotated
            TE.Z = rotationTEZ.Z + outputTEZ.Z;//Added to when rotated

            Vector thrustSum = TB.Add(TC).Add(TD).Add(TE);
            
            //Rotation output negates when over 90 due to rotation matrix, this solution should be expanded to allow multiple increments
            if (CurrentRotation.X >= 90 || CurrentRotation.Z <= -90)
            {
                // % 180
                thrustSum.X *= -1;
            }

            //thrustSum.X *= Math.Sign(CurrentRotation.Z % (90.0 * 2));

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

            TB = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 0, -1)), TB);//Thrust relative to environment origin
            TC = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 0, -1)), TC);
            TD = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 0, -1)), TD);
            TE = Matrix.RotateVector(CurrentRotation.Multiply(new Vector(-1, 0, -1)), TE);

            double torque  = armLength * Math.Sin(Misc.DegreesToRadians(180 - armAngle)) * 5;

            currentAngularAcceleration = new Vector(0, 0, 0)
            {
                X = (TB.Y + TC.Y - TD.Y - TE.Y) * torque,
                Y = (TB.X + TC.X - TD.X - TE.X) * torque + (TB.Z - TC.Z - TD.Z + TE.Z) * torque,
                Z = (-TB.Y + TC.Y + TD.Y - TE.Y) * torque
            };

            //TB + TD - (TC + TE)
            Vector differentialThrustRotation = TB.Add(TD).Add(TC.Add(TE).Multiply(-1)).Multiply(0.15);

            currentAngularAcceleration = currentAngularAcceleration.Add(differentialThrustRotation);

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
