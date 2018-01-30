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

        public static AxisAngle QuaternionToAxisAngle(Quaternion quaternion)
        {
            AxisAngle axisAngle = new AxisAngle(0, 0, 1, 0);
            quaternion = quaternion.W > 1 ? quaternion.UnitQuaternion() : quaternion;

            axisAngle.Rotation = Misc.RadiansToDegrees(2 * Math.Acos(quaternion.W));

            double quaternionCheck = Math.Sqrt(1 - Math.Pow(quaternion.W, 2));//Prevents rotation jumps, and division by zero

            if (quaternionCheck >= 0.00001)//Prevents division by zero
            {
                //Normalizes axis
                axisAngle.X = quaternion.X / quaternionCheck;
                axisAngle.Y = quaternion.Y / quaternionCheck;
                axisAngle.Z = quaternion.Z / quaternionCheck;
            }
            else
            {
                //If X is close to zero the axis doesn't matter
                axisAngle.X = 0;
                axisAngle.Y = 1;
                axisAngle.Z = 0;
            }

            return axisAngle;
        }

        public override string ToString()
        {
            string r = String.Format("{0:0.000}", Rotation).PadLeft(7);
            string x = String.Format("{0:0.000}", X).PadLeft(7);
            string y = String.Format("{0:0.000}", Y).PadLeft(7);
            string z = String.Format("{0:0.000}", Z).PadLeft(7);

            return r + ": [" + x + " " + y + " " + z + "]";
        }
    }
}
