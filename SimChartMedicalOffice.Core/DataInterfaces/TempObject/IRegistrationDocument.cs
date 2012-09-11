using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.TempObject;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.TempObject
{
    public interface IRegistrationDocument:IKeyValueRepository<Registration>
    {
    }
}
