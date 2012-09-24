using SimChartMedicalOffice.Core.DataInterfaces;

namespace SimChartMedicalOffice.Data
{
    public class TestDocument:ITestDocument
    {
        public string Welcome()
        {
            return "Welcome to SimChart for Medical Office";
        }
    }
}
