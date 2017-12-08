using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    class VectorPID
    {
        private PID X;
        private PID Y;
        private PID Z;
        private Vector output;

        public VectorPID(double kP, double kI, double kD, double maxOutput)
        {
            X = new PID(kP, kI, kD, maxOutput);
            Y = new PID(kP, kI, kD, maxOutput);
            Z = new PID(kP, kI, kD, maxOutput);

            output = new Vector(0, 0, 0);
        }
        
        public VectorPID(Vector kP, Vector kI, Vector kD, Vector maxOutput)
        {
            X = new PID(kP.X, kI.X, kD.X, maxOutput.X);
            Y = new PID(kP.Y, kI.Y, kD.Y, maxOutput.Y);
            Z = new PID(kP.Z, kI.Z, kD.Z, maxOutput.Z);

            output = new Vector(0, 0, 0);
        }

        public Vector Calculate(Vector setPoint, Vector processVariable)
        {
            output.X = X.Calculate(setPoint.X, processVariable.X);
            output.Y = Y.Calculate(setPoint.Y, processVariable.Y);
            output.Z = Z.Calculate(setPoint.Z, processVariable.Z);

            return output;
        }
    }
}
