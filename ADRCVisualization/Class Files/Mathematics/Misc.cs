using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    class Misc
    {
        /// <summary>
        /// Constrains the output of the input value to a maximum and minimum value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static double Constrain(double value, double minimum, double maximum)
        {
            if (value > maximum)
            {
                value = maximum;
            }
            else if (value < minimum)
            {
                value = minimum;
            }

            return value;
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees / (180 / Math.PI);
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        public static string DoubleToString(double value)
        {
            return String.Format("{0:0.00}", value).PadLeft(7);
        }
    }
}
