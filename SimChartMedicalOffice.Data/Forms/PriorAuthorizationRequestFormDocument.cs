using System.Collections.Generic;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Core.DataInterfaces.Forms;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Data.Forms
{
    public class PriorAuthorizationRequestFormDocument : KeyValueRepository<PriorAuthorizationRequestForm>, IPriorAuthorizationRequestFormDocument
    {
       

        public IList<PriorAuthorizationRequestForm> GetAllPriorAuthorizationRequestFormsForPatient(DropBoxLink dropBox, string patientGuid, string formId)
        {
            string priorAuthorityJson = GetJsonDocument(FormAndSetUrlForStudentPatient(dropBox, patientGuid, formId));
            Dictionary<string, PriorAuthorizationRequestForm> priorAuthorizationRequestForm =
                JsonSerializer.DeserializeObject<Dictionary<string, PriorAuthorizationRequestForm>>(priorAuthorityJson);
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
        /// <param name="dropBox"> </param>
        /// <param name="patientGuid"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        public string FormAndSetUrlForStudentPatient(DropBoxLink dropBox, string patientGuid, string formId)
        {
            return string.Format(GetAssignmentUrl(dropBox,Core.DocumentPath.Module.PriorAuthorizationForm), patientGuid, formId);
        }
    }
}
