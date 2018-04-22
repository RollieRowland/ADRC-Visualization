#include "stdafx.h"
#include "CppUnitTest.h"
#include <FastFourierTransform.h>
#include <FiniteImpulseResponse.h>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace DTRQControllerTest
{
	TEST_CLASS(FiniteImpulseResponseTest) {
	public:
		void Print(std::string str) {
			Logger::WriteMessage((str + "\n").c_str());
		}

		TEST_METHOD(TestFIR) {
			int samples = 10000;
			double samplingFrequency = 44100;
			double hpFrequency = 100;
			double generateFrequency = 20;

			FiniteImpulseResponse hpf = FiniteImpulseResponse(FiniteImpulseResponse::High, 100, samplingFrequency, hpFrequency, 44100);

			double* sineWave = new double[samples];
			double* filteredWave = new double[samples];

			for (int i = 0; i < samples; i++) {
				sineWave[i] = sin(generateFrequency * (2.0 * Mathematics::PI) * double(i) / samplingFrequency);
				filteredWave[i] = hpf.Filter(sineWave[i]);
			}

			Print("Getting samples");

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

	};
}