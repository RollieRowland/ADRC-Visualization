#include <Vector.h>

Vector3D::Vector3D(const Vector3D& vector) {
	this->X = vector.X;
	this->Y = vector.Y;
	this->Z = vector.Z;
}

Vector3D::Vector3D(double X, double Y, double Z) {
	this->X = X;
	this->Y = Y;
	this->Z = Z;
}

Vector3D Vector3D::Normal() {
	return Multiply(this->Magnitude() == 0 ? numeric_limits<double>::infinity() : 1 / this->Magnitude());
}

Vector3D Vector3D::Add(Vector3D vector) {

}

Vector3D Vector3D::Subtract(Vector3D vector) {

}

Vector3D Vector3D::Multiply(Vector3D vector) {

}

Vector3D Vector3D::Divide(Vector3D vector) {

}

Vector3D Vector3D::Multiply(double scalar) {

}

Vector3D Vector3D::Divide(double scalar) {

}

Vector3D Vector3D::CrossProduct(Vector3D vector) {

}

Vector3D Vector3D::Normalize() {

}

double Vector3D::Magnitude() {

}

double Vector3D::GetLength() {

}

double Vector3D::DotProduct(Vector3D vector) {

}

double Vector3D::CalculateEuclideanDistance(Vector3D vector) {

}

bool Vector3D::IsEqual(Vector3D vector) {

}

string Vector3D::ToString() {

}