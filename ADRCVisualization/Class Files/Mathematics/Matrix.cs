using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    class Matrix
    {
        public Vector XAxis { get; set; }
        public Vector YAxis { get; set; }
        public Vector ZAxis { get; set; }

        private Vector InitialVector;
        private bool didRotate = false;
        
        public Matrix(Vector axes)
        {
            XAxis = new Vector(axes.X, 0, 0);
            YAxis = new Vector(0, axes.Y, 0);
            ZAxis = new Vector(0, 0, axes.Z);

            InitialVector = axes;
        }

        public static Matrix QuaternionToMatrixRotation(Quaternion quaternion)
        {
            Vector X = new Vector(1, 0, 0);
            Vector Y = new Vector(0, 1, 0);
            Vector Z = new Vector(0, 0, 1);

            return new Matrix(new Vector(1, 1, 1))
            {
                XAxis = quaternion.RotateVector(X),
                YAxis = quaternion.RotateVector(Y),
                ZAxis = quaternion.RotateVector(Z)
            };
        }
        
        public static Vector RotateVector(Vector Rotate, Vector Coordinates)
        {
            //calculate rotation matrix
            Matrix matrix = new Matrix(Coordinates);

            matrix.Rotate(Rotate);

            return matrix.ConvertCoordinateToVector();
        }

        public Vector ConvertCoordinateToVector()
        {
            if (didRotate)
            {
                return new Vector((XAxis.X + YAxis.X + ZAxis.X), (XAxis.Y + YAxis.Y + ZAxis.Y), (XAxis.Z + YAxis.Z + ZAxis.Z));
            }
            else
            {
                return InitialVector;
            }
        }

        /// <summary>
        /// Run between individual rotations to prevent gimbal lock
        /// </summary>
        public void ReadjustMatrix()
        {
            double X = (XAxis.X + YAxis.X + ZAxis.X);
            double Y = (XAxis.Y + YAxis.Y + ZAxis.Y);
            double Z = (XAxis.Z + YAxis.Z + ZAxis.Z);

            XAxis = new Vector(X, X, X);
            YAxis = new Vector(Y, Y, Y);
            ZAxis = new Vector(Z, Z, Z);
        }
        
        /// <summary>
        /// Rotate about arbitrary axis
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="theta"></param>
        public Matrix(Vector axis, double theta)
        {
            Vector u = axis.Normalize();

            double sin = Math.Sin(theta);
            double cos = Math.Cos(theta);

            double uxy = u.X * u.Y * (1 - cos);
            double uyz = u.Y * u.Z * (1 - cos);
            double uxz = u.X * u.Z * (1 - cos);
            double ux2 = u.X * u.X * (1 - cos);
            double uy2 = u.Y * u.Y * (1 - cos);
            double uz2 = u.Z * u.Z * (1 - cos);

            double uxsin = u.X * sin;
            double uysin = u.Y * sin;
            double uzsin = u.Z * sin;

            XAxis = new Vector(cos + ux2, uxy + uzsin, uxz - uysin);
            YAxis = new Vector(uxy - uzsin, cos + uy2, uyz + uxsin);
            ZAxis = new Vector(uxz + uysin, uyz - uxsin, cos + uz2);
        }

        /// <summary>
        /// Rotation with the right-hand rule
        /// </summary>
        /// <param name="alpha">Pitch</param>
        /// <param name="beta">Heading</param>
        /// <param name="gamma">Bank</param>
        public void Rotate(Vector rotation)
        {
            if (rotation.X != 0)
            {
                RotateX(rotation.X);
                didRotate = true;

                if (rotation.Y != 0 || rotation.Z != 0)
                {
                    ReadjustMatrix();
                }
            }

            if (rotation.Y != 0)
            {
                RotateY(rotation.Y);
                didRotate = true;

                if (rotation.Z != 0)
                {
                    ReadjustMatrix();
                }
            }

            if (rotation.Z != 0)
            {
                RotateZ(rotation.Z);
                didRotate = true;
            }
        }

        /// <summary>
        /// Rotates around the X axis, pitch
        /// </summary>
        /// <param name="theta"></param>
        public void RotateX(double theta)
        {
            double cos = Math.Cos(Misc.DegreesToRadians(theta));
            double sin = Math.Sin(Misc.DegreesToRadians(theta));

            XAxis = new Vector(1, 0,      0).Multiply(XAxis);
            YAxis = new Vector(0, cos, -sin).Multiply(YAxis);
            ZAxis = new Vector(0, sin,  cos).Multiply(ZAxis);
        }

        /// <summary>
        /// Rotates around the Y axis, heading
        /// </summary>
        /// <param name="theta"></param>
        public void RotateY(double theta)
        {
            double cos = Math.Cos(Misc.DegreesToRadians(theta));
            double sin = Math.Sin(Misc.DegreesToRadians(theta));

            XAxis = new Vector(cos,  0,  sin).Multiply(XAxis);
            YAxis = new Vector(0,    1,  0  ).Multiply(YAxis);
            ZAxis = new Vector(-sin, 0,  cos).Multiply(ZAxis);
        }

        /// <summary>
        /// Rotates around the Z axis, bank
        /// </summary>
        /// <param name="theta"></param>
        public void RotateZ(double theta)
        {
            double cos = Math.Cos(Misc.DegreesToRadians(theta));
            double sin = Math.Sin(Misc.DegreesToRadians(theta));

            XAxis = new Vector(cos, -sin, 0).Multiply(XAxis);
            YAxis = new Vector(sin, cos,  0).Multiply(YAxis);
            ZAxis = new Vector(0,   0,    1).Multiply(ZAxis);
        }

        public void Multiply(double d)
        {
            XAxis = XAxis.Multiply(d);
            YAxis = YAxis.Multiply(d);
            ZAxis = ZAxis.Multiply(d);
        }

        private void Multiply(Matrix m)
        {
            XAxis = XAxis.Multiply(m.XAxis);
            YAxis = YAxis.Multiply(m.YAxis);
            ZAxis = ZAxis.Multiply(m.ZAxis);
        }

        public void RotateRelative(Matrix m)
        {
            Multiply(m);
        }
        
        public void Normalize()
        {
            Vector vz = Vector.CrossProduct(XAxis, YAxis);
            Vector vy = Vector.CrossProduct(vz, XAxis);

            XAxis = XAxis.Normalize();
            YAxis = vy.Normalize();
            ZAxis = vz.Normalize();
        }

        public void Transpose()//opposite rotation matrix
        {
            XAxis = new Vector(XAxis.X, YAxis.X, ZAxis.X);
            YAxis = new Vector(XAxis.Y, YAxis.Y, ZAxis.Y);
            ZAxis = new Vector(XAxis.Z, YAxis.Z, ZAxis.Z);
        }

        public double Determinant()
        {
            return XAxis.X * (YAxis.Y * ZAxis.Z - ZAxis.Y * YAxis.Z) -
                   YAxis.X * (ZAxis.Z * XAxis.Y - ZAxis.Y * XAxis.Z) +
                   ZAxis.X * (XAxis.Y * YAxis.Z - YAxis.Y * XAxis.Z);
        }

        public void Inverse()
        {
            XAxis = Vector.CrossProduct(YAxis, ZAxis);
            YAxis = Vector.CrossProduct(ZAxis, XAxis);
            ZAxis = Vector.CrossProduct(XAxis, YAxis);

            Transpose();
            Multiply(1 / Determinant());
        }
        
        
        public bool IsEqual(Matrix m)
        {
            return m == this || XAxis.IsEqual(m.XAxis) && YAxis.IsEqual(m.YAxis) && ZAxis.IsEqual(m.ZAxis);
        }

        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}]\n[{3}, {4}, {5}]\n[{6}, {7}, {8}]",
                Math.Round(XAxis.X, 3), Math.Round(YAxis.X, 3), Math.Round(ZAxis.X, 3),
                Math.Round(XAxis.Y, 3), Math.Round(YAxis.Y, 3), Math.Round(ZAxis.Y, 3),
                Math.Round(XAxis.Z, 3), Math.Round(YAxis.Z, 3), Math.Round(ZAxis.Z, 3));
        }

        public static void TestRotationMatrix()
        {
            Vector point = new Vector(110, -50, 60);
            Matrix rotation = new Matrix(point);

            rotation.Rotate(new Vector(60, 60, 30));

            Console.WriteLine(rotation.ToString());

            Console.WriteLine(rotation.ConvertCoordinateToVector().ToString());
        }
    }
}
