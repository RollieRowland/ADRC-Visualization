using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    class Turbulence
    {
        /// <summary>
        /// Change to use consistent scaling in 3d, not 3 separate linear turbulence
        /// </summary>
       
        private VectorKalmanFilter turbulenceScale;
        private Vector output;
        private Random random;
        private int max;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="max">Maximum and Minimum outputs of the turbulence.</param>
        /// <param name="scale">Determines the smoothing of the turbulence over time.</param>
        public Turbulence(int max, int scale)
        {
            turbulenceScale = new VectorKalmanFilter(0.5, scale);
            output = new Vector(0, 0, 0);
            random = new Random(999);
            
            this.max = max;
        }
        
        /// <summary>
        /// Calculates a new iteration of the turbulence output
        /// </summary>
        /// <returns></returns>
        public Vector CalculateTurbulence()
        {
            //returns value between -1 and 1 and scaled
            output.X = ((random.NextDouble() * 2) - 1) * max;
            output.Y = ((random.NextDouble() * 2) - 1) * max;
            output.Z = ((random.NextDouble() * 2) - 1) * max;

            turbulenceScale.Filter(output);

            return output;
        }
    }
}
