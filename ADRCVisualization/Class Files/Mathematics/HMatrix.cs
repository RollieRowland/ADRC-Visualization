using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    class HMatrix
    {
        private double[,] hierarchicalMatrixValues = new double[4, 4];

        public HMatrix()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    hierarchicalMatrixValues[i, k] = 0;
                }
            }
        }

        public HMatrix(double[,] hMatrix)
        {
            hierarchicalMatrixValues = hMatrix;
        }

        public double this[int index1, int index2]
        {
            get
            {
                return hierarchicalMatrixValues[index1, index2];
            }
            set
            {
                hierarchicalMatrixValues[index1, index2] = value;
            }
        }
    }

}
