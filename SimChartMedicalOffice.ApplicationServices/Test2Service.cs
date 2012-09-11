using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface;
using SimChartMedicalOffice.Core.DataInterfaces;
using SimChartMedicalOffice.Data;

namespace SimChartMedicalOffice.ApplicationServices
{
    public class Test2Service:ITestService
    {
        public Test2Service(ITestDocument testDoc)
        {
            this.testDocumentObject = testDoc;
        }
        public virtual string HelloWorld()
        {
            return "HelloWorld";
        }
        private ITestDocument testDocumentObject;
    }
}
