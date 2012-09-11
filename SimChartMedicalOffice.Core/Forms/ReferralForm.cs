namespace SimChartMedicalOffice.Core.Forms
{
    public class ReferralForm : AbstractForms
    {
        public string PatientReferenceId { get; set; }

        public string DiagnosisOrCode { get; set; }

        public string SurgicalProcedureDate { get; set; }

        public string SurgicalClinicalInformation { get; set; }

        public string PreviousClinicalTreatments { get; set; }

        public string PatientName { get; set; }

        public string Address { get; set; }

        public string PatientPhone { get; set; }

        public string AlternateContact { get; set; }

        public string HealthId { get; set; }

        public string DateOfBirth { get; set; }

        public string Allergies { get; set; }

        public string Medications { get; set; }

        public string IsDiabetic { get; set; }

        public string PlaceOfService { get; set; }

        public string NumberOfVisits { get; set; }

        public string AddressOfService { get; set; }

        public string LengthOfStay { get; set; }

        public string ReferringProvider { get; set; }

        public string Phone { get; set; }

        public string NameToPrint { get; set; }

        public string NpiNumber { get; set; }

        public string Signature { get; set; }

        public string Date { get; set; }

        public string FamilyPhysicianName { get; set; }

        public string IsSameReferringPhysician { get; set; }

        public string FormInitiatedBy { get; set; }

        public string AuthorizationNumber { get; set; }

        public string EffectiveDate { get; set; }

        public string ExpirationDate { get; set; }

        public string NameAndPosition { get; set; }

        public string PhoneOfInitiatedPerson { get; set; }

        public string SignatureOfInitiatedPerson { get; set; }

        public string DateInitiated { get; set; }
    }
}