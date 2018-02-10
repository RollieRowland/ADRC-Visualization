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

            axisAngle.Rotation = MathE.RadiansToDegrees(2.0 * Math.Acos(quaternion.W));

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
        /// This form of axis-angle is a custom type of rotation, the orientation is defined as the up vector of the object pointing at a specific 
        /// point in cartesian space; defining an axis of rotation, in which the object rotates about.
        /// </summary>
        /// <param name="quaternion">Quaternion rotation of the current object.</param>
        /// <returns></returns>
        public static AxisAngle QuaternionToCustomAxisAngle(Quaternion quaternion)
        {
            Vector up = new Vector(0, 1, 0);//up vector
            Vector right = new Vector(1, 0, 0);
            Vector rotatedUp = quaternion.RotateVector(up);//new direction vector
            Vector rotatedRight = quaternion.RotateVector(right);
            Quaternion rotationChange = Quaternion.QuaternionFromTwoVectors(up, rotatedUp);
            
            //rotate forward vector by direction vector rotation
            Vector rightXZCompensated = rotationChange.UnrotateVector(rotatedRight);//should only be two points on circle, compare against right
            
            //define angles that define the forward vector, and the rotated then compensated forward vector
            double rightAngle = MathE.RadiansToDegrees(Math.Atan2(right.Z, right.X));//forward as zero
            double rightRotatedAngle = MathE.RadiansToDegrees(Math.Atan2(rightXZCompensated.Z, rightXZCompensated.X));//forward as zero
            
            //angle about the axis defined by the direction of the object
            double angle = rightAngle - rightRotatedAngle;
            
            //returns the angle rotated about the rotated up vector as an axis
            return new AxisAngle(angle, rotatedUp);
        }

        /// <summary>
        /// Rotates vector by axis-angle
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
            string r = String.Format("{0:0.000}", Rotation).PadLeft(8);
            string x = String.Format("{0:0.000}", X       ).PadLeft(8);
            string y = String.Format("{0:0.000}", Y       ).PadLeft(8);
            string z = String.Format("{0:0.000}", Z       ).PadLeft(8);

            return r + ": [" + x + " " + y + " " + z + "]";
        }
    }
}
