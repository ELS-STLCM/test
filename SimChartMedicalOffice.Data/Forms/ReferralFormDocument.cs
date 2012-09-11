using System.Collections.Generic;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Core.DataInterfaces.Forms;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Data.Forms
{
    public class ReferralFormDocument : KeyValueRepository<ReferralForm>, IReferralFormDocument
    {
        public override string Url
        {
            get
            {
                //return "SimApp/Courses/ALL_swhitcomb5_0001/Student/UID1/Assignments/ScenarioId1/Patients/{0}";
                return "SimApp/Courses/{0}/Patients/{1}/ReferralForms/{2}";
            }
        }

        public IList<ReferralForm> GetAllReferralFormsForPatient(string courseId, string userRole, string UID, string SID, string patientGuid, string formId)
        {
            string priorAuthorityJSON = GetJsonDocument(FormAndSetUrlForStudentPatient(courseId, userRole, UID, SID, patientGuid, formId));
            Dictionary<string, ReferralForm> referralFormsDict =
                JsonSerializer.DeserializeObject<Dictionary<string, ReferralForm>>(priorAuthorityJSON);
            return ConvertDictionarytoObject(referralFormsDict);
        }

        /// <summary>
        /// To convert Dictionary of Folder objects to List of Folder Objects
        /// </summary>
        /// <param name="referralFormsDict"></param>
        /// <returns></returns>
        private IList<ReferralForm> ConvertDictionarytoObject(Dictionary<string, ReferralForm> referralFormsDict)
        {
            IList<ReferralForm> referralFormsList = new List<ReferralForm>();
            if (referralFormsDict != null)
            {
                foreach (KeyValuePair<string, ReferralForm> referralForm in referralFormsDict)
                {
                    referralFormsList.Add(referralForm.Value);
                }
            }
            return referralFormsList;
        }

        /// <summary>
        /// To form/set the URL for the form object
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userRole"></param>
        /// <param name="UID"></param>
        /// <param name="SID"></param>
        /// <param name="patientGuid"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        public string FormAndSetUrlForStudentPatient(string courseId, string userRole, string UID, string SID, string patientGuid, string formId)
        {
            if (userRole == "Student")
            {
                return string.Format(Url, courseId + "/" + userRole + "/" + UID + "/Assignments/" + SID, patientGuid, formId);
            }
            // if Admin or Instructor
            return "";
        }
    }
}
