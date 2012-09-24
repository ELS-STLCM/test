namespace SimChartMedicalOffice.Core.Forms
{
    public class PriorAuthorizationRequestForm : AbstractForms
    {
        public string MemberName { get; set; }

        public string MemberId { get; set; }

        public string PatientReferenceId { get; set; }

        public string DateOfBirth { get; set; }

        public string OrderingPhysician { get; set; }

        public string PhysicianAddress { get; set; }

        //public string PhysicianAddress2 { get; set; }

        //public string City { get; set; }

        //public string State { get; set; }

        //public int ZipCode { get; set; }

        public string Provider { get; set; }

        public string ProviderFax { get; set; }

        public string ProviderPhone { get; set; }

        public string ProviderContactNumber { get; set; }

        public string ProviderPlaceOfService { get; set; }

        public string ServiceRequested { get; set; }

        public string DiagonsisOrIcd9Code8 { get; set; }

        public string ProcedureCpt4Codes { get; set; }

        public string StartingServiceDate { get; set; }

        public string ServiceFrequency { get; set; }

        public string EndingServiceDate { get; set; }

        public string InjuryRelated { get; set; }

        public string DateOfInjury { get; set; }

        public string WorkerCompentationRelated { get; set; }

        public string WorkerCompDateOfInjury { get; set; }

        public string AuthorizationNumber { get; set; }

        public string EffectiveDate { get; set; }

        public string ExpiryDate { get; set; }
    }
}