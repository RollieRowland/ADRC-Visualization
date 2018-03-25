#include "stdafx.h"
#include "CppUnitTest.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace DTRQControllerTest
{

}
/*
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

            HMatrix hM = HMatrix.EulerToHMatrix(eulerAngles);

            Vector eulerConverted = EulerAngles.HMatrixToEuler(hM, EulerConstants.EulerOrderXYZR).Angles;//bad translation

            testContextInstance.WriteLine(eulerConverted.ToString());

            Assert.AreEqual(euler.X, eulerConverted.X, 0.01, "Bad translation in X dimension" + eulerConverted);
            Assert.AreEqual(euler.Y, eulerConverted.Y, 0.01, "Bad translation in X dimension" + eulerConverted);
            Assert.AreEqual(euler.Z, eulerConverted.Z, 0.01, "Bad translation in X dimension" + eulerConverted);
        }*/