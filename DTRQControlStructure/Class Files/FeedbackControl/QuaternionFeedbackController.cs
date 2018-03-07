using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualization.Class_Files.FeedbackControl
{
    class QuaternionFeedbackController
    {
        public FeedbackController W { get; set; }
        public FeedbackController X { get; set; }
        public FeedbackController Y { get; set; }
        public FeedbackController Z { get; set; }
        private Quaternion output;

        public QuaternionFeedbackController()
        {
            output = new Quaternion(0, 0, 0, 0);
        }

        public QuaternionFeedbackController(FeedbackController W, FeedbackController X, FeedbackController Y, FeedbackController Z)
        {
            this.W = W;
            this.X = X;
            this.Y = Y;
            this.Z = Z;

            output = new Quaternion(0, 0, 0, 0);
        }

        public Quaternion Calculate(Quaternion setPoint, Quaternion processVariable)
        {
            output.W = W.Calculate(setPoint.W, processVariable.W);
            output.X = X.Calculate(setPoint.X, processVariable.X);
            output.Y = Y.Calculate(setPoint.Y, processVariable.Y);
            output.Z = Z.Calculate(setPoint.Z, processVariable.Z);

            return output;
        }

        public Quaternion Calculate(Quaternion setPoint, Quaternion processVariable, double samplingPeriod)
        {
            output.W = W.Calculate(setPoint.W, processVariable.W, samplingPeriod);
            output.X = X.Calculate(setPoint.X, processVariable.X, samplingPeriod);
            output.Y = Y.Calculate(setPoint.Y, processVariable.Y, samplingPeriod);
            output.Z = Z.Calculate(setPoint.Z, processVariable.Z, samplingPeriod);

            return output;
        }
    }
}
