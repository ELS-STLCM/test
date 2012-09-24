using System.Collections.Generic;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Core.DataInterfaces.Forms;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DropBox;


namespace SimChartMedicalOffice.Data.Forms
{

    public class MedicalRecordsReleaseDocument : KeyValueRepository<MedicalRecordsRelease>, IMedicalRecordsRelease
    {
       
        public string FormAndSetUrlForStudentPatient(DropBoxLink dropBox, string patientGuid, string formId)
        {
            return string.Format(GetAssignmentUrl(dropBox, Core.DocumentPath.Module.MedicalRecordsReleaseDocument), patientGuid, formId);
        }
        public IList<MedicalRecordsRelease> GetAllMedicalRecordsReleaseFormsForPatient(DropBoxLink dropBox, string patientGuid, string formId)
        {
            string medicalRecordsReleaseJson = GetJsonDocument(FormAndSetUrlForStudentPatient(dropBox, patientGuid, formId));
            Dictionary<string, MedicalRecordsRelease> medicalRecordsReleaseForm =
                JsonSerializer.DeserializeObject<Dictionary<string, MedicalRecordsRelease>>(medicalRecordsReleaseJson);
            return ConvertDictionarytoObject(medicalRecordsReleaseForm);
        }
        private IList<MedicalRecordsRelease> ConvertDictionarytoObject(Dictionary<string, MedicalRecordsRelease> medicalRecordsReleaseDict)
        {
            IList<MedicalRecordsRelease> medicalRecordsReleaseList = new List<MedicalRecordsRelease>();
            if (medicalRecordsReleaseDict != null)
            {
                foreach (KeyValuePair<string, MedicalRecordsRelease> medicalRecordsReleaseForm in medicalRecordsReleaseDict)
                {
                    medicalRecordsReleaseList.Add(medicalRecordsReleaseForm.Value);
                }
            }
            return medicalRecordsReleaseList;
        }
    }

}