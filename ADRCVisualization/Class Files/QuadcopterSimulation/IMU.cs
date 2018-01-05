using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualization.Class_Files.QuadcopterSimulation
{
    class IMU//Inertial Measurement Unit
    {
        private Vector rotation;

        //Dead reckoning
        //Double integration

        public IMU()
        {
            rotation = new Vector(0, 0, 0);

            throw new NotImplementedException();
        }

        //given 3 axis acceleration
        //given 3 axis angular velocity
        public Vector CalculateRotation()
        {
            throw new NotImplementedException();
        }

        public Vector CalculatePosition()
        {
            throw new NotImplementedException();
        }
    }
}
