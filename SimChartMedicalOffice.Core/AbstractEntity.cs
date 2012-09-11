using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimChartMedicalOffice.Core
{
    public class AbstractEntity
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTimeStamp { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedTimeStamp { get; set; }
        public bool IsActive { get; set; }
        public bool IsAutoSave { get; set; }
    }
}
