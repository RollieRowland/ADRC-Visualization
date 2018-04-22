#include "FastFourierTransform.h"

//Cooley-Tukey method
void FastFourierTransform::FFT(std::complex<double> *real, int length) {
	Perform(real, length, false);
}

void FastFourierTransform::IFFT(std::complex<double> *imag, int length, bool scale) {
	Perform(imag, length, true);

	if (scale) {
		Scale(imag, length);
	}
}

void FastFourierTransform::Perform(std::complex<double> *data, int length, bool inverse) {
	double flip = 1.0;
	if (inverse) {
		flip = -1.0;
	}

	if (length < 2) {
		// bottom of recursion.
		// Do nothing here, because already X[0] = x[0]
	}
	else {
		Rearrange(data, length);// all evens to lower half, all odds to upper half
		Perform(data, length / 2, inverse);// recurse even items
		Perform(data + length / 2, length / 2, inverse);// recurse odd  items
		
		// combine results of two half recursions
		for (int k = 0; k < length / 2; k++) {
			std::complex<double> e = data[k];// even
			std::complex<double> o = data[k + length / 2];// odd
			std::complex<double> w = exp(std::complex<double>(0, -2.0 * Mathematics::PI * flip * k / length));// w is the "twiddle-factor"

			data[k] = e + w * o;
			data[k + length / 2] = e - w * o;
		}
	}
}

void FastFourierTransform::Rearrange(std::complex<double> *a, int length) {
	std::complex<double>* b = new std::complex<double>[length / 2];  // get temp heap storage

	for (int i = 0; i < length / 2; i++) {
		// copy all odd elements to heap storage
		b[i] = a[i * 2 + 1];
	}    
	for (int i = 0; i < length / 2; i++) {
		// copy all even elements to lower-half of a[]
		a[i] = a[i * 2];
	}    
	for (int i = 0; i < length / 2; i++) {
		// copy all odd (from heap) to upper-half of a[]
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
