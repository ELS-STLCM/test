using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Common;

namespace SimChartMedicalOffice.Core.DataInterfaces.FrontOffice
{
    public interface IOtherAppointmentDocument: IKeyValueRepository<OtherAppointment>
    {
        List<Appointment> GetAppointmentsForOtherType(CalendarFilterProxy calendarFilter, AppEnum.CalendarFilterTypes calendarFilterType);
    }
}
