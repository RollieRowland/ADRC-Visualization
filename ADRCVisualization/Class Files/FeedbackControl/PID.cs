using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.Mathematics;
using ADRCVisualization.Class_Files.FeedbackControl;

namespace ADRCVisualization.Class_Files
{
    class PID : FeedbackController
    {
        public double KP { get; set; }
        public double KI { get; set; }
        public double KD { get; set; }

        private double MaxOutput;
        private double integral;
        private double error;
        private double previousError;
        private double output;
        private DateTime time;

        private double min;
        private double max;
        
        /// <summary>
        /// Initializes the PID.
        /// </summary>
        /// <param name="kp">Proportional gain</param>
        /// <param name="ki">Integral gain</param>
        /// <param name="kd">Derivative gain</param>
        /// <param name="maxOutput">Maximum output for constraint</param>
        public PID(double kp, double ki, double kd, double maxOutput)
        {
            KP = kp;
            KI = ki;
            KD = kd;
            this.MaxOutput = maxOutput;

            time = DateTime.Now;
            min = -maxOutput;
            max = maxOutput;
        }

        /// <summary>
        /// Calculates the setpoint with an automatically set sampling period.
        /// </summary>
        /// <param name="setpoint">Target</param>
        /// <param name="processVariable">Actual</param>
        /// <returns>Returns output of PID</returns>
        public override double Calculate(double setpoint, double processVariable)
        {
            double POut, IOut, DOut, dt;
            
            DateTime currentTime = DateTime.Now;
            dt = currentTime.Subtract(time).TotalSeconds;

            if (dt > 0)
            {
                error = setpoint - processVariable;

                POut = KP * error;

                integral += error * dt;
                IOut = KI * integral;

                DOut = KD * ((error - previousError) / dt);

                output = MathE.Constrain(POut + IOut + DOut, min, max);

                time = currentTime;
                previousError = error;
            }

            return output;
        }

        /// <summary>
        /// Calculates the setpoint with a custom sampling period.
        /// </summary>
        /// <param name="setpoint">Target</param>
        /// <param name="processVariable">Actual</param>
        /// <param name="samplingPeriod">Period between calculations</param>
        /// <returns>Returns output of PID</returns>
        public override double Calculate(double setpoint, double processVariable, double samplingPeriod)
        {
            double POut, IOut, DOut;

            if (samplingPeriod > 0)
            {
                error = setpoint - processVariable;

                POut = KP * error;

                integral += error * samplingPeriod;
                IOut = KI * integral;

                DOut = KD * ((error - previousError) / samplingPeriod);

                output = MathE.Constrain(POut + IOut + DOut, min, max);
                
                previousError = error;
            }

            return output;
        }
        
        public override string SetOffset(double offset)
        {
            min = -MaxOutput + offset;
            max = MaxOutput + offset;

            return min + " " + max;
        }
    }
}
