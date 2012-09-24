using System.Collections.Generic;

namespace SimChartMedicalOffice.Core.Competency
{
    public class Competency : DocumentEntity
    {
        /// <summary>
        /// This property holds the description of the current competency
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This property holds the List of source for current competency
        /// </summary>
        public List<Source> Sources { get; set; }

        /// <summary>
        /// This property holds the Value where to show this competency
        /// </summary>
        public string Focus { get; set; }

        /// <summary>
        /// This property holds readonly category value while retrieving from firebase 
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// This property holds Notes
        /// </summary>
        public string Notes { get; set; }
         
    }
}