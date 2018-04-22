#include "stdafx.h"
#include "CppUnitTest.h"
#include <FastFourierTransform.h>
#include <HighPassFilter.h>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace DTRQControllerTest
{
	TEST_CLASS(FourierTransformTest) {
	public:
		void Print(std::string str) {
			Logger::WriteMessage((str + "\n").c_str());
		}

		TEST_METHOD(TestHighPassFilter) {
			int samples = 10000;
			double samplingFrequency = 44100;
			double hp = 500;
			double generateFrequency = 40000;

			HighPassFilter hpf = HighPassFilter(hp, samples - 1);

			double* sineWave = new double[samples];
			double* filteredWave = new double[samples];

			for (int i = 0; i < samples; i++) {
				sineWave[i] = sin(generateFrequency * (2.0 * Mathematics::PI) * double(i) / samplingFrequency);
				hpf.Filter(sineWave[i]);
			}

			Print("Getting samples");

			filteredWave = hpf.GetSamples();

			//generate fourier complex samples
			std::complex<double>* filtered = new std::complex<double>[samples];  // get temp heap storage
			std::complex<double>* unfiltered = new std::complex<double>[samples];  // get temp heap storage

			for (int i = 0; i < samples; i++) {
				// copy all odd elements to heap storage
				filtered[i] = std::complex<double>(filteredWave[i], 0.0);
				unfiltered[i] = std::complex<double>(sineWave[i], 0.0);
			}

			Print("Performing FFT");

			FastFourierTransform::FFT(filtered, 10000);
			FastFourierTransform::FFT(unfiltered, 10000);

			double* filt = FastFourierTransform::GetImagValues(filtered, 10000);
			double* unfi = FastFourierTransform::GetImagValues(unfiltered, 10000);

			for (int i = 0; i < samples; i++) {
				Print(Mathematics::DoubleToCleanString(filt[i]) + ", " + Mathematics::DoubleToCleanString(unfi[i]));
			}

			delete[] filtered;
			delete[] unfiltered;
			delete[] filt;
			delete[] unfi;
			delete[] sineWave;
			delete[] filteredWave;
		}

		TEST_METHOD(TestFourierDoubleConversion) {
			double realSet[] = { 1, 0, 0, 0,  0, 0, 0, 0, 
								  0.125, 0.125, 0.125, 0.125,  0.125, 0.125, 0.125, 0.125,
								  0, 1, 0, 0,  0, 0, 0, 0,
								  0.125, 0.088, 0, -0.088,  -0.125, -0.088, 0, 0.088 };

			TestFFTDouble(realSet);
		}

		void TestFFTDouble(double real[32]) {
			std::complex<double>* re = new std::complex<double>[32];  // get temp heap storage

			for (int i = 0; i < 32; i++) {
				// copy all odd elements to heap storage
				re[i] = std::complex<double>(real[i], 0.0);
			}

			FastFourierTransform::FFT(re, 32);
			FastFourierTransform::IFFT(re, 32);

			for (int i = 0; i < 32; i++) {
				std::string a = Mathematics::DoubleToCleanString(real[i]);
				std::string b = Mathematics::DoubleToCleanString(re[i].real());

				Print("R:" + a + " " + b);

				Assert::AreEqual(real[i], re[i].real(), 0.05, L"C2R");
			}

			Print("");

			delete[] re;
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
			std::complex<double>* re = new std::complex<double>[8];  // get temp heap storage

			for (int i = 0; i < 8; i++) {
				// copy all odd elements to heap storage
				re[i] = std::complex<double>(real[i], 0.0);
			}

			FastFourierTransform::FFT(re, 8);

			for (int i = 0; i < 8; i++) {
				std::string a = Mathematics::DoubleToCleanString(imag[i]);
				std::string b = Mathematics::DoubleToCleanString(re[i].imag());

				Print("I:" + a + " " + b);

				Assert::AreEqual(imag[i], re[i].imag(), 0.005, L"R2C");
			}

			Print("");

			FastFourierTransform::IFFT(re, 8);

			for (int i = 0; i < 8; i++) {
				std::string a = Mathematics::DoubleToCleanString(real[i]);
				std::string b = Mathematics::DoubleToCleanString(re[i].real());

				Print("R:" + a + " " + b);

				Assert::AreEqual(real[i], re[i].real(), 0.005, L"C2R");
			}

			Print("");

			delete[] re;
		}

	};
}