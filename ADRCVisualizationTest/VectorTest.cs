using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualizationTest
{
    [TestClass]
    public class VectorTest
    {
        [TestMethod]
        public void TestSphericalCoordinateVectorConversions()
        {
            TestSphericalCoordinateVectorConversion(new Vector(0, 1, 0));
            TestSphericalCoordinateVectorConversion(new Vector(1, 0, 0));
            TestSphericalCoordinateVectorConversion(new Vector(0, 0, 1));
            TestSphericalCoordinateVectorConversion(new Vector(1, 0, 1));
            TestSphericalCoordinateVectorConversion(new Vector(1, 1, 0));
            TestSphericalCoordinateVectorConversion(new Vector(0, 1, 1));
            TestSphericalCoordinateVectorConversion(new Vector(1, 1, 1));
            TestSphericalCoordinateVectorConversion(new Vector(0,   0.5, 0));
            TestSphericalCoordinateVectorConversion(new Vector(0.5, 0,   0));
            TestSphericalCoordinateVectorConversion(new Vector(0,   0,   0.5));
            TestSphericalCoordinateVectorConversion(new Vector(0.5, 0,   0.5));
            TestSphericalCoordinateVectorConversion(new Vector(0.5, 0.5, 0));
            TestSphericalCoordinateVectorConversion(new Vector(0,   0.5, 0.5));
            TestSphericalCoordinateVectorConversion(new Vector(0.5, 0.5, 0.5));

            TestSphericalCoordinateVectorConversion(new Vector( 0, -1,  0));
            TestSphericalCoordinateVectorConversion(new Vector(-1,  0,  0));
            TestSphericalCoordinateVectorConversion(new Vector( 0,  0, -1));
            TestSphericalCoordinateVectorConversion(new Vector(-1,  0, -1));
            TestSphericalCoordinateVectorConversion(new Vector(-1, -1,  0));
            TestSphericalCoordinateVectorConversion(new Vector( 0, -1, -1));
            TestSphericalCoordinateVectorConversion(new Vector(-1, -1, -1));
            TestSphericalCoordinateVectorConversion(new Vector( 0,   -0.5,  0));
            TestSphericalCoordinateVectorConversion(new Vector(-0.5,  0,    0));
            TestSphericalCoordinateVectorConversion(new Vector( 0,    0,   -0.5));
            TestSphericalCoordinateVectorConversion(new Vector(-0.5,  0,   -0.5));
            TestSphericalCoordinateVectorConversion(new Vector(-0.5, -0.5,  0));
            TestSphericalCoordinateVectorConversion(new Vector( 0,   -0.5, -0.5));
            TestSphericalCoordinateVectorConversion(new Vector(-0.5, -0.5, -0.5));
        }

        public void TestSphericalCoordinateVectorConversion(Vector directionVector)
        {
            SphericalCoordinate sphericalCoordinate = SphericalCoordinate.VectorToSphericalCoordinate(directionVector);

            Vector spherCToDirV = Vector.SphericalCoordinateToVector(sphericalCoordinate);

            Console.WriteLine(directionVector.ToString() + " " + sphericalCoordinate.ToString() + " " + spherCToDirV.ToString());

            Assert.AreEqual(directionVector.X, spherCToDirV.X, 0.05, "Bad translation in X dimension");
            Assert.AreEqual(directionVector.Y, spherCToDirV.Y, 0.05, "Bad translation in Y dimension");
            Assert.AreEqual(directionVector.Z, spherCToDirV.Z, 0.05, "Bad translation in Z dimension");
        }
    }
}
