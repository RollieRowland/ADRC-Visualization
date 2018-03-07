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
        public AxisRepitition InitialAxisRepitition { get; }
        public AxisFrame FrameTaken { get; }
        public Vector Permutation { get; }

        public EulerOrder(Axis axis, Parity parity, AxisRepitition axisRepitition, AxisFrame axisFrame, Vector permutation)
        {
            InitialAxis = axis;
            AxisPermutation = parity;
            InitialAxisRepitition = axisRepitition;
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

        public enum AxisRepitition
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
