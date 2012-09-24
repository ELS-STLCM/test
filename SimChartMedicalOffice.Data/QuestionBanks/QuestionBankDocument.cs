using System;
using System.Collections.Generic;
using System.Linq;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core;


namespace SimChartMedicalOffice.Data
{
    public class QuestionBankDocument : KeyValueRepository<QuestionBankFolder>, IQuestionBankDocument
    {

        private static Dictionary<string, int> _mQuestionTypeList;

        public QuestionBankDocument()
        {
            LoadQuestionType();
        }


        public void LoadQuestionType()
        {
            string jsonString = GetJsonDocument(GetAssignmentUrl(DocumentPath.Module.Masters,AppConstants.QuestionType));
            if (jsonString != "null")
            {
                ClearQuestionType();
                Dictionary<string, int> questionTemplate = JsonSerializer.DeserializeObject<Dictionary<string, int>>(jsonString);
                _mQuestionTypeList = questionTemplate;
            }
            
        }

        private static void ClearQuestionType()
        {
            if (_mQuestionTypeList != null && _mQuestionTypeList.Count > 0)
            {
                _mQuestionTypeList.Clear();
            }
        }


        
        public IList<DocumentProxy> AllQuestions = new List<DocumentProxy>();
        /// <summary>
        /// To get question bank object
        /// </summary>
        /// <returns></returns>
        public Folder GetQuestionBank()
        {
            string jsonString = GetJsonDocument(GetAssignmentUrl(DocumentPath.Module.QuestionBank));
            Folder questionBank = JsonSerializer.DeserializeObject<Folder>(jsonString);
            return questionBank;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentFolder"></param>
        private void GetTotalQuestionList(Folder parentFolder)
        {
            if (parentFolder != null)
            {
                TraverseEachFolderForQuestions(parentFolder.SubFolders);
                CollectQuestionsFromQuestionBank(parentFolder.QuestionItems, parentFolder);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderContent"></param>
        private void TraverseEachFolderForQuestions(Dictionary<string,Folder> folderContent)
        {
            if (folderContent != null && folderContent.Count > 0)
            {
                IList<Folder> folders = folderContent.Select(folder => folder.Value).ToList();
                folders.ToList().ForEach(f => GetTotalQuestionList(f));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="parentFolder"> </param>
        private void CollectQuestionsFromQuestionBank(Dictionary<string, Question> questions,Folder parentFolder)
        {
            if (questions != null && questions.Count > 0)
            {
                var questionList = questions.Select(question => question.Value).ToList();
                AllQuestions = AllQuestions.Concat(TransformQuestionsToDocumentProxy(questionList, parentFolder)).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="questionList"></param>
        /// <param name="parentFolder"></param>
        /// <returns></returns>
        private List<DocumentProxy> TransformQuestionsToDocumentProxy(IList<Question> questionList,Folder parentFolder)
        {
            List<DocumentProxy> lstOfQuestionProxy = (from lstquestionItem in questionList
                                                      select new DocumentProxy
                                                      {
                                                          FolderName = (!String.IsNullOrEmpty(parentFolder.Name) ? parentFolder.Name : AppCommon.QuestionBankLandingPageFolderName),
                                                          FolderIdentifier =(!String.IsNullOrEmpty(parentFolder.Url)?parentFolder.Url.Split('/').Last():String.Empty),
                                                          FolderUrl = (!String.IsNullOrEmpty(parentFolder.Url) ? parentFolder.Url.Replace((!String.IsNullOrEmpty(parentFolder.Url) ? "/" + parentFolder.Url.Split('/').Last() : String.Empty), "") : String.Empty),
                                                          Text = lstquestionItem.QuestionText,
                                                          LinkedItemReference = lstquestionItem.CompetencyReferenceGuid,
                                                          TypeOfQuestion = (AppCommon.QuestionTypeOptionsForLanding.Single(x => x.Key == Convert.ToInt32(lstquestionItem.QuestionType)).Value),
                                                          CreatedTimeStamp = lstquestionItem.CreatedTimeStamp,
                                                          Url = lstquestionItem.Url,
                                                          UniqueIdentifier = (!String.IsNullOrEmpty(lstquestionItem.Url) ? lstquestionItem.Url.Split('/').Last() : String.Empty),
                                                          AnswerTexts = ((lstquestionItem.AnswerOptions != null && lstquestionItem.AnswerOptions.Count>0) ? lstquestionItem.AnswerOptions.Select(f => f.AnswerText).ToList() : ((lstquestionItem.CorrectOrder != null && lstquestionItem.CorrectOrder.Count>0)? lstquestionItem.CorrectOrder : new List<string>())),
                                                          Rationale=lstquestionItem.Rationale
                                                      }).ToList();
            return lstOfQuestionProxy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<DocumentProxy> GetAllQuestionsInAQuestionBank()
        {
            Folder questionBank = GetQuestionBank();
            AllQuestions = new List<DocumentProxy>();
            GetTotalQuestionList(questionBank); 
            return AllQuestions.ToList();
        }


        public Dictionary<string, int> GetQuestionType()
        {
            return _mQuestionTypeList;
        }
    }
 
}
