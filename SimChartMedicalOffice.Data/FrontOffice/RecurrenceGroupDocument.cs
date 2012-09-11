using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Core.DataInterfaces.FrontOffice;

namespace SimChartMedicalOffice.Data.FrontOffice
{
    public class RecurrenceGroupDocument : KeyValueRepository<RecurrenceGroup>, IRecurrenceGroupDocument
    {
        public override string Url
        {
            get
            {
                //"SimApp/Courses/{CourseId}/{Role}/Assignments/{SID}/{DateTimeFormat}
                return "SimApp/Courses/{0}/{1}/{2}/Assignments/{3}/Appointments/Recurrence/{4}";
            }
        }
    }
}
