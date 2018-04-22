#include "HighPassFilter.h"

HighPassFilter::HighPassFilter() {
	this->frequency = 50;
	this->memory = 25;
}

HighPassFilter::HighPassFilter(double frequency, int memory) {
	this->frequency = frequency;
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

	FastFourierTransform::SetRealValues(complex, tempSamples, length);

	std::cout << "Performing Fourier" << std::endl;

	//COMPLEX SPACE
	FastFourierTransform::FFT(complex, length);

	std::cout << "Getting imaginary" << std::endl;

	double* imaginary = FastFourierTransform::GetImagValues(complex, length);

	double ns = (double)samples.size() / 2.0;
	double cutoff = frequency / ns;

	std::cout << "Cutoff:" << cutoff << " NS:" << ns << " F:" << frequency << " " << samples.size() << std::endl;

	if (frequency > ns) {
		return value;
	}

	for (int i = 0; i < (int)(length * cutoff); i++) {
		imaginary[i] = 0;
	}

	FastFourierTransform::SetImagValues(complex, imaginary, length);

	FastFourierTransform::IFFT(complex, length);

	double* real = FastFourierTransform::GetRealValues(complex, length);

	double temp = real[(int)(length / 2)];

	delete[] imaginary;
	delete[] real;
	delete[] complex;
	delete[] tempSamples;

	return temp;
}

double* HighPassFilter::GetSamples() {
	double* out = new double[samples.size()];

	for (int i = 0; i < samples.size(); i++) {
		out[i] = samples.at(i);
	}

	return out;
}
