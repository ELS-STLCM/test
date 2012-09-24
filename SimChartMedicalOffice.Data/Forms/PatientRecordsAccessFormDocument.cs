using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.Forms;
using System.Collections.Generic;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Core.DropBox;
namespace SimChartMedicalOffice.Data.Forms
{
    public class PatientRecordsAccessFormDocument : KeyValueRepository<PatientRecordsAccessForm>, IPatientRecordsAccessFormDocument
    {
        

        public IList<PatientRecordsAccessForm> GetAllPatientRecordsAccessFormsForPatient(DropBoxLink dropBox, string patientGuid, string formId)
        {
            string patientRecordsJson = GetJsonDocument(FormAndSetUrlForStudentPatient(dropBox, patientGuid, formId));
            Dictionary<string, PatientRecordsAccessForm> patientRecordsAccessForms =
                JsonSerializer.DeserializeObject<Dictionary<string, PatientRecordsAccessForm>>(patientRecordsJson);
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
        /// <param name="dropBox"> </param>
        /// <param name="patientGuid"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        public string FormAndSetUrlForStudentPatient(DropBoxLink dropBox, string patientGuid, string formId)
        {
            return string.Format(GetAssignmentUrl(dropBox,Core.DocumentPath.Module.PatientRecordsAccessForms), patientGuid, formId);
        }
    }
}
