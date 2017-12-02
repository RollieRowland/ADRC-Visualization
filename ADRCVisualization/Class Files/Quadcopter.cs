using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.QuadcopterSimulation;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualization.Class_Files
{
    class Quadcopter
    {
        /*
        B     C
         \   /
           A
         /   \
        E     D
        
        < -X- >
           
           ^
           Z
           v
        */
        private Thruster ThrusterB;
        private Thruster ThrusterC;
        private Thruster ThrusterD;
        private Thruster ThrusterE;

        private IMU imu;

        public Quadcopter(double ArmLength, double ArmAngle)
        {
            CalculateArmPositions(ArmLength, ArmAngle);
        }

        private void CalculateArmPositions(double ArmLength, double ArmAngle)
        {
            double XLength = ArmLength * Math.Cos(Misc.DegreesToRadians(ArmAngle));
            double ZLength = ArmLength * Math.Sin(Misc.DegreesToRadians(ArmAngle));

            ThrusterB = new Thruster(new Vector(-XLength, 0, ZLength));
            ThrusterC = new Thruster(new Vector(XLength, 0, ZLength));
            ThrusterD = new Thruster(new Vector(XLength, 0, -ZLength));
            ThrusterE = new Thruster(new Vector(-XLength, 0, -ZLength));
        }

        /// <summary>
        /// Call from the controller
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void Control(Vector position, Vector rotation)
        {
            SetTarget(position, rotation);
            //SetCurrent(imu.CalculatePosition(), imu.CalculateRotation());
        }

        public void Calculate()
        {
            CalculateThrust();
            CalculateThrusterOrientation();
        }

        public void SetTarget(Vector position, Vector rotation)
        {
            //calculate rotation matrix
            Matrix matrixB = new Matrix(ThrusterB.QuadCenterOffset);
            Matrix matrixC = new Matrix(ThrusterC.QuadCenterOffset);
            Matrix matrixD = new Matrix(ThrusterD.QuadCenterOffset);
            Matrix matrixE = new Matrix(ThrusterE.QuadCenterOffset);

            matrixB.Rotate(rotation);
            matrixC.Rotate(rotation);
            matrixD.Rotate(rotation);
            matrixE.Rotate(rotation);

            ThrusterB.SetTargetPosition(matrixB.ConvertCoordinateToVector().Add(position));
            ThrusterC.SetTargetPosition(matrixC.ConvertCoordinateToVector().Add(position));
            ThrusterD.SetTargetPosition(matrixD.ConvertCoordinateToVector().Add(position));
            ThrusterE.SetTargetPosition(matrixE.ConvertCoordinateToVector().Add(position));
        }

        public void SetCurrent(Vector position, Vector rotation)
        {
            //calculate rotation matrix
            Matrix matrixB = new Matrix(ThrusterB.QuadCenterOffset);
            Matrix matrixC = new Matrix(ThrusterC.QuadCenterOffset);
            Matrix matrixD = new Matrix(ThrusterD.QuadCenterOffset);
            Matrix matrixE = new Matrix(ThrusterE.QuadCenterOffset);

            matrixB.Rotate(rotation);
            matrixC.Rotate(rotation);
            matrixD.Rotate(rotation);
            matrixE.Rotate(rotation);

            ThrusterB.SetCurrentPosition(matrixB.ConvertCoordinateToVector().Add(position));
            ThrusterC.SetCurrentPosition(matrixC.ConvertCoordinateToVector().Add(position));
            ThrusterD.SetCurrentPosition(matrixD.ConvertCoordinateToVector().Add(position));
            ThrusterE.SetCurrentPosition(matrixE.ConvertCoordinateToVector().Add(position));
        }

        private void CalculateThrusterOrientation()
        {
            ThrusterB.CalculateOrientation();
            ThrusterC.CalculateOrientation();
            ThrusterD.CalculateOrientation();
            ThrusterE.CalculateOrientation();
        }

        private void CalculateThrust()
        {
            ThrusterB.CalculateThrust();
            ThrusterC.CalculateThrust();
            ThrusterD.CalculateThrust();
            ThrusterE.CalculateThrust();
        }
    }
}
