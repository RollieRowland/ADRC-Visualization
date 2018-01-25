using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    public class HMatrix//Hierarchical matrix
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

        public double this[double index1, double index2]
        {
            get
            {
                return hierarchicalMatrixValues[(int)index1, (int)index2];
            }
            set
            {
                hierarchicalMatrixValues[(int)index1, (int)index2] = value;
            }
        }
    }

}
