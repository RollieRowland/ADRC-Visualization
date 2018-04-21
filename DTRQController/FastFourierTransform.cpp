#include "FastFourierTransform.h"

//Cooley-Tukey method
void FastFourierTransform::FFT(std::vector<std::complex<double>> *real) {
	Rearrange(real);
	Perform(real, false);
}

void FastFourierTransform::IFFT(std::vector<std::complex<double>> *imag) {
	Rearrange(imag);
	Perform(imag, true);
	Scale(imag);
}

void FastFourierTransform::Perform(std::vector<std::complex<double>> *data, bool inverse) {
	const double pi = inverse ? 3.14159265358979323846 : -3.14159265358979323846;
	unsigned int length = data->size() - 1;

	//iterate through multiples
	for (unsigned int step = 1; step < length; step <<= 1) {
		const unsigned int jump = step << 1;//next entry of transform factor
		const double delta = pi / double(step);//increment angle
		const double sine = sin(delta * 0.5);//delta sine auxiliary
		const std::complex<double> multiplier(-2.0 * sine * sine, sin(delta));//multiple factor

		std::complex<double> factor(1.0, 0.0);//start transform factor

		//iterate through groups of transform factors
		for (unsigned int group = 0; group < step; ++group) {
			//group iteration
			for (unsigned int pair = group; pair < length; pair += jump)
			{
				const unsigned int match = pair + step;//match position
				std::cout << "match" << match << " " << pair << " " << step << std::endl;
				const std::complex<double> product(factor * data->at(match));//two-point transform second term
				std::cout << "  product " << product.real() << " " << product.imag() << std::endl;

				data->at(match) = data->at(pair) - product;//transform for fi + pi

				data->at(pair) += product;//transform for fi
			}

			//successive transform factor via trigonometric recurrence
			factor = multiplier * factor + factor;
		}
	}
}

void FastFourierTransform::Rearrange(std::vector<std::complex<double>> *data) {
	unsigned int target = 0;//swap position
	unsigned int length = data->size() - 1;

	//check all positions of signal
	for (unsigned int position = 0; position < length; ++position){
		if (target > position){//swaps if not swapped
			std::complex<double> temp(data->at(target));

			data->at(target) = data->at(position);
			data->at(position) = temp;
		}

		unsigned int mask = length;//bit mask

		//while bit is set
		while (target & (mask >>= 1)) {
			target &= ~mask;//drop
		}

		target |= mask;//set current bit, value is 0
	}
}

void FastFourierTransform::Scale(std::vector<std::complex<double>> *imag) {
	const double scale = 1.0 / (double)imag->size();

	for (unsigned int position = 0; position < imag->size(); ++position) {
		imag->at(position) *= scale;
	}
}


std::vector<double> FastFourierTransform::GetRealValues(std::vector<std::complex<double>> complex) {
	std::vector<double> real;

	for (unsigned int i = 0; i < complex.size(); i++) {
		real.push_back(complex.at(i).real());
	}

	return real;
}

std::vector<double> FastFourierTransform::GetImagValues(std::vector<std::complex<double>> complex) {
	std::vector<double> imag;

	for (unsigned int i = 0; i < complex.size(); i++) {
		imag.push_back(complex.at(i).imag());
	}

	return imag;
}

void FastFourierTransform::SetRealValues(std::vector<std::complex<double>> *complex, std::vector<double> real) {
	bool empty = false;

	if (complex->size() < 1u) {
		empty = true;
	}

	for (unsigned int i = 0; i < real.size(); i++) {
		if (empty) {
			complex->push_back(std::complex<double>(real.at(i), 0.0));
		}
		else {
			complex->at(i) = std::complex<double>(real.at(i), complex->at(i).imag());
		}
	}
}

void FastFourierTransform::SetImagValues(std::vector<std::complex<double>> *complex, std::vector<double> imag) {
	bool empty = false;

	if (complex->size() < 1u) {
		empty = true;
	}

	for (unsigned int i = 0; i < imag.size(); i++) {
		if (empty) {
			complex->push_back(std::complex<double>(0, imag.at(i)));
		}
		else {
			complex->at(i) = std::complex<double>(complex->at(i).real(), imag.at(i));
		}
	}
}