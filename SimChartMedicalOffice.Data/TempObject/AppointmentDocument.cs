using SimChartMedicalOffice.Core.DataInterfaces.TempObject;
using SimChartMedicalOffice.Core.TempObject;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Data.TempObject
{
    public class AppointmentDocument : KeyValueRepository<Appointment>,IAppointmentDocument
    {
        //protected override string NewUrl
        //{
        //    get
        //    {
        //        return "SimApp/Appointments/{0}";
        //    }
        //}
    }
}
