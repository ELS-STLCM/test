using SimChartMedicalOffice.Core.DataInterfaces.TempObject;
using SimChartMedicalOffice.Core.TempObject;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Data.TempObject
{
    public class RegistrationDocument : KeyValueRepository<Registration>,IRegistrationDocument
    {
        //protected override string NewUrl
        //{
        //    get
        //    {
        //        return "SimApp/Patients";
        //    }
        //}
    }
}
