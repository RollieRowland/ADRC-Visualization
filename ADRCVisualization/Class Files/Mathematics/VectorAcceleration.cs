using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    class VectorAcceleration
    {
        private Acceleration X;
        private Acceleration Y;
        private Acceleration Z;

        public VectorAcceleration(double dT, double springConstant, double mass)
        {
            X = new Acceleration(dT, springConstant, mass);
            Y = new Acceleration(dT, springConstant, mass);
            Z = new Acceleration(dT, springConstant, mass);
        }

        public VectorAcceleration(Vector dT, Vector springConstant, Vector mass)
        {
            X = new Acceleration(dT.X, springConstant.X, mass.X);
            Y = new Acceleration(dT.Y, springConstant.Y, mass.Y);
            Z = new Acceleration(dT.Z, springConstant.Z, mass.Z);
        }

        public Vector Accelerate(Vector force)
        {
            return new Vector(0, 0, 0)
            {
                X = X.Accelerate(force.X),
                Y = Y.Accelerate(force.Y),
                Z = Z.Accelerate(force.Z)
            };
        }

        public Vector GetVelocities()
        {
            return new Vector(0, 0, 0)
            {
                X = X.GetVelocity(),
                Y = Y.GetVelocity(),
                Z = Z.GetVelocity()
            };
        }
    }
}
