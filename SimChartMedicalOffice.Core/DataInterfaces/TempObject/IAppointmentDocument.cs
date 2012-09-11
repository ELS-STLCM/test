using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.TempObject;

namespace SimChartMedicalOffice.Core.DataInterfaces.TempObject
{
    public interface IAppointmentDocument:IKeyValueRepository<Appointment>
    {

    }
}
