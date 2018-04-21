#include "stdafx.h"
#include "CppUnitTest.h"
#include <Rotation.h>
#include <Quaternion.h>
#include <AxisAngle.h>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace DTRQControllerTest
{
	TEST_CLASS(AxisAngleTest) {
	public:
		void Print(std::string str) {
			Logger::WriteMessage((str + "\n").c_str());
		}

		TEST_METHOD(AxisAngleBVA) {
			//All
			TestAxisAngleQuatConversions(AxisAngle(85.801, 0.679, 0.281, 0.679), Quaternion(0.733, 0.462, 0.191, 0.462));//45 45 45

																																 //Two
			TestAxisAngleQuatConversions(AxisAngle(62.799, 0.679, 0.679, 0.281), Quaternion(0.854, 0.354, 0.354, 0.147));//45 45 0
			TestAxisAngleQuatConversions(AxisAngle(62.799, 0.281, 0.679, 0.679), Quaternion(0.854, 0.147, 0.354, 0.354));//0  45 45
			TestAxisAngleQuatConversions(AxisAngle(62.799, 0.679, 0.281, 0.679), Quaternion(0.854, 0.354, 0.147, 0.354));//45 0  45

																																 //One
			TestAxisAngleQuatConversions(AxisAngle(45, 0, 0, 1), Quaternion(0.924, 0, 0, 0.383));//0 0 45
			TestAxisAngleQuatConversions(AxisAngle(45, 0, 1, 0), Quaternion(0.924, 0, 0.383, 0));//0 45 0
			TestAxisAngleQuatConversions(AxisAngle(45, 1, 0, 0), Quaternion(0.924, 0.383, 0, 0));//45 0 0

																										 //Extreme values
			TestAxisAngleQuatConversions(AxisAngle(0, 0, 1, 0), Quaternion(1, 0, 0, 0));//0   0   0 - standard is normalized to 0, [0, 1, 0]
			TestAxisAngleQuatConversions(AxisAngle(180, 1, 0, 0), Quaternion(0, 1, 0, 0));//180 0   0
			TestAxisAngleQuatConversions(AxisAngle(180, 0, 1, 0), Quaternion(0, 0, 1, 0));//0   180 0
			TestAxisAngleQuatConversions(AxisAngle(180, 0, 0, 1), Quaternion(0, 0, 0, 1));//0   0   180

			TestAxisAngleQuatConversions(AxisAngle(180, -1, 0, 0), Quaternion(0, -1, 0, 0));//180 0   0
			TestAxisAngleQuatConversions(AxisAngle(180, 0, -1, 0), Quaternion(0, 0, -1, 0));//0   180 0
			TestAxisAngleQuatConversions(AxisAngle(180, 0, 0, -1), Quaternion(0, 0, 0, -1));//0   0   180

			TestAxisAngleQuatConversions(AxisAngle(360, 0, 1, 0), Quaternion(-1, 0, 0, 0));//360 0   0
			TestAxisAngleQuatConversions(AxisAngle(360, 0, 1, 0), Quaternion(-1, 0, 0, 0));//0   360 0
			TestAxisAngleQuatConversions(AxisAngle(360, 0, 1, 0), Quaternion(-1, 0, 0, 0));//0   0   360

																								   //Possibly strange internal values
			TestAxisAngleQuatConversions(AxisAngle(90, 1, 0, 0), Quaternion(0.707, 0.707, 0, 0));//90 0  0
			TestAxisAngleQuatConversions(AxisAngle(90, 0, 1, 0), Quaternion(0.707, 0, 0.707, 0));//0  90 0
			TestAxisAngleQuatConversions(AxisAngle(90, 0, 0, 1),  Quaternion(0.707, 0, 0, 0.707));//0  0  90

			TestAxisAngleQuatConversions(AxisAngle(90, -1, 0, 0), Quaternion(0.707, -0.707, 0, 0));//90 0  0
			TestAxisAngleQuatConversions(AxisAngle(90, 0, -1, 0), Quaternion(0.707, 0, -0.707, 0));//0  90 0
			TestAxisAngleQuatConversions(AxisAngle(90, 0, 0, -1), Quaternion(0.707, 0, 0, -0.707));//0  0  90
		}

		void TestAxisAngleQuatConversions(AxisAngle aa, Quaternion q)
		{
			TestAxisAngleQuatConversion(aa, q);
			TestQuatAxisAngleConversion(aa, q);
		}

		void TestQuatAxisAngleConversion(AxisAngle axisAngle, Quaternion q)
		{
			Quaternion quaternion = Rotation(axisAngle).GetQuaternion();

			Print(q.ToString() + " | " + quaternion.ToString() + " | " + q.Subtract(quaternion).ToString());

			Assert::AreEqual(q.W, quaternion.W, 0.05, L"Bad translation in W dimension");
			Assert::AreEqual(q.X, quaternion.X, 0.05, L"Bad translation in X dimension");
			Assert::AreEqual(q.Y, quaternion.Y, 0.05, L"Bad translation in Y dimension");
			Assert::AreEqual(q.Z, quaternion.Z, 0.05, L"Bad translation in Z dimension");
		}

		void TestAxisAngleQuatConversion(AxisAngle axisAngle, Quaternion q)
		{
			AxisAngle aa = Rotation(q).GetAxisAngle();

			Print(aa.ToString() + " | " + axisAngle.ToString());

			Assert::AreEqual(axisAngle.Rotation, aa.Rotation, 0.1, L"Bad translation in R rotation ");
			Assert::AreEqual(axisAngle.Axis.X, aa.Axis.X, 0.05, L"Bad translation in X dimension");
			Assert::AreEqual(axisAngle.Axis.Y, aa.Axis.Y, 0.05, L"Bad translation in Y dimension");
			Assert::AreEqual(axisAngle.Axis.Z, aa.Axis.Z, 0.05, L"Bad translation in Z dimension");
		}
	};
}