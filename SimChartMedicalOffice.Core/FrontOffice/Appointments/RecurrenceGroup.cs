using System;
using System.Collections.Generic;
using SimChartMedicalOffice.Common;

namespace SimChartMedicalOffice.Core.FrontOffice.Appointments
{
    public class RecurrenceGroup : AbstractChartData
    {
        public string Description { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int Duration { get; set; }
        public IList<string> RecurrenceList { set; get; }
        public AppEnum.RecurrencePattern Pattern { get; set; }
        public int NumberOfOccurences { get; set; }
        public string EndBy { get; set; }
        
    }
}
