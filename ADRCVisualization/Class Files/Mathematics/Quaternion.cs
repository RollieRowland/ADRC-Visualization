using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    class Quaternion
    {
        public double W { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Quaternion(double W, double X, double Y, double Z)
        {
            this.W = W;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Quaternion(Vector vector)
        {
            W = 0;
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public static Quaternion FromEulerAngle(Vector vector)
        {
            /*
            //Euler angles to quaternion
            const Quaternion Quaternion::from_euler_rotation(float x, float y, float z) {
                float c1 = cos(y / 2);
                float c2 = cos(z / 2);
                float c3 = cos(x / 2);

                float s1 = sin(y / 2);
                float s2 = sin(z / 2);
                float s3 = sin(x / 2);
                Quaternion ret;
                ret.a = c1 * c2 * c3 - s1 * s2 * s3;
                ret.b = s1 * s2 * c3 + c1 * c2 * s3;
                ret.c = s1 * c2 * c3 + c1 * s2 * s3;
                ret.d = c1 * s2 * c3 - s1 * c2 * s3;
                return ret;
            }
            return new ;*/
            throw new NotImplementedException();
        }

        public static Vector RotateVector(Vector vector)
        {
            Quaternion quaternion;
            /*
            //Othogonal rotation is non-commutative

            // Rotation
            double w; // real part of quaternion
            double x; // imaginary i part of quaternion
            double y; // imaginary j part of quaternion
            double z; // imaginary k part of quaternion

        	p2.x = w*w*p1.x + 2*y*w*p1.z - 2*z*w*p1.y + x*x*p1.x + 2*y*x*p1.y + 2*z*x*p1.z - z*z*p1.x - y*y*p1.x;
	        p2.y = 2*x*y*p1.x + y*y*p1.y + 2*z*y*p1.z + 2*w*z*p1.x - z*z*p1.y + w*w*p1.y - 2*x*w*p1.z - x*x*p1.y;
	        p2.z = 2*x*z*p1.x + 2*y*z*p1.y + z*z*p1.z - 2*w*y*p1.x - y*y*p1.z + 2*w*x*p1.y - x*x*p1.z + w*w*p1.z;
            */

            throw new NotImplementedException();

            //return quaternion;
        }

        public static Quaternion Add(Quaternion quaternion)
        {
            /*
            quaternion
            quaternion_add(quaternion q1, quaternion q2)
            {
               return (quaternion) {
                  q1.w+q2.w,
                  q1.x+q2.x,
                  q1.y+q2.y,
                  q1.z+q2.z,
               };
            }*/
            throw new NotImplementedException();
        }

        public static Quaternion Subtract(Quaternion quaternion)
        {
            /*
            quaternion
            quaternion_subtract(quaternion q1, quaternion q2)
            {
               return (quaternion) {
                  q1.w-q2.w,
                  q1.x-q2.x,
                  q1.y-q2.y,
                  q1.z-q2.z,
               };
            }
             */
            throw new NotImplementedException();
        }

        public static Quaternion Multiply(double scale)
        {
            /*
            quaternion
            quaternion_multiply_scalar(quaternion q, double s)
            {
               return (quaternion) {s*q.w, s*q.x, s*q.y, s*q.z};
            }
             */
            throw new NotImplementedException();
        }

        public static Quaternion Multiply(Quaternion quaternion)
        {
            /*
            quaternion
            quaternion_multiply(quaternion q1, quaternion q2)
            {
               return (quaternion) {
                  q1.w*q2.w - q1.x*q2.x - q1.y*q2.y - q1.z*q2.z,
                  q1.w*q2.x + q1.x*q2.w + q1.y*q2.z - q1.z*q2.y,
                  q1.w*q2.y - q1.x*q2.z + q1.y*q2.w + q1.z*q2.x,
                  q1.w*q2.z + q1.x*q2.y - q1.y*q2.x + q1.z*q2.w,
               };
            }
            
            // Multiplication
            q.x = (q1.w * q2.x) + (q1.x * q2.w) + (q1.y * q2.z) - (q1.z * q2.y)
            q.y = (q1.w * q2.y) - (q1.x * q2.z) + (q1.y * q2.w) + (q1.z * q2.x)
            q.z = (q1.w * q2.z) + (q1.x * q2.y) - (q1.y * q2.x) + (q1.z * q2.w)
            q.w = (q1.w * q2.w) - (q1.x * q2.x) - (q1.y * q2.y) - (q1.z * q2.z)
            
             */
            throw new NotImplementedException();
        }

        public static Quaternion Divide(double scale)
        {
            /*
            quaternion
            quaternion_divide_scalar(quaternion q, double s)
            {
               return (quaternion) {q.w/s, q.x/s, q.y/s, q.z/s};
            }*/
            throw new NotImplementedException();
        }

        public static Quaternion Divide(Quaternion quaternion)
        {
            /*
            quaternion
            quaternion_divide(quaternion q1, quaternion q2)
            {
               double s = q2.w*q2.w + q2.x*q2.x + q2.y*q2.y + q2.z*q2.z;
               return (quaternion) {
                  (  q1.w*q2.w + q1.x*q2.x + q1.y*q2.y + q1.z*q2.z) / s,
                  (- q1.w*q2.x + q1.x*q2.w + q1.y*q2.z - q1.z*q2.y) / s,
                  (- q1.w*q2.y - q1.x*q2.z + q1.y*q2.w + q1.z*q2.x) / s,
                  (- q1.w*q2.z + q1.x*q2.y - q1.y*q2.x + q1.z*q2.w) / s
               };
            }*/
            throw new NotImplementedException();
        }

        public static Quaternion Absolute(Quaternion quaternion)
        {
            /*
            double
            quaternion_absolute(quaternion q)
            {
               return sqrt(q.w*q.w + q.x*q.x + q.y*q.y + q.z*q.z);
            }*/
            throw new NotImplementedException();
        }

        public static Quaternion Inverse(Quaternion quaternion)
        {
            /*
            quaternion
            quaternion_inverse(quaternion q)
            {
               return (quaternion) {-q.w, -q.x, -q.y, -q.z};
            }*/
            throw new NotImplementedException();
        }

        public static Quaternion Conjugate(Quaternion quaternion)
        {
            /*
            quaternion
            quaternion_conjugate(quaternion q)
            {
               return (quaternion) {q.w, -q.x, -q.y, -q.z};
            }*/
            throw new NotImplementedException();
        }

        public static Quaternion Power(Quaternion quaternion, Quaternion quaternionExp)
        {
            /*
            quaternion
            quaternion_power(quaternion q, quaternion p)
            {
               return quaternion_exp(quaternion_multiply(quaternion_log(q), p));
            }
            */
            throw new NotImplementedException();
        }

        public static Quaternion Normalize()
        {
            /*
            Quaternion & Quaternion::normalize() {
                float n = sqrt(a*a + b*b + c*c + d*d);
                a /= n;
                b /= n;
                c /= n;
                d /= n;
                return *this;
            }*/
            throw new NotImplementedException();
        }
        
        public static bool IsNaN(Quaternion quaternion)
        {
            /*
            int
            quaternion_isnan(quaternion q)
            {
                return isnan(q.w) || isnan(q.x) || isnan(q.y) || isnan(q.z);
            }*/
            throw new NotImplementedException();
        }

        public static bool IsFinite(Quaternion quaternion)
        {
            /*
            int
            quaternion_isfinite(quaternion q)
            {
                return isfinite(q.w) && isfinite(q.x) && isfinite(q.y) && isfinite(q.z);
            }*/
            throw new NotImplementedException();
        }

        public static bool IsInfinite(Quaternion quaternion)
        {
            /*
            int
            quaternion_isinf(quaternion q)
            {
                return isinf(q.w) || isinf(q.x) || isinf(q.y) || isnan(q.z);
            }*/
            throw new NotImplementedException();
        }

        public static bool IsNonZero(Quaternion quaternion)
        {
            /*
            int
            quaternion_isnonzero(quaternion q)
            {
                return q.w != 0 && q.x != 0 && q.y != 0 && q.z != 0;
            }*/
            throw new NotImplementedException();
        }

        public static bool IsEqual(Quaternion quaternionA, Quaternion quaternionB)
        {
            /*
            bool
            quaternion_equal(quaternion q1, quaternion q2)
            {
                return 
                    !quaternion_isnan(q1) &&
                    !quaternion_isnan(q2) &&
                    q1.w == q2.w && 
                    q1.x == q2.x && 
                    q1.y == q2.y && 
                    q1.z == q2.z;
            }*/
            throw new NotImplementedException();
        }


        /*

            
            
            





            


            
        */
    }
}
