#include "HighPassFilter.h"

HighPassFilter::HighPassFilter() {
	this->cutoffFrequency = 50;
	this->samplingFrequency = 1000;
	this->memory = 25;
}

HighPassFilter::~HighPassFilter() {
	delete[] previousTransform;
}

HighPassFilter::HighPassFilter(double samplingFrequency, double cutoffFrequency, int memory) {
	this->cutoffFrequency = cutoffFrequency;
	this->samplingFrequency = samplingFrequency;
	this->memory = memory;
}

double HighPassFilter::Filter(double value) {
	samples.push_back(value);
	int length = samples.size();

	if (length < memory) {
		return value;
	}

	if (length > memory) {
		samples.erase(samples.begin());
		length = samples.size();
	}
	
	std::complex<double>* complex = new std::complex<double>[length];

	std::cout << "Setting Real" << std::endl;

	double* tempSamples = new double[length];

	for (int i = 0; i < length; i++) {
		tempSamples[i] = samples.at(i);
	}

	FastFourierTransform::SetRealValues(complex, tempSamples, length);

	std::cout << "Performing Fourier" << std::endl;

	//COMPLEX SPACE
	FastFourierTransform::FFT(complex, length);

	std::cout << "Getting imaginary" << std::endl;

	double* imaginary = FastFourierTransform::GetImagValues(complex, length);
	double* real = FastFourierTransform::GetRealValues(complex, length);

	double ns = (double)samples.size() / 2.0;
	double cutoffRatio = cutoffFrequency / samplingFrequency;

	std::cout << "Cutoff:" << cutoffRatio << " NS:" << ns << " CF:" << cutoffFrequency << " " << samples.size() << std::endl;

	if (cutoffFrequency > ns) {
		return value;
	}

	double range = ns * cutoffRatio / 2.0;

	for (int i = 0; i < int(range); i++) {
		imaginary[i] = 0;
		real[i] = 0;
	}

	for (int i = length - 1; i > length - int(range); i--) {
		imaginary[i] = 0;
		real[i] = 0;
	}

	FastFourierTransform::SetImagValues(complex, imaginary, length);
	FastFourierTransform::SetRealValues(complex, real, length);

	FastFourierTransform::IFFT(complex, length, true);

	previousTransform = FastFourierTransform::GetRealValues(complex, length);

	double temp = previousTransform[(int)(length / 2)];

	delete[] imaginary;
	delete[] real;
	delete[] complex;
	delete[] tempSamples;

	return temp;
}

double* HighPassFilter::GetSamples() {
	return previousTransform;
}
