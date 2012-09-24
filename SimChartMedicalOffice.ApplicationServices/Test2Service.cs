using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface;
using SimChartMedicalOffice.Core.DataInterfaces;

namespace SimChartMedicalOffice.ApplicationServices
{
    public class Test2Service:ITestService
    {
        public Test2Service(ITestDocument testDoc)
        {
      //      testDocumentObject = testDoc;
        }
        public virtual string HelloWorld()
        {
            return "HelloWorld";
        }
       // private ITestDocument testDocumentObject;
    }
}
