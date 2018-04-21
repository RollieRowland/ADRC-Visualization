#include "LeastSquares.h"

LeastSquares::LeastSquares() {
	this->memory = 25;
}

LeastSquares::LeastSquares(int memory) {
	this->memory = memory;
}

double LeastSquares::Calculate(double x, double y, double target) {
	X.push_back(x);
	Y.push_back(y);

	if ((signed int)X.size() > memory) {
		X.erase(X.begin());
		Y.erase(Y.begin());
	}

	if (X.size() == 0) {
		return 1.0;
	}
	else {
		double sx = 0, sx2 = 0, sxy = 0, sy = 0, sy2 = 0;

		for (unsigned int i = 0; i < X.size(); i++) {
			sx  += X.at(i);
			sx2 += pow(X.at(i), 2.0);
			sxy += X.at(i) * Y.at(i);
			sy  += Y.at(i);
			sy2 += pow(Y.at(i), 2.0);
		}

		double denom = (X.size() * sx2 - pow(sx, 2.0));

		if (denom == 0) {
			slope = 0;
			intercept = 0;
		}
		else {
			slope = (X.size() * sxy - sx * sy) / denom;
			intercept = (sy * sx2 - sx * sxy) / denom;
		}

		return slope * target + intercept;
	}
}
