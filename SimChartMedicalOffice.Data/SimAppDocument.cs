using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DataInterfaces;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Data
{
    public class SimAppDocument : KeyValueRepository<SimApp>, ISimAppDocument
    {
        public override string Url
        {
            get
            {
                return "SimApp";
            }
        }
    }
}
