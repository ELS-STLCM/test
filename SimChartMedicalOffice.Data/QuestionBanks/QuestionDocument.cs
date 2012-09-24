using System.Collections.Generic;
using System.Linq;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Data
{
    public class QuestionDocument : KeyValueRepository<Question>, IQuestionDocument
    {
        /// <summary>
        /// Method to get the question items
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="dropBox"> </param>
        /// <returns></returns>
        public IList<Question> GetQuestionItems(string parentFolderIdentifier, int folderType,DropBoxLink dropBox)
        {
            string jsonString = GetJsonDocument(((parentFolderIdentifier.Equals("")) ? string.Format(GetAssignmentUrl(dropBox,DocumentPath.Module.QuestionBank,AppConstants.Create), "", "") : (parentFolderIdentifier + "/" + Respository.QuestionItems)));
            Dictionary<string, Question> questionList = (jsonString!="null")?JsonSerializer.DeserializeObject<Dictionary<string, Question>>(jsonString):new Dictionary<string, Question>();
            foreach (var folderItem in questionList)
            {
                folderItem.Value.UniqueIdentifier = folderItem.Key;
                folderItem.Value.Url = string.Concat(string.Format(GetAssignmentUrl(dropBox, DocumentPath.Module.QuestionBank, AppConstants.Create), "", ""), folderItem.Value.UniqueIdentifier);
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
