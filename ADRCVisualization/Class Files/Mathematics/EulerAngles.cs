using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    public class EulerAngles
    {
        public Vector Angles { get; set; }
        public EulerOrder Order { get; set; }

        public EulerAngles(Vector angles, EulerOrder order)
        {
            Angles = angles;
            Order  = order;
        }

        public static HMatrix EulerToHMatrix(EulerAngles eulerAngles)
        {
            HMatrix hM = new HMatrix();
            double sx, sy, sz, cx, cy, cz, cc, cs, sc, ss;
            Vector p = eulerAngles.Order.Permutation;

            if (eulerAngles.Order.FrameTaken == EulerOrder.AxisFrame.Rotating)
            {
                double t = eulerAngles.Angles.X;
                eulerAngles.Angles.X = eulerAngles.Angles.Z;
                eulerAngles.Angles.Z = t;
            }

            if (eulerAngles.Order.AxisPermutation == EulerOrder.Parity.Odd)
            {
                eulerAngles.Angles.X = -eulerAngles.Angles.X;
                eulerAngles.Angles.Y = -eulerAngles.Angles.Y;
                eulerAngles.Angles.Z = -eulerAngles.Angles.Z;
            }

            sx = Math.Sin(eulerAngles.Angles.X);
            sy = Math.Sin(eulerAngles.Angles.Y);
            sz = Math.Sin(eulerAngles.Angles.Z);
            cx = Math.Cos(eulerAngles.Angles.X);
            cy = Math.Cos(eulerAngles.Angles.Y);
            cz = Math.Cos(eulerAngles.Angles.Z);

            cc = cx * cz;
            cs = cx * sz;
            sc = sx * cz;
            ss = sx * sz;

            if (eulerAngles.Order.InitialAxisRepitition == EulerOrder.AxisRepitition.Yes)
            {
                hM[p.X, p.X] = cy;          hM[p.X, p.Y] = sy * sx;         hM[p.X, p.Z] = sy * cx;         hM[0, 3] = 0;
                hM[p.Y, p.X] = sy * sz;     hM[p.Y, p.Y] = -cy * ss + cc;   hM[p.Y, p.Z] = -cy * cs - sc;   hM[1, 3] = 0;
                hM[p.Z, p.X] = -sy * cz;    hM[p.Z, p.Y] = cy * sc + cs;    hM[p.Z, p.Z] = cy * cc - ss;    hM[2, 3] = 0;
                hM[3, 0] = 0;               hM[3, 1] = 0;                   hM[3, 2] = 0;                   hM[3, 3] = 1;
            }
            else
            {
                hM[p.X, p.X] = cy * cz;     hM[p.X, p.Y] = sy * sc - cs;    hM[p.X, p.Z] = sy * cc + ss;    hM[0, 3] = 0;
                hM[p.Y, p.X] = cy * sz;     hM[p.Y, p.Y] = sy * ss + cc;    hM[p.Y, p.Z] = sy * cs - sc;    hM[1, 3] = 0;
                hM[p.Z, p.X] = -sy;         hM[p.Z, p.Y] = cy * sx;         hM[p.Z, p.Z] = cy * cx;         hM[2, 3] = 0;
                hM[3, 0] = 0;               hM[3, 1] = 0;                   hM[3, 2] = 0;                   hM[3, 3] = 1;
            }

            return hM;
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
                q.W = cy * (cc - ss);
                q.X = cy * (cs + sc);
                q.Y = sy * (cc + ss);
                q.Z = sy * (cs - sc);
            }
            else
            {
                q.W = cy * cc + sy * ss;
                q.X = cy * sc - sy * cs;
                q.Y = cy * ss + sy * cc;
                q.Z = cy * cs - sy * sc;
            }

            if (eulerAngles.Order.AxisPermutation == EulerOrder.Parity.Odd)
            {
                q.Y = -q.Y;
            }

            q.Permutate(eulerAngles.Order.Permutation);

            return q;
        }
        
        public static EulerAngles HMatrixToEuler(HMatrix hM, EulerOrder order)
        {
            EulerAngles eulerAngles = new EulerAngles(new Vector(0, 0, 0), order);
            Vector p = order.Permutation;

            if (order.InitialAxisRepitition == EulerOrder.AxisRepitition.Yes)
            {
                double sy = Math.Sqrt(Math.Pow(hM[p.X, p.Y], 2) + Math.Pow(hM[p.X, p.Z], 2));

                if (sy > 16 * double.Epsilon)
                {
                    eulerAngles.Angles.X = Math.Atan2(hM[p.X, p.Y],  hM[p.X, p.Z]);
                    eulerAngles.Angles.Y = Math.Atan2(sy,            hM[p.X, p.X]);
                    eulerAngles.Angles.Z = Math.Atan2(hM[p.Y, p.X], -hM[p.Z, p.X]);
                }
                else
                {
                    eulerAngles.Angles.X = Math.Atan2(-hM[p.Y, p.Z], hM[p.Y, p.Y]);
                    eulerAngles.Angles.Y = Math.Atan2(sy, hM[p.X, p.X]);
                    eulerAngles.Angles.Z = 0;
                }
            }
            else
            {
                double cy = Math.Sqrt(Math.Pow(hM[p.X, p.X], 2) + Math.Pow(hM[p.Y, p.X], 2));

                if (cy > 16 * double.Epsilon)
                {
                    eulerAngles.Angles.X = Math.Atan2( hM[p.Z, p.Y], hM[p.Z, p.Z]);
                    eulerAngles.Angles.Y = Math.Atan2(-hM[p.Z, p.X], cy);
                    eulerAngles.Angles.Z = Math.Atan2( hM[p.Y, p.X], hM[p.X, p.X]);
                }
                else
                {
                    eulerAngles.Angles.X = Math.Atan2(-hM[p.Y, p.Z], hM[p.Y, p.Y]);
                    eulerAngles.Angles.Y = Math.Atan2(-hM[p.Z, p.X], cy);
                    eulerAngles.Angles.Z = 0;
                }
            }

            if (order.AxisPermutation == EulerOrder.Parity.Odd)
            {
                eulerAngles.Angles.X = -eulerAngles.Angles.X;
                eulerAngles.Angles.Y = -eulerAngles.Angles.Y;
                eulerAngles.Angles.Z = -eulerAngles.Angles.Z;
            }

            if (order.FrameTaken == EulerOrder.AxisFrame.Rotating)
            {
                double temp = eulerAngles.Angles.X;
                eulerAngles.Angles.X = eulerAngles.Angles.Z;
                eulerAngles.Angles.Z = temp;
            }
            
            eulerAngles.Angles.X = Misc.RadiansToDegrees(eulerAngles.Angles.X);
            eulerAngles.Angles.Y = Misc.RadiansToDegrees(eulerAngles.Angles.Y);
            eulerAngles.Angles.Z = Misc.RadiansToDegrees(eulerAngles.Angles.Z);

            return eulerAngles;
        }

        public static EulerAngles QuaternionToEuler(Quaternion q, EulerOrder order)
        {
            double norm = q.Normal();
            double scale = norm > 0.0 ? 2.0 / norm : 0.0;
            HMatrix hM = new HMatrix();

            Vector s = new Vector(q.X * scale, q.Y * scale, q.Z * scale);
            Vector w = new Vector(q.W * s.X,   q.W * s.Y,   q.W * s.Z  );
            Vector x = new Vector(q.X * s.X,   q.X * s.Y,   q.X * s.Z  );
            Vector y = new Vector(0,           q.Y * s.Y,   q.Y * s.Z  );
            Vector z = new Vector(0,           0,           q.Z * s.Z  );

            //0X, 1Y, 2Z, 3W
            hM[0, 0] = 1.0 - y.Y + z.Z;     hM[0, 1] = x.Y - w.Z;           hM[0, 2] = x.Z + w.Y;           hM[0, 3] = 0;
            hM[1, 0] = x.Y + w.Z;           hM[1, 1] = 1.0 - x.X + z.Z;     hM[1, 2] = y.Z - w.X;           hM[1, 3] = 0;
            hM[2, 0] = x.Z - w.Y;           hM[2, 1] = y.Z + w.X;           hM[2, 2] = 1.0 - x.X + y.Y;     hM[2, 3] = 0;
            hM[3, 0] = 0;                   hM[3, 1] = 0;                   hM[3, 2] = 0;                   hM[3, 3] = 1;

            return HMatrixToEuler(hM, order);
        }
    }
}
