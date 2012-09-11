using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;
using SimChartMedicalOffice.Core.QuestionBanks;
using System.Collections.Generic;
using SimChartMedicalOffice.Common.Utility;
using System.Linq;
using System;
using SimChartMedicalOffice.Common;
using System.Globalization;


namespace SimChartMedicalOffice.Data
{
    public class QuestionBankDocument : KeyValueRepository<Core.QuestionBanks.QuestionBankFolder>, IQuestionBankDocument
    {

        private static Dictionary<string, int> m_QuestionTypeList;

        public QuestionBankDocument()
        {
            this.LoadQuestionType();
        }


        public void LoadQuestionType()
        {
            Dictionary<string, int> questionTemplate = new Dictionary<string, int>();
            string jsonString = GetJsonDocument("SimApp/Master/QuestionType");
            if (jsonString.ToString() != "null")
            {
                ClearQuestionType();
                questionTemplate = JsonSerializer.DeserializeObject<Dictionary<string, int>>(jsonString);
                m_QuestionTypeList = questionTemplate;
            }
            
        }

        private static void ClearQuestionType()
        {
            if (m_QuestionTypeList != null && m_QuestionTypeList.Count > 0)
            {
                m_QuestionTypeList.Clear();
            }
        }


        public override string Url
        {
            get
            {
                return "SimApp/Courses/ELSEVIER_CID/Admin/QuestionBank";
            }
        }
        public IList<DocumentProxy> allQuestions = new List<DocumentProxy>();
        /// <summary>
        /// To get question bank object
        /// </summary>
        /// <returns></returns>
        public Folder GetQuestionBank()
        {
            string jsonString = GetJsonDocument(this.Url);
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
            return;
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
                folders.ToList().ForEach(F => GetTotalQuestionList(F));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="questions"></param>
        private void CollectQuestionsFromQuestionBank(Dictionary<string, Question> questions,Folder parentFolder)
        {
            if (questions != null && questions.Count > 0)
            {
                var questionList = questions.Select(question => question.Value).ToList();
                allQuestions = allQuestions.Concat(TransformQuestionsToDocumentProxy(questionList, parentFolder)).ToList();
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
                                                          LinkedItemReference = lstquestionItem.CompetencyReferenceGUID,
                                                          TypeOfQuestion = (AppCommon.QuestionTypeOptionsForLanding.Single(x => x.Key == Convert.ToInt32(lstquestionItem.QuestionType)).Value.ToString()),
                                                          CreatedTimeStamp = lstquestionItem.CreatedTimeStamp,
                                                          Url = lstquestionItem.Url,
                                                          UniqueIdentifier = (!String.IsNullOrEmpty(lstquestionItem.Url) ? lstquestionItem.Url.Split('/').Last() : String.Empty),
                                                          AnswerTexts = ((lstquestionItem.AnswerOptions != null && lstquestionItem.AnswerOptions.Count>0) ? lstquestionItem.AnswerOptions.Select(F => F.AnswerText).ToList() : ((lstquestionItem.CorrectOrder != null && lstquestionItem.CorrectOrder.Count>0)? lstquestionItem.CorrectOrder : new List<string>())),
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
            allQuestions = new List<DocumentProxy>();
            GetTotalQuestionList(questionBank); 
            return allQuestions.ToList();
        }


        public Dictionary<string, int> GetQuestionType()
        {
            return m_QuestionTypeList;
        }
    }
 
}
