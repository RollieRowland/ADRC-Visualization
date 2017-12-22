using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualization.Class_Files
{
    public class VectorKalmanFilter
    {
        private Vector filteredValue;

        private KalmanFilter X;
        private KalmanFilter Y;
        private KalmanFilter Z;

        /// <summary>
        /// Implementation of a kalman filter. Commonly used to smooth output of noisy functions
        /// </summary>
        /// <param name="gain">Kalman Gain</param>
        public VectorKalmanFilter(double gain, int memory)
        {
            X = new KalmanFilter(gain, memory);
            Y = new KalmanFilter(gain, memory);
            Z = new KalmanFilter(gain, memory);

            filteredValue = new Vector(0, 0, 0);
        }

        /// <summary>
        /// Implementation of a kalman filter. Commonly used to smooth output of noisy functions
        /// </summary>
        /// <param name="gain">Kalman Gain</param>
        public VectorKalmanFilter(Vector gain, Vector memory)
        {
            X = new KalmanFilter(gain.X, (int)memory.X);
            Y = new KalmanFilter(gain.Y, (int)memory.Y);
            Z = new KalmanFilter(gain.Z, (int)memory.Z);

            filteredValue = new Vector(0, 0, 0);
        }

        /// <summary>
        /// Filters the input variable(s)
        /// </summary>
        /// <param name="values">Allows input of a single or multiple input values to be added to the filter</param>
        /// <returns>Returns filtered value</returns>
        public Vector Filter(Vector input)
        {
            filteredValue.X = X.Filter(input.X);
            filteredValue.Y = Y.Filter(input.Y);
            filteredValue.Z = Z.Filter(input.Z);

            return filteredValue;
        }

        public Vector GetFilteredValue()
        {
            return filteredValue;
        }
    }
}
