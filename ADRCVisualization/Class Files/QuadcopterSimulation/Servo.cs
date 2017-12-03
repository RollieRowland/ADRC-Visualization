using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.QuadcopterSimulation
{
    class Servo
    {
        private double SetPoint;

        public void SetAngle(double SetPoint)
        {
            this.SetPoint = SetPoint;
        }

        public double GetAngle()
        {
            return SetPoint;
        }
    }
}
