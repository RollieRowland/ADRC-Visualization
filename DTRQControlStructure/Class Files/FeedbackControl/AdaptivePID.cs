using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.FeedbackControl
{
    class AdaptivePID : FeedbackController
    {
        private PID pid;
        private FeedbackController proportional;
        private FeedbackController integral;
        private FeedbackController derivative;

        private KalmanFilter proportionalKalmanFilter;
        private KalmanFilter derivativeKalmanFilter;

        private double min;
        private double max;
        private double maxOutput;

        public AdaptivePID(double P, double I, double D, double maxOutput)
        {
            pid          = new PID(0, 0, 0, 45);
            proportional = new PID(25, 0, 0, 45);
            integral     = new PID(1, 0, 0, 45);
            derivative   = new PID(0.25, 0, 0, 45);

            proportional = new ADRC(2000, 2.5, 0.05, 0.005, maxOutput)
            {
                PID = new PID(P, 0, 0, maxOutput)
            };

            integral = new ADRC(2000, 2.5, 0.05, 0.005, maxOutput)
            {
                PID = new PID(I, 0, 0, maxOutput)
            };

            derivative = new ADRC(2000, 2.5, 0.05, 0.005, maxOutput)
            {
                PID = new PID(D, 0, 0, maxOutput)
            };

            proportionalKalmanFilter = new KalmanFilter(0.125, 20);
            derivativeKalmanFilter = new KalmanFilter(0.5, 10);

            this.maxOutput = maxOutput;
        }

        public override double Calculate(double setPoint, double processVariable)
        {
            pid.KP = EstimateProportionalOffset(setPoint, processVariable);
            //pid.KI = EstimateIntegralOffset(setPoint, processVariable);
            //pid.KD = EstimateProportionalOffset(setPoint, processVariable);

            return pid.Calculate(setPoint, processVariable);
        }
        public override double Calculate(double setPoint, double processVariable, double samplingPeriod)
        {
            pid.KP = EstimateProportionalOffset(setPoint, processVariable);
            //pid.KI = EstimateIntegralOffset(setPoint, processVariable);
            //pid.KD = EstimateProportionalOffset(setPoint, processVariable);

            return pid.Calculate(setPoint, processVariable);
        }

        private double EstimateProportionalOffset(double setPoint, double processVariable)
        {
            //average offset from setpoint
            //use KF to smooth pv

            //actual power output from the system vs what is expected

            double stepChange = proportionalKalmanFilter.Filter(Math.Abs(setPoint - processVariable));
            
            return Math.Abs(proportional.Calculate(0, stepChange));
        }

        private double EstimateIntegralOffset(double setPoint, double processVariable)
        {
            //time between when the system was told to output and when it actually output
            throw new NotImplementedException();
        }

        private double EstimateDerivativeOffset(double setPoint, double processVariable)
        {
            //maintain memory of offset from setPoint, check for oscillation
            //time for the process variable to reach 63% (1 - 1/e) of its target value

            double oscillation = derivativeKalmanFilter.Filter(setPoint - processVariable);

            return derivative.Calculate(0, oscillation);
        }

        public override string SetOffset(double offset)
        {
            min = -maxOutput + offset;
            max = maxOutput + offset;

            return min + " " + max;
        }
    }
}
