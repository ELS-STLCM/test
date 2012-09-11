using System.Collections.Generic;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Core.DataInterfaces.Forms;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Data.Forms
{
    public class PriorAuthorizationRequestFormDocument : KeyValueRepository<PriorAuthorizationRequestForm>, IPriorAuthorizationRequestFormDocument
    {
        public override string Url
        {
            get
            {
                //return "SimApp/Courses/ALL_swhitcomb5_0001/Student/UID1/Assignments/ScenarioId1/Patients/{0}";
                return "SimApp/Courses/{0}/Patients/{1}/PriorAuthorizationRequestForms/{2}";
            }
        }

        public IList<PriorAuthorizationRequestForm> GetAllPriorAuthorizationRequestFormsForPatient(string courseId, string userRole, string UID, string SID, string patientGuid, string formId)
        {
            string priorAuthorityJSON = GetJsonDocument(FormAndSetUrlForStudentPatient(courseId, userRole, UID, SID, patientGuid, formId));
            Dictionary<string, PriorAuthorizationRequestForm> priorAuthorizationRequestForm =
                JsonSerializer.DeserializeObject<Dictionary<string, PriorAuthorizationRequestForm>>(priorAuthorityJSON);
            return ConvertDictionarytoObject(priorAuthorizationRequestForm);
        }

        /// <summary>
        /// To convert Dictionary of Folder objects to List of Folder Objects
        /// </summary>
        /// <param name="priorAuthorizationRequestDict"></param>
        /// <returns></returns>
        private IList<PriorAuthorizationRequestForm> ConvertDictionarytoObject(Dictionary<string, PriorAuthorizationRequestForm> priorAuthorizationRequestDict)
        {
            IList<PriorAuthorizationRequestForm> priorAuthorizationList = new List<PriorAuthorizationRequestForm>();
            if (priorAuthorizationRequestDict != null)
            {
                foreach (KeyValuePair<string, PriorAuthorizationRequestForm> priorAuthorizationRequestForm in priorAuthorizationRequestDict)
                {
                    priorAuthorizationList.Add(priorAuthorizationRequestForm.Value);
                }
            }
            return priorAuthorizationList;
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
