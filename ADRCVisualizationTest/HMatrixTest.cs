using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualizationTest
{
    [TestClass]
    public class HMatrixTest
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
        public void BVATestEulerAnglesHMatrixConversion180()
        {
            for (int i = -90; i <= 90; i += 10)
            {
                BVATestEulerAngleHMatrixConversion(i);
            }
        }

        public void BVATestEulerAngleHMatrixConversion(double angle)
        {
            EulerHMatrixConversion(new Vector(angle, 0, 0));
            EulerHMatrixConversion(new Vector(0, angle, 0));
            EulerHMatrixConversion(new Vector(0, 0, angle));

            testContextInstance.WriteLine("");

            EulerHMatrixConversion(new Vector(angle, angle, 0));
            EulerHMatrixConversion(new Vector(0, angle, angle));
            EulerHMatrixConversion(new Vector(angle, 0, angle));

            testContextInstance.WriteLine("");

            EulerHMatrixConversion(new Vector(0, 0, 0));
            EulerHMatrixConversion(new Vector(angle, angle, angle));

            testContextInstance.WriteLine("\n/////////////////////////\n");
        }

        public void EulerHMatrixConversion(Vector euler)
        {
            EulerAngles eulerAngles = new EulerAngles(new Vector(euler.X, euler.Y, euler.Z), EulerConstants.EulerOrderXYZR);

            HMatrix hM = EulerAngles.EulerToHMatrix(eulerAngles);

            Vector eulerConverted = EulerAngles.HMatrixToEuler(hM, EulerConstants.EulerOrderXYZR).Angles;//bad translation

            testContextInstance.WriteLine(eulerConverted.ToString());

            Assert.AreEqual(euler.X, eulerConverted.X, 0.01, "Bad translation in X dimension" + eulerConverted);
            Assert.AreEqual(euler.Y, eulerConverted.Y, 0.01, "Bad translation in X dimension" + eulerConverted);
            Assert.AreEqual(euler.Z, eulerConverted.Z, 0.01, "Bad translation in X dimension" + eulerConverted);
        }

        /*
        [TestMethod]
        public void BVATestEulerAnglesHMatrixConversion360()
        {
            //Responds correctly for angle output, it just resolves to a more efficient euler angle
            /*
            for (int i = -350; i <= -90; i += 10)
            {
                BVATestEulerAngleHMatrixConversion(i);
            }

            for (int i = 90; i <= 350; i += 10)
            {
                BVATestEulerAngleHMatrixConversion(i);
            }
        }*/
    }
}
