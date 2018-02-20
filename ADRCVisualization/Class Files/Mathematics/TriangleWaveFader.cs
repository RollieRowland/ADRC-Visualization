using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    class TriangleWaveFader
    {
        private double curvature;
        private double amplitude;
        
        /// <summary>
        /// Initializes the triangle wave function, values return between 0 and 1.
        /// </summary>
        /// <param name="curvature">Rate of increase in change. 0.5: |‾ , 1: /, 2, _|</param>
        /// <param name="amplitude">The amplitude that the input value will oscillate between.</param>
        public TriangleWaveFader(double curvature, double amplitude)
        {
            this.curvature = curvature;
            this.amplitude = amplitude;
        }

        /// <summary>
        /// Calculates the amplitude of the wave given the point on the x axis.
        /// </summary>
        /// <param name="value">Point on the x axis.</param>
        /// <returns>Returns the amplitude ratio.</returns>
        public double CalculateRatio(double value)
        {
            return (1.0 / amplitude) * Math.Pow((amplitude - Math.Abs(value % (amplitude * 2.0) - amplitude)), curvature) / Math.Pow(amplitude, curvature - 1.0);
        }

        /// <summary>
        /// Calculates the inverse of the amplitude of the wave given on the point on the x axis.
        /// </summary>
        /// <param name="value">Point on the x axis.</param>
        /// <returns>Returns the inverse of the amplitude.</returns>
        public double CalculateInverseRatio(double value)
        {
            return 1 - CalculateRatio(value);
        }
    }
}
