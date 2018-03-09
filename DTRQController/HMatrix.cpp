#include <HMatrix.h>
#include <math.h>
#include <Math.h>


HMatrix::HMatrix() {
	for (int i = 0; i < 3; i++)
	{
		for (int k = 0; k < 3; k++)
		{
			hierarchicalMatrix[i][k] = 0;
		}
	}
}

HMatrix::HMatrix(vector<vector<double>> hMatrix) {
	hierarchicalMatrix = hMatrix;
}

HMatrix HMatrix::EulerToHMatrix(EulerAngles eulerAngles) {
	HMatrix hM = HMatrix();
	double sx, sy, sz, cx, cy, cz, cc, cs, sc, ss; Vector3D p = eulerAngles.Order.Permutation;

	eulerAngles.Angles.X = Math::DegreesToRadians(eulerAngles.Angles.X);
	eulerAngles.Angles.Y = Math::DegreesToRadians(eulerAngles.Angles.Y);
	eulerAngles.Angles.Z = Math::DegreesToRadians(eulerAngles.Angles.Z);

	if (eulerAngles.Order.FrameTaken == EulerOrder::AxisFrame::Rotating)
	{
		double t = eulerAngles.Angles.X;
		eulerAngles.Angles.X = eulerAngles.Angles.Z;
		eulerAngles.Angles.Z = t;
	}

	if (eulerAngles.Order.AxisPermutation == EulerOrder::Parity::Odd)
	{
		eulerAngles.Angles.X = -eulerAngles.Angles.X;
		eulerAngles.Angles.Y = -eulerAngles.Angles.Y;
		eulerAngles.Angles.Z = -eulerAngles.Angles.Z;
	}

	sx = sin(eulerAngles.Angles.X);
	sy = sin(eulerAngles.Angles.Y);
	sz = sin(eulerAngles.Angles.Z);
	cx = cos(eulerAngles.Angles.X);
	cy = cos(eulerAngles.Angles.Y);
	cz = cos(eulerAngles.Angles.Z);

	cc = cx * cz;
	cs = cx * sz;
	sc = sx * cz;
	ss = sx * sz;

	if (eulerAngles.Order.InitialAxisRepetition == EulerOrder::AxisRepetition::Yes)
	{
		hM(p.X, p.X) = cy; hM(p.X, p.Y) = sy * sx; hM(p.X, p.Z) = sy * cx; hM(0, 3) = 0;
		hM(p.Y, p.X) = sy * sz; hM(p.Y, p.Y) = -cy * ss + cc; hM(p.Y, p.Z) = -cy * cs - sc; hM(1, 3) = 0;
		hM(p.Z, p.X) = -sy * cz; hM(p.Z, p.Y) = cy * sc + cs; hM(p.Z, p.Z) = cy * cc - ss; hM(2, 3) = 0;
		hM(3, 0) = 0; hM(3, 1) = 0; hM(3, 2) = 0; hM(3, 3) = 1;
	}
	else
	{
		hM(p.X, p.X) = cy * cz; hM(p.X, p.Y) = sy * sc - cs; hM(p.X, p.Z) = sy * cc + ss; hM(0, 3) = 0;
		hM(p.Y, p.X) = cy * sz; hM(p.Y, p.Y) = sy * ss + cc; hM(p.Y, p.Z) = sy * cs - sc; hM(1, 3) = 0;
		hM(p.Z, p.X) = -sy; hM(p.Z, p.Y) = cy * sx; hM(p.Z, p.Z) = cy * cx; hM(2, 3) = 0;
		hM(3, 0) = 0; hM(3, 1) = 0; hM(3, 2) = 0; hM(3, 3) = 1;
	}

	return hM;
}