using System.Collections.Generic;

namespace SimChartMedicalOffice.Core.ProxyObjects
{
    public class AutoCompleteProxy
    {
        public string id { get; set; } 
        public string name { get; set; }
        public List<string> Sources { get; set; }

    }
}
