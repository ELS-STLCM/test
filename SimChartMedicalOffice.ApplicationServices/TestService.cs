using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface;
using SimChartMedicalOffice.Data;
using SimChartMedicalOffice.Core.DataInterfaces;

namespace SimChartMedicalOffice.ApplicationServices
{
    public class TestService:ITestService
    {
        public TestService(ITestDocument testDoc)
        {
            this.testDocumentObject = testDoc;
        }
        public virtual string HelloWorld()
        {
            return testDocumentObject.Welcome();
        }
        private ITestDocument testDocumentObject;
    }
}
