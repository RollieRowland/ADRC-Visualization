#include "stdafx.h"
#include "CppUnitTest.h"
#include <Quaternion.h>
#include <Rotation.h>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace DTRQControllerTest
{
	TEST_CLASS(QuaternionTest) {
	public:
		void Print(std::string str) {
			Logger::WriteMessage((str + "\n").c_str());
		}

		TEST_METHOD(SphericalInterpolationTest) {
			Quaternion q1 = Quaternion(1, 0, 0, 0);
			Quaternion q2 = Quaternion(0, 1, 0, 0);

			TestSlerp(q1, q2, 0.0, Quaternion(1, 0, 0, 0));
			TestSlerp(q1, q2, 0.2, Quaternion(0.951056516295154, 0.309016994374947, 0, 0));
			TestSlerp(q1, q2, 0.4, Quaternion(0.809016994374947, 0.587785252292473, 0, 0));
			TestSlerp(q1, q2, 0.6, Quaternion(0.587785252292473, 0.809016994374947, 0, 0));
			TestSlerp(q1, q2, 0.8, Quaternion(0.309016994374947, 0.951056516295154, 0, 0));
			TestSlerp(q1, q2, 1.0, Quaternion(0, 1, 0, 0));
		}

		void TestSlerp(Quaternion q1, Quaternion q2, double ratio, Quaternion expected) {
			Quaternion q3 = Quaternion::SphericalInterpolation(q1, q2, ratio);

			Print(q3.ToString() + " " + expected.ToString());

			Assert::AreEqual(expected.W, q3.W, 0.001, L"W Slerp failed.");
			Assert::AreEqual(expected.X, q3.X, 0.001, L"X Slerp failed.");
			Assert::AreEqual(expected.Y, q3.Y, 0.001, L"Y Slerp failed.");
			Assert::AreEqual(expected.Z, q3.Z, 0.001, L"Z Slerp failed.");
		}



		TEST_METHOD(TestXRotation90XX) {
			Quaternion quaternion = Quaternion(sqrt(0.5), sqrt(0.5), 0, 0);
			Vector3D vector = Vector3D(1, 0, 0);

			Vector3D output = quaternion.RotateVector(vector);

			Assert::AreEqual(1.0, output.X, 0.001, L"Test X Rotation 90 X (1, 0, 0) failed.");
			Assert::AreEqual(0.0, output.Y, 0.001, L"Test X Rotation 90 Y (1, 0, 0) failed.");
			Assert::AreEqual(0.0, output.Z, 0.001, L"Test X Rotation 90 Z (1, 0, 0) failed.");
		}

		TEST_METHOD(TestXRotation90XY) {
			Quaternion quaternion = Quaternion(sqrt(0.5), sqrt(0.5), 0, 0);
			Vector3D vector = Vector3D(0, 1, 0);

			Vector3D output = quaternion.RotateVector(vector);

			Assert::AreEqual(0.0, output.X, 0.001, L"Test X Rotation 90 X (1, 0, 0) failed.");
			Assert::AreEqual(0.0, output.Y, 0.001, L"Test X Rotation 90 Y (1, 0, 0) failed.");
			Assert::AreEqual(1.0, output.Z, 0.001, L"Test X Rotation 90 Z (1, 0, 0) failed.");
		}

		TEST_METHOD(TestXRotation90XZ) {
			Quaternion quaternion = Quaternion(sqrt(0.5), sqrt(0.5), 0, 0);
			Vector3D vector = Vector3D(0, 0, 1);

			Vector3D output = quaternion.RotateVector(vector);

			Assert::AreEqual(0.0, output.X, 0.001, L"Test X Rotation 90 X (1, 0, 0) failed.");
			Assert::AreEqual(-1.0, output.Y, 0.001, L"Test X Rotation 90 Y (1, 0, 0) failed.");
			Assert::AreEqual(0.0, output.Z, 0.001, L"Test X Rotation 90 Z (1, 0, 0) failed.");
		}

		TEST_METHOD(TestXRotation90YX) {
			Quaternion quaternion = Quaternion(sqrt(0.5), 0, sqrt(0.5), 0);
			Vector3D vector = Vector3D(1, 0, 0);

			Vector3D output = quaternion.RotateVector(vector);

			Assert::AreEqual(0.0, output.X, 0.001, L"Test X Rotation 90 X (1, 0, 0) failed.");
			Assert::AreEqual(0.0, output.Y, 0.001, L"Test X Rotation 90 Y (1, 0, 0) failed.");
			Assert::AreEqual(-1.0, output.Z, 0.001, L"Test X Rotation 90 Z (1, 0, 0) failed.");
		}

		TEST_METHOD(TestXRotation90YY) {
			Quaternion quaternion = Quaternion(sqrt(0.5), 0, sqrt(0.5), 0);
			Vector3D vector = Vector3D(0, 1, 0);

			Vector3D output = quaternion.RotateVector(vector);

			Assert::AreEqual(0.0, output.X, 0.001, L"Test X Rotation 90 X (1, 0, 0) failed.");
			Assert::AreEqual(1.0, output.Y, 0.001, L"Test X Rotation 90 Y (1, 0, 0) failed.");
			Assert::AreEqual(0.0, output.Z, 0.001, L"Test X Rotation 90 Z (1, 0, 0) failed.");
		}

		TEST_METHOD(TestXRotation90YZ) {
			Quaternion quaternion = Quaternion(sqrt(0.5), 0, sqrt(0.5), 0);
			Vector3D vector = Vector3D(0, 0, 1);

			Vector3D output = quaternion.RotateVector(vector);

			Assert::AreEqual(1.0, output.X, 0.001, L"Test X Rotation 90 X (1, 0, 0) failed.");
			Assert::AreEqual(0.0, output.Y, 0.001, L"Test X Rotation 90 Y (1, 0, 0) failed.");
			Assert::AreEqual(0.0, output.Z, 0.001, L"Test X Rotation 90 Z (1, 0, 0) failed.");
		}

		TEST_METHOD(TestXRotation90ZX) {
			Quaternion quaternion = Quaternion(sqrt(0.5), 0, 0, sqrt(0.5));
			Vector3D vector = Vector3D(1, 0, 0);

			Vector3D output = quaternion.RotateVector(vector);

			Assert::AreEqual(0.0, output.X, 0.001, L"Test X Rotation 90 X (1, 0, 0) failed.");
			Assert::AreEqual(1.0, output.Y, 0.001, L"Test X Rotation 90 Y (1, 0, 0) failed.");
			Assert::AreEqual(0.0, output.Z, 0.001, L"Test X Rotation 90 Z (1, 0, 0) failed.");
		}

		TEST_METHOD(TestXRotation90ZY) {
			Quaternion quaternion = Quaternion(sqrt(0.5), 0, 0, sqrt(0.5));
			Vector3D vector = Vector3D(0, 1, 0);

			Vector3D output = quaternion.RotateVector(vector);

			Assert::AreEqual(-1.0, output.X, 0.001, L"Test X Rotation 90 X (1, 0, 0) failed.");
			Assert::AreEqual(0.0, output.Y, 0.001, L"Test X Rotation 90 Y (1, 0, 0) failed.");
			Assert::AreEqual(0.0, output.Z, 0.001, L"Test X Rotation 90 Z (1, 0, 0) failed.");
		}

		TEST_METHOD(TestXRotation90ZZ) {
			Quaternion quaternion = Quaternion(sqrt(0.5), 0, 0, sqrt(0.5));
			Vector3D vector = Vector3D(0, 0, 1);

			Vector3D output = quaternion.RotateVector(vector);

			Assert::AreEqual(0.0, output.X, 0.001, L"Test X Rotation 90 X (1, 0, 0) failed.");
			Assert::AreEqual(0.0, output.Y, 0.001, L"Test X Rotation 90 Y (1, 0, 0) failed.");
			Assert::AreEqual(1.0, output.Z, 0.001, L"Test X Rotation 90 Z (1, 0, 0) failed.");
		}

		TEST_METHOD(TestXRotation90XYUn) {
			Quaternion quaternion = Quaternion(sqrt(0.5), sqrt(0.5), 0, 0);
			Vector3D vector = Vector3D(0, 1, 0);

			Vector3D output = quaternion.UnrotateVector(vector);

			Assert::AreEqual(0.0, output.X, 0.001, L"Test X Rotation 90 X (1, 0, 0) failed.");
			Assert::AreEqual(0.0, output.Y, 0.001, L"Test X Rotation 90 Y (1, 0, 0) failed.");
			Assert::AreEqual(-1.0, output.Z, 0.001, L"Test X Rotation 90 Z (1, 0, 0) failed.");
		}
	};
}

/*

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

[TestMethod]
public void TestMultipleEulerConversions()
{
TestEulerAngleConversions(new Vector(5, 5, 5),    new Quaternion(0.997, 0.045, 0.042, 0.045));
TestEulerAngleConversions(new Vector(5, 10, 15),  new Quaternion(0.986, 0.055, 0.081, 0.134));
TestEulerAngleConversions(new Vector(10, 10, 10), new Quaternion(0.987, 0.094, 0.078, 0.094));
TestEulerAngleConversions(new Vector(25, 25, 25), new Quaternion(0.920, 0.252, 0.160, 0.252));

TestEulerAngleConversions(new Vector(45, 45, 45), new Quaternion(0.732, 0.461, 0.191, 0.461));
TestEulerAngleConversions(new Vector(15, 30, 45), new Quaternion(0.872, 0.214, 0.189, 0.398));
TestEulerAngleConversions(new Vector(45, 30, 15), new Quaternion(0.872, 0.398, 0.189, 0.215));
}

public void TestEulerAngleConversions(Vector euler, Quaternion q)
{
TestEulerQuaternion(euler, q);
TestQuaternionEuler(euler, q);
}

public void TestEulerQuaternion(Vector euler, Quaternion q)
{
EulerAngles eulerAngles = new EulerAngles(new Vector(euler.X, euler.Y, euler.Z), EulerConstants.EulerOrderZYXS);

Quaternion quaternion = Quaternion.EulerToQuaternion(eulerAngles);

testContextInstance.WriteLine(q + " | " + quaternion + " | " + q.Subtract(quaternion));

Assert.AreEqual(q.W, quaternion.W, 0.05, "Bad translation in W dimension" + quaternion);
Assert.AreEqual(q.X, quaternion.X, 0.05, "Bad translation in X dimension" + quaternion);
Assert.AreEqual(q.Y, quaternion.Y, 0.05, "Bad translation in Y dimension" + quaternion);
Assert.AreEqual(q.Z, quaternion.Z, 0.05, "Bad translation in Z dimension" + quaternion);
}

public void TestQuaternionEuler(Vector euler, Quaternion q)
{
Vector eulerConverted = EulerAngles.QuaternionToEuler(q, EulerConstants.EulerOrderXYZR).Angles;//bad translation

testContextInstance.WriteLine(eulerConverted.ToString());

Assert.AreEqual(euler.X, eulerConverted.X, 0.5, "Bad translation in X dimension" + eulerConverted);
Assert.AreEqual(euler.Y, eulerConverted.Y, 0.5, "Bad translation in Y dimension" + eulerConverted);
Assert.AreEqual(euler.Z, eulerConverted.Z, 0.5, "Bad translation in Z dimension" + eulerConverted);
}

[TestMethod]
public void BVATestEulerAnglesConversion()
{
//More or less than 90 resolves to better translations for Euler angles
BVATestEulerAngleConversion(-90);
BVATestEulerAngleConversion(-75);
BVATestEulerAngleConversion(-45);
BVATestEulerAngleConversion(-25);
BVATestEulerAngleConversion(-5);
BVATestEulerAngleConversion(0);
BVATestEulerAngleConversion(5);
BVATestEulerAngleConversion(25);
BVATestEulerAngleConversion(45);
BVATestEulerAngleConversion(75);
BVATestEulerAngleConversion(90);
}

public void BVATestEulerAngleConversion(double angle)
{
TestEulerAngleQuaternionConversion(new Vector(angle, 0,     0));
TestEulerAngleQuaternionConversion(new Vector(0,     angle, 0));
TestEulerAngleQuaternionConversion(new Vector(0,     0,     angle));

testContextInstance.WriteLine("");

TestEulerAngleQuaternionConversion(new Vector(angle, angle, 0));
TestEulerAngleQuaternionConversion(new Vector(0,     angle, angle));
TestEulerAngleQuaternionConversion(new Vector(angle, 0,     angle));

testContextInstance.WriteLine("");

TestEulerAngleQuaternionConversion(new Vector(0,     0,     0));
TestEulerAngleQuaternionConversion(new Vector(angle, angle, angle));

testContextInstance.WriteLine("\n/////////////////////////\n");
}

//Test from euler angle
//Test to euler angle
public void TestEulerAngleQuaternionConversion(Vector euler)
{
EulerAngles eulerAngles = new EulerAngles(new Vector(euler.X, euler.Y, euler.Z), EulerConstants.EulerOrderXYZS);

Quaternion quaternion = Quaternion.EulerToQuaternion(eulerAngles);

Vector eulerConverted = EulerAngles.QuaternionToEuler(quaternion, EulerConstants.EulerOrderXYZS).Angles;//bad translation

testContextInstance.WriteLine(eulerConverted.ToString());

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

Vector eulerConvertedX = EulerAngles.QuaternionToEuler(XRotation, EulerConstants.EulerOrderXYZR).Angles;
Vector eulerConvertedY = EulerAngles.QuaternionToEuler(YRotation, EulerConstants.EulerOrderXYZR).Angles;
Vector eulerConvertedZ = EulerAngles.QuaternionToEuler(ZRotation, EulerConstants.EulerOrderXYZR).Angles;

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

Vector eulerConvertedNo = EulerAngles.QuaternionToEuler(noRotation, EulerConstants.EulerOrderXYZS).Angles;
Vector eulerConvertedX  = EulerAngles.QuaternionToEuler(XRotation,  EulerConstants.EulerOrderXYZS).Angles;
Vector eulerConvertedY  = EulerAngles.QuaternionToEuler(YRotation,  EulerConstants.EulerOrderXYZS).Angles;
Vector eulerConvertedZ  = EulerAngles.QuaternionToEuler(ZRotation,  EulerConstants.EulerOrderXYZS).Angles;

Assert.AreEqual(0, eulerConvertedNo.X, 0.01, "No rotation" + eulerConvertedNo);
Assert.AreEqual(0, eulerConvertedNo.Y, 0.01, "No rotation" + eulerConvertedNo);
Assert.AreEqual(0, eulerConvertedNo.Z, 0.01, "No rotation" + eulerConvertedNo);

Assert.AreEqual(180, eulerConvertedX.X, 0.01, "180 X Rotation" + eulerConvertedX);
Assert.AreEqual(0,   eulerConvertedX.Y, 0.01, "180 X Rotation" + eulerConvertedX);
Assert.AreEqual(0,   eulerConvertedX.Z, 0.01, "180 X Rotation" + eulerConvertedX);

Assert.AreEqual(180,   eulerConvertedY.X, 0.01, "180 Y Rotation" + eulerConvertedY);
Assert.AreEqual(0, eulerConvertedY.Y, 0.01, "180 Y Rotation" + eulerConvertedY);
Assert.AreEqual(180,   eulerConvertedY.Z, 0.01, "180 Y Rotation" + eulerConvertedY);

Assert.AreEqual(0,   eulerConvertedZ.X, 0.01, "180 Z Rotation" + eulerConvertedZ);
Assert.AreEqual(0,   eulerConvertedZ.Y, 0.01, "180 Z Rotation" + eulerConvertedZ);
Assert.AreEqual(180, eulerConvertedZ.Z, 0.01, "180 Z Rotation" + eulerConvertedZ);
}

[TestMethod]
public void TestFromEulerAngle90()
{
EulerAngles XRotation = new EulerAngles(new Vector(90, 0, 0), EulerConstants.EulerOrderXYZS);
EulerAngles YRotation = new EulerAngles(new Vector(0, 90, 0), EulerConstants.EulerOrderXYZS);
EulerAngles ZRotation = new EulerAngles(new Vector(0, 0, 90), EulerConstants.EulerOrderXYZS);

Quaternion eulerConvertedX = Quaternion.EulerToQuaternion(XRotation);
Quaternion eulerConvertedY = Quaternion.EulerToQuaternion(YRotation);
Quaternion eulerConvertedZ = Quaternion.EulerToQuaternion(ZRotation);

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
EulerAngles noRotation = new EulerAngles(new Vector(0,   0,   0  ), EulerConstants.EulerOrderXYZS);
EulerAngles XRotation  = new EulerAngles(new Vector(180, 0,   0  ), EulerConstants.EulerOrderXYZS);
EulerAngles YRotation  = new EulerAngles(new Vector(0,   180, 0  ), EulerConstants.EulerOrderXYZS);
EulerAngles ZRotation  = new EulerAngles(new Vector(0,   0,   180), EulerConstants.EulerOrderXYZS);

Quaternion eulerConvertedNo = Quaternion.EulerToQuaternion(noRotation);
Quaternion eulerConvertedX  = Quaternion.EulerToQuaternion(XRotation);
Quaternion eulerConvertedY  = Quaternion.EulerToQuaternion(YRotation);
Quaternion eulerConvertedZ  = Quaternion.EulerToQuaternion(ZRotation);

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

[TestMethod]
public void TestRotationMatrixQuaternionConversions()
{
RotationMatrixQuaternionConversion(new Quaternion(0.9239,  0.3827,  0,       0     ));
RotationMatrixQuaternionConversion(new Quaternion(0.9239, -0.3827,  0,       0     ));
RotationMatrixQuaternionConversion(new Quaternion(0.9239,  0,       0.3827,  0     ));
RotationMatrixQuaternionConversion(new Quaternion(0.9239,  0,      -0.3827,  0     ));
RotationMatrixQuaternionConversion(new Quaternion(0.9239,  0,       0,       0.3827));
RotationMatrixQuaternionConversion(new Quaternion(0.9239,  0,       0,      -0.3827));
RotationMatrixQuaternionConversion(new Quaternion(0.6533,  0.2706,  0.6533,  0.2706));
RotationMatrixQuaternionConversion(new Quaternion(0.6533, -0.2706,  0.6533, -0.2706));
RotationMatrixQuaternionConversion(new Quaternion(0.6533, -0.2706, -0.6533,  0.2706));
}

public void RotationMatrixQuaternionConversion(Quaternion init)
{
RotationMatrix rM = RotationMatrix.QuaternionToMatrixRotation(init);
Quaternion post = Quaternion.RotationMatrixToQuaternionEfficient(rM);

Console.WriteLine(init + " |  " + post + " | " + (init - post));

Assert.AreEqual(init.W, post.W, 0.01, "DAQC W" + (init - post));
Assert.AreEqual(init.X, post.X, 0.01, "DAQC X" + (init - post));
Assert.AreEqual(init.Y, post.Y, 0.01, "DAQC Y" + (init - post));
Assert.AreEqual(init.Z, post.Z, 0.01, "DAQC Z" + (init - post));
}*/