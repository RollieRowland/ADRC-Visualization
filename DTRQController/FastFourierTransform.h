#pragma once

#include <complex>
#include <iostream>
#include <valarray>
#include "Mathematics.h"
#include <vector>

class FastFourierTransform {
public:
	static void FFT(std::vector<std::complex<double>> *real);
	static void IFFT(std::vector<std::complex<double>> *imag);
	static void Perform(std::vector<std::complex<double>> *data, bool inverse);
	static void Rearrange(std::vector<std::complex<double>> *data);
	static void Scale(std::vector<std::complex<double>> *imag);

	static std::vector<double> GetRealValues(std::vector<std::complex<double>> complex);
	static std::vector<double> GetImagValues(std::vector<std::complex<double>> complex);

	static void SetRealValues(std::vector<std::complex<double>> *complex, std::vector<double> real);
	static void SetImagValues(std::vector<std::complex<double>> *complex, std::vector<double> imag);

private:


};

