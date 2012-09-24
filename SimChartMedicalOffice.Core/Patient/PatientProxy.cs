
namespace SimChartMedicalOffice.Core.Patient
{
    public class PatientProxy : DocumentEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string DateOfBirth { get; set; }
        public string Age { get; set; }
        public string Status { get; set; }
        public int AgeInYears { get; set; }
        public int AgeInMonths { get; set; }
        public int AgeInDays { get; set; }

    }
}
