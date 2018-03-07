using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files
{
    //NLC
    public class NonlinearCombiner
    {
        /*
            //INPUTS: e1, e2
            //OUTPUTS: u0

            // Nonlinear combination of errors and differential errors
            // where c, r, and h1 are positive parameters

            e1 = v1 − z1
            e2 = v2 − z2

            u0 = −fhan(e1, c * e2, r, h1)

            // Disturbance rejection
            // where b0 is a positive parameter

            u = u0 − z3 / b0

            
        */

        private double amplificationCoefficient;//r
        private double dampingCoefficient;//c

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amplificationCoefficient">Corresponds to the limit of acceleration.</param>
        /// <param name="dampingCoefficient">Damping coefficient to be adjusted in the neighborhood of unity.</param>
        public NonlinearCombiner(double amplificationCoefficient, double dampingCoefficient)
        {
            this.amplificationCoefficient = amplificationCoefficient;
            this.dampingCoefficient = dampingCoefficient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trajectory">v1,v2</param>
        /// <param name="b0">Estimate of coefficient b within +-50%</param>
        /// <param name="eso">Extended State Observer</param>
        /// <returns></returns>
        public double Combine(Tuple<double, double> td, double b0, Tuple<double, double, double> eso, double precisionCoefficient)
        {
            double e1, e2, u0;

            e1 = td.Item1 - eso.Item1;
            e2 = td.Item2 - eso.Item2;

            u0 = -SetPointJumpPrevention(e1, dampingCoefficient * e2, amplificationCoefficient, precisionCoefficient);

            //Contains disturbance rejection
            return (u0 + eso.Item3) / b0;// b0 must be positive
        }


        /*
            // fhan function is denoted below
            d = r * h
            d0 = h * d
            y = x1 + h * x2

            a0 = sqrt(d2 + 8 * r * abs(y))

            a = {
                x2 + (a0−d) / 2 * sign(y),  if abs(y) > d0
                xx2 + yh,                   if abs(y) <= d0

            fhan = {
                −r * sign(a),               if abs(a) > d
                -r * a / d,                 if abs(a) <= d

        */
        public double SetPointJumpPrevention(double target, double targetDerivative, double r0, double h)//Get actual name of function,  setpoint jump prevention
        {
            double d, a, a0, a1, a2, y, sy, sa;

            d = Math.Pow(r0, 2) * h;
            a0 = h * targetDerivative;
            y = target + a0;

            a1 = Math.Sqrt(d * (d + 8 * Math.Abs(y)));
            a2 = a0 + Math.Sign(y) * (a1 - d) / 2;
            sy = (Math.Sign(y + d) - Math.Sign(y - d)) / 2;//returns 1, or -1

            a = (a0 + y - a2) * sy + a2;
            sa = (Math.Sign(a + d) - Math.Sign(a - d)) / 2;//returns 1, or -1

            return -r0 * ((a / d) - Math.Sign(a)) * sa - r0 * Math.Sign(a);
        }
    }
}
