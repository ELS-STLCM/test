
namespace SimChartMedicalOffice.Common
{
    public static class AppConstants
    {
        public const string EmptyDataForReports = "";
        public const string FileUploadSuccess = "Saved";
        public const string FileUploadFailure = "Upload failure";
        public const string FileUploadNoFileSelected = "Please select a valid file";
        public const string FileUploadError = "Please select a valid file";
        public const string StandardFileName = "File";
        public const char Demiliter = 'Ø';
        public const string LogOutSuccess = "You are logged out of SimChart for the medical office";
        public const string SelectDropDown = "-Select-";
        public const string AdminCourseId = "ELSEVIER_CID";
        public enum Role
        {
            Admin=1,
            Instructor=2,
            Student=3
        }

        public const string FormNotFound = "No saved Forms for the Patient";

        public const string PatientAgeYearText = "yrs";
        public const string PatientAgeMonthText = "mos";
        public const string PatientAgeDaysText = "days";

        public const string NoticePrivacyPractice = "NoticeOfPrivacyPractice";
        public const string BillofRights = "BillofRights";

        public const string Select = "-SELECT-";
        public const string Error = "Error";
        public const string Save = "Save";
        public const string Deleted = "Deleted";

        public const string StudentRole = "Student";
        public const string InstructorRole = "Instructor";
        public const string AdminRole = "Admin";
        public const string AppointmentDurationValidationMessage = "End Time cannot occur prior to Start Time";
        public const string StartEndTimeSameWarningMessage = "Start time and End time cannot be the same";
        public const string AppointmentExistsValidationMessage = "New appointment conflicts with an existing appointment";
        public const string AppointmentSave = "Appointment saved successfully";


        //CustomValues parameter used in Respository.GetDocumentPath method
        public const string CalendarMonthFilters = "MonthFilter";
        //public const string READ_PATIENT = "ReadPatient";
        //public const string INCLUDE_PATIENT_NODE = "IncludePatientNode";
        public const string Read = "Read";
        public const string Create = "Create";
        public const string Cancelled = "Cancelled";
        public const string ApplicationModule = "ApplicationModule";
        public const string Competencies = "Competencies";
        public const string CompetencySource = "CompetencySource";
        public const string DateTimeFormate = "mm/dd/yyyy HH:MM:ss";
        public const string QuestionType = "QuestionType";
        public const string TransientAttachment = "Transient";
        public const string PersistentAttachment = "Persistent";
    }
}
