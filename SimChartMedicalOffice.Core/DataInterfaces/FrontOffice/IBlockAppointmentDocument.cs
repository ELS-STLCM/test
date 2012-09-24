using System.Collections.Generic;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.FrontOffice
{
    public interface IBlockAppointmentDocument : IKeyValueRepository<BlockAppointment>
    {
        List<Appointment> GetAppointmentsForBlockType(CalendarFilterProxy calendarFilter, AppEnum.CalendarFilterTypes calendarFilterType);
    }
}
