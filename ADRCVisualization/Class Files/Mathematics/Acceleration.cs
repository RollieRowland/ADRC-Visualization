using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    public class Acceleration
    {
        //critically damped spring
        private double dT;
        private double currentPosition = 0;
        private double currentVelocity = 0;
        private double springConstant;
        private double mass;
        
        public Acceleration(double dT, double springConstant, double mass)
        {
            this.dT = dT;
            this.springConstant = springConstant;
            this.mass = mass;
        }
        
        public double Accelerate(double target)
        {
            double currentToTarget = target - currentPosition;
            double springForce = currentToTarget * springConstant;
            double dampingForce = -currentVelocity * mass * Math.Sqrt(springConstant);
            double force = springForce + dampingForce;

            currentVelocity += force * dT;

            double displacement = currentVelocity * dT;

            currentPosition = currentPosition + displacement;

            return currentPosition;
        }

        public double GetVelocity()
        {
            return currentPosition;
        }
    }
}
