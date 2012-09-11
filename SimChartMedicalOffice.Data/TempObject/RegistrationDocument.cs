using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.TempObject;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.TempObject;

namespace SimChartMedicalOffice.Data.TempObject
{
    public class RegistrationDocument : KeyValueRepository<Registration>,IRegistrationDocument
    {
        public override string Url
        {
            get
            {
                return "SimApp/Patients";
            }
        }
    }
}
