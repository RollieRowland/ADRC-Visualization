using System;

namespace ADRCVisualization.Class_Files.Mathematics
{
    public class AxisAngle
    {
        public double Rotation { get; set; }//
        public Vector Axis { get; set; }//

        public AxisAngle(double Rotation, double X, double Y, double Z)
        {
            this.Rotation = Rotation;
            Axis = new Vector(X, Y, Z);
            
        }

        public AxisAngle(double Rotation, Vector vector)
        {
            this.Rotation = Rotation;

            Axis = new Vector(vector);
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
                axisAngle.Axis = new Vector(0, 0, 0)
                {
                    X = quaternion.X / quaternionCheck,
                    Y = quaternion.Y / quaternionCheck,
                    Z = quaternion.Z / quaternionCheck,
                };
            }
            else
            {
                //If X is close to zero the axis doesn't matter
                axisAngle.Axis = new Vector(0, 0, 0)
                {
                    X = 0.0,
                    Y = 1.0,
                    Z = 0.0
                };
            }

            return axisAngle;
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

        public override string ToString()
        {
            string r = String.Format("{0:0.000}", Rotation).PadLeft(8);
            string x = String.Format("{0:0.000}", Axis.X  ).PadLeft(8);
            string y = String.Format("{0:0.000}", Axis.Y  ).PadLeft(8);
            string z = String.Format("{0:0.000}", Axis.Z  ).PadLeft(8);

            return r + ": [" + x + " " + y + " " + z + "]";
        }
    }
}
