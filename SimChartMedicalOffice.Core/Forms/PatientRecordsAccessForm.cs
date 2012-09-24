using System.Collections.Generic;

namespace SimChartMedicalOffice.Core.Forms
{
    public class PatientRecordsAccessForm : AbstractForms
    {
        public IList<PatientMedicalRecordRequest> PatientMedicalRecordRequest { get; set; }

        public string PatientReferenceId { get; set; }

        public string MedicalRecordPeriodFrom { get; set; }

        public string MedicalRecordPeriodTo { get; set; }

        public string ReasonforDisclosure { get; set; }

        public string ReleasingTo { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string EmergencyPatientName { get; set; }

        public string EmergencyPatientDob { get; set; }

        public string EmergencyPatientAddress { get; set; }

        public string EmergencyPatientCity { get; set; }

        public string EmergencyPatientState { get; set; }

        public string EmergencyPatientZipCode { get; set; }

        public string RequestExpiryDate { get; set; }

        public string Signature { get; set; }

        public string WitnessSignature { get; set; }

        public string SignatureDate { get; set; }

        public string WitnessSignatureDate { get; set; }

        public string DateCompleted { get; set; }

        public string CompletedBy { get; set; }

        public string Charge { get; set; }
      
    }

    public class PatientMedicalRecordRequest : DocumentEntity
    {
        public string Id { get; set; }
        public string Value{ get; set; }
    }
}