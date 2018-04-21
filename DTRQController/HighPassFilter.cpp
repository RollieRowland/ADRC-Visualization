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

	if ((int)samples.size() < memory) {
		return value;
	}

	if ((signed int)samples.size() > memory) {
		samples.erase(samples.begin());
	}

	std::vector<std::complex<double>> complex;

	std::cout << "Setting Real" << std::endl;

	FastFourierTransform::SetRealValues(&complex, samples);

	std::cout << "Performing Fourier" << std::endl;

	//COMPLEX SPACE
	FastFourierTransform::FFT(&complex);

	std::cout << "Getting imaginary" << std::endl;

	std::vector<double> imaginary = FastFourierTransform::GetImagValues(complex);

	double ns = (double)imaginary.size() / 2.0;
	double cutoff = frequency / ns;

	std::cout << "Cutoff:" << cutoff << " NS:" << ns << " F:" << frequency << " " << complex.size() << " " << imaginary.size() << " " << samples.size() << std::endl;

	if (frequency > ns) {
		return value;
	}

	for (int i = 0; i < (int)((double)imaginary.size() * cutoff); i++) {
		imaginary.at(i) = 0;
	}


	FastFourierTransform::SetImagValues(&complex, imaginary);

	FastFourierTransform::IFFT(&complex);

	std::vector<double> real = FastFourierTransform::GetRealValues(complex);

	return real.at((int)(real.size() / 2));
}