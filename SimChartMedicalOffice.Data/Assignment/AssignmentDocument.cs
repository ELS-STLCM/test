using System.Collections.Generic;
using System.Linq;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.AssignmentBuilder;
using SimChartMedicalOffice.Core.DataInterfaces.AssignmentBuilder;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DropBox;
using SimChartMedicalOffice.Common;

namespace SimChartMedicalOffice.Data.AssignmentBuilder
{
    public class AssignmentDocument : KeyValueRepository<Assignment>,IAssignmentDocument
    {       
        private readonly IPatientDocument _patientDocument;
        /// <summary>
        /// To get Assignment object
        /// </summary>
        /// <returns></returns>
        public Folder GetAssignmentRepository()
        {
            string jsonString = GetJsonDocument(GetAssignmentUrl(DocumentPath.Module.Assignments));
            Folder assignment = JsonSerializer.DeserializeObject<Folder>(jsonString);
            return assignment;
        }
        public AssignmentDocument(IPatientDocument patientDocument)
        {
            _patientDocument = patientDocument;
        }

        public string FormAssignmentUrl(DropBoxLink dropBox,string assignmentGuid)
        {
            if (dropBox == null)
            {
                dropBox = GetAdminDropBox();                
            }
            return string.Format(GetAssignmentUrl(dropBox, DocumentPath.Module.Assignments, AppConstants.Create), "", assignmentGuid);
        }
        
        public IList<Assignment> GetAssignmentItems(string parentFolderIdentifier, int folderType, DropBoxLink dropBox)
        {
            if (dropBox == null)
            {
                dropBox = GetAdminDropBox();
            }
            string jsonString = GetJsonDocument(((parentFolderIdentifier == "") ? string.Format(GetAssignmentUrl(dropBox,DocumentPath.Module.Assignments,AppConstants.Create), "", "") : (parentFolderIdentifier + "/" + Respository.QuestionItems)));
            Dictionary<string, Assignment> assignmentList = (jsonString != "null") ? JsonSerializer.DeserializeObject<Dictionary<string, Assignment>>(jsonString) : new Dictionary<string, Assignment>();
            foreach (var folderItem in assignmentList)
            {
                folderItem.Value.UniqueIdentifier = folderItem.Key;
                folderItem.Value.Url = string.Concat(string.Format(GetAssignmentUrl(dropBox,DocumentPath.Module.Assignments,AppConstants.Create), "", ""), folderItem.Value.UniqueIdentifier);
            }
            return ConvertDictionarytoObject(assignmentList);
        }

        public IList<Core.SkillSetBuilder.SkillSet> GetSkillSetsForAnAssignment(string assignmentUrl)
        {
            string jsonString = GetJsonDocument(assignmentUrl);
            Dictionary<string, Core.SkillSetBuilder.SkillSet> skillSetList = (jsonString != "null") ? JsonSerializer.DeserializeObject<Dictionary<string, Core.SkillSetBuilder.SkillSet>>(jsonString) : new Dictionary<string, Core.SkillSetBuilder.SkillSet>();
            foreach (var folderItem in skillSetList)
            {
                folderItem.Value.UniqueIdentifier = folderItem.Key;
                folderItem.Value.Url = string.Concat(assignmentUrl, "/" + Respository.Skillsets +"/", folderItem.Value.UniqueIdentifier);
            }
            return ConvertDictionarytoObject(skillSetList);
        }

        public Core.Patient.Patient GetPatientFromPatientRepository(string patientUrl)
        {
            return _patientDocument.Get(patientUrl);
        }
        
        private IList<Assignment> ConvertDictionarytoObject(Dictionary<string, Assignment> assignmentItems)
        {
            return ((assignmentItems != null) ? (assignmentItems.Select(assignmentItem => assignmentItem.Value).ToList()) : new List<Assignment>());
        }

        private IList<Core.SkillSetBuilder.SkillSet> ConvertDictionarytoObject(Dictionary<string, Core.SkillSetBuilder.SkillSet> skillSetItems)
        {
            return ((skillSetItems != null) ? (skillSetItems.Select(skillSetItem => skillSetItem.Value).ToList()) : new List<Core.SkillSetBuilder.SkillSet>());
        }
    }
}
