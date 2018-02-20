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
        public double samplingPeriod { get; private set; }

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
            /*
            PositionFeedbackController = new VectorFeedbackController()
            {
                X = new PID(1, 0, 0.2, 1000),
                Y = new PID(1, 0, 0.2, 1000),
                Z = new PID(1, 0, 0.2, 1000)
            };
            */
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
            //Omega = 2 * (qt - qc) * qc^-1 / dt -> only bivector quantity, real value is disregarded
            Vector change = (2 * (QuatTargetRotation - QuatCurrentRotation) * QuatCurrentRotation.Conjugate() / samplingPeriod).GetBiVector();
            //change = (CurrentEulerRotation - TargetEulerRotation) * -1;

            //Console.WriteLine(change + " " + QuatTargetRotation + " " + QuatCurrentRotation);
            
            Vector rotationOutput = RotationFeedbackController.Calculate(new Vector(0, 0, 0), change, samplingPeriod);
            Vector positionOutput = PositionFeedbackController.Calculate(new Vector(0, 0, 0), CurrentPosition.Subtract(TargetPosition), samplingPeriod);

            rotationOutput = rotationOutput.Multiply(new Vector(1, 1, 1));
            positionOutput = new Vector(0, 0, 0);

            //Console.WriteLine(rotationOutput + " " + CurrentEulerRotation);

            Vector thrusterOutputB = new Vector(-rotationOutput.Y, -rotationOutput.X + rotationOutput.Z, -rotationOutput.Y);//Thruster output relative to environment origin
            Vector thrusterOutputC = new Vector(-rotationOutput.Y, -rotationOutput.X - rotationOutput.Z,  rotationOutput.Y);
            Vector thrusterOutputD = new Vector( rotationOutput.Y,  rotationOutput.X - rotationOutput.Z,  rotationOutput.Y);
            Vector thrusterOutputE = new Vector( rotationOutput.Y,  rotationOutput.X + rotationOutput.Z, -rotationOutput.Y);

            //CalculateGimbalLockedMotion(ref positionOutput, ref thrusterOutputB, ref thrusterOutputC, ref thrusterOutputD, ref thrusterOutputE);
            //CalculateGimbalLockedRotation(ref rotationOutput, ref thrusterOutputB, ref thrusterOutputC, ref thrusterOutputD, ref thrusterOutputE);

            Vector hoverAngles = RotationQuaternionToHoverAngles();

            //Add in after position calculation is fixed

            //positionOutput = RotationMatrix.RotateVector(new Vector(0, -CurrentEulerRotation.Y, 0), positionOutput);//adjust thruster output to quad frame, only Y dimension

            //Due to XYZ permutation order of Euler angle
            positionOutput.X = positionOutput.X + hoverAngles.Z;//Adjust main joint to rotation
            positionOutput = RotationMatrix.RotateVector(new Vector(0, CurrentEulerRotation.Y, 0), positionOutput);//adjust thruster output to quad frame, only Y dimension
            positionOutput.Z = positionOutput.Z - hoverAngles.X;//Adjust secondary joint to rotation

            //positionOutput.X = 0;
            //positionOutput.Z = 0;

            ThrusterB.Calculate(thrusterOutputB.Add(positionOutput));
            ThrusterC.Calculate(thrusterOutputC.Add(positionOutput));
            ThrusterD.Calculate(thrusterOutputD.Add(positionOutput));
            ThrusterE.Calculate(thrusterOutputE.Add(positionOutput));
        }

        private TriangleWaveFader gimbalLockFader = new TriangleWaveFader(8, 90);

        private void CalculateGimbalLockedMotion(ref Vector positionControl, ref Vector thrusterOutputB, ref Vector thrusterOutputC, ref Vector thrusterOutputD, ref Vector thrusterOutputE)
        {
            Vector hoverAngles = RotationQuaternionToHoverAngles();
            
            double fadeIn = gimbalLockFader.CalculateRatio(hoverAngles.Z);//0 -> 1, New position control faders, approaches 1 when z rot is -90/90
            double fadeOut = 1 - fadeIn;//1 -> 0, Rotation magnitude faders, approaches 0 when Z rot is -90/90 degrees

            double rotation = 40 * fadeIn;
            
            double magnitude = Math.Sqrt(Math.Pow(positionControl.X, 2) + Math.Pow(positionControl.Z, 2));//Give hypotenuse for origin rotation, magnitude
            double angle = MathE.RadiansToDegrees(Math.Sign(hoverAngles.Z) * Math.Atan2(magnitude, 0) - Math.Atan2(positionControl.Z, positionControl.X));//Determine angle of output, -180 -> 180

            //Rotation matrix on position control copy
            Vector RotatedControl = RotationMatrix.RotateVector(new Vector(0, CurrentEulerRotation.Y, 0), new Vector(positionControl.X, 0, positionControl.Z));

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

        public void CalculateCurrent()
        {
            //EstimatePosition(samplingPeriod);
            //EstimatePositionFix(samplingPeriod);
            EstimateRotation(samplingPeriod);

            //calculate rotation matrix
            ThrusterB.CurrentPosition = QuatCurrentRotation.RotateVector(ThrusterB.QuadCenterOffset).Add(CurrentPosition);
            ThrusterC.CurrentPosition = QuatCurrentRotation.RotateVector(ThrusterC.QuadCenterOffset).Add(CurrentPosition);
            ThrusterD.CurrentPosition = QuatCurrentRotation.RotateVector(ThrusterD.QuadCenterOffset).Add(CurrentPosition);
            ThrusterE.CurrentPosition = QuatCurrentRotation.RotateVector(ThrusterE.QuadCenterOffset).Add(CurrentPosition);
        }

        public void SetTarget(Vector position, DirectionAngle rotation)
        {
            TargetPosition = new Vector(position);
            DirectionAngle targetDA = new DirectionAngle(rotation);

            QuatTargetRotation = Quaternion.DirectionAngleToQuaternion(targetDA);

            //calculate rotation matrix
            ThrusterB.TargetPosition = QuatTargetRotation.RotateVector(ThrusterB.QuadCenterOffset).Add(TargetPosition);
            ThrusterC.TargetPosition = QuatTargetRotation.RotateVector(ThrusterC.QuadCenterOffset).Add(TargetPosition);
            ThrusterD.TargetPosition = QuatTargetRotation.RotateVector(ThrusterD.QuadCenterOffset).Add(TargetPosition);
            ThrusterE.TargetPosition = QuatTargetRotation.RotateVector(ThrusterE.QuadCenterOffset).Add(TargetPosition);
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
            
            //current rotation x is the primary hover angle
            //current rotation z is the secondary hover angle
            Vector hoverAngles = RotationQuaternionToHoverAngles();
            
            //Adjusts rotation output for Z when rotating about Z - simulated gimbal lock
            Vector rotationTBX = RotationMatrix.RotateVector(new Vector(0, 0, -hoverAngles.Z), new Vector(TB.X, 0, 0));
            Vector rotationTBZ = RotationMatrix.RotateVector(new Vector(-hoverAngles.Z, 0, 0), new Vector(0, 0, TB.Z));
            Vector rotationTCX = RotationMatrix.RotateVector(new Vector(0, 0, hoverAngles.Z), new Vector(TC.X, 0, 0));
            Vector rotationTCZ = RotationMatrix.RotateVector(new Vector(hoverAngles.Z, 0, 0), new Vector(0, 0, TC.Z));
            Vector rotationTDX = RotationMatrix.RotateVector(new Vector(0, 0, hoverAngles.Z), new Vector(TD.X, 0, 0));
            Vector rotationTDZ = RotationMatrix.RotateVector(new Vector(hoverAngles.Z, 0, 0), new Vector(0, 0, TD.Z));
            Vector rotationTEX = RotationMatrix.RotateVector(new Vector(0, 0, -hoverAngles.Z), new Vector(TE.X, 0, 0));
            Vector rotationTEZ = RotationMatrix.RotateVector(new Vector(-hoverAngles.Z, 0, 0), new Vector(0, 0, TE.Z));

            //Rotate the Inner joint about the outer joint
            Vector outputTBZ = RotationMatrix.RotateVector(new Vector(0, TB.Z, 0), new Vector(TB.X, 0, 0));
            Vector outputTCZ = RotationMatrix.RotateVector(new Vector(0, TC.Z, 0), new Vector(TC.X, 0, 0));
            Vector outputTDZ = RotationMatrix.RotateVector(new Vector(0, TD.Z, 0), new Vector(TD.X, 0, 0));
            Vector outputTEZ = RotationMatrix.RotateVector(new Vector(0, TE.Z, 0), new Vector(TE.X, 0, 0));

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
            /*
            //thrustSum.X *= Math.Sign(CurrentRotation.Z % (90.0 * 2));
            currentAcceleration = thrustSum.Add(externalAcceleration);

            //calculate velocity: finalVelocity(vf) = vi + at
            currentVelocity = currentVelocity.Add(currentAcceleration.Multiply(dT));//.Multiply(airResistance);

            //calculate position: displacement(s) = si + vt
            CurrentPosition = CurrentPosition.Add(currentVelocity.Multiply(dT));
            */
        }

        public void EstimatePositionFix(double dT)
        {
            Vector TB = ThrusterB.ReturnThrusterOutput();
            Vector TC = ThrusterC.ReturnThrusterOutput();
            Vector TD = ThrusterD.ReturnThrusterOutput();
            Vector TE = ThrusterE.ReturnThrusterOutput();
            
            Vector TBThrust = new Vector(0, TB.Y, 0);
            Vector TCThrust = new Vector(0, TC.Y, 0);
            Vector TDThrust = new Vector(0, TD.Y, 0);
            Vector TEThrust = new Vector(0, TE.Y, 0);
            
            Quaternion TBR = Quaternion.EulerToQuaternion(new EulerAngles(new Vector(TB.X, 0, -TB.Z), EulerConstants.EulerOrderZYXS));
            Quaternion TCR = Quaternion.EulerToQuaternion(new EulerAngles(new Vector(TC.X, 0, -TC.Z), EulerConstants.EulerOrderZYXS));
            Quaternion TDR = Quaternion.EulerToQuaternion(new EulerAngles(new Vector(TD.X, 0, -TD.Z), EulerConstants.EulerOrderZYXS));
            Quaternion TER = Quaternion.EulerToQuaternion(new EulerAngles(new Vector(TE.X, 0, -TE.Z), EulerConstants.EulerOrderZYXS));

            TBThrust = TBR.RotateVector(TBThrust);
            TCThrust = TCR.RotateVector(TCThrust);
            TDThrust = TDR.RotateVector(TDThrust);
            TEThrust = TER.RotateVector(TEThrust);

            //when z is greater than 90 the primary joint has negative output
            
            //Summation of the force vectors
            Vector thrustSum = TBThrust.Add(TBThrust).Add(TBThrust).Add(TBThrust);

            thrustSum = QuatCurrentRotation.RotateVector(thrustSum);//relative to world orientation
            
            //thrustSum.X *= Math.Sign(CurrentRotation.Z % (90.0 * 2));
            currentAcceleration = thrustSum.Add(externalAcceleration);

            //calculate velocity: finalVelocity(vf) = vi + at
            currentVelocity = currentVelocity.Add(currentAcceleration.Multiply(dT));//.Multiply(0.99);

            //calculate position: displacement(s) = si + vt
            CurrentPosition = CurrentPosition.Add(currentVelocity.Multiply(dT));
        }

        public void EstimateRotation(double dT)
        {
            Vector TB = ThrusterB.ReturnThrustVector();//Thrust relative to quad origin
            Vector TC = ThrusterC.ReturnThrustVector();
            Vector TD = ThrusterD.ReturnThrustVector();
            Vector TE = ThrusterE.ReturnThrustVector();

            //Rotate Thrust Vector about current quaternion rotation
            TB = QuatCurrentRotation.RotateVector(TB);//Thrust relative to world origin
            TC = QuatCurrentRotation.RotateVector(TC);
            TD = QuatCurrentRotation.RotateVector(TD);
            TE = QuatCurrentRotation.RotateVector(TE);
            
            double torque = armLength * Math.Sin(MathE.DegreesToRadians(180 - armAngle)) * 5;

            //calculate current inertia tensor
            currentAngularAcceleration = new Vector(0, 0, 0)
            {//rigid body forces in world orientation
                X = (TB.Y + TC.Y - TD.Y - TE.Y) * torque,
                Y = (TB.X + TC.X - TD.X - TE.X) * torque + (TB.Z - TC.Z - TD.Z + TE.Z) * torque,
                Z = (-TB.Y + TC.Y + TD.Y - TE.Y) * torque
            };

            //TB + TD - (TC + TE)
            Vector differentialThrustRotation = TB.Add(TD).Add(TC.Add(TE).Multiply(-1)).Multiply(0.15);//Relative to world origin

            //currentAngularAcceleration += differentialThrustRotation;//Eulerian quantity
            currentAngularAcceleration = MathE.DegreesToRadians(currentAngularAcceleration);
            currentAngularVelocity = currentAngularVelocity + currentAngularAcceleration * dT;
            
            Quaternion angularRotation = new Quaternion(0.5 * currentAngularVelocity * dT);

            QuatCurrentRotation += angularRotation * QuatCurrentRotation;
            QuatCurrentRotation = QuatCurrentRotation.UnitQuaternion();
            //QuatCurrentRotation = Quaternion.EulerToQuaternion(new EulerAngles(new Vector(0, 90, 0), EulerConstants.EulerOrderXYZS));

            CurrentEulerRotation = EulerAngles.QuaternionToEuler(QuatCurrentRotation, EulerConstants.EulerOrderXYZS).Angles * new Vector(1, -1, 1);
        }

        private TriangleWaveFader hoverAngles = new TriangleWaveFader(2, 90);

        //Ratio from Y 1 -> -1, amount of rotation importance is highest at 0
        //
        //Quaternion rotation to hover
        private Vector RotationQuaternionToHoverAngles()
        {
            double primaryJoint = 0;
            double secondaryJoint = 0;
            DirectionAngle directionAngle = DirectionAngle.QuaternionToDirectionAngle(QuatCurrentRotation);

            directionAngle.Direction = RotationMatrix.RotateVector(new Vector(0, -90, 0), directionAngle.Direction);

            Vector directionVector = new Vector(directionAngle.Direction);

            directionVector = RotationMatrix.RotateVector(new Vector(0, directionAngle.Rotation, 0), directionVector);

            //arctan2 of z, x will give the direction that it is facing starting at x1, z0. Add 90 so it starts at x0, z1

            double radius = 1 - Math.Abs(directionVector.Y);
            //position on circle is dependent by sign of Y coordinate

            //when Y coordinate vector dips below 0, the secondary and primary joints no longer produce the correct angle results

            if (radius > 0)
            {
                //These are cartesian coordinates, convert them to the angle from 1, 0 to the point it is at
                secondaryJoint = MathE.RadiansToDegrees(Math.Asin(directionVector.Z));// * Math.Sign(directionVector.Y);
                primaryJoint = MathE.RadiansToDegrees(Math.Asin(directionVector.X));// + directionAngle.Rotation;

                //secondaryJoint = Math.Sign(directionVector.Y) == -1 ? 180 - secondaryJoint : secondaryJoint;
            }

            Console.WriteLine(primaryJoint + " " + secondaryJoint + " " + hoverAngles.CalculateRatio(directionAngle.Rotation));

            return new Vector(primaryJoint, 0, secondaryJoint);
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
