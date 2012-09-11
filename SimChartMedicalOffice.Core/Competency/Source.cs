using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimChartMedicalOffice.Core.Competency
{
    public class Source: DocumentEntity
    {
        public Source()
        {

        }
        /// <summary>
        /// This property holds the source Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This property holds the value assigned to the current competency.
        /// </summary>
        public string Number { get; set; }         
    }
}
