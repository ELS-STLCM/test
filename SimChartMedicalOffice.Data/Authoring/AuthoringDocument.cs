using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.Authoring;

namespace SimChartMedicalOffice.Data
{
    public class AuthoringDocument : KeyValueRepository<Core.Authoring.Authoring>, IAuthoringDocument
    {
        public override string Url
        {
            get
            {
                return "SimApp/Authoring";
            }
        }
    }
}
