using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualization.Class_Files.QuadcopterSimulation
{
    class Servo
    {
        private double SetPoint;
        private Acceleration acceleration;

        public Servo(double dT, double springConstant, double mass)
        {
            SetPoint = 0;
            acceleration = new Acceleration(dT, springConstant, mass);
        }

        public void SetAngle(double SetPoint)
        {
            acceleration.Accelerate(SetPoint);

            this.SetPoint = acceleration.GetVelocity();
        }

        public double GetAngle()
        {
            return SetPoint;
        }
    }
}
