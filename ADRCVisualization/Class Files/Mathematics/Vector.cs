using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public static readonly Vector ORIGIN = new Vector(0, 0, 0);

        public Vector(double x, double y, double z)
        {
            X = x; Y = y; Z = z;
        }

        public static Vector Normal(Vector vector)
        {
            double mult = Magnitude(vector) == 0 ? Double.PositiveInfinity : 1 / Magnitude(vector);

            return Multiply(mult, vector);
        }

        public static bool IsEqual(Vector vectorO, Vector vectorT)
        {
            return (vectorO.X == vectorT.X) && (vectorO.Y == vectorT.Y) && (vectorO.Z == vectorT.Z);
        }

        public static Vector Multiply(double k, Vector vector)
        {
            return new Vector((vector.X * k), (vector.Y * k), (vector.Z * k));
        }

        public static Vector Multiply(Vector vectorO, Vector vectorT)
        {
            return new Vector((vectorO.X * vectorT.X), (vectorO.Y * vectorT.Y), (vectorO.Z * vectorT.Z));
        }
        
        public static Vector Multiply(double k, double i)
        {
            return new Vector((k * i), (k * i), (k * i));
        }

        public Vector Multiply(double k)
        {
            return new Vector((X * k), (Y * k), (Z * k));
        }

        public static Vector Divide(double k, Vector vector)
        {
            return new Vector((vector.X / k), (vector.Y / k), (vector.Z / k));
        }

        public static Vector Divide(Vector vectorO, Vector vectorT)
        {
            return new Vector((vectorT.X / vectorO.X), (vectorT.Y / vectorO.Y), (vectorT.Z / vectorO.Z));
        }

        public static Vector Subtract(Vector vectorO, Vector vectorT)
        {
            return new Vector(vectorO.X - vectorT.X, vectorO.Y - vectorT.Y, vectorO.Z - vectorT.Z);
        }

        public static Vector Add(Vector vectorO, Vector vectorT)
        {
            return new Vector(vectorO.X + vectorT.X, vectorO.Y + vectorT.Y, vectorO.Z + vectorT.Z);
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
    }
}

/*
public class Vec {
    public final static Vec X_AXIS=new Vec(1,0,0);
    public final static Vec Y_AXIS=new Vec(0,1,0);
    public final static Vec Z_AXIS=new Vec(0,0,1);
    public final static Vec ORIGIN=new Vec(0,0,0);
 
    public final double x,y,z;
   
    public Vec(double x, double y, double z) {
        this.x=x;
        this.y=y;
        this.z=z;
    }
   
    public double dotProduct(Vec v) {
        return x*v.x+y*v.y+z*v.z;
    }
   
    public Vec crossProduct(Vec v) {
        double rx=y*v.z-z*v.y;
        double ry=z*v.x-x*v.z;
        double rz=x*v.y-y*v.x;
        return new Vec(rx, ry, rz);
    }
   
    public Vec newLength(double newLength) {
        double length=getLength();
        if(length==newLength) return this;
        if(length==0) return X_AXIS.newLength(newLength);
        return new Vec(x*newLength/length, y*newLength/length, z*newLength/length);
    }
   
    public Vec rotationAxis() {
        return rotationAxis(X_AXIS);
    }
   
    //The rotation axis to rotate v onto this
    public Vec rotationAxis(Vec v) {
        return normalizedCrossProduct(v);
    }
   
    public Vec normalizedCrossProduct(Vec v) {
        Vec r=crossProduct(v);
        if(r.getLength()<0.0001) {
            r=crossProduct(X_AXIS);
        }
        if(r.getLength()<0.0001) {
            r=crossProduct(Y_AXIS);
        }
        if(r.getLength()<0.0001) {
            return X_AXIS;
        }
        return r.normalize();
    }
   
    public double angle() {
        return angleTo(X_AXIS);
    }
   
    public double angleTo(Vec v) {
        double cosTheta=dotProduct(v)/(getLength()*v.getLength());
       
        return Math.acos(cosTheta);
    }
   
    public double getLength() {
        return Math.sqrt(x*x+y*y+z*z);
    }        
   
   
    public Vec rotate(Matrix m) {
        return m.mul(this);
    }
   
    public Vec normalize() {
        double length=getLength();
       
        if(length==1) return this;
        if(length==0) return X_AXIS;
        return new Vec(x/length, y/length, z/length);
    }
   
    public Vec addX(double a) {
        return new Vec(x+a, y, z);
    }
   
    public Vec addY(double a) {
        return new Vec(x, y+a, z);
    }
   
    public Vec addZ(double a) {
        return new Vec(x, y, z+a);
    }
   
    public Vec add(Vec v) {
        return new Vec(x+v.x, y+v.y, z+v.z);
    }
           
    public Vec sub(Vec v) {
        return new Vec(x-v.x, y-v.y, z-v.z);
    }
   
    public Vec mul(double m) {
        return new Vec(x*m, y*m, z*m);
    }
   
    public Vec div(double d) {
        return new Vec(x/d, y/d, z/d);
    }
   
    public Vec neg() {
        return new Vec(-x, -y, -z);
    }
   
    @Override
    public boolean equals(Object o) {
        if(!(o instanceof Vec)) return false;
        Vec v=(Vec)o;
        return v==this || sub(v).getLength() < 0.0001;
    }
   
    public String toString() {
    return String.format(java.util.Locale.ENGLISH,
        "(%.3f, %.3f, %.3f)", x, y, z);
    }
   
}

*/
