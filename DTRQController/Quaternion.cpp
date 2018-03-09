#include <Quaternion.h>

Quaternion::Quaternion(const Quaternion& quaternion) {

}

Quaternion::Quaternion(Vector3D vector) {

}

Quaternion::Quaternion(double W, double X, double Y, double Z) {

}

Vector3D Quaternion::RotateVector(Vector3D coordinate) {

}
Vector3D Quaternion::UnrotateVector(Vector3D coordinate) {

}
Vector3D Quaternion::GetBiVector() {

}

Quaternion Quaternion::EulerToQuaternion(EulerAngles eulerAngles) {

}

Quaternion Quaternion::AxisAngleToQuaternion(AxisAngle axisAngle) {

}

Quaternion Quaternion::DirectionAngleToQuaternion(DirectionAngle directionAngle) {

}

Quaternion Quaternion::RotationMatrixToQuaternion(RotationMatrix rotationMatrix) {

}

Quaternion Quaternion::QuaternionFromDirectionVectors(Vector3D initial, Vector3D final) {

}

Quaternion Quaternion::SphericalInterpolation(Quaternion quaternion, double ratio) {

}

Quaternion Quaternion::Add(Quaternion quaternion) {

}

Quaternion Quaternion::Subtract(Quaternion quaternion) {

}

Quaternion Quaternion::Multiply(Quaternion quaternion) {

}

Quaternion Quaternion::Divide(Quaternion quaternion) {

}

Quaternion Quaternion::Power(Quaternion quaternion) {

}

Quaternion Quaternion::DotProduct(Quaternion quaternion) {

}

Quaternion Quaternion::Power(double exponent) {

}

Quaternion Quaternion::Permutate(Vector3D permutation) {

}

Quaternion Quaternion::Absolute() {

}

Quaternion Quaternion::AdditiveInverse() {

}

Quaternion Quaternion::MultiplicativeInverse() {

}

Quaternion Quaternion::Conjugate() {

}

Quaternion Quaternion::UnitQuaternion() {

}

Quaternion Quaternion::Magnitude() {

}

double Quaternion::Normal() {

}

bool Quaternion::IsNan() {

}

bool Quaternion::IsFinite() {

}

bool Quaternion::IsInfinite() {

}

bool Quaternion::IsNonZero() {

}

bool Quaternion::IsEqual(Quaternion quaternion) {

}

string Quaternion::ToString() {

}