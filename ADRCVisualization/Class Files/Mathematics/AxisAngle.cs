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
            Vector right = new Vector(1, 0, 0);
            Vector rotatedUp = quaternion.RotateVector(up);//new direction vector
            Vector rotatedRight = quaternion.RotateVector(right);

            Console.WriteLine("QCAA Up/Right Vector: " + rotatedUp + " " + rotatedRight);

            //Convert rotated up vector to spherical coordinate system
            //Calculate individual 2dof rotations, ensure that order is correct
            double radius  = Math.Sqrt(Math.Pow(rotatedUp.X, 2) + Math.Pow(rotatedUp.Y, 2) + Math.Pow(rotatedUp.Z, 2));
            double XZTheta = Misc.RadiansToDegrees(Math.Atan2(rotatedUp.Z, rotatedUp.X));//azimuthal rotation, Z/X, adjust as forward z being zero
            double YPhi    = Misc.RadiansToDegrees(Math.Acos(rotatedUp.Y / radius));//polar rotation, Y/R
            
            Console.WriteLine("QCAA Spherical Coordinates: R" + radius + " XZ" + XZTheta + " Y" + YPhi);

            //define spherical coordinate as Euler angle rotation with order
            EulerAngles rotatedUpVectorEulerRotation = new EulerAngles(new Vector(0, XZTheta, YPhi), EulerConstants.EulerOrderXYZR);

            //computed Euler angles to quaternion
            Quaternion rotatedUpVectorQuatRotation = Quaternion.EulerToQuaternion(rotatedUpVectorEulerRotation);

            //rotate forward vector by direction vector rotation
            Vector rightXZCompensated = rotatedUpVectorQuatRotation.RotateVector(rotatedRight);//should only be two points on circle, compare against forward

            Console.WriteLine("QCAA Right Vectors: " + right + " " + rightXZCompensated);

            //define angles that define the forward vector, and the rotated then compensated forward vector
            double forwardAngle = Misc.RadiansToDegrees(Math.Atan2(right.Z, right.X));//forward as zero
            double forwardRotatedAngle = Misc.RadiansToDegrees(Math.Atan2(rightXZCompensated.Z, rightXZCompensated.X));//forward as zero

            Console.WriteLine("QCAA Right Angles: " + forwardAngle + " " + forwardRotatedAngle);

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
