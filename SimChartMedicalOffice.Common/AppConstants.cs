using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public const char demiliter = 'Ø';
        public const string LogOutSuccess = "You are logged out of SimChart for the medical office";
        public const string Select_DropDown = "-Select-";
        public const string ADMIN_COURSE_ID = "ELSEVIER_CID";
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

        public const string STUDENT_ROLE = "Student";
        public const string INSTRUCTOR_ROLE = "Instructor";
        public const string ADMIN_ROLE = "Admin";
        public const string AppointmentDurationValidationMessage = "End Time cannot occur prior to Start Time";
        public const string AppointmentExistsValidationMessage = "New appointment conflicts with an existing appointment";
    }
}
