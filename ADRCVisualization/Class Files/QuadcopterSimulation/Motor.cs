﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.QuadcopterSimulation
{
    class Motor
    {
        private double SetPoint;

        public void SetOutput(double SetPoint)
        {
            this.SetPoint = SetPoint;
        }

        public double GetOutput()
        {
            return SetPoint;
        }
    }
}
