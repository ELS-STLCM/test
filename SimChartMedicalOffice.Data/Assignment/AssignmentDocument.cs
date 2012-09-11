using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.AssignmentBuilder;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.AssignmentBuilder;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.DataInterfaces.SkillSetBuilder;

namespace SimChartMedicalOffice.Data.AssignmentBuilder
{
    public class AssignmentDocument : KeyValueRepository<Core.AssignmentBuilder.Assignment>,IAssignmentDocument
    {
        public override string Url
        {
            get
            {
                return "SimApp/Courses/{0}/AssignmentRepository{1}/Assignments/{2}";
            }
        }

        private readonly IPatientDocument _patientDocument;

        public AssignmentDocument(IPatientDocument patientDocument)
        {
            this._patientDocument = patientDocument;
        }

        public string FormAssignmentUrl(string courseId,string assignmentGuid)
        {
            return string.Format(Url, courseId,"", assignmentGuid);
        }
        
        public IList<Assignment> GetAssignmentItems(string parentFolderIdentifier, int folderType, string courseId)
        {
            string jsonString = GetJsonDocument(((parentFolderIdentifier == "") ? string.Format(Url, courseId, "", "") : (parentFolderIdentifier + "/QuestionItems")));
            Dictionary<string, Assignment> assignmentList;
            assignmentList = (jsonString != "null") ? JsonSerializer.DeserializeObject<Dictionary<string, Assignment>>(jsonString) : new Dictionary<string, Assignment>();
            foreach (var folderItem in assignmentList)
            {
                folderItem.Value.UniqueIdentifier = folderItem.Key.ToString();
                folderItem.Value.Url = string.Concat(string.Format(Url, courseId, "", ""), folderItem.Value.UniqueIdentifier);
            }
            return ConvertDictionarytoObject(assignmentList);
        }

        public IList<Core.SkillSetBuilder.SkillSet> GetSkillSetsForAnAssignment(string assignmentUrl)
        {
            string jsonString = GetJsonDocument(assignmentUrl);
            Dictionary<string, Core.SkillSetBuilder.SkillSet> skillSetList;
            skillSetList = (jsonString != "null") ? JsonSerializer.DeserializeObject<Dictionary<string, Core.SkillSetBuilder.SkillSet>>(jsonString) : new Dictionary<string, Core.SkillSetBuilder.SkillSet>();
            foreach (var folderItem in skillSetList)
            {
                folderItem.Value.UniqueIdentifier = folderItem.Key.ToString();
                folderItem.Value.Url = string.Concat(assignmentUrl, "/SkillSets/", folderItem.Value.UniqueIdentifier);
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
