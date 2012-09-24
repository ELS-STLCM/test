using System.Collections.Generic;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.FrontOffice
{
    public interface IAppointmentDocument:IKeyValueRepository<Core.FrontOffice.Appointments.Appointment>
    {
        IList<Core.FrontOffice.Appointments.Appointment> GetAppointments(CalendarFilterProxy calendarFilter, AppEnum.CalendarFilterTypes filterType);
        List<Core.FrontOffice.Appointments.Appointment> GetPatientVisitAppointments(CalendarFilterProxy calendarFilter, AppEnum.CalendarFilterTypes filterType);
    }
}
