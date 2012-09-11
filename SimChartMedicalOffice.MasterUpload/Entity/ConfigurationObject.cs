using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.MasterUpload.Forms;

namespace SimChartMedicalOffice.MasterUpload
{
    public class ConfigurationObject
    {
        public string DataFileName { get; set; }
        public SimOfficeForm  FormObject { get; set; }
        public string DataSheetName { get; set; }
    }
}
