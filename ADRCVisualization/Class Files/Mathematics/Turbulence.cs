using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    class Turbulence
    {
        private VectorKalmanFilter turbulenceScale;
        private Vector output;
        private Random random;
        private int min;
        private int max;

        public Turbulence(int min, int max, int scale)
        {
            turbulenceScale = new VectorKalmanFilter(0.5, scale);
            output = new Vector(0, 0, 0);
            random = new Random(999);

            this.min = min;
            this.max = max;
        }
        /*
        public Vector CalculateTurbulence()
        {
            output.X = random.NextDouble() * max;
        }
        */
    }
}
