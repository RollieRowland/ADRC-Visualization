using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    public class SphericalCoordinate
    {
        public double Radius { get; set; }//Radius of sphere
        public double Theta { get; set; }//Rotation on XZ plane about Y axis
        public double Phi { get; set; }//Rotation on Y -XZ axis about X axis

        /// <summary>
        /// Creates a unit spherical coordinate from designated coordinates.
        /// </summary>
        /// <param name="Theta">Rotation about XZ plane</param>
        /// <param name="Phi">Rotation away from Y axis</param>
        public SphericalCoordinate(double Theta, double Phi)
        {
            Radius = 1;
            this.Theta = Theta;
            this.Phi = Phi;
        }

        /// <summary>
        /// Creates a spherical coordinate from designated coordinates.
        /// </summary>
        /// <param name="Radius">Radius of sphere</param>
        /// <param name="Theta">Rotation about XZ plane</param>
        /// <param name="Phi">Rotation away from Y axis</param>
        public SphericalCoordinate(double Radius, double Theta, double Phi)
        {
            this.Radius = Radius;
            this.Theta = Theta;
            this.Phi = Phi;
        }

        /// <summary>
        /// Creates a deep copy of the coordinate value.
        /// </summary>
        /// <param name="sphericalCoordinate">Input spherical coordinate to be copied.</param>
        public SphericalCoordinate(SphericalCoordinate sphericalCoordinate)
        {
            Radius = sphericalCoordinate.Radius;
            Theta = sphericalCoordinate.Theta;
            Phi = sphericalCoordinate.Phi;
        }

        /// <summary>
        /// Creates a spherical coordinate from a cartesian coordinate (or direction vector)
        /// </summary>
        /// <param name="cartesianCoordinate">Vector coordinate used to calculate the spherical coordinate parameters</param>
        public static SphericalCoordinate VectorToSphericalCoordinate(Vector cartesianCoordinate)
        {
            double radius = Math.Sqrt(Math.Pow(cartesianCoordinate.X, 2) + Math.Pow(cartesianCoordinate.Y, 2) + Math.Pow(cartesianCoordinate.Z, 2));

            return new SphericalCoordinate(0, 0, 0)
            {
                Radius = radius,
                Theta = MathE.RadiansToDegrees(Math.Atan2(cartesianCoordinate.Z, cartesianCoordinate.X)) - 90,//azimuthal rotation, Z/X, adjust as forward z being zero
                Phi = MathE.RadiansToDegrees(Math.Acos(cartesianCoordinate.Y / radius))//polar rotation, Y/R
            };
        }

        /// <summary>
        /// Gives a clean output for a spherical coordinate in a string.
        /// </summary>
        /// <returns>String value of spherical coordinate.</returns>
        public override string ToString()
        {
            string radius = MathE.DoubleToCleanString(Radius);
            string theta = MathE.DoubleToCleanString(Theta);
            string phi = MathE.DoubleToCleanString(Phi);

            return "[" + radius + " " + theta + " " + phi + "]";
        }
    }
}
