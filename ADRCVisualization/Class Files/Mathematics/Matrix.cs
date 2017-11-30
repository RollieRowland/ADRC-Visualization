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

        public Matrix(params double[] e)
        {
            if (e.Length != 9) throw new Exception();

            XAxis = new Vector(e[0], e[3], e[6]);
            YAxis = new Vector(e[1], e[4], e[7]);
            ZAxis = new Vector(e[2], e[5], e[8]);
        }

        public Matrix(Vector XAxis, Vector YAxis, Vector ZAxis)
        {
            this.XAxis = XAxis;
            this.YAxis = YAxis;
            this.ZAxis = ZAxis;
        }

        public Matrix(Vector axis, double theta)
        {
            Vector u = Vector.Normalize(axis);

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

        static public Matrix XRotationMatrix(Vector vectorX, double theta)
        {
            double cos = Math.Cos(theta);
            double sin = Math.Sin(theta);

            return new Matrix(vectorX,
                              new Vector(0, cos, sin),
                              new Vector(0, -sin, cos));
        }

        static public Matrix YRotationMatrix(Vector vectorY, double theta)
        {
            double cos = Math.Cos(theta);
            double sin = Math.Sin(theta);

            return new Matrix(new Vector(cos, 0, -sin),
                              vectorY,
                              new Vector(sin, 0, cos));
        }

        static public Matrix ZRotationMatrix(Vector vectorZ, double theta)
        {
            double cos = Math.Cos(theta);
            double sin = Math.Sin(theta);

            return new Matrix(new Vector(cos, sin, 0),
                              new Vector(-sin, cos, 0),
                              vectorZ);
        }

        public Matrix RotateX(double theta)
        {
            return XRotationMatrix(XAxis, theta).Multiply(this);
        }

        public Matrix RotateY(double theta)
        {
            return YRotationMatrix(YAxis, theta).Multiply(this);
        }

        public Matrix RotateZ(double theta)
        {
            return ZRotationMatrix(ZAxis, theta).Multiply(this);
        }

        public Matrix Multiply(double d)
        {
            return new Matrix(XAxis.Multiply(d), YAxis.Multiply(d), ZAxis.Multiply(d));
        }

        private Matrix Multiply(Matrix m)
        {
            return new Matrix(Vector.Multiply(XAxis, m.XAxis), Vector.Multiply(YAxis, m.YAxis), Vector.Multiply(ZAxis, m.ZAxis));
        }

        public Matrix RotateRelative(Matrix m)
        {
            return Multiply(m);
        }

        public Matrix RotateAbsolute(Matrix m)
        {
            return m.Multiply(this);
        }

        public Matrix Normalize()
        {
            Vector vz = Vector.CrossProduct(XAxis, YAxis);
            Vector vy = Vector.CrossProduct(vz, XAxis);

            return new Matrix(Vector.Normalize(XAxis),
                              Vector.Normalize(vy),
                              Vector.Normalize(vz));
        }

        public Matrix Transpose()
        {
            return new Matrix(XAxis.X, XAxis.Y, XAxis.Z,
                              YAxis.X, YAxis.Y, YAxis.Z,
                              ZAxis.X, ZAxis.Y, ZAxis.Z);
        }

        public double Determinant()
        {
            return XAxis.X * (YAxis.Y * ZAxis.Z - ZAxis.Y * YAxis.Z) -
                   YAxis.X * (ZAxis.Z * XAxis.Y - ZAxis.Y * XAxis.Z) +
                   ZAxis.X * (XAxis.Y * YAxis.Z - YAxis.Y * XAxis.Z);
        }

        public Matrix Inverse()
        {
            Vector A = Vector.CrossProduct(YAxis, ZAxis);
            Vector B = Vector.CrossProduct(ZAxis, XAxis);
            Vector C = Vector.CrossProduct(XAxis, YAxis);

            return new Matrix(A, B, C).Transpose().Multiply(1 / Determinant());
        }

        public Matrix OppositeRotationMatrix()
        {
            return Transpose();
        }
        
        public bool IsEqual(Matrix m)
        {
            return m == this || Vector.IsEqual(XAxis, m.XAxis) && Vector.IsEqual(YAxis, m.YAxis) && Vector.IsEqual(ZAxis, m.ZAxis);
        }

        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}]\n[{3}, {4}, {5}]\n[{6}, {7}, {8}]",
                XAxis.X, YAxis.Y, ZAxis.Z,
                XAxis.Y, YAxis.Y, ZAxis.Y,
                XAxis.Z, YAxis.Z, ZAxis.Z);
        }
    }
}
