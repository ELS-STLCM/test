using System.Collections.Generic;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.FrontOffice
{
    public interface IPatientVisitAppointmentDocument: IKeyValueRepository<PatientVisit>
    {
        List<Appointment> GetAppointmentsForPatientVisit(CalendarFilterProxy calendarFilter,AppEnum.CalendarFilterTypes calendarFilterType);
    }
}
