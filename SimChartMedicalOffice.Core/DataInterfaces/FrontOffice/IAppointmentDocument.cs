using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.TempObject;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;

namespace SimChartMedicalOffice.Core.DataInterfaces.FrontOffice
{
    public interface IAppointmentDocument:IKeyValueRepository<SimChartMedicalOffice.Core.FrontOffice.Appointments.Appointment>
    {
        IList<SimChartMedicalOffice.Core.FrontOffice.Appointments.Appointment> GetAppointments(CalendarFilterProxy calendarFilter, AppEnum.CalendarFilterTypes filterType);
        List<SimChartMedicalOffice.Core.FrontOffice.Appointments.Appointment> GetPatientVisitAppointments(CalendarFilterProxy calendarFilter, AppEnum.CalendarFilterTypes filterType);
    }
}
