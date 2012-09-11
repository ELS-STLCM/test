using System.Collections.Generic;

namespace SimChartMedicalOffice.Core
{
    public class SimApp : DocumentEntity
    {
        //public List<Patient.Patient> Patients { get; set; }
        public List<Attachment> Attachments { get; set; }
        //public List<string> FormsList { get; set; }

        public Authoring.Authoring Authoring { get; set; }
    }
}