using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    public class EulerOrder
    {
        public Axis InitialAxis { get; }
        public Parity AxisPermutation { get; }
        public AxisRepetition InitialAxisRepetition { get; }
        public AxisFrame FrameTaken { get; }
        public Vector Permutation { get; }

        public EulerOrder(Axis axis, Parity parity, AxisRepetition axisRepetition, AxisFrame axisFrame, Vector permutation)
        {
            InitialAxis = axis;
            AxisPermutation = parity;
            InitialAxisRepetition = axisRepetition;
            FrameTaken = axisFrame;
            Permutation = permutation;
        }

        public enum Axis
        {
            X,
            Y,
            Z
        };

        public enum Parity
        {
            Even,
            Odd
        };

        public enum AxisRepetition
        {
            Yes,
            No
        };

        public enum AxisFrame
        {
            Static,
            Rotating
        };
    }
}
