using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualization.Class_Files.QuadcopterSimulation
{
    class Thruster
    {
        private Motor propellor;
        private Servo primaryJoint;
        private Servo secondaryJoint;
        private IMU jointMotion;

        private ADRC_PD thrustADRC;
        private ADRC_PD primaryJointADRC;
        private ADRC_PD secondaryJointADRC;

        private Vector targetPosition;
        private Vector currentPosition;
        public Vector QuadCenterOffset { get; }

        public Thruster(Vector QuadCenterOffset)
        {
            this.QuadCenterOffset = QuadCenterOffset;

            thrustADRC = new ADRC_PD(2000, 500, 2.875, 0.00085, 0.36, 0.2485, 1000);
            primaryJointADRC = new ADRC_PD(2000, 500, 2.875, 0.00085, 0.36, 0.2485, 1000);
            secondaryJointADRC = new ADRC_PD(2000, 500, 2.875, 0.00085, 0.36, 0.2485, 1000);
        }

        public void SetTargetPosition(Vector position)
        {
            targetPosition = position;
        }

        public void SetCurrentPosition(Vector position)
        {
            currentPosition = position;
        }

        public void CalculateOrientation()
        {
            //Calculate X offset, Y offset
        }

        public void CalculateThrust()
        {
            //Calculate Z offset

        }
    }
}
