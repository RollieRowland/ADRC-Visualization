using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualizationTest
{
    [TestClass]
    public class AxisAngleTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///  https://stackoverflow.com/questions/4786884/how-to-write-output-from-a-unit-test/4787047
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestMethod]
        public void BVATestMultipleAxisAngleQuatConversions()
        {
            //All
            TestAxisAngleQuatConversions(new AxisAngle(85.801, 0.679, 0.281, 0.679), new Quaternion(0.733, 0.462, 0.191, 0.462));//45 45 45
            
            //Two
            TestAxisAngleQuatConversions(new AxisAngle(62.799, 0.679, 0.679, 0.281), new Quaternion(0.854, 0.354, 0.354, 0.147));//45 45 0
            TestAxisAngleQuatConversions(new AxisAngle(62.799, 0.281, 0.679, 0.679), new Quaternion(0.854, 0.147, 0.354, 0.354));//0  45 45
            TestAxisAngleQuatConversions(new AxisAngle(62.799, 0.679, 0.281, 0.679), new Quaternion(0.854, 0.354, 0.147, 0.354));//45 0  45

            //One 
            TestAxisAngleQuatConversions(new AxisAngle(45,     0,     0,     1),     new Quaternion(0.924, 0,     0,     0.383));//0 0 45
            TestAxisAngleQuatConversions(new AxisAngle(45,     0,     1,     0),     new Quaternion(0.924, 0,     0.383, 0    ));//0 45 0
            TestAxisAngleQuatConversions(new AxisAngle(45,     1,     0,     0),     new Quaternion(0.924, 0.383, 0,     0    ));//45 0 0

            //Extreme values
            TestAxisAngleQuatConversions(new AxisAngle(0,      0,     1,     0),     new Quaternion(1,     0,     0,     0));//0   0   0 - standard is normalized to 0, [0, 1, 0]
            TestAxisAngleQuatConversions(new AxisAngle(180,    1,     0,     0),     new Quaternion(0,     1,     0,     0));//180 0   0 
            TestAxisAngleQuatConversions(new AxisAngle(180,    0,     1,     0),     new Quaternion(0,     0,     1,     0));//0   180 0 
            TestAxisAngleQuatConversions(new AxisAngle(180,    0,     0,     1),     new Quaternion(0,     0,     0,     1));//0   0   180 

            TestAxisAngleQuatConversions(new AxisAngle(180,   -1,     0,     0),     new Quaternion(0,    -1,     0,     0));//180 0   0 
            TestAxisAngleQuatConversions(new AxisAngle(180,    0,    -1,     0),     new Quaternion(0,     0,    -1,     0));//0   180 0 
            TestAxisAngleQuatConversions(new AxisAngle(180,    0,     0,    -1),     new Quaternion(0,     0,     0,    -1));//0   0   180 

            TestAxisAngleQuatConversions(new AxisAngle(360,    0,     1,     0),     new Quaternion(-1,    0,     0,     0));//360 0   0 
            TestAxisAngleQuatConversions(new AxisAngle(360,    0,     1,     0),     new Quaternion(-1,    0,     0,     0));//0   360 0 
            TestAxisAngleQuatConversions(new AxisAngle(360,    0,     1,     0),     new Quaternion(-1,    0,     0,     0));//0   0   360 

            //Possibly strange internal values
            TestAxisAngleQuatConversions(new AxisAngle(90,    1,     0,     0),     new Quaternion(0.707,  0.707,  0,      0    ));//90 0  0 
            TestAxisAngleQuatConversions(new AxisAngle(90,    0,     1,     0),     new Quaternion(0.707,  0,      0.707,  0    ));//0  90 0 
            TestAxisAngleQuatConversions(new AxisAngle(90,    0,     0,     1),     new Quaternion(0.707,  0,      0,      0.707));//0  0  90 

            TestAxisAngleQuatConversions(new AxisAngle(90,   -1,     0,     0),     new Quaternion(0.707, -0.707,  0,      0    ));//90 0  0 
            TestAxisAngleQuatConversions(new AxisAngle(90,    0,    -1,     0),     new Quaternion(0.707,  0,     -0.707,  0    ));//0  90 0 
            TestAxisAngleQuatConversions(new AxisAngle(90,    0,     0,    -1),     new Quaternion(0.707,  0,      0,     -0.707));//0  0  90
        }

        public void TestAxisAngleQuatConversions(AxisAngle aa, Quaternion q)
        {
            TestAxisAngleQuatConversion(aa, q);
            TestQuatAxisAngleConversion(aa, q);
        }

        public void TestQuatAxisAngleConversion(AxisAngle axisAngle, Quaternion q)
        {
            Quaternion quaternion = Quaternion.AxisAngleToQuaternion(axisAngle);

            testContextInstance.WriteLine(q + " | " + quaternion + " | " + q.Subtract(quaternion));

            Assert.AreEqual(q.W, quaternion.W, 0.05, "Bad translation in W dimension" + quaternion);
            Assert.AreEqual(q.X, quaternion.X, 0.05, "Bad translation in X dimension" + quaternion);
            Assert.AreEqual(q.Y, quaternion.Y, 0.05, "Bad translation in Y dimension" + quaternion);
            Assert.AreEqual(q.Z, quaternion.Z, 0.05, "Bad translation in Z dimension" + quaternion);
        }

        public void TestAxisAngleQuatConversion(AxisAngle axisAngle, Quaternion q)
        {
            AxisAngle aa = AxisAngle.QuaternionToAxisAngle(q);

            testContextInstance.WriteLine(aa + " | " + axisAngle);

            Assert.AreEqual(axisAngle.Rotation, aa.Rotation, 0.1,  "Bad translation in R rotation " + aa);
            Assert.AreEqual(axisAngle.X,        aa.X,        0.05, "Bad translation in X dimension" + aa);
            Assert.AreEqual(axisAngle.Y,        aa.Y,        0.05, "Bad translation in Y dimension" + aa);
            Assert.AreEqual(axisAngle.Z,        aa.Z,        0.05, "Bad translation in Z dimension" + aa);
        }
    }
}
