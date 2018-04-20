#include "QuaternionKalmanFilter.h"

QuaternionKalmanFilter::QuaternionKalmanFilter() {
	gain = 0.25;
	memory = 25;
}

QuaternionKalmanFilter::QuaternionKalmanFilter(double gain, int memory) {
	this->gain = gain;
	this->memory = memory;
}

Quaternion QuaternionKalmanFilter::Filter(Quaternion value) {
	values.push_back(value);

	if ((signed int)values.size() > memory) {
		values.erase(values.begin());
	}

	Quaternion out = Quaternion(0, 0, 0, 0);

	for (std::vector <Quaternion>::iterator i = values.begin(); i != values.end(); ++i) {
		//out = Quaternion::SphericalInterpolation(out, *i, gain);
		out = out.Add( (*i).Divide(values.size()) );
	}

	out = out.UnitQuaternion();

	return Quaternion::SphericalInterpolation(value, out, 1 - gain);
}
