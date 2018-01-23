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
        /// Creates a quaternion given Euler rotation.
        /// Converted from https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
        /// </summary>
        /// <param name="euler">Euler rotation coordinates.</param>
        /// <returns>Quaternion rotation coordinates.</returns>
        public static Quaternion FromEulerAngle(Vector euler)
        {
            //pitch, yaw, roll

            //Euler angles to quaternion rotation
            double cy, sy, cr, sr, cp, sp;

            cy = Math.Cos(Misc.DegreesToRadians(euler.Y) * 0.5);
            sy = Math.Cos(Misc.DegreesToRadians(euler.Y) * 0.5);
            cr = Math.Cos(Misc.DegreesToRadians(euler.Z) * 0.5);
            sr = Math.Cos(Misc.DegreesToRadians(euler.Z) * 0.5);
            cp = Math.Cos(Misc.DegreesToRadians(euler.X) * 0.5);
            sp = Math.Cos(Misc.DegreesToRadians(euler.X) * 0.5);

            Quaternion quaternion = new Quaternion(0, 0, 0, 0)
            {
                W = cy * cr * cp + sy * sr * sp,
                X = cy * sr * cp - sy * cr * sp,
                Y = cy * cr * sp + sy * sr * cp,
                Z = sy * cr * cp - cy * sr * sp
            };

            return quaternion;
        }

        /// <summary>
        /// Creates a vector euler rotation given a quaterion.
        /// Converted from https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
        /// </summary>
        /// <param name="quaternion">Quaternion rotation coordinates.</param>
        /// <returns>Euler rotation coordinates.</returns>
        public static Vector ToEulerAngle(Quaternion quaternion)
        {
            //X = pitch, Y = yaw, Z = roll
            double pitch, roll, yaw;

            double sinr =       2.0 * (quaternion.W * quaternion.X + quaternion.Y * quaternion.Z);
            double cosr = 1.0 - 2.0 * (quaternion.X * quaternion.X + quaternion.Y * quaternion.Y);
            double sinp =       2.0 * (quaternion.W * quaternion.X - quaternion.Y * quaternion.Z);
            double siny =       2.0 * (quaternion.W * quaternion.Z + quaternion.X * quaternion.Y);
            double cosy = 1.0 - 2.0 * (quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z);
            
            roll = Math.Atan2(sinr, cosr);
            yaw = Math.Atan2(siny, cosy);
            
            if (Math.Abs(sinp) >= 1)
                pitch = Math.PI / 2 * Math.Sign(sinp); // use 90 degrees if out of range
            else
                pitch = Math.Asin(sinp);

            roll = Misc.RadiansToDegrees(roll);
            yaw = Misc.RadiansToDegrees(yaw);
            pitch = Misc.RadiansToDegrees(pitch);

            return new Vector(pitch, yaw, roll);
        }

        /// <summary>
        /// Rotates a vector coordinate in space given a quaternion value.
        /// </summary>
        /// <param name="quaternion">Quaternion used to rotate coordinates.</param>
        /// <param name="coordinate">Vector that is rotated.</param>
        /// <returns>Returns new vector position coordinates.</returns>
        public static Vector RotateVector(Quaternion quaternion, Vector coordinate)
        {
            //Othogonal rotation is non-commutative
            Vector rotatedCoordinate = new Vector(0, 0, 0)
            {
                X = Math.Pow(quaternion.W, 2) * coordinate.X + 2 * quaternion.Y * quaternion.W * coordinate.Z - 2 * quaternion.Z * quaternion.W * coordinate.Y +
                    Math.Pow(quaternion.X, 2) * coordinate.X + 2 * quaternion.Y * quaternion.X * coordinate.Y + 2 * quaternion.Z * quaternion.X * coordinate.Z -
                    Math.Pow(quaternion.Z, 2) * coordinate.X - Math.Pow(quaternion.Y, 2) * coordinate.X,

                Y = 2 * quaternion.X * quaternion.Y * coordinate.X + Math.Pow(quaternion.Y, 2) * coordinate.Y + 2 * quaternion.Z * quaternion.Y * coordinate.Z +
                    2 * quaternion.W * quaternion.Z * coordinate.X - Math.Pow(quaternion.Z, 2) * coordinate.Y + Math.Pow(quaternion.W, 2) * coordinate.Y       -
                    2 * quaternion.X * quaternion.W * coordinate.Z - Math.Pow(quaternion.X, 2) * coordinate.Y,

                Z = 2 * quaternion.X * quaternion.Z * coordinate.X + 2 * quaternion.Y * quaternion.Z * coordinate.Y + Math.Pow(quaternion.Z, 2) * coordinate.Z       -
                    2 * quaternion.W * quaternion.Y * coordinate.X - Math.Pow(quaternion.Y, 2) * coordinate.Z       + 2 * quaternion.W * quaternion.X * coordinate.Y -
                    Math.Pow(quaternion.X, 2) * coordinate.Z       + Math.Pow(quaternion.W, 2) * coordinate.Z,
            };

            return rotatedCoordinate;
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
        public Quaternion Inverse()
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
        /// Normalizes the input quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is normalized.</param>
        /// <returns>Returns the normalized input quaternion.</returns>
        public Quaternion Normalize()
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
