using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    public class DirectionAngle
    {
        public double Rotation { get; set; }//
        public Vector Direction { get; set; }//

        public DirectionAngle(double Rotation, double X, double Y, double Z)
        {
            this.Rotation = Rotation;

            Direction = new Vector(X, Y, Z);
        }

        public DirectionAngle(double Rotation, Vector direction)
        {
            this.Rotation = Rotation;

            Direction = new Vector(direction);
        }

        /// <summary>
        /// This form of axis-angle is a custom type of rotation, the orientation is defined as the up vector of the object pointing at a specific 
        /// point in cartesian space; defining an axis of rotation, in which the object rotates about.
        /// </summary>
        /// <param name="quaternion">Quaternion rotation of the current object.</param>
        /// <returns></returns>
        public static DirectionAngle QuaternionToDirectionAngle(Quaternion quaternion)
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
            return new DirectionAngle(angle, rotatedUp);
        }
        
        public override string ToString()
        {
            string r = String.Format("{0:0.000}", Rotation   ).PadLeft(8);
            string x = String.Format("{0:0.000}", Direction.X).PadLeft(8);
            string y = String.Format("{0:0.000}", Direction.Y).PadLeft(8);
            string z = String.Format("{0:0.000}", Direction.Z).PadLeft(8);

            return r + ": [" + x + " " + y + " " + z + "]";
        }
    }
}
