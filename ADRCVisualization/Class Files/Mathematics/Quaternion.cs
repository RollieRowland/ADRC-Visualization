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
        /// </summary>
        /// <param name="euler">Euler rotation coordinates.</param>
        /// <returns>Quaternion rotation coordinates.</returns>
        public static Quaternion FromEulerAngle(Vector euler)
        {
            //Euler angles to quaternion rotation
            Vector cosine = new Vector(0, 0, 0)
            {
                X = Math.Cos(Misc.DegreesToRadians(euler.Y) / 2),
                Y = Math.Cos(Misc.DegreesToRadians(euler.Z) / 2),
                Z = Math.Cos(Misc.DegreesToRadians(euler.X) / 2)
            };

            Vector sine = new Vector(0, 0, 0)
            {
                X = Math.Sin(Misc.DegreesToRadians(euler.Y) / 2),
                Y = Math.Sin(Misc.DegreesToRadians(euler.Z) / 2),
                Z = Math.Sin(Misc.DegreesToRadians(euler.X) / 2)
            };

            Quaternion quaternion = new Quaternion(0, 0, 0, 0)
            {
                W = cosine.X * cosine.Y * cosine.Z - sine.X   * sine.Y   * sine.Z,
                X = sine.X   * sine.Y   * cosine.Z + cosine.X * cosine.Y * sine.Z,
                Y = sine.X   * cosine.Y * cosine.Z + cosine.X * sine.Y   * sine.Z,
                Z = cosine.X * sine.Y   * cosine.Z - sine.X   * cosine.Y * sine.Z
            };

            return quaternion;
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
        public static Quaternion Add(Quaternion quaternionOne, Quaternion quaternionTwo)
        {
            Quaternion added = new Quaternion(0, 0, 0, 0)
            {
                W = quaternionOne.W + quaternionTwo.W,
                X = quaternionOne.X + quaternionTwo.X,
                Y = quaternionOne.Y + quaternionTwo.Y,
                Z = quaternionOne.Z + quaternionTwo.Z
            };

            return added;
        }

        /// <summary>
        /// Subtracts two quaternions.
        /// </summary>
        /// <param name="quaternionOne">Quaternion that is subtracted from.</param>
        /// <param name="quaternionTwo">Quaternion that subtracts from.</param>
        /// <returns>Returns the subtracted quaternion.</returns>
        public static Quaternion Subtract(Quaternion quaternionOne, Quaternion quaternionTwo)
        {
            Quaternion subtracted = new Quaternion(0, 0, 0, 0)
            {
                W = quaternionOne.W - quaternionTwo.W,
                X = quaternionOne.X - quaternionTwo.X,
                Y = quaternionOne.Y - quaternionTwo.Y,
                Z = quaternionOne.Z - quaternionTwo.Z
            };

            throw new NotImplementedException();
        }

        /// <summary>
        /// Multiplies a scalar by a quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is scaled.</param>
        /// <param name="scale">Scalar that scales the quaternion.</param>
        /// <returns>Returns the scaled quaternion.</returns>
        public static Quaternion Multiply(Quaternion quaternion, double scale)
        {
            Quaternion multiplied = new Quaternion(0, 0, 0, 0)
            {
                W = quaternion.W * scale,
                X = quaternion.X * scale,
                Y = quaternion.Y * scale,
                Z = quaternion.Z * scale
            };

            return multiplied;
        }

        /// <summary>
        /// Multiplies two quaternions by each other.
        /// </summary>
        /// <param name="quaternionOne">Quaternion that is multiplied to.</param>
        /// <param name="quaternionTwo">Quathernion that multiplies to.</param>
        /// <returns>Returns the multiplied quaternions.</returns>
        public static Quaternion Multiply(Quaternion quaternionOne, Quaternion quaternionTwo)
        {
            Quaternion multiplied = new Quaternion(0, 0, 0, 0)
            {
                W = quaternionOne.W * quaternionTwo.W - quaternionOne.X * quaternionTwo.X - quaternionOne.Y * quaternionTwo.Y - quaternionOne.Z * quaternionTwo.Z,
                X = quaternionOne.W * quaternionTwo.X + quaternionOne.X * quaternionTwo.W + quaternionOne.Y * quaternionTwo.Z - quaternionOne.Z * quaternionTwo.Y,
                Y = quaternionOne.W * quaternionTwo.Y - quaternionOne.X * quaternionTwo.Z + quaternionOne.Y * quaternionTwo.W + quaternionOne.Z * quaternionTwo.X,
                Z = quaternionOne.W * quaternionTwo.Z + quaternionOne.X * quaternionTwo.Y - quaternionOne.Y * quaternionTwo.X + quaternionOne.Z * quaternionTwo.W
            };

            return multiplied;
        }

        /// <summary>
        /// Divides a quaternion by a scalar.
        /// </summary>
        /// <param name="quaternion">Quaternion that is scaled.</param>
        /// <param name="scale">Scalar that scales quaternion.</param>
        /// <returns>Returns the scaled quaternion.</returns>
        public static Quaternion Divide(Quaternion quaternion, double scale)
        {
            Quaternion divided = new Quaternion(0, 0, 0, 0)
            {
                W = quaternion.W / scale,
                X = quaternion.X / scale,
                Y = quaternion.Y / scale,
                Z = quaternion.Z / scale
            };

            return divided;
        }

        /// <summary>
        /// Divides two quaternions by each other.
        /// </summary>
        /// <param name="quaternionOne">Quaternion that is divided.</param>
        /// <param name="quaternionTwo">Quaternion that divides by.</param>
        /// <returns>Returns the divided quaternion.</returns>
        public static Quaternion Divide(Quaternion quaternionOne, Quaternion quaternionTwo)
        {
            double scale = quaternionTwo.W * quaternionTwo.W + quaternionTwo.X * quaternionTwo.X + quaternionTwo.Y * quaternionTwo.Y + quaternionTwo.Z * quaternionTwo.Z;

            Quaternion divided = new Quaternion(0, 0, 0, 0)
            {
                W = (  quaternionOne.W * quaternionTwo.W + quaternionOne.X * quaternionTwo.X + quaternionOne.Y * quaternionTwo.Y + quaternionOne.Z * quaternionTwo.Z) / scale,
                X = (- quaternionOne.W * quaternionTwo.X + quaternionOne.X * quaternionTwo.W + quaternionOne.Y * quaternionTwo.Z - quaternionOne.Z * quaternionTwo.Y) / scale,
                Y = (- quaternionOne.W * quaternionTwo.Y - quaternionOne.X * quaternionTwo.Z + quaternionOne.Y * quaternionTwo.W + quaternionOne.Z * quaternionTwo.X) / scale,
                Z = (- quaternionOne.W * quaternionTwo.Z + quaternionOne.X * quaternionTwo.Y - quaternionOne.Y * quaternionTwo.X + quaternionOne.Z * quaternionTwo.W) / scale 
            };

            return divided;
        }

        /// <summary>
        /// Returns the absolute value of each individual quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is converted to a magnitude.</param>
        /// <returns>Returns the absolute value of the input quaternion.</returns>
        public static Quaternion Absolute(Quaternion quaternion)
        {
            Quaternion absolute = new Quaternion(0, 0, 0, 0)
            {
                W = Math.Abs(quaternion.W),
                X = Math.Abs(quaternion.X),
                Y = Math.Abs(quaternion.Y),
                Z = Math.Abs(quaternion.Z)
            };

            return absolute;
        }

        /// <summary>
        /// Returns the inverse of the input quaternion. (Not the conjugate.)
        /// </summary>
        /// <param name="quaternion">Quaternion that is inverted.</param>
        /// <returns>Returns the inverse of the quaternion.</returns>
        public static Quaternion Inverse(Quaternion quaternion)
        {
            Quaternion inverse = new Quaternion(0, 0, 0, 0)
            {
                W = -quaternion.W,
                X = -quaternion.X,
                Y = -quaternion.Y,
                Z = -quaternion.Z
            };

            return inverse;
        }

        /// <summary>
        /// Returns the conjugate of the input quaternion. (Not the inverse.)
        /// </summary>
        /// <param name="quaternion">Quaternion that is conjugated.</param>
        /// <returns>Returns the conjugate of the input quaternion.</returns>
        public static Quaternion Conjugate(Quaternion quaternion)
        {
            Quaternion conjugate = new Quaternion(0, 0, 0, 0)
            {
                W =  quaternion.W,
                X = -quaternion.X,
                Y = -quaternion.Y,
                Z = -quaternion.Z
            };

            return conjugate;
        }

        /// <summary>
        /// Returns the scalar power of the input quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is exponentiated.</param>
        /// <param name="exponent">Scalar that is used as the exponent.</param>
        /// <returns>Returns the scalar power of the input quaternion.</returns>
        public static Quaternion Power(Quaternion quaternion, double exponent)
        {
            Quaternion power = new Quaternion(0, 0, 0, 0)
            {
                W = Math.Pow(quaternion.W, exponent),
                X = Math.Pow(quaternion.X, exponent),
                Y = Math.Pow(quaternion.Y, exponent),
                Z = Math.Pow(quaternion.Z, exponent)
            };

            return power;
        }

        /// <summary>
        /// Returns the quaternion power of the input quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is exponentiated.</param>
        /// <param name="exponent">Quaternion that is used as the exponent.</param>
        /// <returns>Returns the quaternion power of the input quaternion.</returns>
        public static Quaternion Power(Quaternion quaternion, Quaternion exponent)
        {
            Quaternion power = new Quaternion(0, 0, 0, 0)
            {
                W = Math.Pow(quaternion.W, exponent.W),
                X = Math.Pow(quaternion.X, exponent.X),
                Y = Math.Pow(quaternion.Y, exponent.Y),
                Z = Math.Pow(quaternion.Z, exponent.Z)
            };

            return power;
        }

        /// <summary>
        /// Normalizes the input quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is normalized.</param>
        /// <returns>Returns the normalized input quaternion.</returns>
        public static Quaternion Normalize(Quaternion quaternion)
        {
            double n = Math.Sqrt(Math.Pow(quaternion.W, 2) + Math.Pow(quaternion.X, 2) + Math.Pow(quaternion.Y, 2) + Math.Pow(quaternion.Z, 2));

            quaternion.W /= n;
            quaternion.X /= n;
            quaternion.Y /= n;
            quaternion.Z /= n;

            return quaternion;
        }
        
        /// <summary>
        /// Determines if any individual value of the quaternion is not a number.
        /// </summary>
        /// <param name="quaternion">Quaternion that isNaN checked.</param>
        /// <returns>Returns true if all any of the values are not a number.</returns>
        public static bool IsNaN(Quaternion quaternion)
        {
            return double.IsNaN(quaternion.W) || double.IsNaN(quaternion.X) || double.IsNaN(quaternion.Y) || double.IsNaN(quaternion.Z);
        }

        /// <summary>
        /// Determines if all of the quaternions values are finite.
        /// </summary>
        /// <param name="quaternion">Quaternion that is checked.</param>
        /// <returns>Returns true if all values are finite.</returns>
        public static bool IsFinite(Quaternion quaternion)
        {
            return !double.IsInfinity(quaternion.W) && !double.IsInfinity(quaternion.X) && !double.IsInfinity(quaternion.Y) && !double.IsInfinity(quaternion.Z);
        }

        /// <summary>
        /// Determines if all of the quaternions values are infinite.
        /// </summary>
        /// <param name="quaternion">Quaternion that is checked.</param>
        /// <returns>Returns true if all values are infinite.</returns>
        public static bool IsInfinite(Quaternion quaternion)
        {
            return double.IsInfinity(quaternion.W) && double.IsInfinity(quaternion.X) && double.IsInfinity(quaternion.Y) && double.IsInfinity(quaternion.Z);
        }

        /// <summary>
        /// Determines if all of the quaternions values are nonzero.
        /// </summary>
        /// <param name="quaternion">Quaternion that is checked.</param>
        /// <returns>Returns true if all values are nonzero.</returns>
        public static bool IsNonZero(Quaternion quaternion)
        {
            return quaternion.W != 0 && quaternion.X != 0 && quaternion.Y != 0 && quaternion.Z != 0;
        }

        /// <summary>
        /// Determines if the two input quaternions are equal.
        /// </summary>
        /// <param name="quaternionA">Quaternion that is checked.</param>
        /// <param name="quaternionB">Quaternion that is checked.</param>
        /// <returns>Returns true if both quaternions are equal.</returns>
        public static bool IsEqual(Quaternion quaternionA, Quaternion quaternionB)
        {
            return !IsNaN(quaternionA) && !IsNaN(quaternionB) &&
                    quaternionA.W == quaternionB.W &&
                    quaternionA.X == quaternionB.X &&
                    quaternionA.Y == quaternionB.Y &&
                    quaternionA.Z == quaternionB.Z;
        }
    }
}
