using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimChartMedicalOffice.Core.Forms
{
    public class BillOfRights : DocumentEntity
    {
        public string FormName { get; set; }
        public string PatientReferenceId { get; set; }
    }
}