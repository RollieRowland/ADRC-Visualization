using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        /// <summary>
        /// Creates a three dimensional vector from three axis values.
        /// </summary>
        /// <param name="X">X axis</param>
        /// <param name="Y">Y axis</param>
        /// <param name="Z">Z axis</param>
        public Vector(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        /// <summary>
        /// Creates a deep copy of the input vector.
        /// </summary>
        /// <param name="vector">Vector that is to be copied.</param>
        public Vector(Vector vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Produces the normal vector from the input vector.
        /// </summary>
        /// <param name="vector">The vector to be normalized.</param>
        /// <returns></returns>
        public Vector Normal(Vector vector)
        {
            return Multiply(Magnitude(vector) == 0 ? Double.PositiveInfinity : 1 / Magnitude(vector));
        }

        /// <summary>
        /// Compares if the input vector is equal to this object.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool IsEqual(Vector vector)
        {
            return (X == vector.X) && (Y == vector.Y) && (Z == vector.Z);
        }

        /// <summary>
        /// Compares if two vectors are equal.
        /// </summary>
        /// <param name="v1">First vector for comparison.</param>
        /// <param name="v2">Second vector for comparison.</param>
        /// <returns>Returns the boolean comparison between the objects.</returns>
        public static bool IsEqual(Vector v1, Vector v2)
        {
            return (v1.X == v2.X) && (v1.Y == v2.Y) && (v1.Z == v2.Z);
        }

        /// <summary>
        /// Compares if two vectors are equal.
        /// </summary>
        /// <param name="v1">First vector for comparison.</param>
        /// <param name="v2">Second vector for comparison.</param>
        /// <returns>Returns the boolean comparison between the objects.</returns>
        public static bool operator ==(Vector v1, Vector v2)
        {
            return IsEqual(v1, v2);
        }

        /// <summary>
        /// Compares if two vectors are not equal.
        /// </summary>
        /// <param name="v1">First vector for comparison.</param>
        /// <param name="v2">Second vector for comparison.</param>
        /// <returns>Returns the boolean comparison between the objects.</returns>
        public static bool operator !=(Vector v1, Vector v2)
        {
            return !IsEqual(v1, v2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Adds the input vector object to this object.
        /// </summary>
        /// <param name="vector">Vector to be added to this object.</param>
        /// <returns>Returns the addition of the two vectors.</returns>
        public Vector Add(Vector vector)
        {
            return new Vector(X + vector.X, Y + vector.Y, Z + vector.Z);
        }

        /// <summary>
        /// Adds the two vectors together.
        /// </summary>
        /// <param name="v1">The first vector to be added.</param>
        /// <param name="v2">The second vector to be added.</param>
        /// <returns>Returns the addition of the two vectors.</returns>
        public static Vector operator +(Vector v1, Vector v2)
        {
            return v1.Add(v2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector Subtract(Vector vector)
        {
            return new Vector(X - vector.X, Y - vector.Y, Z - vector.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector operator -(Vector v1, Vector v2)
        {
            return v1.Subtract(v2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vector Multiply(double k)
        {
            return new Vector((X * k), (Y * k), (Z * k));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector operator *(double s, Vector v1)
        {
            return v1.Multiply(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector operator *(Vector v1, double s)
        {
            return v1.Multiply(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector Multiply(Vector vector)
        {
            return new Vector((X * vector.X), (Y * vector.Y), (Z * vector.Z));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector operator *(Vector v1, Vector v2)
        {
            return v1.Multiply(v2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vector Divide(double k)
        {
            return new Vector((X / k), (Y / k), (Z / k));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector operator /(Vector v1, double s)
        {
            return v1.Divide(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector Divide(Vector vector)
        {
            return new Vector((X / vector.X), (Y / vector.Y), (Z / vector.Z));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector operator /(Vector v1, Vector v2)
        {
            return v1.Divide(v2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectorO"></param>
        /// <param name="vectorT"></param>
        /// <returns></returns>
        public static double DotProduct(Vector vectorO, Vector vectorT)
        {
            return (vectorO.X * vectorT.X) + (vectorO.Y * vectorT.Y) + (vectorO.Z * vectorT.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectorO"></param>
        /// <param name="vectorT"></param>
        /// <returns></returns>
        public static Vector CrossProduct(Vector vectorO, Vector vectorT)
        {
            return new Vector(((vectorO.Y * vectorT.Z) - (vectorO.Z * vectorT.Y)), 
                              ((vectorO.Z * vectorT.X) - (vectorO.X * vectorT.Z)), 
                              ((vectorO.X * vectorT.Y) - (vectorO.Y * vectorT.X)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static double Magnitude(Vector vector)
        {
            return Math.Sqrt(DotProduct(vector, vector));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector Normalize()
        {
            Vector vector = new Vector(this);
            double length = vector.GetLength();

            if (length == 1) return vector;
            if (length == 0) return new Vector(0, 1, 0);

            return new Vector(vector.X / length, vector.Y / length, vector.Z / length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetLength()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static double CalculateEuclideanDistance(Vector one, Vector two)
        {
            return Math.Sqrt(Math.Pow(one.X - two.X, 2) + Math.Pow(one.Y - two.Y, 2) + Math.Pow(one.Z - two.Z, 2));
        }
        public override bool Equals(System.Object obj)
        {
            var vector = obj as Vector;

            if (vector == null)
            {
                return false;
            }
            
            return IsEqual(this, vector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string x = MathE.DoubleToCleanString(X);
            string y = MathE.DoubleToCleanString(Y);
            string z = MathE.DoubleToCleanString(Z);
            
            return "[" + x + " " + y + " " + z + "]";
        }
    }
}