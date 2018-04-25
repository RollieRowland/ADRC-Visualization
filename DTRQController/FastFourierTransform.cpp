#include "FastFourierTransform.h"

//Cooley-Tukey method
void FastFourierTransform::FFT(std::complex<double> *real, int length) {
	Perform(real, length, false);
}

void FastFourierTransform::IFFT(std::complex<double> *imag, int length, bool scale) {
	Perform(imag, length, true);

	//scales outputs of transform due to change in size during forward pass
	if (scale) {
		Scale(imag, length);
	}
}

void FastFourierTransform::Perform(std::complex<double> *data, int length, bool inverse) {
	double flip = 1.0;//forward Fourier transform

	//inverse Fourier transform
	if (inverse) {
		flip = -1.0;
	}

	if (length >= 2) {
		Rearrange(data, length);//rearranges data
		Perform(data, length / 2, inverse);//recursion on even data
		Perform(data + length / 2, length / 2, inverse);//recursion on odd data

		//Joint two bottom recursions
		for (int k = 0; k < length / 2; k++) {
			std::complex<double> even = data[k];
			std::complex<double> odd = data[k + length / 2];
			std::complex<double> twiddle = exp(std::complex<double>(0, -2.0 * Mathematics::PI * flip * k / length));

			data[k] = even + twiddle * odd;
			data[k + length / 2] = even - twiddle * odd;
		}
	}

	//else the recursion ends, x at zero is x at zero
}

void FastFourierTransform::Rearrange(std::complex<double> *a, int length) {
	std::complex<double>* b = new std::complex<double>[length / 2];

	//Splits odd an even elements into the lower and upper halves of the output data, respectively
	for (int i = 0; i < length / 2; i++) {
		b[i] = a[i * 2 + 1];
	}

	for (int i = 0; i < length / 2; i++) {
		a[i] = a[i * 2];
	}

	for (int i = 0; i < length / 2; i++) {
		a[i + length / 2] = b[i];
	}    

	delete[] b;
}

void FastFourierTransform::Scale(std::complex<double> *imag, int length) {
	const double scale = 1.0 / (double)length;

	for (int position = 0; position < length; ++position) {
		imag[position] *= scale;
	}
}

double* FastFourierTransform::GetRealValues(std::complex<double>* complex, int length) {
	double* real = new double[length];

	for (int i = 0; i < length; i++) {
		real[i] = complex[i].real();
	}

	return real;
}

double* FastFourierTransform::GetImagValues(std::complex<double>* complex, int length) {
	double* imag = new double[length];

	for (int i = 0; i < length; i++) {
		imag[i] = complex[i].imag();
	}

	return imag;
}

void FastFourierTransform::SetRealValues(std::complex<double>* complex, double* real, int length) {
	for (int i = 0; i < length; i++) {
		complex[i] = std::complex<double>(real[i], complex[i].imag());
	}
}

void FastFourierTransform::SetImagValues(std::complex<double>* complex, double* imag, int length) {
	for (int i = 0; i < length; i++) {
		complex[i] = std::complex<double>(complex[i].real(), imag[i]);
	}
}
