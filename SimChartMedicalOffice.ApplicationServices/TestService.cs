using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface;
using SimChartMedicalOffice.Core.DataInterfaces;

namespace SimChartMedicalOffice.ApplicationServices
{
    public class TestService:ITestService
    {
        public TestService(ITestDocument testDoc)
        {
            testDocumentObject = testDoc;
        }
        public virtual string HelloWorld()
        {
            return testDocumentObject.Welcome();
        }
        private readonly ITestDocument testDocumentObject;
    }
}
