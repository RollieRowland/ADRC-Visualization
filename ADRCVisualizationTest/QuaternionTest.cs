using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualizationTest
{
    [TestClass]
    public class QuaternionTest
    {
        //Correct values taken from http://wiki.ogre3d.org/Quaternion+and+Rotation+Primer
        [TestMethod]
        public void TestXRotation90XX()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), Math.Sqrt(0.5), 0, 0);
            Vector vector = new Vector(1, 0, 0);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(1, output.X, 0.001, "Test X Rotation 90 X (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test X Rotation 90 Y (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test X Rotation 90 Z (1, 0, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation90XY()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), Math.Sqrt(0.5), 0, 0);
            Vector vector = new Vector(0, 1, 0);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(0, output.X, 0.001, "Test X Rotation 90 X (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test X Rotation 90 Y (0, 1, 0) failed.");
            Assert.AreEqual(1, output.Z, 0.001, "Test X Rotation 90 Z (0, 1, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation90XZ()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), Math.Sqrt(0.5), 0, 0);
            Vector vector = new Vector(0, 0, 1);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(0, output.X, 0.001, "Test X Rotation 90 X (0, 0, 1) failed.");
            Assert.AreEqual(-1, output.Y, 0.001, "Test X Rotation 90 Y (0, 0, 1) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test X Rotation 90 Z (0, 0, 1) failed.");
        }

        [TestMethod]
        public void TestXRotation90YX()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), 0, Math.Sqrt(0.5), 0);
            Vector vector = new Vector(1, 0, 0);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Y Rotation 90 X (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Y Rotation 90 Y (1, 0, 0) failed.");
            Assert.AreEqual(-1, output.Z, 0.001, "Test Y Rotation 90 Z (1, 0, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation90YY()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), 0, Math.Sqrt(0.5), 0);
            Vector vector = new Vector(0, 1, 0);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Y Rotation 90 X (0, 1, 0) failed.");
            Assert.AreEqual(1, output.Y, 0.001, "Test Y Rotation 90 Y (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Y Rotation 90 Z (0, 1, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation90YZ()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), 0, Math.Sqrt(0.5), 0);
            Vector vector = new Vector(0, 0, 1);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(1, output.X, 0.001, "Test Y Rotation 90 X (0, 0, 1) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Y Rotation 90 Y (0, 0, 1) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Y Rotation 90 Z (0, 0, 1) failed.");
        }

        [TestMethod]
        public void TestXRotation90ZX()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), 0, 0, Math.Sqrt(0.5));
            Vector vector = new Vector(1, 0, 0);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Z Rotation 90 X (1, 0, 0) failed.");
            Assert.AreEqual(1, output.Y, 0.001, "Test Z Rotation 90 Y (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Z Rotation 90 Z (1, 0, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation90ZY()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), 0, 0, Math.Sqrt(0.5));
            Vector vector = new Vector(0, 1, 0);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(-1, output.X, 0.001, "Test Z Rotation 90 X (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Z Rotation 90 Y (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Z Rotation 90 Z (0, 1, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation90ZZ()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), 0, 0, Math.Sqrt(0.5));
            Vector vector = new Vector(0, 0, 1);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Z Rotation 90 X (0, 0, 1) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Z Rotation 90 Y (0, 0, 1) failed.");
            Assert.AreEqual(1, output.Z, 0.001, "Test Z Rotation 90 Z (0, 0, 1) failed.");
        }

        //Test 180 degree
        [TestMethod]
        public void TestXRotation180XX()
        {
            Quaternion quaternion = new Quaternion(0, 1, 0, 0);
            Vector vector = new Vector(1, 0, 0);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(1, output.X, 0.001, "Test X Rotation 180 X (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test X Rotation 180 Y (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test X Rotation 180 Z (1, 0, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation180XY()
        {
            Quaternion quaternion = new Quaternion(0, 1, 0, 0);
            Vector vector = new Vector(0, 1, 0);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(0, output.X, 0.001, "Test X Rotation 180 X (0, 1, 0) failed.");
            Assert.AreEqual(-1, output.Y, 0.001, "Test X Rotation 180 Y (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test X Rotation 180 Z (0, 1, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation180XZ()
        {
            Quaternion quaternion = new Quaternion(0, 1, 0, 0);
            Vector vector = new Vector(0, 0, 1);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(0, output.X, 0.001, "Test X Rotation 180 X (0, 0, 1) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test X Rotation 180 Y (0, 0, 1) failed.");
            Assert.AreEqual(-1, output.Z, 0.001, "Test X Rotation 180 Z (0, 0, 1) failed.");
        }

        [TestMethod]
        public void TestXRotation180YX()
        {
            Quaternion quaternion = new Quaternion(0, 0, 1, 0);
            Vector vector = new Vector(1, 0, 0);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(-1, output.X, 0.001, "Test Y Rotation 180 X (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Y Rotation 180 Y (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Y Rotation 180 Z (1, 0, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation180YY()
        {
            Quaternion quaternion = new Quaternion(0, 0, 1, 0);
            Vector vector = new Vector(0, 1, 0);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Y Rotation 180 X (0, 1, 0) failed.");
            Assert.AreEqual(1, output.Y, 0.001, "Test Y Rotation 180 Y (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Y Rotation 180 Z (0, 1, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation180YZ()
        {
            Quaternion quaternion = new Quaternion(0, 0, 1, 0);
            Vector vector = new Vector(0, 0, 1);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Y Rotation 180 X (0, 0, 1) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Y Rotation 180 Y (0, 0, 1) failed.");
            Assert.AreEqual(-1, output.Z, 0.001, "Test Y Rotation 180 Z (0, 0, 1) failed.");
        }

        [TestMethod]
        public void TestXRotation180ZX()
        {
            Quaternion quaternion = new Quaternion(0, 0, 0, 1);
            Vector vector = new Vector(1, 0, 0);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(-1, output.X, 0.001, "Test Z Rotation 180 X (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Z Rotation 180 Y (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Z Rotation 180 Z (1, 0, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation180ZY()
        {
            Quaternion quaternion = new Quaternion(0, 0, 0, 1);
            Vector vector = new Vector(0, 1, 0);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Z Rotation 180 X (0, 1, 0) failed.");
            Assert.AreEqual(-1, output.Y, 0.001, "Test Z Rotation 180 Y (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Z Rotation 180 Z (0, 1, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation180ZZ()
        {
            Quaternion quaternion = new Quaternion(0, 0, 0, 1);
            Vector vector = new Vector(0, 0, 1);

            Vector output = Quaternion.RotateVector(quaternion, vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Z Rotation 180 X (0, 0, 1) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Z Rotation 180 Y (0, 0, 1) failed.");
            Assert.AreEqual(1, output.Z, 0.001, "Test Z Rotation 180 Z (0, 0, 1) failed.");
        }

        //Test from euler angle 

        //Test all individual functions in quaternion class
    }
}
