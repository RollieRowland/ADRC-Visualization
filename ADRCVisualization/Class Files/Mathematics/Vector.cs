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

        public Vector(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Vector Normal(Vector vector)
        {
            double mult = Magnitude(vector) == 0 ? Double.PositiveInfinity : 1 / Magnitude(vector);

            return Multiply(mult);
        }

        public bool IsEqual(Vector vector)
        {
            return (X == vector.X) && (Y == vector.Y) && (Z == vector.Z);
        }

        public Vector Add(Vector vector)
        {
            return new Vector(X + vector.X, Y + vector.Y, Z + vector.Z);
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return v1.Add(v2);
        }

        public Vector Subtract(Vector vector)
        {
            return new Vector(X - vector.X, Y - vector.Y, Z - vector.Z);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return v1.Subtract(v2);
        }

        public Vector Multiply(double k)
        {
            return new Vector((X * k), (Y * k), (Z * k));
        }

        public static Vector operator *(Vector v1, double s)
        {
            return v1.Multiply(s);
        }

        public Vector Multiply(Vector vector)
        {
            return new Vector((X * vector.X), (Y * vector.Y), (Z * vector.Z));
        }

        public static Vector operator *(Vector v1, Vector v2)
        {
            return v1.Multiply(v2);
        }

        public Vector Divide(double k)
        {
            return new Vector((X / k), (Y / k), (Z / k));
        }

        public static Vector operator /(Vector v1, double s)
        {
            return v1.Divide(s);
        }

        public Vector Divide(Vector vector)
        {
            return new Vector((X / vector.X), (Y / vector.Y), (Z / vector.Z));
        }

        public static Vector operator /(Vector v1, Vector v2)
        {
            return v1.Divide(v2);
        }

        public static double DotProduct(Vector vectorO, Vector vectorT)
        {
            return (vectorO.X * vectorT.X) + (vectorO.Y * vectorT.Y) + (vectorO.Z * vectorT.Z);
        }

        public static Vector CrossProduct(Vector vectorO, Vector vectorT)
        {
            return new Vector(((vectorO.Y * vectorT.Z) - (vectorO.Z * vectorT.Y)), ((vectorO.Z * vectorT.X) - (vectorO.X * vectorT.Z)), ((vectorO.X * vectorT.Y) - (vectorO.Y * vectorT.X)));
        }

        public static double Magnitude(Vector vector)
        {
            return Math.Sqrt(DotProduct(vector, vector));
        }

        public static Vector Normalize(Vector vector)
        {
            double length = GetLength(vector);

            if (length == 1) return vector;
            if (length == 0) return new Vector(1, 0, 0);

            return new Vector(vector.X / length, vector.Y / length, vector.Z / length);
        }

        public static double GetLength(Vector vector)
        {
            return Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2) + Math.Pow(vector.Z, 2));
        }

        public Vector RotateEuler(double pitch, double roll, double yaw)
        {
            var cosa = Math.Cos(yaw);
            var sina = Math.Sin(yaw);

            var cosb = Math.Cos(pitch);
            var sinb = Math.Sin(pitch);

            var cosc = Math.Cos(roll);
            var sinc = Math.Sin(roll);

            var Axx = cosa * cosb;
            var Axy = cosa * sinb * sinc - sina * cosc;
            var Axz = cosa * sinb * cosc + sina * sinc;

            var Ayx = sina * cosb;
            var Ayy = sina * sinb * sinc + cosa * cosc;
            var Ayz = sina * sinb * cosc - cosa * sinc;

            var Azx = -sinb;
            var Azy = cosb * sinc;
            var Azz = cosb * cosc;

            X = Axx * X + Axy * Y + Axz * Z;
            Y = Ayx * X + Ayy * Y + Ayz * Z;
            Z = Azx * X + Azy * Y + Azz * Z;

            return new Vector(Axx * X + Axy * Y + Axz * Z,
                              Ayx * X + Ayy * Y + Ayz * Z,
                              Azx * X + Azy * Y + Azz * Z);
        }

        public static double CalculateEuclideanDistance(Vector one, Vector two)
        {
            return Math.Sqrt(Math.Pow(one.X - two.X, 2) + Math.Pow(one.Y - two.Y, 2) + Math.Pow(one.Z - two.Z, 2));
        }

        public override string ToString()
        {
            string x = String.Format("{0:0.000}", X).PadLeft(7);
            string y = String.Format("{0:0.000}", Y).PadLeft(7);
            string z = String.Format("{0:0.000}", Z).PadLeft(7);
            
            return x + " " + y + " " + z;
        }
    }
}