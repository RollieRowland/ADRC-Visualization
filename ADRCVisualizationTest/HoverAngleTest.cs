using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADRCVisualization.Class_Files.Mathematics;
using ADRCVisualization.Class_Files;

namespace ADRCVisualizationTest
{
    [TestClass]
    public class HoverAngleTest
    {
        [TestMethod]
        public void TestHoverAngleConversions()
        {
            //rotation about direction axis, rotation forward, rotation about yaw
            for (int i = 0; i <= 360; i += 90)
            {
                for (int k = 0; k <= 180; k += 30)
                {
                    for (int j = -90; j <= 90; j += 15)
                    {
                        TestHoverAngles(new DirectionAngle(i, RotationMatrix.RotateVector(new Vector(k, j, 0), new Vector(0, 1, 0))));
                    }
                }
            }
        }

        public void TestHoverAngles(DirectionAngle dA)
        {
            Vector rotAngles = Quadcopter.RotationQuaternionToHoverAngles(Quaternion.DirectionAngleToQuaternion(dA));

            Console.WriteLine(dA + "    [" + MathE.DoubleToCleanString(rotAngles.X) + ", " + MathE.DoubleToCleanString(rotAngles.Z) + "]");
        }
    }
}
