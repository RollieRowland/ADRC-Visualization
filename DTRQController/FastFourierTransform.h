#pragma once

#include <complex>
#include <iostream>
#include <valarray>
#include "Mathematics.h"
#include <vector>

class FastFourierTransform {
public:
	static void FFT(std::complex<double> *real, int length);
	static void IFFT(std::complex<double> *imag, int length, bool scale);
	static void Perform(std::complex<double> *data, int length, bool inverse);
	static void Rearrange(std::complex<double> *data, int length);
	static void Scale(std::complex<double> *imag, int length);
	
	static double* GetRealValues(std::complex<double>* complex, int length);
	static double* GetImagValues(std::complex<double>* complex, int length);

	static void SetRealValues(std::complex<double>* complex, double* real, int length);
	static void SetImagValues(std::complex<double>* complex, double* imag, int length);
	
private:


};

