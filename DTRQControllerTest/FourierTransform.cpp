#include "stdafx.h"
#include "CppUnitTest.h"
#include <FastFourierTransform.h>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace DTRQControllerTest
{
	TEST_CLASS(FourierTransformTest) {
	public:
		void Print(std::string str) {
			Logger::WriteMessage((str + "\n").c_str());
		}

		TEST_METHOD(TestFourier) {
			double realSet1[] = { 1, 0, 0, 0,  0, 0, 0, 0 };
			double imagSet1[] = { 0, 0, 0, 0,  0, 0, 0, 0 };

			double realSet2[] = { 0.125, 0.125, 0.125, 0.125,  0.125, 0.125, 0.125, 0.125 };
			double imagSet2[] = { 0, 0, 0, 0,  0, 0, 0, 0 };

			double realSet3[] = { 0, 1, 0, 0,  0, 0, 0, 0 };
			double imagSet3[] = { 0, -0.707, -1, -0.707,  0, 0.707, 1, 0.707 };

			double realSet4[] = { 0.125, 0.088, 0, -0.088,  -0.125, -0.088, 0, 0.088 };
			double imagSet4[] = { 0, -0, 0, 0,  0, 0, 0, -0 };

			TestFFT(realSet1, imagSet1);
			TestFFT(realSet2, imagSet2);
			TestFFT(realSet3, imagSet3);
			TestFFT(realSet4, imagSet4);
		}

		void TestFFT(double real[8], double imag[8]) {
			std::vector<std::complex<double>> re;

			for (int i = 0; i < 8; i++) {
				re.push_back(std::complex<double>(real[i], 0.0));
			}

			FastFourierTransform::FFT(&re);

			for (int i = 0; i < 8; i++) {
				std::string a = Mathematics::DoubleToCleanString(imag[i]);
				std::string b = Mathematics::DoubleToCleanString(re.at(i).imag());

				Print("I:" + a + " " + b);

				Assert::AreEqual(imag[i], re.at(i).imag(), 0.005, L"R2C");
			}

			Print("");

			FastFourierTransform::IFFT(&re);

			for (int i = 0; i < 8; i++) {
				std::string a = Mathematics::DoubleToCleanString(real[i]);
				std::string b = Mathematics::DoubleToCleanString(re.at(i).real());

				Print("R:" + a + " " + b);

				Assert::AreEqual(real[i], re.at(i).real(), 0.005, L"C2R");
			}

			Print("");
		}

	};
}