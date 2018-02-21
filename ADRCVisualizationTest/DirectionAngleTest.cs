using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualizationTest
{
    [TestClass]
    public class DirectionAngleTest
    {
        [TestMethod]
        public void TestDirectionAngleQuaternionDoubleConversion()
        {
            DirectionAngleQuaternionDoubleConversion(new Quaternion(0.9239,  0.3827,  0,       0     ));
            DirectionAngleQuaternionDoubleConversion(new Quaternion(0.9239, -0.3827,  0,       0     ));
            DirectionAngleQuaternionDoubleConversion(new Quaternion(0.9239,  0,       0.3827,  0     ));
            DirectionAngleQuaternionDoubleConversion(new Quaternion(0.9239,  0,      -0.3827,  0     ));
            DirectionAngleQuaternionDoubleConversion(new Quaternion(0.9239,  0,       0,       0.3827));
            DirectionAngleQuaternionDoubleConversion(new Quaternion(0.9239,  0,       0,      -0.3827));
            DirectionAngleQuaternionDoubleConversion(new Quaternion(0.6533,  0.2706,  0.6533,  0.2706));
            DirectionAngleQuaternionDoubleConversion(new Quaternion(0.6533, -0.2706,  0.6533, -0.2706));
            DirectionAngleQuaternionDoubleConversion(new Quaternion(0.6533, -0.2706, -0.6533,  0.2706));
        }

        public void DirectionAngleQuaternionDoubleConversion(Quaternion init)
        {
            DirectionAngle dA = DirectionAngle.QuaternionToDirectionAngle(init);
            Quaternion post = Quaternion.DirectionAngleToQuaternion(dA);

            Console.WriteLine(init + " |  " + post + " | " + (init - post));
            
            Assert.AreEqual(init.W, post.W, 0.01, "DAQC W" + (init - post));
            Assert.AreEqual(init.X, post.X, 0.01, "DAQC X" + (init - post));
            Assert.AreEqual(init.Y, post.Y, 0.01, "DAQC Y" + (init - post));
            Assert.AreEqual(init.Z, post.Z, 0.01, "DAQC Z" + (init - post));
        }

        [TestMethod]
        public void TestDirectionAngleToQuaternion()
        {
            Console.WriteLine("X Axis");

            for (int i = -180; i <= 180; i += 30)
            {
                for (int k = -180; k <= 180; k += 30)
                {
                    Console.WriteLine(DirectionAngle.QuaternionToDirectionAngle(Quaternion.EulerToQuaternion(new EulerAngles(new Vector(i, k, 0), EulerConstants.EulerOrderYXZS))));
                }
            }

            Console.WriteLine("Z Axis");

            for (int i = -180; i <= 180; i += 30)
            {
                for (int k = -180; k <= 180; k += 30)
                {
                    Console.WriteLine(DirectionAngle.QuaternionToDirectionAngle(Quaternion.EulerToQuaternion(new EulerAngles(new Vector(0, k, i), EulerConstants.EulerOrderYZXS))));
                }
            }

            Console.WriteLine("Test Cases");

            DirectionAngleQuaternion(new DirectionAngle(0,  new Vector(0, 0.707, 0.707)), new Quaternion(0.9239, 0.3827, 0, 0)); //XYZ: 45, 0, 0
            DirectionAngleQuaternion(new DirectionAngle(0,  new Vector(-0.707, 0.707, 0)), new Quaternion(0.9239, 0, 0, 0.3827));//XYZ: 0,   0,   45
            DirectionAngleQuaternion(new DirectionAngle(0,  new Vector(0.707, 0.707, 0)), new Quaternion(0.9239, 0, 0, -0.3827));//XYZ: 0, 0,   -45
            DirectionAngleQuaternion(new DirectionAngle(90, new Vector(0, 0.707, 0.707)), new Quaternion(0.6533, 0.2706, 0.6533, 0.2706));//YXZ: 0, 90, 45
            DirectionAngleQuaternion(new DirectionAngle(0,  new Vector(0, 1, 0)), new Quaternion(1, 0, 0, 0));//YXZ: 0, 90, 45
            DirectionAngleQuaternion(new DirectionAngle(0,  new Vector(0, -1, 0)), new Quaternion(0, 1, 0, 0));//YXZ: 0, 90, 45
        }

        public void DirectionAngleQuaternion(DirectionAngle expected, Quaternion q)
        {
            DirectionAngle nonStandardAA = DirectionAngle.QuaternionToDirectionAngle(q);

            Assert.AreEqual(expected.Rotation,    nonStandardAA.Rotation,    0.1, "Bad translation in R rotation " + nonStandardAA);
            Assert.AreEqual(expected.Direction.X, nonStandardAA.Direction.X, 0.05, "Bad translation in X dimension" + nonStandardAA);
            Assert.AreEqual(expected.Direction.Y, nonStandardAA.Direction.Y, 0.05, "Bad translation in Y dimension" + nonStandardAA);
            Assert.AreEqual(expected.Direction.Z, nonStandardAA.Direction.Z, 0.05, "Bad translation in Z dimension" + nonStandardAA);
        }
    }
}
