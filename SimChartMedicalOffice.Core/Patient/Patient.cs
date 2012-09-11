using SimChartMedicalOffice.Core.Forms;
using System.Collections.Generic;
using System;
using SimChartMedicalOffice.Common;

namespace SimChartMedicalOffice.Core.Patient
{
    public class Patient : DocumentEntity
    {
        /// <summary>
        /// This property holds the  FirstName of a Patient
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// This property holds the  LastName of a Patient
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// This property holds the  MiddleInitial of a Patient
        /// </summary>
        public string MiddleInitial { get; set; }

        /// <summary>
        /// This property holds the  Age of a Patient
        /// </summary>
        public int AgeInYears { get; set; }

        /// <summary>
        /// This property holds the  Age of a Patient
        /// </summary>
        public int AgeInMonths { get; set; }

        /// <summary>
        /// This property holds the  Age of a Patient
        /// </summary>
        public int AgeInDays { get; set; }

        /// <summary>
        /// This property holds the  date of Birth of a Patient 
        /// </summary>
        public String DateOfBirth { get; set; }

        /// <summary>
        /// This property holds the  Gender of a Patient 
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// This property holds the  Image of a Patient 
        /// </summary>
        public string UploadImage { get; set; }

        /// <summary>
        /// This property holds the  MedicalRecordNumber of a Patient 
        /// </summary>
        public string MedicalRecordNumber { get; set; }

        /// <summary>
        /// This property holds the  Phone number of a Patient 
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// This property holds the  Address of a Patient 
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// This property holds the  OfficeType 
        /// </summary>
        public string OfficeType { get; set; }

        /// <summary>
        /// This property holds the  Provider Name 
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// This property holds the  Status of patient published or not
        /// </summary>
        public string Status { get; set; }

        //public PriorAuthorizationRequestForm PriorAuthorizationRequestForm { get; set; }

        /// <summary>
        /// This property holds the  NoticeOfPrivacyPractice form object
        /// </summary>
        public NoticeOfPrivacyPractice NoticeOfPrivacyPractice { get; set; }

        /// <summary>
        /// This property holds the  BillOfRights form object
        /// </summary>
        public BillOfRights BillOfRights { get; set; }

        /// <summary>
        /// This property holds the dictionary of PatientRecordsAccessForm form object as value and its uniqueIdentifier as key
        /// </summary>
        public Dictionary<string, PatientRecordsAccessForm> PatientRecordsAccessForms { get; set; }

        /// <summary>
        /// This property holds the dictionary of PriorAuthorizationRequestForm form object as value and its uniqueIdentifier as key
        /// </summary>
        public Dictionary<string, PriorAuthorizationRequestForm> PriorAuthorizationRequestForms { get; set; }

        /// <summary>
        /// This property holds the dictionary of ReferralForm form object as value and its uniqueIdentifier as key
        /// </summary>
        public Dictionary<string, ReferralForm> ReferralForms { get; set; }

        /// <summary>
        /// This property holds whether the patient is a assignment patient
        /// </summary>
        public bool IsAssignmentPatient { get; set; }

        /// <summary>
        /// This property holds the Insurance of patient
        /// </summary>
        public string Insurance { get; set; }

        /// <summary>
        /// This property holds the ID Number of patient
        /// </summary>
        public string IDNumber{ get; set;}

        /// <summary>
        /// This property holds the Name of Policy holder for the patient
        /// </summary>
        public string NameofPolicyHolder { get; set; }

        /// <summary>
        /// This property holds the Goup Number of patient
        /// </summary>
        public string GroupNumber { get; set; }

        /// <summary>
        /// This property holds the SSN of Policy holder for the patient
        /// </summary>
        public string SSNofPolicyHolder { get; set; }
        
        public string FormAgeText()
        {
            string patientAge = ((this.AgeInYears > 0) ? this.AgeInYears + " " + AppConstants.PatientAgeYearText + " " : "");
            patientAge += ((this.AgeInMonths > 0) ? this.AgeInMonths + " " + AppConstants.PatientAgeMonthText + " " : "");
            patientAge += ((this.AgeInDays > 0) ? this.AgeInDays + " " + AppConstants.PatientAgeDaysText + " " : "");
            return patientAge;
        }
    }
}
