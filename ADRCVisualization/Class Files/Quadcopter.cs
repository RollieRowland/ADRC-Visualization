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
        public Vector CurrentEulerRotation { get; private set; }
        public Vector TargetEulerRotation { get; private set; }

        public Quaternion QuatCurrentRotation { get; private set; }
        public Quaternion QuatTargetRotation { get; private set; }

        private Vector currentVelocity;
        private Vector currentAngularVelocity;
        private Vector currentAcceleration;
        private Vector currentAngularAcceleration;

        private Vector externalAcceleration;

        private VectorFeedbackController PositionFeedbackController;
        private VectorFeedbackController RotationFeedbackController;

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

            CurrentPosition            = new Vector(0, 0, 0);
            TargetPosition             = new Vector(0, 0, 0);
            CurrentEulerRotation       = new Vector(0, 0, 0);
            TargetEulerRotation        = new Vector(0, 0, 0);
            currentVelocity            = new Vector(0, 0, 0);
            currentAngularVelocity     = new Vector(0, 0, 0);
            currentAngularAcceleration = new Vector(0, 0, 0);
            currentAcceleration        = new Vector(0, 0, 0);
            externalAcceleration       = new Vector(0, 0, 0);

            QuatCurrentRotation = new Quaternion(1, 0, 0, 0);
            QuatTargetRotation  = new Quaternion(1, 0, 0, 0);

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
            double XLength = ArmLength * Math.Cos(MathE.DegreesToRadians(ArmAngle));
            double ZLength = ArmLength * Math.Sin(MathE.DegreesToRadians(ArmAngle));

            Console.WriteLine("Quad Arm Length X:" + XLength + " Z:" + ZLength + " Period:" + samplingPeriod);

            ThrusterB = new Thruster(new Vector(-XLength, 0,  ZLength), "TB", samplingPeriod);
            ThrusterC = new Thruster(new Vector( XLength, 0,  ZLength), "TC", samplingPeriod);
            ThrusterD = new Thruster(new Vector( XLength, 0, -ZLength), "TD", samplingPeriod);
            ThrusterE = new Thruster(new Vector(-XLength, 0, -ZLength), "TE", samplingPeriod);
        }

        public void CalculateCombinedThrustVector()
        {
            Vector rotationOutput = RotationFeedbackController.Calculate(new Vector(0, 0, 0), CurrentEulerRotation.Subtract(TargetEulerRotation), samplingPeriod).Multiply(-1);
            Vector positionOutput = PositionFeedbackController.Calculate(new Vector(0, 0, 0), CurrentPosition.Subtract(TargetPosition), samplingPeriod);
            
            Vector thrusterOutputB = new Vector(0, -rotationOutput.X + rotationOutput.Z - rotationOutput.Y, 0);//Thruster output relative to environment origin
            Vector thrusterOutputC = new Vector(0, -rotationOutput.X - rotationOutput.Z + rotationOutput.Y, 0);
            Vector thrusterOutputD = new Vector(0,  rotationOutput.X - rotationOutput.Z - rotationOutput.Y, 0);
            Vector thrusterOutputE = new Vector(0,  rotationOutput.X + rotationOutput.Z + rotationOutput.Y, 0);

            CalculateGimbalLockedMotion(ref positionOutput, ref thrusterOutputB, ref thrusterOutputC, ref thrusterOutputD, ref thrusterOutputE);
            //CalculateGimbalLockedRotation(ref rotationOutput, ref thrusterOutputB, ref thrusterOutputC, ref thrusterOutputD, ref thrusterOutputE);

            //Due to XYZ permutation order of Euler angle
            positionOutput.X = positionOutput.X + CurrentEulerRotation.Z;//Adjust main joint to rotation
            positionOutput = Matrix.RotateVector(new Vector(0, CurrentEulerRotation.Y, 0), positionOutput);//adjust thruster output to quad frame, only Y dimension
            positionOutput.Z = positionOutput.Z - CurrentEulerRotation.X;//Adjust secondary joint to rotation

            ThrusterB.Calculate(thrusterOutputB.Add(positionOutput));
            ThrusterC.Calculate(thrusterOutputC.Add(positionOutput));
            ThrusterD.Calculate(thrusterOutputD.Add(positionOutput));
            ThrusterE.Calculate(thrusterOutputE.Add(positionOutput));
        }

        private void CalculateGimbalLockedMotion(ref Vector positionControl, ref Vector thrusterOutputB, ref Vector thrusterOutputC, ref Vector thrusterOutputD, ref Vector thrusterOutputE)
        {
            double curvature = 8;// 0.5: |‾ , 1: /, 2, _|
            double fadeIn = (1.0 / 90.0) * Math.Pow((90.0 - Math.Abs(CurrentEulerRotation.Z % (90.0 * 2) - 90.0)), curvature) / Math.Pow(90, curvature - 1);//0 -> 1, New position control faders, approaches 1 when z rot is -90/90
            double fadeOut = 1 - fadeIn;//1 -> 0, Rotation magnitude faders, approaches 0 when Z rot is -90/90 degrees

            double rotation = 30 * fadeIn;
            
            double magnitude = Math.Sqrt(Math.Pow(positionControl.X, 2) + Math.Pow(positionControl.Z, 2));//Give hypotenuse for origin rotation, magnitude
            double angle = MathE.RadiansToDegrees(Math.Sign(CurrentEulerRotation.Z) * Math.Atan2(magnitude, 0) - Math.Atan2(positionControl.Z, positionControl.X));//Determine angle of output, -180 -> 180

            //Rotation matrix on position control copy
            Vector RotatedControl = Matrix.RotateVector(new Vector(0, -CurrentEulerRotation.Y, 0), new Vector(positionControl.X, 0, positionControl.Z));

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
            ThrusterB.CurrentPosition = QuatCurrentRotation.UnrotateVector(ThrusterB.QuadCenterOffset).Add(CurrentPosition);
            ThrusterC.CurrentPosition = QuatCurrentRotation.UnrotateVector(ThrusterC.QuadCenterOffset).Add(CurrentPosition);
            ThrusterD.CurrentPosition = QuatCurrentRotation.UnrotateVector(ThrusterD.QuadCenterOffset).Add(CurrentPosition);
            ThrusterE.CurrentPosition = QuatCurrentRotation.UnrotateVector(ThrusterE.QuadCenterOffset).Add(CurrentPosition);
        }

        public void SetTarget(Vector position, Vector rotation)
        {
            TargetPosition = new Vector(position);
            TargetEulerRotation = new Vector(rotation);

            QuatTargetRotation = Quaternion.EulerToQuaternion(new EulerAngles(TargetEulerRotation, EulerConstants.EulerOrderXYZS));

            //calculate rotation matrix
            ThrusterB.TargetPosition = QuatTargetRotation.UnrotateVector(ThrusterB.QuadCenterOffset).Add(TargetPosition);
            ThrusterC.TargetPosition = QuatTargetRotation.UnrotateVector(ThrusterC.QuadCenterOffset).Add(TargetPosition);
            ThrusterD.TargetPosition = QuatTargetRotation.UnrotateVector(ThrusterD.QuadCenterOffset).Add(TargetPosition);
            ThrusterE.TargetPosition = QuatTargetRotation.UnrotateVector(ThrusterE.QuadCenterOffset).Add(TargetPosition);
        }

        public void ApplyForce(Vector externalAcceleration)
        {
            this.externalAcceleration = new Vector(externalAcceleration);
        }
        
        public void EstimatePosition(double dT)
        {
            Vector TB = ThrusterB.ReturnThrustVector();
            Vector TC = ThrusterC.ReturnThrustVector();
            Vector TD = ThrusterD.ReturnThrustVector();
            Vector TE = ThrusterE.ReturnThrustVector();
            
            TB = QuatCurrentRotation.RotateVector(TB);
            TC = QuatCurrentRotation.RotateVector(TC);
            TD = QuatCurrentRotation.RotateVector(TD);
            TE = QuatCurrentRotation.RotateVector(TE);

            //current rotation z is the secondary hover angle
            //current rotation x is the primary hover angle
            Vector hoverAngles = RotationQuaternionToHoverAngles();

            //Console.WriteLine(CurrentRotation + " " + hoverAngles);

            //Adjusts rotation output for Z when rotating about Z - simulated gimbal lock
            Vector rotationTBX = Matrix.RotateVector(new Vector(0, 0, -CurrentEulerRotation.Z), new Vector(TB.X, 0, 0));
            Vector rotationTBZ = Matrix.RotateVector(new Vector(-CurrentEulerRotation.Z, 0, 0), new Vector(0, 0, TB.Z));
            Vector rotationTCX = Matrix.RotateVector(new Vector(0, 0, CurrentEulerRotation.Z), new Vector(TC.X, 0, 0));
            Vector rotationTCZ = Matrix.RotateVector(new Vector(CurrentEulerRotation.Z, 0, 0), new Vector(0, 0, TC.Z));
            Vector rotationTDX = Matrix.RotateVector(new Vector(0, 0, CurrentEulerRotation.Z), new Vector(TD.X, 0, 0));
            Vector rotationTDZ = Matrix.RotateVector(new Vector(CurrentEulerRotation.Z, 0, 0), new Vector(0, 0, TD.Z));
            Vector rotationTEX = Matrix.RotateVector(new Vector(0, 0, -CurrentEulerRotation.Z), new Vector(TE.X, 0, 0));
            Vector rotationTEZ = Matrix.RotateVector(new Vector(-CurrentEulerRotation.Z, 0, 0), new Vector(0, 0, TE.Z));

            //Rotate the Inner joint about the outer joint
            Vector outputTBZ = Matrix.RotateVector(new Vector(0, TB.Z, 0), new Vector(TB.X, 0, 0));
            Vector outputTCZ = Matrix.RotateVector(new Vector(0, TC.Z, 0), new Vector(TC.X, 0, 0));
            Vector outputTDZ = Matrix.RotateVector(new Vector(0, TD.Z, 0), new Vector(TD.X, 0, 0));
            Vector outputTEZ = Matrix.RotateVector(new Vector(0, TE.Z, 0), new Vector(TE.X, 0, 0));

            //Corrected force outputs, including gimbal lock
            TB.X = rotationTBX.X + outputTBZ.X;//Reduced when rotated
            TB.Z = rotationTBZ.Z + outputTBZ.Z;//Added to when rotated
            TC.X = rotationTCX.X + outputTCZ.X;//Reduced when rotated
            TC.Z = rotationTCZ.Z + outputTCZ.Z;//Added to when rotated
            TD.X = rotationTDX.X + outputTDZ.X;//Reduced when rotated
            TD.Z = rotationTDZ.Z + outputTDZ.Z;//Added to when rotated
            TE.X = rotationTEX.X + outputTEZ.X;//Reduced when rotated
            TE.Z = rotationTEZ.Z + outputTEZ.Z;//Added to when rotated

            //Summation of the force vectors
            Vector thrustSum = TB.Add(TC).Add(TD).Add(TE);
            
            //Rotation output negates when over 90 due to rotation matrix, this solution should be expanded to allow multiple increments
            if (CurrentEulerRotation.X >= 90 || CurrentEulerRotation.Z <= -90)
            {
                // % 180
                thrustSum.X *= -1;
            }

            //thrustSum.X *= Math.Sign(CurrentRotation.Z % (90.0 * 2));
            currentAcceleration = thrustSum.Add(externalAcceleration);

            //calculate velocity: finalVelocity(vf) = vi + at
            currentVelocity = currentVelocity.Add(currentAcceleration.Multiply(dT));//.Multiply(airResistance);

            //calculate position: displacement(s) = si + vt
            CurrentPosition = CurrentPosition.Add(currentVelocity.Multiply(dT));
        }

        public void EstimateRotation(double dT)
        {
            //Rotate Thrust Vector about current quaternion rotation
            Vector TB = ThrusterB.ReturnThrustVector();//Thrust relative to quad origin
            Vector TC = ThrusterC.ReturnThrustVector();
            Vector TD = ThrusterD.ReturnThrustVector();
            Vector TE = ThrusterE.ReturnThrustVector();
            
            TB = QuatCurrentRotation.RotateVector(TB);//Thrust relative to world origin
            TC = QuatCurrentRotation.RotateVector(TC);
            TD = QuatCurrentRotation.RotateVector(TD);
            TE = QuatCurrentRotation.RotateVector(TE);

            double torque = armLength * Math.Sin(MathE.DegreesToRadians(180 - armAngle)) * 5;

            currentAngularAcceleration = new Vector(0, 0, 0)
            {//rigid body forces in world orientation
                X = (TB.Y + TC.Y - TD.Y - TE.Y) * torque,
                Y = (TB.X + TC.X - TD.X - TE.X) * torque + (TB.Z - TC.Z - TD.Z + TE.Z) * torque,
                Z = (-TB.Y + TC.Y + TD.Y - TE.Y) * torque
            };

            //TB + TD - (TC + TE)
            Vector differentialThrustRotation = TB.Add(TD).Add(TC.Add(TE).Multiply(-1)).Multiply(0.15);//Relative to world origin

            currentAngularAcceleration = currentAngularAcceleration + differentialThrustRotation;//Eulerian quantity
            currentAngularAcceleration = MathE.DegreesToRadians(currentAngularAcceleration);
            currentAngularVelocity = currentAngularVelocity + currentAngularAcceleration * dT;
            
            Quaternion angularRotation = new Quaternion(0.5 * currentAngularVelocity * dT);

            QuatCurrentRotation += angularRotation * QuatCurrentRotation;
            QuatCurrentRotation = QuatCurrentRotation.UnitQuaternion();
            CurrentEulerRotation = EulerAngles.QuaternionToEuler(QuatCurrentRotation, EulerConstants.EulerOrderXYZS).Angles;
        }

        //Ratio from Y 1 -> -1, amount that rotation matters is highest at 0
        //
        //Quaternion rotation to hover
        private Vector RotationQuaternionToHoverAngles()
        {
            double primaryJoint = 0;
            double secondaryJoint = 0;
            
            DirectionAngle custAxisAngle = DirectionAngle.QuaternionToCustomAxisAngle(QuatCurrentRotation);
            Vector directionVector = new Vector(custAxisAngle.Direction.X, custAxisAngle.Direction.Y, custAxisAngle.Direction.Z);

            directionVector = Matrix.RotateVector(new Vector(0, custAxisAngle.Rotation, 0), directionVector);

            primaryJoint = directionVector.X * 90;//ratio upscaled to angle
            secondaryJoint = directionVector.Z * 90;//ratio upscaled to angle

            return new Vector(secondaryJoint, 0, -primaryJoint);
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
