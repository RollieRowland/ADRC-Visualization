#include "stdafx.h"
#include "CppUnitTest.h"
#include <Quaternion.h>
#include <DirectionAngle.h>
#include <Rotation.h>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace DTRQControllerTest
{		
	TEST_CLASS(DirectionAngleTest)
	{
	public:
		void Print(std::string str) {
			Logger::WriteMessage((str + "\n").c_str());
		}

		TEST_METHOD(TestDirectionAngleQuaternionDoubleConversion)
		{
			DirectionAngleQuaternionDoubleConversion(Quaternion(0.9239, 0.3827, 0, 0));
			DirectionAngleQuaternionDoubleConversion(Quaternion(0.9239, -0.3827, 0, 0));
			DirectionAngleQuaternionDoubleConversion(Quaternion(0.9239, 0, 0.3827, 0));
			DirectionAngleQuaternionDoubleConversion(Quaternion(0.9239, 0, -0.3827, 0));
			DirectionAngleQuaternionDoubleConversion(Quaternion(0.9239, 0, 0, 0.3827));
			DirectionAngleQuaternionDoubleConversion(Quaternion(0.9239, 0, 0, -0.3827));
			DirectionAngleQuaternionDoubleConversion(Quaternion(0.6533, 0.2706, 0.6533, 0.2706));
			DirectionAngleQuaternionDoubleConversion(Quaternion(0.6533, -0.2706, 0.6533, -0.2706));
			DirectionAngleQuaternionDoubleConversion(Quaternion(0.6533, -0.2706, -0.6533, 0.2706));
		}

		void DirectionAngleQuaternionDoubleConversion(Quaternion init)
		{
			DirectionAngle dA = Rotation(init).GetDirectionAngle();
			Quaternion post = Rotation(dA).GetQuaternion();

			Assert::AreEqual(init.W, post.W, 0.01, L"DAQC W");
			Assert::AreEqual(init.X, post.X, 0.01, L"DAQC X");
			Assert::AreEqual(init.Y, post.Y, 0.01, L"DAQC Y");
			Assert::AreEqual(init.Z, post.Z, 0.01, L"DAQC Z");
		}

		TEST_METHOD(TestDirectionAngleToQuaternion)
		{
			Print("X Axis");

			for (int i = -180; i <= 180; i += 30)
			{
				for (int k = -180; k <= 180; k += 30)
				{
					Print(Rotation(EulerAngles(Vector3D(i, k, 0), EulerConstants::EulerOrderYXZS)).GetQuaternion().ToString());
				}
			}

			Print("Z Axis");

			for (int i = -180; i <= 180; i += 30)
			{
				for (int k = -180; k <= 180; k += 30)
				{
					Print(Rotation(EulerAngles(Vector3D(0, k, i), EulerConstants::EulerOrderYZXS)).GetQuaternion().ToString());
				}
			}

			Print("Test Cases");

			DirectionAngleQuaternion(DirectionAngle(0, Vector3D(0, 0.707, 0.707)), Quaternion(0.9239, 0.3827, 0, 0)); //XYZ: 45, 0, 0
			DirectionAngleQuaternion(DirectionAngle(0, Vector3D(-0.707, 0.707, 0)), Quaternion(0.9239, 0, 0, 0.3827));//XYZ: 0,   0,   45
			DirectionAngleQuaternion(DirectionAngle(0, Vector3D(0.707, 0.707, 0)), Quaternion(0.9239, 0, 0, -0.3827));//XYZ: 0, 0,   -45
			DirectionAngleQuaternion(DirectionAngle(90, Vector3D(0, 0.707, 0.707)), Quaternion(0.6533, 0.2706, 0.6533, 0.2706));//YXZ: 0, 90, 45
			DirectionAngleQuaternion(DirectionAngle(0, Vector3D(0, 1, 0)), Quaternion(1, 0, 0, 0));//YXZ: 0, 90, 45
			DirectionAngleQuaternion(DirectionAngle(0, Vector3D(0, -1, 0)), Quaternion(0, 1, 0, 0));//YXZ: 0, 90, 45
		}

		void DirectionAngleQuaternion(DirectionAngle expected, Quaternion q)
		{
			DirectionAngle nonStandardAA = Rotation(q).GetDirectionAngle();

			Print(nonStandardAA.ToString());

			Assert::AreEqual(expected.Rotation, nonStandardAA.Rotation, 0.1, L"Bad translation in R rotation ");
			Assert::AreEqual(expected.Direction.X, nonStandardAA.Direction.X, 0.05, L"Bad translation in X dimension");
			Assert::AreEqual(expected.Direction.Y, nonStandardAA.Direction.Y, 0.05, L"Bad translation in Y dimension");
			Assert::AreEqual(expected.Direction.Z, nonStandardAA.Direction.Z, 0.05, L"Bad translation in Z dimension");
		}
	};
}