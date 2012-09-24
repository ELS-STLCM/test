using System.Collections.Generic;

namespace SimChartMedicalOffice.Core.Forms
{
   public  class MedicalRecordsRelease : AbstractForms

    {
       public IList<MedicalRecordsReleaseForm> MedicalRecordsReleaseForm { get; set; }
       public IList<MedicalRecordsReleaseForm> MedicalRecordsReleaseFormTwo { get; set; }
       public string PatientReferenceId { get; set; }
       public string PatientName { get; set; }
        public string Ssn { get; set; }
        public string Address { get; set; }
        public string DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string DiffName { get; set; }
        public string Authorize { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public string DiffAddress { get; set; }
        public string Other { get; set; }
        public string DiffOther { get; set; }
        public string DiffPhone { get; set; }
        public string Fax { get; set; }
        public string Signature { get; set; }
        public string DiffDate { get; set; }
        public string PrintedDate { get; set; }
        public string AuthoritySign { get; set; }
        public string LaterThan { get; set; }
        public string Event { get; set; }
        public string DoctorName { get; set; }
        public string PrintedName { get; set; }
    }
        public class MedicalRecordsReleaseForm : DocumentEntity
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }
   
}
