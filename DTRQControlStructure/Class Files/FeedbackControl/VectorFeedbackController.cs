using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualization.Class_Files.FeedbackControl
{
    class VectorFeedbackController
    {
        public FeedbackController X { get; set; }
        public FeedbackController Y { get; set; }
        public FeedbackController Z { get; set; }
        private Vector output;

        public VectorFeedbackController()
        {
            output = new Vector(0, 0, 0);
        }

        public VectorFeedbackController(FeedbackController X, FeedbackController Y, FeedbackController Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;

            output = new Vector(0, 0, 0);
        }

        public Vector Calculate(Vector setPoint, Vector processVariable)
        {
            output.X = X.Calculate(setPoint.X, processVariable.X);
            output.Y = Y.Calculate(setPoint.Y, processVariable.Y);
            output.Z = Z.Calculate(setPoint.Z, processVariable.Z);

            return output;
        }

        public Vector Calculate(Vector setPoint, Vector processVariable, double samplingPeriod)
        {
            output.X = X.Calculate(setPoint.X, processVariable.X, samplingPeriod);
            output.Y = Y.Calculate(setPoint.Y, processVariable.Y, samplingPeriod);
            output.Z = Z.Calculate(setPoint.Z, processVariable.Z, samplingPeriod);

            return output;
        }
    }
}
