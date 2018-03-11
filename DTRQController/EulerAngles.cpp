#include <EulerAngles.h>
#include <EulerConstants.h>
#include <Math.h>

EulerAngles::EulerAngles() {
	Angles = Vector3D(0, 0, 0);
	Order = EulerConstants::EulerOrderXYZS;
}

EulerAngles::EulerAngles(Vector3D angles, EulerOrder order) {
	Angles = angles;
	Order = order;
}