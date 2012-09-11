using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;

namespace SimChartMedicalOffice.Core.DataInterfaces.FrontOffice
{
    public interface IRecurrenceGroupDocument : IKeyValueRepository<RecurrenceGroup>
    {
    }
}
