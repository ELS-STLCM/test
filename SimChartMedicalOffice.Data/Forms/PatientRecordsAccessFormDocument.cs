using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.Forms;
using System.Collections.Generic;
using SimChartMedicalOffice.Core.Forms;
namespace SimChartMedicalOffice.Data.Forms
{
    public class PatientRecordsAccessFormDocument : KeyValueRepository<PatientRecordsAccessForm>, IPatientRecordsAccessFormDocument
    {
        public override string Url
        {
            get
            {
                //return "SimApp/Courses/ALL_swhitcomb5_0001/Student/UID1/Assignments/ScenarioId1/Patients/{0}";
                return "SimApp/Courses/{0}/Patients/{1}/PatientRecordsAccessForms/{2}";
            }
        }

        public IList<PatientRecordsAccessForm> GetAllPatientRecordsAccessFormsForPatient(string courseId, string userRole, string UID, string SID, string patientGuid, string formId)
        {
            string patientRecordsJSON = GetJsonDocument(FormAndSetUrlForStudentPatient(courseId,userRole,UID,SID,patientGuid, formId));
            Dictionary<string, PatientRecordsAccessForm> patientRecordsAccessForms =
                JsonSerializer.DeserializeObject<Dictionary<string, PatientRecordsAccessForm>>(patientRecordsJSON);
            return ConvertDictionarytoObject(patientRecordsAccessForms);
        }

        /// <summary>
        /// To convert Dictionary of Folder objects to List of Folder Objects
        /// </summary>
        /// <param name="patientRecordsDict"></param>
        /// <returns></returns>
        private IList<PatientRecordsAccessForm> ConvertDictionarytoObject(Dictionary<string, PatientRecordsAccessForm> patientRecordsDict)
        {
            IList<PatientRecordsAccessForm> patientRecordsList = new List<PatientRecordsAccessForm>();
            if (patientRecordsDict != null)
            {
                foreach (KeyValuePair<string, PatientRecordsAccessForm> patientRecordsAccessForm in patientRecordsDict)
                {
                    patientRecordsList.Add(patientRecordsAccessForm.Value);
                }
            }
            return patientRecordsList;
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
                return string.Format(Url, courseId + "/" + userRole + "/" + UID + "/Assignments/" + SID, patientGuid,formId);
            }
            // if Admin or Instructor
            return "";
        }
    }
}
