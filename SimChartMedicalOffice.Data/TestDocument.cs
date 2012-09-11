using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.DataInterfaces;

namespace SimChartMedicalOffice.Data
{
    public class TestDocument:ITestDocument
    {
        public TestDocument()
        {

        }
        public string Welcome()
        {
            return "Welcome to SimChart for Medical Office";
        }
    }
}
