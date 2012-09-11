using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;

namespace SimChartMedicalOffice.ApplicationServices.FrontOffice.AppointmentPatterns
{
    public interface IAppointmentTypeStrategy
    {
        string GenerateAppointments();
    }
}
