using System;
using System.Collections.Generic;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.Competency;

namespace SimChartMedicalOffice.Core.ProxyObjects
{
    public class AutoCompleteProxy
    {
        public string id { get; set; } 
        public string name { get; set; }
        public List<string> Sources { get; set; }

    }
}
