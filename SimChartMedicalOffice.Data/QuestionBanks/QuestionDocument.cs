using System.Collections.Generic;
using System.Linq;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;

namespace SimChartMedicalOffice.Data
{
    public class QuestionDocument : KeyValueRepository<Core.QuestionBanks.Question>, IQuestionDocument
    {
        public override string Url
        {
            get
            {
                return "SimApp/Courses/{0}/QuestionBank{1}/QuestionItems/{2}";
            }
        }
        /// <summary>
        /// Method to get the question items
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public IList<Question> GetQuestionItems(string parentFolderIdentifier, int folderType,string courseId)
        {
            string jsonString = GetJsonDocument(((parentFolderIdentifier == "") ? string.Format(Url, courseId, "", "") : (parentFolderIdentifier + "/QuestionItems")));
            Dictionary<string, Question> questionList;
            questionList = (jsonString!="null")?JsonSerializer.DeserializeObject<Dictionary<string, Question>>(jsonString):new Dictionary<string, Question>();
            foreach (var folderItem in questionList)
            {
                folderItem.Value.UniqueIdentifier = folderItem.Key.ToString();
                folderItem.Value.Url = string.Concat(string.Format(Url, courseId, "", ""), folderItem.Value.UniqueIdentifier);
            }
            return ConvertDictionarytoObject(questionList);
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
    }
}
