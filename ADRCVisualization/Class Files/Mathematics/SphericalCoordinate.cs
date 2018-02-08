using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    class SphericalCoordinate
    {
        public double Radius { get; set; }//Radius of sphere
        public double Theta { get; set; }//Rotation on XZ plane about Y axis
        public double Phi { get; set; }//Rotation on Y -XZ axis about X axis

        /// <summary>
        /// Creates a spherical coordinate from designated coordinates.
        /// </summary>
        /// <param name="Radius">Radius of sphere</param>
        /// <param name="Theta">Rotation about XZ plane</param>
        /// <param name="Phi">Rotation away from Y axis</param>
        public SphericalCoordinate(double Radius, double Theta, double Phi)
        {
            this.Radius = Radius;
            this.Theta  = Theta;
            this.Phi    = Phi;
        }

        /// <summary>
        /// Creates a spherical coordinate from a cartesian coordinate (or direction vector)
        /// </summary>
        /// <param name="cartesianCoordinate">Vector coordinate used to calculate the spherical coordinate parameters</param>
        public SphericalCoordinate(Vector cartesianCoordinate)
        {
            Radius = Math.Sqrt(Math.Pow(cartesianCoordinate.X, 2) + Math.Pow(cartesianCoordinate.Y, 2) + Math.Pow(cartesianCoordinate.Z, 2));
            Theta = Misc.RadiansToDegrees(Math.Atan2(cartesianCoordinate.Z, cartesianCoordinate.X)) - 90;//azimuthal rotation, Z/X, adjust as forward z being zero
            Phi = Misc.RadiansToDegrees(Math.Acos(cartesianCoordinate.Y / Radius));//polar rotation, Y/R
            /*
            if (Math.Abs((int)Theta) == 180)
            {
                Theta = 0;
            }
            */
        }
    }
}
