using System.Collections.Generic;
using System.Linq;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.DataInterfaces.SkillSetBuilder;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Data.SkillSet
{
    public class SkillSetDocument : KeyValueRepository<Core.SkillSetBuilder.SkillSet>, ISkillSetDocument
    {

        public override string Url
        {
            get
            {
                return "SimApp/Courses/{0}/SkillSetRepository{1}/SkillSets/{2}";
            }
        }

        /// <summary>
        /// Method to get the question items
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public IList<Core.SkillSetBuilder.SkillSet> GetSkillSetItems(string parentFolderIdentifier, int folderType, string courseId)
        {
            string jsonString = GetJsonDocument(((parentFolderIdentifier == "") ? string.Format(Url, courseId, "", "") : (parentFolderIdentifier + "/SkillSets")));
            Dictionary<string, Core.SkillSetBuilder.SkillSet> skillSetList;
            skillSetList = (jsonString != "null") ? JsonSerializer.DeserializeObject<Dictionary<string, Core.SkillSetBuilder.SkillSet>>(jsonString) : new Dictionary<string, Core.SkillSetBuilder.SkillSet>();
            foreach (var folderItem in skillSetList)
            {
                folderItem.Value.UniqueIdentifier = folderItem.Key.ToString();
            }
            return ConvertDictionarytoObject(skillSetList);
        }
        public Core.SkillSetBuilder.SkillSet GetSkillSet(string skillSetUrl)
        {
            string jsonString = GetJsonDocument(skillSetUrl);
            return JsonSerializer.DeserializeObject<Core.SkillSetBuilder.SkillSet>(jsonString);
        }
        private IList<Core.SkillSetBuilder.SkillSet> ConvertDictionarytoObject(Dictionary<string, Core.SkillSetBuilder.SkillSet> skillSetItems)
        {
            return ((skillSetItems != null) ? (skillSetItems.Select(skillSetItem => skillSetItem.Value).ToList()) : new List<Core.SkillSetBuilder.SkillSet>());
        }

        public IList<Question> GetQuestionsForSkillSet(string skillSetUrl)
        {

            string strUrl = skillSetUrl + "/Questions";
            string jsonString = GetJsonDocument(strUrl);
            Dictionary<string, Question> questionsForSkillSet;
            questionsForSkillSet = (jsonString != "null") ? JsonSerializer.DeserializeObject<Dictionary<string, Question>>(jsonString) : new Dictionary<string, Question>();
            if (questionsForSkillSet != null && questionsForSkillSet.Count > 0)
            {
                foreach (var folderItem in questionsForSkillSet)
                {
                    folderItem.Value.UniqueIdentifier = folderItem.Key.ToString();
                    folderItem.Value.Url = skillSetUrl + "/Questions/" + folderItem.Key.ToString();
                }
            }
            return ConvertDictionarytoObject(questionsForSkillSet);
        }

        public IList<string> GetCompetencyGuidListInSkillSet(string skillSetUniqueIdentifier)
        {
            string strUrl = skillSetUniqueIdentifier + "/Competencies";
            string jsonString = GetJsonDocument(strUrl);
            IList<string> competencyGuidList;
            competencyGuidList = (jsonString != "null") ? JsonSerializer.DeserializeObject <IList<string>>(jsonString) : new List<string>();
            return competencyGuidList;
        }

        /// <summary>
        /// method to convert dictionary to object
        /// </summary>
        /// <param name="questionItems"></param>
        /// <returns></returns>
        private IList<Question> ConvertDictionarytoObject(Dictionary<string, Question> questionItems)
        {
            return ((questionItems != null) ? (questionItems.Select(questionItem => questionItem.Value).ToList()) : new List<Question>());
        }
        /// <summary>
        /// To get question bank object
        /// </summary>
        /// <returns></returns>
        public Folder GetSkillSets()
        {
            string jsonString = GetJsonDocument(this.Url);
            Folder skillSet = JsonSerializer.DeserializeObject<Folder>(jsonString);
            return skillSet;
        }
        

    }
}
