using System.Collections.Generic;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Core.DataInterfaces.Forms;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Data.Forms
{
    public class ReferralFormDocument : KeyValueRepository<ReferralForm>, IReferralFormDocument
    {
       

        public IList<ReferralForm> GetAllReferralFormsForPatient(DropBoxLink dropBox, string patientGuid, string formId)
        {
            string priorAuthorityJson = GetJsonDocument(FormAndSetUrlForStudentPatient(dropBox, patientGuid, formId));
            Dictionary<string, ReferralForm> referralFormsDict =
                JsonSerializer.DeserializeObject<Dictionary<string, ReferralForm>>(priorAuthorityJson);
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
        /// <param name="dropBox"> </param>
        /// <param name="patientGuid"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        public string FormAndSetUrlForStudentPatient(DropBoxLink dropBox, string patientGuid, string formId)
        {
            return string.Format(GetAssignmentUrl(dropBox,Core.DocumentPath.Module.ReferralForm), patientGuid, formId);
        }
    }
}
