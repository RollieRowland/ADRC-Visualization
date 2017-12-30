using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.Mathematics;
using ADRCVisualization.Class_Files.FeedbackControl;

namespace ADRCVisualization.Class_Files
{
    class ADRC : FeedbackController
    {
        public PID PID { get; set; }
        public double MaxOutput { get; set; }
        private ExtendedStateObserver ExtendedStateObserver;
        private NonlinearCombiner NonlinearCombiner;

        private DateTime dateTime;
        
        private double amplificationCoefficient;
        private double dampingCoefficient;
        private double precisionCoefficient;//0.2
        private double samplingPeriod;//0.05
        private double plantCoefficient;//b0 approximation
        private double precisionModifier;
        private double previousPD;
        private double output;
        private double min;
        private double max;

        /// <summary>
        /// ADRC implementation utilizing a PD controller in place of a tracking differentiator.
        /// </summary>
        /// <param name="amplificationCoefficient">R</param>
        /// <param name="dampingCoefficient">C</param>
        /// <param name="plantCoefficient">B</param>
        /// <param name="precisionModifier">H0</param>
        /// <param name="kp">P Gain</param>
        /// <param name="kd">D Gain</param>
        /// <param name="maxOutput">Constrained maximum output</param>
        public ADRC(double amplificationCoefficient, double dampingCoefficient, double plantCoefficient, double precisionModifier, double maxOutput)
        {
            this.amplificationCoefficient = amplificationCoefficient;
            this.dampingCoefficient = dampingCoefficient;
            this.plantCoefficient = plantCoefficient;
            this.precisionModifier = precisionModifier;
            this.MaxOutput = maxOutput;

            ExtendedStateObserver = new ExtendedStateObserver(false);
            NonlinearCombiner = new NonlinearCombiner(amplificationCoefficient, dampingCoefficient);

            dateTime = DateTime.Now;

            SetOffset(0);
        }

        /// <summary>
        /// ADRC implementation utilizing a PD controller in place of a tracking differentiator.
        /// </summary>
        /// <param name="amplificationCoefficient">R</param>
        /// <param name="dampingCoefficient">C</param>
        /// <param name="plantCoefficient">B</param>
        /// <param name="precisionModifier">H0</param>
        /// <param name="kp">P Gain</param>
        /// <param name="kd">D Gain</param>
        /// <param name="maxOutput">Constrained maximum output</param>
        public ADRC(double amplificationCoefficient, double dampingCoefficient, double plantCoefficient, double precisionModifier, PID pid, double maxOutput)
        {
            this.amplificationCoefficient = amplificationCoefficient;
            this.dampingCoefficient = dampingCoefficient;
            this.plantCoefficient = plantCoefficient;
            this.precisionModifier = precisionModifier;
            this.MaxOutput = maxOutput;
            this.PID = pid;

            ExtendedStateObserver = new ExtendedStateObserver(false);
            NonlinearCombiner = new NonlinearCombiner(amplificationCoefficient, dampingCoefficient);

            dateTime = DateTime.Now;

            SetOffset(0);
        }

        /// <summary>
        /// Calculates the output given the target value and actual value.
        /// </summary>
        /// <param name="setpoint">Target</param>
        /// <param name="processVariable">Actual</param>
        /// <returns></returns>
        public override double Calculate(double setpoint, double processVariable)
        {
            //samplingPeriod = DateTime.Now.Subtract(dateTime).TotalSeconds;

            samplingPeriod = 0.05;

            if (samplingPeriod > 0)
            {
                precisionCoefficient = samplingPeriod * precisionModifier;
                
                double pdValue = PID.Calculate(setpoint, processVariable, samplingPeriod);

                Tuple<double, double> pd = new Tuple<double, double>(pdValue, previousPD);
                Tuple<double, double, double> eso = ExtendedStateObserver.ObserveState(samplingPeriod, output, plantCoefficient, processVariable);//double u, double y, double b0

                output = NonlinearCombiner.Combine(pd, plantCoefficient, eso, precisionCoefficient);

                previousPD = pdValue;
                dateTime = DateTime.Now;
            }

            return Misc.Constrain(output, min, max);
        }

        public string SetOffset(double offset)
        {
            min = -MaxOutput + offset;
            max =  MaxOutput + offset;

            return min + " " + max;
        }
    }
}
