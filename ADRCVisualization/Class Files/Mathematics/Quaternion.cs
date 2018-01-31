using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    public class Quaternion
    {
        public double W { get; set; }// Real
        public double X { get; set; }// Imaginary I
        public double Y { get; set; }// Imaginary J
        public double Z { get; set; }// Imaginary K

        /// <summary>
        /// Creates quaternion object.
        /// </summary>
        /// <param name="W">Real part of quaternion.</param>
        /// <param name="X">Imaginary I of quaternion.</param>
        /// <param name="Y">Imaginary J of quaternion.</param>
        /// <param name="Z">Imaginary K of quaternion.</param>
        public Quaternion(double W, double X, double Y, double Z)
        {
            this.W = W;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        /// <summary>
        /// Intializes quaternion with vector parameters for imaginary part.
        /// Creates quaternion bivector.
        /// </summary>
        /// <param name="vector">Imaginary values of quaternion.</param>
        public Quaternion(Vector vector)
        {
            W = 0;
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Rotates a vector coordinate in space given a quaternion value.
        /// </summary>
        /// <param name="coordinate">Coordinate vector that is rotated.</param>
        /// <returns>Returns new vector position coordinate.</returns>
        public Vector RotateVector(Vector coordinate)
        {
            Quaternion current = new Quaternion(W, X, Y, Z);
            Quaternion qv = new Quaternion(0, coordinate.X, coordinate.Y, coordinate.Z);
            Quaternion qr = current.Multiply(qv).Multiply(current.MultiplicativeInverse());

            Vector rotatedVector = new Vector(0, 0, 0)
            {
                X = qr.X,
                Y = qr.Y,
                Z = qr.Z
            };

            return rotatedVector;
        }

        public static Quaternion EulerToQuaternion(EulerAngles eulerAngles)
        {
            Quaternion q = new Quaternion(0, 0, 0, 0);
            double sx, sy, sz, cx, cy, cz, cc, cs, sc, ss;

            eulerAngles.Angles.X = Misc.DegreesToRadians(eulerAngles.Angles.X);
            eulerAngles.Angles.Y = Misc.DegreesToRadians(eulerAngles.Angles.Y);
            eulerAngles.Angles.Z = Misc.DegreesToRadians(eulerAngles.Angles.Z);

            if (eulerAngles.Order.FrameTaken == EulerOrder.AxisFrame.Rotating)
            {
                double t = eulerAngles.Angles.X;
                eulerAngles.Angles.X = eulerAngles.Angles.Z;
                eulerAngles.Angles.Z = t;
            }

            if (eulerAngles.Order.AxisPermutation == EulerOrder.Parity.Odd)
            {
                eulerAngles.Angles.Y = -eulerAngles.Angles.Y;
            }

            sx = Math.Sin(eulerAngles.Angles.X * 0.5);
            sy = Math.Sin(eulerAngles.Angles.Y * 0.5);
            sz = Math.Sin(eulerAngles.Angles.Z * 0.5);

            cx = Math.Cos(eulerAngles.Angles.X * 0.5);
            cy = Math.Cos(eulerAngles.Angles.Y * 0.5);
            cz = Math.Cos(eulerAngles.Angles.Z * 0.5);

            cc = cx * cz;
            cs = cx * sz;
            sc = sx * cz;
            ss = sx * sz;

            if (eulerAngles.Order.InitialAxisRepitition == EulerOrder.AxisRepitition.Yes)
            {
                q.X = cy * (cs + sc);
                q.Y = sy * (cc + ss);
                q.Z = sy * (cs - sc);
                q.W = cy * (cc - ss);
            }
            else
            {
                q.X = cy * sc - sy * cs;
                q.Y = cy * ss + sy * cc;
                q.Z = cy * cs - sy * sc;
                q.W = cy * cc + sy * ss;
            }

            q.Permutate(eulerAngles.Order.Permutation);

            if (eulerAngles.Order.AxisPermutation == EulerOrder.Parity.Odd)
            {
                q.Y = -q.Y;
            }

            return q;
        }

        /// <summary>
        /// Converts axis angle rotation representation to quaternion.
        /// </summary>
        /// <param name="axisAngle">Axis-Angle rotation representation.</param>
        /// <returns>Converted quaternion value.</returns>
        public static Quaternion AxisAngleToQuaternion(AxisAngle axisAngle)
        {
            double rotation = Misc.DegreesToRadians(axisAngle.Rotation);
            double scale = Math.Sin(rotation / 2);

            return new Quaternion(1, 0, 0, 0)
            {
                W = Math.Cos(rotation / 2),
                X = axisAngle.X * scale,
                Y = axisAngle.Y * scale,
                Z = axisAngle.Z * scale
            };
        }


        /// <summary>
        /// Adds two quaternions together.
        /// </summary>
        /// <param name="quaternionOne">Quaternion that is added to.</param>
        /// <param name="quaternionTwo">Quaternion that adds to.</param>
        /// <returns>Returns the combined quaternions.</returns>
        public Quaternion Add(Quaternion quaternion)
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return new Quaternion(0, 0, 0, 0)
            {
                W = current.W + quaternion.W,
                X = current.X + quaternion.X,
                Y = current.Y + quaternion.Y,
                Z = current.Z + quaternion.Z
            };
        }

        public static Quaternion operator +(Quaternion q1, Quaternion q2)
        {
            return q1.Add(q2);
        }

        /// <summary>
        /// Subtracts two quaternions.
        /// </summary>
        /// <param name="quaternionOne">Quaternion that is subtracted from.</param>
        /// <param name="quaternionTwo">Quaternion that subtracts from.</param>
        /// <returns>Returns the subtracted quaternion.</returns>
        public Quaternion Subtract(Quaternion quaternion)
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return new Quaternion(0, 0, 0, 0)
            {
                W = current.W - quaternion.W,
                X = current.X - quaternion.X,
                Y = current.Y - quaternion.Y,
                Z = current.Z - quaternion.Z
            };
        }
        
        public static Quaternion operator -(Quaternion q1, Quaternion q2)
        {
            return q1.Subtract(q2);
        }

        /// <summary>
        /// Multiplies a scalar by a quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is scaled.</param>
        /// <param name="scale">Scalar that scales the quaternion.</param>
        /// <returns>Returns the scaled quaternion.</returns>
        public Quaternion Multiply(double scale)
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return new Quaternion(0, 0, 0, 0)
            {
                W = current.W * scale,
                X = current.X * scale,
                Y = current.Y * scale,
                Z = current.Z * scale
            };
        }

        /// <summary>
        /// Multiplies two quaternions by each other.
        /// </summary>
        /// <param name="quaternionOne">Quaternion that is multiplied to.</param>
        /// <param name="quaternionTwo">Quathernion that multiplies to.</param>
        /// <returns>Returns the multiplied quaternions.</returns>
        public Quaternion Multiply(Quaternion quaternion)
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return new Quaternion(0, 0, 0, 0)
            {
                W = current.W * quaternion.W - current.X * quaternion.X - current.Y * quaternion.Y - current.Z * quaternion.Z,
                X = current.W * quaternion.X + current.X * quaternion.W + current.Y * quaternion.Z - current.Z * quaternion.Y,
                Y = current.W * quaternion.Y - current.X * quaternion.Z + current.Y * quaternion.W + current.Z * quaternion.X,
                Z = current.W * quaternion.Z + current.X * quaternion.Y - current.Y * quaternion.X + current.Z * quaternion.W
            };
        }

        public static Quaternion operator *(double s, Quaternion q1)
        {
            return q1.Multiply(s);
        }

        public static Quaternion operator *(Quaternion q1, double s)
        {
            return q1.Multiply(s);
        }

        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            return q1.Multiply(q2);
        }

        /// <summary>
        /// Divides a quaternion by a scalar.
        /// </summary>
        /// <param name="quaternion">Quaternion that is scaled.</param>
        /// <param name="scale">Scalar that scales quaternion.</param>
        /// <returns>Returns the scaled quaternion.</returns>
        public Quaternion Divide(double scale)
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return new Quaternion(0, 0, 0, 0)
            {
                W = current.W / scale,
                X = current.X / scale,
                Y = current.Y / scale,
                Z = current.Z / scale
            };
        }

        /// <summary>
        /// Divides two quaternions by each other.
        /// </summary>
        /// <param name="quaternionOne">Quaternion that is divided.</param>
        /// <param name="quaternionTwo">Quaternion that divides by.</param>
        /// <returns>Returns the divided quaternion.</returns>
        public Quaternion Divide(Quaternion quaternion)
        {
            double scale = quaternion.W * quaternion.W + quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z;
            Quaternion current = new Quaternion(W, X, Y, Z);

            return new Quaternion(0, 0, 0, 0)
            {
                W = (  current.W * quaternion.W + current.X * quaternion.X + current.Y * quaternion.Y + current.Z * quaternion.Z) / scale,
                X = (- current.W * quaternion.X + current.X * quaternion.W + current.Y * quaternion.Z - current.Z * quaternion.Y) / scale,
                Y = (- current.W * quaternion.Y - current.X * quaternion.Z + current.Y * quaternion.W + current.Z * quaternion.X) / scale,
                Z = (- current.W * quaternion.Z + current.X * quaternion.Y - current.Y * quaternion.X + current.Z * quaternion.W) / scale 
            };
        }

        public static Quaternion operator /(double s, Quaternion q1)
        {
            return q1.Divide(s);
        }

        public static Quaternion operator /(Quaternion q1, double s)
        {
            return q1.Divide(s);
        }

        public static Quaternion operator /(Quaternion q1, Quaternion q2)
        {
            return q1.Divide(q2);
        }

        /// <summary>
        /// Returns the absolute value of each individual quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is converted to a magnitude.</param>
        /// <returns>Returns the absolute value of the input quaternion.</returns>
        public Quaternion Absolute()
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return new Quaternion(0, 0, 0, 0)
            {
                W = Math.Abs(current.W),
                X = Math.Abs(current.X),
                Y = Math.Abs(current.Y),
                Z = Math.Abs(current.Z)
            };
        }

        /// <summary>
        /// Returns the inverse of the input quaternion. (Not the conjugate.)
        /// </summary>
        /// <param name="quaternion">Quaternion that is inverted.</param>
        /// <returns>Returns the inverse of the quaternion.</returns>
        public Quaternion AdditiveInverse()
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return new Quaternion(0, 0, 0, 0)
            {
                W = -current.W,
                X = -current.X,
                Y = -current.Y,
                Z = -current.Z
            };
        }

        /// <summary>
        /// Returns the conjugate of the input quaternion. (Not the inverse.)
        /// </summary>
        /// <param name="quaternion">Quaternion that is conjugated.</param>
        /// <returns>Returns the conjugate of the input quaternion.</returns>
        public Quaternion Conjugate()
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return new Quaternion(0, 0, 0, 0)
            {
                W =  current.W,
                X = -current.X,
                Y = -current.Y,
                Z = -current.Z
            };
        }

        /// <summary>
        /// Returns the scalar power of the input quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is exponentiated.</param>
        /// <param name="exponent">Scalar that is used as the exponent.</param>
        /// <returns>Returns the scalar power of the input quaternion.</returns>
        public Quaternion Power(double exponent)
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return new Quaternion(0, 0, 0, 0)
            {
                W = Math.Pow(current.W, exponent),
                X = Math.Pow(current.X, exponent),
                Y = Math.Pow(current.Y, exponent),
                Z = Math.Pow(current.Z, exponent)
            };
        }

        /// <summary>
        /// Returns the quaternion power of the input quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is exponentiated.</param>
        /// <param name="exponent">Quaternion that is used as the exponent.</param>
        /// <returns>Returns the quaternion power of the input quaternion.</returns>
        public Quaternion Power(Quaternion exponent)
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return new Quaternion(0, 0, 0, 0)
            {
                W = Math.Pow(current.W, exponent.W),
                X = Math.Pow(current.X, exponent.X),
                Y = Math.Pow(current.Y, exponent.Y),
                Z = Math.Pow(current.Z, exponent.Z)
            };
        }
        
        /// <summary>
        /// Restricts the quaternion to a single hemisphere of rotation, disables constant rotation around any real or imaginary axis.
        /// This will cause jumping in calculations, similar to that of a tangent function.
        /// </summary>
        /// <returns>Returns the unit quaternion of the current quaternion values.</returns>
        public Quaternion UnitQuaternion()
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            double n = Math.Sqrt(Math.Pow(current.W, 2) + Math.Pow(current.X, 2) + Math.Pow(current.Y, 2) + Math.Pow(current.Z, 2));

            current.W /= n;
            current.X /= n;
            current.Y /= n;
            current.Z /= n;

            return current;
        }

        /// <summary>
        /// Calculates the norm of the quaternion.
        /// </summary>
        /// <returns>Returns the norm of the quaternion.</returns>
        public double Normal()
        {
            Quaternion q = new Quaternion(W, X, Y, Z);

            return Math.Pow(q.W, 2.0) + Math.Pow(q.X, 2.0) + Math.Pow(q.Y, 2.0) + Math.Pow(q.Z, 2.0);
        }

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="q">Input quaternion</param>
        /// <returns>Returns the dot product of the current quaternion and the input quaternion.</returns>
        public double DotProduct(Quaternion q)
        {
            return (W * q.W) + (X * q.X) + (Y * q.Y) + (Z * q.Z);
        }

        /// <summary>
        /// Calculates the magnitude of the quaternion.
        /// </summary>
        /// <returns>Returns the magnitude of the quaternion.</returns>
        public double Magnitude()
        {
            return Math.Sqrt(Normal());
        }

        public double Determinant()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the multiplicative inverse of the quaternion.
        /// </summary>
        /// <returns>Returns the multiplicative inverse of the quaternion.</returns>
        public Quaternion MultiplicativeInverse()
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return current.Conjugate().Multiply(1.0 / current.Normal());
        }

        /// <summary>
        /// Slerp operation or Spherical Linear Interpolation, used to interpolate between quaternions.
        /// Constant speed motion along a unit-radius orthodrome(great circle) arc, given the ends and
        /// an interpolation parameter between 0 and 1.
        /// </summary>
        /// <param name="q2">Second quaternion for interpolation.</param>
        /// <param name="t">Ratio between input quaternions.</param>
        /// <returns>Interpolated quaternion.</returns>
        public Quaternion SphericalLinearInterpolation(Quaternion q2, double t)
        {
            Quaternion q1 = this;

            q1 = q1.UnitQuaternion();
            q2 = q2.UnitQuaternion();

            double dot = q1.DotProduct(q2);//Cosine between the two quaternions

            if (dot < 0.0)//Shortest path correction
            {
                q1  = q1.AdditiveInverse();
                dot = -dot;
            }

            if (dot > 0.9995)//Linearly interpolates if results are close
            {
                Quaternion result = (q1 + t * (q1 - q2)).UnitQuaternion();
                return result;
            }
            else
            {
                dot = Misc.Constrain(dot, -1, 1);

                double theta0 = Math.Acos(dot);
                double theta = theta0 * t;

                Quaternion q3 = (q2 - q1 * dot).UnitQuaternion();//UQ for orthonomal 

                return q1 * Math.Cos(theta) + q3 * Math.Sin(theta);
            }
        }

        /// <summary>
        /// Determines if any individual value of the quaternion is not a number.
        /// </summary>
        /// <param name="quaternion">Quaternion that isNaN checked.</param>
        /// <returns>Returns true if all any of the values are not a number.</returns>
        public bool IsNaN()
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return double.IsNaN(current.W) || double.IsNaN(current.X) || double.IsNaN(current.Y) || double.IsNaN(current.Z);
        }

        /// <summary>
        /// Determines if all of the quaternions values are finite.
        /// </summary>
        /// <param name="quaternion">Quaternion that is checked.</param>
        /// <returns>Returns true if all values are finite.</returns>
        public bool IsFinite()
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return !double.IsInfinity(current.W) && !double.IsInfinity(current.X) && !double.IsInfinity(current.Y) && !double.IsInfinity(current.Z);
        }

        /// <summary>
        /// Determines if all of the quaternions values are infinite.
        /// </summary>
        /// <param name="quaternion">Quaternion that is checked.</param>
        /// <returns>Returns true if all values are infinite.</returns>
        public bool IsInfinite()
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return double.IsInfinity(current.W) && double.IsInfinity(current.X) && double.IsInfinity(current.Y) && double.IsInfinity(current.Z);
        }

        /// <summary>
        /// Determines if all of the quaternions values are nonzero.
        /// </summary>
        /// <param name="quaternion">Quaternion that is checked.</param>
        /// <returns>Returns true if all values are nonzero.</returns>
        public bool IsNonZero()
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return current.W != 0 && current.X != 0 && current.Y != 0 && current.Z != 0;
        }

        /// <summary>
        /// Determines if the two input quaternions are equal.
        /// </summary>
        /// <param name="quaternionA">Quaternion that is checked.</param>
        /// <param name="quaternionB">Quaternion that is checked.</param>
        /// <returns>Returns true if both quaternions are equal.</returns>
        public bool IsEqual(Quaternion quaternion)
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            return !current.IsNaN() && !quaternion.IsNaN() &&
                    current.W == quaternion.W &&
                    current.X == quaternion.X &&
                    current.Y == quaternion.Y &&
                    current.Z == quaternion.Z;
        }

        public Quaternion Permutate(Vector permutation)
        {
            Quaternion current = new Quaternion(W, X, Y, Z);

            double[] perm = new double[3];

            perm[(int)permutation.X] = current.X;
            perm[(int)permutation.Y] = current.Y;
            perm[(int)permutation.Z] = current.Z;

            current.X = perm[0];
            current.Y = perm[1];
            current.Z = perm[2];

            return current;
        }

        public override string ToString()
        {
            string w = String.Format("{0:0.000}", W).PadLeft(7);
            string x = String.Format("{0:0.000}", X).PadLeft(7);
            string y = String.Format("{0:0.000}", Y).PadLeft(7);
            string z = String.Format("{0:0.000}", Z).PadLeft(7);

            return w + " " + x + " " + y + " " + z;
        }

    }
}
