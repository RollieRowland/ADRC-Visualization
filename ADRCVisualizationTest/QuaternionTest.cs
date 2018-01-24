﻿using System;
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

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(1, output.X, 0.001, "Test X Rotation 90 X (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test X Rotation 90 Y (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test X Rotation 90 Z (1, 0, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation90XY()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), Math.Sqrt(0.5), 0, 0);
            Vector vector = new Vector(0, 1, 0);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(0, output.X, 0.001, "Test X Rotation 90 X (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test X Rotation 90 Y (0, 1, 0) failed.");
            Assert.AreEqual(1, output.Z, 0.001, "Test X Rotation 90 Z (0, 1, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation90XZ()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), Math.Sqrt(0.5), 0, 0);
            Vector vector = new Vector(0, 0, 1);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(0, output.X, 0.001, "Test X Rotation 90 X (0, 0, 1) failed.");
            Assert.AreEqual(-1, output.Y, 0.001, "Test X Rotation 90 Y (0, 0, 1) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test X Rotation 90 Z (0, 0, 1) failed.");
        }

        [TestMethod]
        public void TestXRotation90YX()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), 0, Math.Sqrt(0.5), 0);
            Vector vector = new Vector(1, 0, 0);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Y Rotation 90 X (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Y Rotation 90 Y (1, 0, 0) failed.");
            Assert.AreEqual(-1, output.Z, 0.001, "Test Y Rotation 90 Z (1, 0, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation90YY()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), 0, Math.Sqrt(0.5), 0);
            Vector vector = new Vector(0, 1, 0);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Y Rotation 90 X (0, 1, 0) failed.");
            Assert.AreEqual(1, output.Y, 0.001, "Test Y Rotation 90 Y (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Y Rotation 90 Z (0, 1, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation90YZ()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), 0, Math.Sqrt(0.5), 0);
            Vector vector = new Vector(0, 0, 1);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(1, output.X, 0.001, "Test Y Rotation 90 X (0, 0, 1) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Y Rotation 90 Y (0, 0, 1) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Y Rotation 90 Z (0, 0, 1) failed.");
        }

        [TestMethod]
        public void TestXRotation90ZX()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), 0, 0, Math.Sqrt(0.5));
            Vector vector = new Vector(1, 0, 0);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Z Rotation 90 X (1, 0, 0) failed.");
            Assert.AreEqual(1, output.Y, 0.001, "Test Z Rotation 90 Y (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Z Rotation 90 Z (1, 0, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation90ZY()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), 0, 0, Math.Sqrt(0.5));
            Vector vector = new Vector(0, 1, 0);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(-1, output.X, 0.001, "Test Z Rotation 90 X (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Z Rotation 90 Y (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Z Rotation 90 Z (0, 1, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation90ZZ()
        {
            Quaternion quaternion = new Quaternion(Math.Sqrt(0.5), 0, 0, Math.Sqrt(0.5));
            Vector vector = new Vector(0, 0, 1);

            Vector output = quaternion.RotateVector(vector);

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

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(1, output.X, 0.001, "Test X Rotation 180 X (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test X Rotation 180 Y (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test X Rotation 180 Z (1, 0, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation180XY()
        {
            Quaternion quaternion = new Quaternion(0, 1, 0, 0);
            Vector vector = new Vector(0, 1, 0);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(0, output.X, 0.001, "Test X Rotation 180 X (0, 1, 0) failed.");
            Assert.AreEqual(-1, output.Y, 0.001, "Test X Rotation 180 Y (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test X Rotation 180 Z (0, 1, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation180XZ()
        {
            Quaternion quaternion = new Quaternion(0, 1, 0, 0);
            Vector vector = new Vector(0, 0, 1);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(0, output.X, 0.001, "Test X Rotation 180 X (0, 0, 1) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test X Rotation 180 Y (0, 0, 1) failed.");
            Assert.AreEqual(-1, output.Z, 0.001, "Test X Rotation 180 Z (0, 0, 1) failed.");
        }

        [TestMethod]
        public void TestXRotation180YX()
        {
            Quaternion quaternion = new Quaternion(0, 0, 1, 0);
            Vector vector = new Vector(1, 0, 0);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(-1, output.X, 0.001, "Test Y Rotation 180 X (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Y Rotation 180 Y (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Y Rotation 180 Z (1, 0, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation180YY()
        {
            Quaternion quaternion = new Quaternion(0, 0, 1, 0);
            Vector vector = new Vector(0, 1, 0);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Y Rotation 180 X (0, 1, 0) failed.");
            Assert.AreEqual(1, output.Y, 0.001, "Test Y Rotation 180 Y (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Y Rotation 180 Z (0, 1, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation180YZ()
        {
            Quaternion quaternion = new Quaternion(0, 0, 1, 0);
            Vector vector = new Vector(0, 0, 1);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Y Rotation 180 X (0, 0, 1) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Y Rotation 180 Y (0, 0, 1) failed.");
            Assert.AreEqual(-1, output.Z, 0.001, "Test Y Rotation 180 Z (0, 0, 1) failed.");
        }

        [TestMethod]
        public void TestXRotation180ZX()
        {
            Quaternion quaternion = new Quaternion(0, 0, 0, 1);
            Vector vector = new Vector(1, 0, 0);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(-1, output.X, 0.001, "Test Z Rotation 180 X (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Z Rotation 180 Y (1, 0, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Z Rotation 180 Z (1, 0, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation180ZY()
        {
            Quaternion quaternion = new Quaternion(0, 0, 0, 1);
            Vector vector = new Vector(0, 1, 0);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Z Rotation 180 X (0, 1, 0) failed.");
            Assert.AreEqual(-1, output.Y, 0.001, "Test Z Rotation 180 Y (0, 1, 0) failed.");
            Assert.AreEqual(0, output.Z, 0.001, "Test Z Rotation 180 Z (0, 1, 0) failed.");
        }

        [TestMethod]
        public void TestXRotation180ZZ()
        {
            Quaternion quaternion = new Quaternion(0, 0, 0, 1);
            Vector vector = new Vector(0, 0, 1);

            Vector output = quaternion.RotateVector(vector);

            Assert.AreEqual(0, output.X, 0.001, "Test Z Rotation 180 X (0, 0, 1) failed.");
            Assert.AreEqual(0, output.Y, 0.001, "Test Z Rotation 180 Y (0, 0, 1) failed.");
            Assert.AreEqual(1, output.Z, 0.001, "Test Z Rotation 180 Z (0, 0, 1) failed.");
        }

        //Test from euler angle
        //Test to euler angle

        [TestMethod]
        public void TestEulerAngleConversion()
        {
            Vector euler = new Vector(5, 10, 15);

            Quaternion quaternion = Quaternion.FromEulerAngle2(euler);

            //0.986, 0.054, 0.081, 0.134 -> 5, 10, 15
            Assert.AreEqual(0.986, quaternion.W, 0.01, "Bad quaternion value W" + quaternion);
            Assert.AreEqual(0.054, quaternion.X, 0.01, "Bad quaternion value X" + quaternion);
            Assert.AreEqual(0.081, quaternion.Y, 0.01, "Bad quaternion value Y" + quaternion);
            Assert.AreEqual(0.134, quaternion.Z, 0.01, "Bad quaternion value Z" + quaternion);

            Vector eulerConverted = Quaternion.ToEulerAngle3(quaternion);//bad translation

            Assert.AreEqual(euler.X, eulerConverted.X, 0.01, "Bad translation in X dimension" + eulerConverted);
            Assert.AreEqual(euler.Y, eulerConverted.Y, 0.01, "Bad translation in X dimension" + eulerConverted);
            Assert.AreEqual(euler.Z, eulerConverted.Z, 0.01, "Bad translation in X dimension" + eulerConverted);
        }

        [TestMethod]
        public void TestToEulerAngle90()
        {
            Quaternion XRotation = new Quaternion(Math.Sqrt(0.5), Math.Sqrt(0.5), 0,              0             );
            Quaternion YRotation = new Quaternion(Math.Sqrt(0.5), 0,              Math.Sqrt(0.5), 0             );
            Quaternion ZRotation = new Quaternion(Math.Sqrt(0.5), 0,              0,              Math.Sqrt(0.5));
            
            Vector eulerConvertedX = Quaternion.ToEulerAngle3(XRotation.UnitQuaternion());
            Vector eulerConvertedY = Quaternion.ToEulerAngle3(YRotation.UnitQuaternion());
            Vector eulerConvertedZ = Quaternion.ToEulerAngle3(ZRotation.UnitQuaternion());

            Assert.AreEqual(90, eulerConvertedX.X, 0.01, "90 X Rotation" + eulerConvertedX);
            Assert.AreEqual(0,  eulerConvertedX.Y, 0.01, "90 X Rotation" + eulerConvertedX);
            Assert.AreEqual(0,  eulerConvertedX.Z, 0.01, "90 X Rotation" + eulerConvertedX);

            Assert.AreEqual(0,  eulerConvertedY.X, 0.01, "90 Y Rotation" + eulerConvertedY);
            Assert.AreEqual(90, eulerConvertedY.Y, 0.01, "90 Y Rotation" + eulerConvertedY);
            Assert.AreEqual(0,  eulerConvertedY.Z, 0.01, "90 Y Rotation" + eulerConvertedY);

            Assert.AreEqual(0,  eulerConvertedZ.X, 0.01, "90 Z Rotation" + eulerConvertedZ);
            Assert.AreEqual(0,  eulerConvertedZ.Y, 0.01, "90 Z Rotation" + eulerConvertedZ);
            Assert.AreEqual(90, eulerConvertedZ.Z, 0.01, "90 Z Rotation" + eulerConvertedZ);
        }

        [TestMethod]
        public void TestToEulerAngle180()
        {
            Quaternion noRotation = new Quaternion(1, 0, 0, 0);
            Quaternion XRotation  = new Quaternion(0, 1, 0, 0);
            Quaternion YRotation  = new Quaternion(0, 0, 1, 0);
            Quaternion ZRotation  = new Quaternion(0, 0, 0, 1);

            Vector eulerConvertedNo = Quaternion.ToEulerAngle3(noRotation.UnitQuaternion());
            Vector eulerConvertedX  = Quaternion.ToEulerAngle3(XRotation.UnitQuaternion());
            Vector eulerConvertedY  = Quaternion.ToEulerAngle3(YRotation.UnitQuaternion());
            Vector eulerConvertedZ  = Quaternion.ToEulerAngle3(ZRotation.UnitQuaternion());

            Assert.AreEqual(0, eulerConvertedNo.X, 0.01, "No rotation" + eulerConvertedNo);
            Assert.AreEqual(0, eulerConvertedNo.Y, 0.01, "No rotation" + eulerConvertedNo);
            Assert.AreEqual(0, eulerConvertedNo.Z, 0.01, "No rotation" + eulerConvertedNo);
            
            Assert.AreEqual(180, eulerConvertedX.X, 0.01, "180 X Rotation" + eulerConvertedX);
            Assert.AreEqual(0,   eulerConvertedX.Y, 0.01, "180 X Rotation" + eulerConvertedX);
            Assert.AreEqual(0,   eulerConvertedX.Z, 0.01, "180 X Rotation" + eulerConvertedX);

            Assert.AreEqual(0,   eulerConvertedY.X, 0.01, "180 Y Rotation" + eulerConvertedY);
            Assert.AreEqual(180, eulerConvertedY.Y, 0.01, "180 Y Rotation" + eulerConvertedY);
            Assert.AreEqual(0,   eulerConvertedY.Z, 0.01, "180 Y Rotation" + eulerConvertedY);

            Assert.AreEqual(0,   eulerConvertedZ.X, 0.01, "180 Z Rotation" + eulerConvertedZ);
            Assert.AreEqual(0,   eulerConvertedZ.Y, 0.01, "180 Z Rotation" + eulerConvertedZ);
            Assert.AreEqual(180, eulerConvertedZ.Z, 0.01, "180 Z Rotation" + eulerConvertedZ);
        }

        [TestMethod]
        public void TestFromEulerAngle90()
        {
            Vector XRotation = new Vector(90, 0, 0);
            Vector YRotation = new Vector(0, 90, 0);
            Vector ZRotation = new Vector(0, 0, 90);
            
            Quaternion eulerConvertedX = Quaternion.FromEulerAngle2(XRotation);
            Quaternion eulerConvertedY = Quaternion.FromEulerAngle2(YRotation);
            Quaternion eulerConvertedZ = Quaternion.FromEulerAngle2(ZRotation);

            Assert.AreEqual(Math.Sqrt(0.5), eulerConvertedX.W, 0.01, "90 X Rotation" + eulerConvertedX);
            Assert.AreEqual(Math.Sqrt(0.5), eulerConvertedX.X, 0.01, "90 X Rotation" + eulerConvertedX);
            Assert.AreEqual(0,              eulerConvertedX.Y, 0.01, "90 X Rotation" + eulerConvertedX);
            Assert.AreEqual(0,              eulerConvertedX.Z, 0.01, "90 X Rotation" + eulerConvertedX);

            Assert.AreEqual(Math.Sqrt(0.5), eulerConvertedX.W, 0.01, "90 Y Rotation" + eulerConvertedX);
            Assert.AreEqual(0,              eulerConvertedY.X, 0.01, "90 Y Rotation" + eulerConvertedY);
            Assert.AreEqual(Math.Sqrt(0.5), eulerConvertedY.Y, 0.01, "90 Y Rotation" + eulerConvertedY);
            Assert.AreEqual(0,              eulerConvertedY.Z, 0.01, "90 Y Rotation" + eulerConvertedY);

            Assert.AreEqual(Math.Sqrt(0.5), eulerConvertedX.W, 0.01, "90 Z Rotation" + eulerConvertedX);
            Assert.AreEqual(0,              eulerConvertedZ.X, 0.01, "90 Z Rotation" + eulerConvertedZ);
            Assert.AreEqual(0,              eulerConvertedZ.Y, 0.01, "90 Z Rotation" + eulerConvertedZ);
            Assert.AreEqual(Math.Sqrt(0.5), eulerConvertedZ.Z, 0.01, "90 Z Rotation" + eulerConvertedZ);
        }

        [TestMethod]
        public void TestFromEulerAngle180()
        {
            Vector noRotation = new Vector(0,   0,   0  );
            Vector XRotation  = new Vector(180, 0,   0  );
            Vector YRotation  = new Vector(0,   180, 0  );
            Vector ZRotation  = new Vector(0,   0,   180);

            Quaternion eulerConvertedNo = Quaternion.FromEulerAngle2(noRotation);
            Quaternion eulerConvertedX  = Quaternion.FromEulerAngle2(XRotation);
            Quaternion eulerConvertedY  = Quaternion.FromEulerAngle2(YRotation);
            Quaternion eulerConvertedZ  = Quaternion.FromEulerAngle2(ZRotation);

            Assert.AreEqual(1, eulerConvertedNo.W, 0.01, "No rotation" + eulerConvertedNo);
            Assert.AreEqual(0, eulerConvertedNo.X, 0.01, "No rotation" + eulerConvertedNo);
            Assert.AreEqual(0, eulerConvertedNo.Y, 0.01, "No rotation" + eulerConvertedNo);
            Assert.AreEqual(0, eulerConvertedNo.Z, 0.01, "No rotation" + eulerConvertedNo);

            Assert.AreEqual(0, eulerConvertedX.W, 0.01, "180 X Rotation" + eulerConvertedX);
            Assert.AreEqual(1, eulerConvertedX.X, 0.01, "180 X Rotation" + eulerConvertedX);
            Assert.AreEqual(0, eulerConvertedX.Y, 0.01, "180 X Rotation" + eulerConvertedX);
            Assert.AreEqual(0, eulerConvertedX.Z, 0.01, "180 X Rotation" + eulerConvertedX);

            Assert.AreEqual(0, eulerConvertedX.W, 0.01, "180 Y Rotation" + eulerConvertedX);
            Assert.AreEqual(0, eulerConvertedY.X, 0.01, "180 Y Rotation" + eulerConvertedY);
            Assert.AreEqual(1, eulerConvertedY.Y, 0.01, "180 Y Rotation" + eulerConvertedY);
            Assert.AreEqual(0, eulerConvertedY.Z, 0.01, "180 Y Rotation" + eulerConvertedY);

            Assert.AreEqual(0, eulerConvertedX.W, 0.01, "180 Z Rotation" + eulerConvertedX);
            Assert.AreEqual(0, eulerConvertedZ.X, 0.01, "180 Z Rotation" + eulerConvertedZ);
            Assert.AreEqual(0, eulerConvertedZ.Y, 0.01, "180 Z Rotation" + eulerConvertedZ);
            Assert.AreEqual(1, eulerConvertedZ.Z, 0.01, "180 Z Rotation" + eulerConvertedZ);
        }

        //Test all individual functions in quaternion class
    }
}
