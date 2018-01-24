using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADRCVisualization.Class_Files.Mathematics
{
    class EulerAngles
    {
        public EulerAngles(double pitch, double yaw, double roll, string order)
        {
            throw new NotImplementedException();
        }

        public HMatrix EulerToHMatrix(EulerAngles eulerAngles)
        {

            throw new NotImplementedException();
        }

        public Quaternion EulerToQuaternion(EulerAngles eulerAngles)
        {

            throw new NotImplementedException();
        }
        
        public EulerAngles EulerFromHMatrix(HMatrix hMatrix, string order)
        {

            throw new NotImplementedException();
        }

        public EulerAngles EulerFromQuaternion(Quaternion quaternion, string order)
        {

            throw new NotImplementedException();
        }
    }
}
