using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    public class AxisAngle
    {
        public double Rotation { get; set; }//
        public double X { get; set; }//
        public double Y { get; set; }//
        public double Z { get; set; }//

        public AxisAngle(double Rotation, double X, double Y, double Z)
        {
            this.Rotation = Rotation;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public AxisAngle(double Rotation, Vector vector)
        {
            this.Rotation = Rotation;

            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public static AxisAngle QuaternionToStandardAxisAngle(Quaternion quaternion)
        {
            AxisAngle axisAngle = new AxisAngle(0, 0, 1, 0);
            quaternion = (Math.Abs(quaternion.W) > 1.0) ? quaternion.UnitQuaternion() : quaternion;

            axisAngle.Rotation = Misc.RadiansToDegrees(2.0 * Math.Acos(quaternion.W));

            double quaternionCheck = Math.Sqrt(1.0 - Math.Pow(quaternion.W, 2.0));//Prevents rotation jumps, and division by zero

            if (quaternionCheck >= 0.001)//Prevents division by zero
            {
                //Normalizes axis
                axisAngle.X = quaternion.X / quaternionCheck;
                axisAngle.Y = quaternion.Y / quaternionCheck;
                axisAngle.Z = quaternion.Z / quaternionCheck;
            }
            else
            {
                //If X is close to zero the axis doesn't matter
                axisAngle.X = 0.0;
                axisAngle.Y = 1.0;
                axisAngle.Z = 0.0;
            }

            return axisAngle;
        }

        /// <summary>
        /// This form of axis angle is a custom type of rotation, the orientation is defined as the up vector of the object pointing at a specific point.
        /// Defining an axis of rotation, in which the object rotates about.
        /// </summary>
        /// <param name="quaternion">Quaternion rotation of the current object.</param>
        /// <returns></returns>
        public static AxisAngle QuaternionToCustomAxisAngle(Quaternion quaternion)
        {
            Vector up = new Vector(0, 1, 0);//up vector
            Vector forward = new Vector(0, 0, 1);
            Vector rotatedUp = quaternion.RotateVector(up);//new direction vector
            Vector rotatedForward = quaternion.RotateVector(forward);

            //rotated up vector to Euler angles
            //Calculate individual 2dof rotations, ensure that order is correct
            double XRotation = 0;
            double ZRotation = 0;

            //CALCULATE X AND Z ROTATION


            //define X and Z rotation as Euler angle rotation with order
            EulerAngles rotatedUpVectorEulerRotation = new EulerAngles(new Vector(XRotation, 0, ZRotation), EulerConstants.EulerOrderXYZS);

            //computed Euler angles to quaternion
            Quaternion rotatedUpVectorQuatRotation = Quaternion.EulerToQuaternion(rotatedUpVectorEulerRotation);

            //rotate forward vector by direction vector rotation
            Vector forwardXZCompensated = rotatedUpVectorQuatRotation.RotateVector(rotatedForward);//should only be two points on circle, compare against forward

            //define angles that define the forward vector, and the rotated then compensated forward vector
            double forwardAngle = Misc.RadiansToDegrees(Math.Atan2(forward.Z, forward.X));
            double forwardRotatedAngle = Misc.RadiansToDegrees(Math.Atan2(forwardXZCompensated.Z, forwardXZCompensated.X));

            //angle about the axis defined by the direction of the object
            double angle = forwardAngle - forwardRotatedAngle;

            //returns the angle rotated about the rotated up vector as an axis
            return new AxisAngle(angle, rotatedUp);
        }

        /// <summary>
        /// Technical Concepts: Orientation, Rotation, Velocity and Acceleration, and the SRM
        /// Page 35
        /// </summary>
        /// <returns></returns>
        public Vector RotateVector(Vector v)
        {
            //r′ = cos(θ)r + ((1− cos (θ))(r • n)n + sin(θ) (n × r)
            Quaternion q = Quaternion.AxisAngleToQuaternion(this);

            return q.RotateVector(v);
        }

        private double RotateAxis(double angle, double axis)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            string r = String.Format("{0:0.000}", Rotation).PadLeft(7);
            string x = String.Format("{0:0.000}", X       ).PadLeft(7);
            string y = String.Format("{0:0.000}", Y       ).PadLeft(7);
            string z = String.Format("{0:0.000}", Z       ).PadLeft(7);

            return r + ": [" + x + " " + y + " " + z + "]";
        }
    }
}
