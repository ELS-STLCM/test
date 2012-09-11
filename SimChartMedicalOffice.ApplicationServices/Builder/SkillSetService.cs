using System;
using System.Collections.Generic;
using System.Linq;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Extensions;
using SimChartMedicalOffice.Core.DataInterfaces;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;
using SimChartMedicalOffice.Core.DataInterfaces.SkillSetBuilder;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.SkillSetBuilder;
namespace SimChartMedicalOffice.ApplicationServices.Builder
{
    public class SkillSetService : ISkillSetService
    {
        private readonly IQuestionDocument _questionDocument;
        private readonly IAttachmentDocument _attachmentDocument;
        private readonly IFolderDocument _folderDocument;
        private readonly IQuestionBankDocument _questionBankDocument;
        private readonly ICompetencyService _competencyService;
        private readonly ISkillSetDocument _skillSetDocument;
        private readonly ISkillSetRepositoryDocument _skillSetRepositoryDocument;
        private readonly IQuestionBankService _questionBankService;
        
        private IList<SkillSetProxy> allSkillSets;

        /// <summary>
        /// initalising the skillset Document document
        /// </summary>
        /// <param name="skillSetDocumnet"></param>
        public SkillSetService(ISkillSetRepositoryDocument skillSetRepositoryDocument, ISkillSetDocument skillSetDocument, IQuestionDocument questionDocument, IAttachmentDocument attachmentDocument, IFolderDocument folderDocumentInstance, IQuestionBankDocument questionBankDocumentInstance, ICompetencyService competencyService, IQuestionBankService questionBankService)
        {
            this._questionDocument = questionDocument;
            this._attachmentDocument = attachmentDocument;
            this._folderDocument = folderDocumentInstance;
            this._questionBankDocument = questionBankDocumentInstance;
            this._competencyService = competencyService;
            this._skillSetDocument = skillSetDocument;
            this._skillSetRepositoryDocument = skillSetRepositoryDocument;
            this._questionBankService = questionBankService;
        }

        public IList<Question> GetQuestionsForSkillSet(string skillSeTUrl)
        {
            IList<Question> questionsForSkillSet = _skillSetDocument.GetQuestionsForSkillSet(skillSeTUrl);
            return questionsForSkillSet;
        }

        
        /// <summary>
        /// To get the proxy object for step 4 of Skill set builder.
        /// </summary>
        /// <param name="skillSeTUrl"></param>
        /// <returns></returns>
        public IList<DocumentProxy> GetQuestionsForPreview(string skillSeTUrl)
        {
            List<DocumentProxy> questionsForSkillSetProxy = new List<DocumentProxy>();
            List<DocumentProxy> questionsForSkillSetProxyTemp = new List<DocumentProxy>();
            IList<Question> questionsForSkillSet = GetQuestionsForSkillSet(skillSeTUrl);
            //To get all the questions to a proxy from a skill set.
            foreach (Question questionItem in questionsForSkillSet)
            {
                DocumentProxy DocumentProxyItem = new DocumentProxy();
                DocumentProxyItem.LinkedItemReference = questionItem.CompetencyReferenceGUID;
                DocumentProxyItem.Text = questionItem.QuestionText;
                questionsForSkillSetProxy.Add(DocumentProxyItem);
            }
            //Grouping the Questions based on Competencies.
            questionsForSkillSetProxy.ToList().ForEach(question => _competencyService.SetLinkedCompetencyTextForAQuestions(question.LinkedItemReference, question));
            var questions = questionsForSkillSetProxy.GroupBy(question => question.LinkedItemReference);

            //Forming the document proxy with list of questions for each Competency
            foreach (var group in questions)
            {
                List<string> qnTextList = new List<string>();
                string competencyVal = String.Empty;
                DocumentProxy DocumentProxyItem = new DocumentProxy();
                foreach (var groupItem in group)
                {
                    qnTextList.Add(groupItem.Text);
                    competencyVal = groupItem.LinkedItemReference;
                }
                DocumentProxyItem.AnswerTexts = qnTextList;
                DocumentProxyItem.Text = competencyVal;
                questionsForSkillSetProxyTemp.Add(DocumentProxyItem);
            }
            return questionsForSkillSetProxyTemp;

        }

        /// <summary>
        /// to set and get dynamic url for skill
        /// </summary>
        /// <param name="skillSetObjFromUI"></param>
        /// <param name="courseId"></param>
        /// <param name="skillSetUrl"></param>
        /// <param name="isEditMode"></param>
        /// <param name="folderIdentifier"></param>
        /// <returns></returns>
        private string FormAndSetUrlForSkillSet(SkillSet skillSetObjFromUI, string courseId, string skillSetUrl, bool isEditMode, string folderIdentifier)
        {
            try
            {
                if (isEditMode)
                {
                    return skillSetUrl;
                }
                else
                {
                    if (String.IsNullOrEmpty(skillSetUrl) && String.IsNullOrEmpty(folderIdentifier))
                    {
                        return string.Format(_skillSetDocument.Url, courseId, skillSetUrl, skillSetObjFromUI.GetNewGuidValue());
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(skillSetUrl) && !String.IsNullOrEmpty(folderIdentifier))
                        {
                            return  folderIdentifier+"/SkillSets/"+ skillSetObjFromUI.GetNewGuidValue();
                        }
                    }
                }
            }
            catch
            {
                //To-Do
            }
            return String.Empty;

        }


        /// <summary>
        /// to get list of SkillSet
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="courseId"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public IList<SkillSet> GetSkillSetItems(string parentFolderIdentifier, int folderType, string courseId, string folderUrl)
        {
            try
            {
                if (AppCommon.CheckIfStringIsEmptyOrNull(parentFolderIdentifier))
                {
                    return _skillSetDocument.GetSkillSetItems(parentFolderIdentifier, folderType, courseId);
                }
                return _folderDocument.GetSkillSetItems(parentFolderIdentifier, courseId, folderUrl);
            }
            catch
            {
                //To-Do                
            }
            return new List<SkillSet>();
        }



        /// <summary>
        /// To get search results from SkillSets
        /// </summary>
        /// <param name="strSearchText"></param>
        /// <param name="sortColumnIndex"></param>
        /// <param name="sortColumnOrder"></param>
        /// <returns></returns>
        public List<SkillSetProxy> GetSearchResultsForSkillSet(string strSearchText, int sortColumnIndex, string sortColumnOrder, string strSource)
        {
            IList<SkillSetProxy> lstSKillSetSearchResult = new List<SkillSetProxy>();
            IList<SkillSetProxy> lstSKillSetSearchResultTemp = new List<SkillSetProxy>();
            IList<Core.Competency.Competency> competenciesForSkillSet = new List<Core.Competency.Competency>();

            lstSKillSetSearchResult = GetAllSkillSetsInSkillSetRepository();
            competenciesForSkillSet = GetAllCompetenciesForSkillSetsProxies(lstSKillSetSearchResult);
            if (lstSKillSetSearchResult != null)
            {
                lstSKillSetSearchResult.ToList().ForEach(s => SetLinkedCompetencySources(s,s.LinkedCompetencies, competenciesForSkillSet));
                if (!String.IsNullOrEmpty(strSearchText))
                {
                    lstSKillSetSearchResultTemp = GetSkillSetsMatchingText(strSearchText, lstSKillSetSearchResult);
                }
            }
            if (!String.IsNullOrEmpty(strSource))
            {
                lstSKillSetSearchResultTemp = GetSkillSetsMatchingSource(strSource, lstSKillSetSearchResultTemp);
            }
            string sortColumnName = AppCommon.gridColumnForSkillSetSearchList[sortColumnIndex];
            var sortableList = lstSKillSetSearchResultTemp.AsQueryable();
            lstSKillSetSearchResultTemp = sortableList.OrderBy<SkillSetProxy>(sortColumnName, sortColumnOrder).ToList<SkillSetProxy>();
            return lstSKillSetSearchResultTemp.ToList();
        }
        /// <summary>
        /// To get the search text matching SkillSets , search for Skill Set Title, Competencies and focus
        /// </summary>
        /// <param name="strSearchText"></param>
        /// <param name="lstSKillSetSearchResult"></param>
        /// <returns></returns>
        private List<SkillSetProxy> GetSkillSetsMatchingText(string strSearchText, IList<SkillSetProxy> lstSKillSetSearchResult)
        {
            var lstSKillSetSearchResultTemp = (from lstSearch in lstSKillSetSearchResult
                                               where (
                                                   (!String.IsNullOrEmpty(lstSearch.SkillSetTitle) && lstSearch.SkillSetTitle.ToLower().Contains(strSearchText.ToLower()))
                                                 ||(lstSearch.Focus != null && lstSearch.Focus.Count(f => (f != null && f.ToLower().Contains(strSearchText.ToLower()))) > 0)
                                                 || (lstSearch.LinkedCompetencies != null && lstSearch.LinkedCompetenciesText.ToLower().Contains(strSearchText.ToLower())))
                                               select lstSearch).ToList();
            return lstSKillSetSearchResultTemp.ToList();
        }

        /// <summary>
        /// To get the search text matching skillsets, filter by source
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="lstQuestionSearchResult"></param>
        /// <returns></returns>
        public List<SkillSetProxy> GetSkillSetsMatchingSource(string strSource, IList<SkillSetProxy> lstSKillSetSearchResult)
        {
            var lstSKillSetSearchResultTemp = (from lstSearch in lstSKillSetSearchResult
                                               where (
                                               (lstSearch.Source != null && lstSearch.Source.Count(f => (f != null && f.ToLower().Contains(strSource.ToLower()))) > 0))
                                               select lstSearch).ToList();
            return lstSKillSetSearchResultTemp.ToList();
        }

        private void SetLinkedCompetencySources(SkillSetProxy skillsetProxy, IList<string> competencyGuids, IList<Core.Competency.Competency> competencyList)
        {
            skillsetProxy.Source = GetLinkedCompetencySources(competencyGuids, competencyList);
            skillsetProxy.SourceNameText = string.Join(" ", skillsetProxy.Source);
        }

        public IList<string> GetLinkedCompetencySources(IList<string> competencyGuids, IList<Core.Competency.Competency> competencyList)
        {
            IList<string> sourceListBasic = AppCommon.SourceOptions.Select(x => x.Value).ToList();
            IList<string> sourceListReqd = new List<string>();
            if (competencyGuids != null)
            {
                foreach (string competencyGuid in competencyGuids)
                {
                    Core.Competency.Competency competencyObj =
                        (from competency in competencyList
                         where competency.UniqueIdentifier == competencyGuid
                         select competency).SingleOrDefault();
                    if (competencyObj != null && competencyObj.Sources != null && competencyObj.Sources.Count > 0)
                    {
                        IList<string> sourceNameLocal =
                            (from source in competencyObj.Sources
                             where (!sourceListReqd.Contains(source.Name) && sourceListBasic.Contains(source.Name))
                             select source.Name).ToList();
                        if (sourceNameLocal != null && sourceNameLocal.Count > 0)
                        {
                            sourceListReqd = sourceListReqd.Concat(sourceNameLocal).ToList();
                        }
                    }
                }
            }

            return sourceListReqd;
        }

        public IList<Core.Competency.Competency> GetAllCompetenciesForSkillSets(IList<SkillSet> skillSets)
        {
            IList<Core.Competency.Competency> competenciesForSkillSet = new List<Core.Competency.Competency>();
            foreach (SkillSet skillSet in skillSets)
            {
                if (skillSet.Competencies != null && skillSet.Competencies.Count > 0)
                {
                    foreach (string competencyGuid in skillSet.Competencies)
                    {
                        Core.Competency.Competency competency = _competencyService.GetCompetency(competencyGuid);
                        if (competency != null && competency.Name != null)
                        {
                            competenciesForSkillSet.Add(competency);
                        }
                    }
                }
            }

            competenciesForSkillSet = competenciesForSkillSet.Distinct().ToList();
            return competenciesForSkillSet;
        }


        public IList<Core.Competency.Competency> GetAllCompetenciesForASkillSet(SkillSet skillSet)
        {
            IList<Core.Competency.Competency> competenciesForSkillSet = new List<Core.Competency.Competency>();

            if (skillSet!=null && skillSet.Competencies != null && skillSet.Competencies.Count > 0)
                {
                    foreach (string competencyGuid in skillSet.Competencies)
                    {
                        Core.Competency.Competency competency = _competencyService.GetCompetency(competencyGuid);
                        if (competency != null && competency.Name != null)
                        {
                            competenciesForSkillSet.Add(competency);
                        }
                    }
                }
            
            competenciesForSkillSet = competenciesForSkillSet.Distinct().ToList();
            return competenciesForSkillSet;
        }

        private IList<Core.Competency.Competency> GetAllCompetenciesForSkillSetsProxies(IList<SkillSetProxy> skillSets)
        {
            IList<Core.Competency.Competency> competenciesForSkillSet = new List<Core.Competency.Competency>();
            foreach (SkillSetProxy skillSet in skillSets)
            {
                if (skillSet.LinkedCompetencies != null && skillSet.LinkedCompetencies.Count > 0)
                {
                    foreach (string competencyGuid in skillSet.LinkedCompetencies)
                    {
                        Core.Competency.Competency competency = _competencyService.GetCompetency(competencyGuid);
                        if (competency != null && competency.Name != null)
                        {
                            competenciesForSkillSet.Add(competency);
                        }
                    }
                }
            }

            competenciesForSkillSet = competenciesForSkillSet.Distinct().ToList();
            return competenciesForSkillSet;
        }

        /// <summary>
        /// To get string for the list of focus in a skill set
        /// </summary>
        /// <param name="skillSeTUrl"></param>
        /// <returns></returns>
        public string GetFocusForSkillSet(string skillSeTUrl)
        {
            string focusOfSkillSet = String.Empty;
            SkillSet skillSetObj = _skillSetDocument.GetSkillSet(skillSeTUrl);
            if (skillSetObj.Focus != null && skillSetObj.Focus.Count > 0)
            {
                focusOfSkillSet = String.Join(", ", skillSetObj.Focus.Select(s => s));
                return focusOfSkillSet;
            }
            return focusOfSkillSet;
        }
        public string GetSourcesForSkillSet(string skillSeTUrl)
        {
            return "ABHES, MAERB";
        }
        /// <summary>
        /// To get a skill set
        /// </summary>
        /// <param name="skillSetUrl"></param>
        /// <returns></returns>
        public SkillSet GetSkillSet(string skillSetUrl)
        {
            return _skillSetDocument.GetSkillSet(skillSetUrl);
        }
        /// <summary>
        /// To publish a skill set 
        /// </summary>
        /// <param name="skillSetUrl"></param>
        /// <returns></returns>
        public bool PublishSkillSet(string skillSetUrl)
        {
            SkillSet skillsetObj = _skillSetDocument.GetSkillSet(skillSetUrl);
            if (skillsetObj != null)
            {
                skillsetObj.Status = AppCommon.Status_Published;
                _skillSetDocument.SaveOrUpdate(skillSetUrl, skillsetObj);
                return true;
            }
            return false;
        }


        public List<DocumentProxy> GetSearchResultsForQuestionBank(IList<string> skillSetCompetencyGUID, string competencySearchGuid, string strQuestionType)
        {
            List<DocumentProxy> lstQuestionSearchResult = new List<DocumentProxy>();
            lstQuestionSearchResult = _questionBankDocument.GetAllQuestionsInAQuestionBank();
            lstQuestionSearchResult = (from qus in lstQuestionSearchResult join compGuid in skillSetCompetencyGUID on qus.LinkedItemReference equals compGuid.ToString() select qus).ToList();
            if (String.IsNullOrEmpty(competencySearchGuid) && String.IsNullOrEmpty(strQuestionType))
            {
                return lstQuestionSearchResult.ToList();
            }
            else
            {
                if (!String.IsNullOrEmpty(competencySearchGuid))
                {
                    lstQuestionSearchResult = lstQuestionSearchResult.Where(qus => (qus.LinkedItemReference.Equals(competencySearchGuid))).ToList();
                }
                if (!String.IsNullOrEmpty(strQuestionType))
                {
                    lstQuestionSearchResult = lstQuestionSearchResult.Where(qus => (qus.TypeOfQuestion.Equals(strQuestionType))).ToList();
                }
            }
            return lstQuestionSearchResult.ToList();
        }
        public IList<DocumentProxy> GetCompetencyQuestionListInSkillSet(string skillSetUniqueIdentifier, string competencySearchText, string strQuestionType)
        {
            List<DocumentProxy> competencyQuestionList = new List<DocumentProxy>();
            IList<string> skillSetCompetencyGUID = GetCompetencyGuidListInSkillSet(skillSetUniqueIdentifier);
            competencyQuestionList = GetSearchResultsForQuestionBank(skillSetCompetencyGUID, competencySearchText, strQuestionType);
            return competencyQuestionList;
        }

        public IList<string> GetCompetencyGuidListInSkillSet(string skillSetUniqueIdentifier)
        {
            IList<string> skillSetCompetencyGUID = _skillSetDocument.GetCompetencyGuidListInSkillSet(skillSetUniqueIdentifier);
            return skillSetCompetencyGUID;
        }

        public List<AutoCompleteProxy> GetCompetencyGuidListInSkillSetForFlexBox(string skillSetUniqueIdentifier)
        {
            IList<string> skillSetCompetencyGUID = GetCompetencyGuidListInSkillSet(skillSetUniqueIdentifier);

            List<AutoCompleteProxy> competencyStringList = _competencyService.GetAllCompetencyListForDropDown();
            competencyStringList = (from com in competencyStringList join compGuid in skillSetCompetencyGUID on com.id equals compGuid.ToString() select com ).ToList();            
            return competencyStringList;
        }


        public void SetLinkedCompetencyTextForAQuestions(string guidOfLinkedCompetency, SkillSetProxy skillSetProxy)
        {
            throw new NotImplementedException();
        }

        public void SwapSkillSetSave(SkillSet skillSetObjToSave, string skillSetUrl)
        {
            string strUrlToSave = FormAndSetUrlForSkillSet(skillSetObjToSave, "", skillSetUrl, true, "");
            _skillSetDocument.SaveOrUpdate(strUrlToSave, skillSetObjToSave);
        }

        public bool SaveSkillSet(SkillSet skillSetObjectFromUi, string courseId, string skillSetUrl, string folderIdentifier, bool isEditMode, out string skillSetIdentifier)
        {
            try
            {
                // Set Guid referncce and Correct Url
                string strUrlToSave = FormAndSetUrlForSkillSet(skillSetObjectFromUi, courseId, skillSetUrl, isEditMode,
                                                               folderIdentifier);
                skillSetIdentifier = strUrlToSave;

                if (isEditMode)
                {
                    SkillSet skillSetObj=_skillSetDocument.GetSkillSet(strUrlToSave);
                    skillSetObj.Competencies=skillSetObjectFromUi.Competencies;
                    skillSetObj.Focus = skillSetObjectFromUi.Focus;
                    skillSetObj.SkillSetTitle = skillSetObjectFromUi.SkillSetTitle;
                    skillSetObj.SequenceNumber = skillSetObjectFromUi.SequenceNumber;
                    skillSetObj.Url = strUrlToSave;
                    List<string> lstOfGuidsForQuestions = new List<string>();
                    if (skillSetObj.Questions != null && skillSetObj.Questions.Count > 0)
                    {

                        foreach (var item in skillSetObj.Questions)
                        {
                            Question questionObj = item.Value;
                            if (!String.IsNullOrEmpty(questionObj.CompetencyReferenceGUID))
                            {

                                if (!skillSetObj.Competencies.Contains(questionObj.CompetencyReferenceGUID))
                                {
                                    lstOfGuidsForQuestions.Add(item.Key);
                                }
                            }
                        }
                        foreach (var itemGuids in lstOfGuidsForQuestions)
                        {
                            skillSetObj.Questions.Remove(itemGuids);
                        }
                    }

                    _skillSetDocument.SaveOrUpdate(strUrlToSave, skillSetObj);
                }else{
                    skillSetObjectFromUi.Url = strUrlToSave;
                    _skillSetDocument.SaveOrUpdate(strUrlToSave, skillSetObjectFromUi);
                }
            }
            catch
            {
                skillSetIdentifier = String.Empty;
                return false;
            }
            return true;

        }

        public IList<AutoCompleteProxy> GetFormattedCompetenciesForSkillSet(SkillSet skillSetObj)
        {
            IList<AutoCompleteProxy> formattedCompetencyList = null;
            if (skillSetObj != null && skillSetObj.Competencies != null && skillSetObj.Competencies.Count > 0)
            {
                IList<Core.Competency.Competency> competencyList = GetCompetenciesForSkillSet(skillSetObj);
                if (competencyList != null && competencyList.Count > 0)
                {
                    formattedCompetencyList = _competencyService.GetCompetenciesStringListInFormat(competencyList);
                }
            }
            return formattedCompetencyList;
        }

        private IList<Core.Competency.Competency> GetCompetenciesForSkillSet(SkillSet skillSetObj)
        {
            IList<Core.Competency.Competency> competencyList = new List<Core.Competency.Competency>();
            if (skillSetObj.Competencies != null && skillSetObj.Competencies.Count > 0)
            {
                foreach (string competencyId in skillSetObj.Competencies)
                {
                    Core.Competency.Competency competency = _competencyService.GetCompetency(competencyId);
                    if (competency != null)
                    {
                        competencyList.Add(competency);
                    }
                }
            }
            return competencyList;
        }

        public List<SkillSetProxy> GetAllSkillSetsInSkillSetRepository()
        {
            Folder skillSetRepository = _skillSetRepositoryDocument.GetSkillSetRepository();
            allSkillSets = new List<SkillSetProxy>();
            GetTotalSkillSetList(skillSetRepository);
            return allSkillSets.ToList();
        }

        public List<SkillSetProxy> GetFilteredSkillSetsBasedOnCompetency(string competencyGuid)
        {
            IList<SkillSetProxy> skillSetProxies = GetAllSkillSetsInSkillSetRepository();
            if (!string.IsNullOrEmpty(competencyGuid))
            {
                return (from skillSet in skillSetProxies
                        where
                            (skillSet.LinkedCompetencies != null
                                 ? skillSet.LinkedCompetencies.Contains(competencyGuid)
                                 : false)
                        select skillSet).ToList();
            }
            else
            {
                return skillSetProxies.ToList();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentFolder"></param>
        private void GetTotalSkillSetList(Folder parentFolder)
        {
            if (parentFolder != null)
            {
                TraverseEachFolderForSkillSets(parentFolder.SubFolders);
                CollectSkillSetsFromSkillSet(parentFolder.SkillSets, parentFolder);
            }
            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderContent"></param>
        private void TraverseEachFolderForSkillSets(Dictionary<string, Folder> folderContent)
        {
            if (folderContent != null && folderContent.Count > 0)
            {
                IList<Folder> folders = folderContent.Select(folder => folder.Value).ToList();
                folders.ToList().ForEach(F => GetTotalSkillSetList(F));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skillSets"></param>
        private void CollectSkillSetsFromSkillSet(Dictionary<string, Core.SkillSetBuilder.SkillSet> skillSets, Folder parentFolder)
        {
            if (skillSets != null && skillSets.Count > 0)
            {
                var skillSetList = skillSets.Select(s => s.Value).ToList();
                allSkillSets = allSkillSets.Concat(TransformSkillSetsToSkillSetProxy(skillSetList, parentFolder)).ToList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skillSetList"></param>
        /// <param name="parentFolder"></param>
        /// <returns></returns>
        public List<SkillSetProxy> TransformSkillSetsToSkillSetProxy(IList<Core.SkillSetBuilder.SkillSet> skillSetList, Folder parentFolder)
        {
            List<SkillSetProxy> lstOfSkillSetProxy = (from lstskillsetItem in skillSetList
                                                      select new SkillSetProxy
                                                      {
                                                          SkillSetTitle = lstskillsetItem.SkillSetTitle,
                                                          LinkedCompetencies = lstskillsetItem.Competencies,
                                                          LinkedCompetenciesText = (lstskillsetItem.Competencies!=null && lstskillsetItem.Competencies.Count>0)?_competencyService.GetCompetencyNameForListofCompetencies(lstskillsetItem.Competencies):"",
                                                          Status = lstskillsetItem.Status,
                                                          Focus = lstskillsetItem.Focus,
                                                          CreatedTimeStamp = lstskillsetItem.CreatedTimeStamp,
                                                          QuestionCount = (lstskillsetItem.Questions!=null && lstskillsetItem.Questions.Count > 0)?lstskillsetItem.Questions.Count.ToString():"",
                                                          SequenceNumber = lstskillsetItem.SequenceNumber,
                                                          Url = lstskillsetItem.Url,
                                                          UniqueIdentifier = (!String.IsNullOrEmpty(lstskillsetItem.Url) ? lstskillsetItem.Url.Split('/').Last() : String.Empty),
                                                          ParentReferenceGuid = (!String.IsNullOrEmpty(lstskillsetItem.ParentReferenceGuid)?lstskillsetItem.ParentReferenceGuid:"")
                                                      }).ToList();
            return lstOfSkillSetProxy;
        }



        //public bool SaveSkillStructure(string uniqueIdentifier, Dictionary<string, Question> selectQuestionOrderList)
        //{
        //    _questionDocument.SaveOrUpdate(uniqueIdentifier, selectQuestionOrderList);                
        //   return true;
        //}

        public bool SaveSkillStructure(string uniqueIdentifierUrl, List<DocumentProxy> documentProxyQuestionOrderList)
        {
            string identifierUrl = "";
            SkillSet skillSetToSave = new SkillSet();
            skillSetToSave.Questions = new Dictionary<string, Question>();

            try
            {
                List<DocumentProxy> selectQuestionTemplateList = documentProxyQuestionOrderList.Where(qusTemp => qusTemp.IsQuestionFromTemplate).ToList();
                List<DocumentProxy> selectQuestionBankList = documentProxyQuestionOrderList.Where(qusTemp => !(qusTemp.IsQuestionFromTemplate)).ToList();                
                foreach (DocumentProxy docProxyQuestionBank in selectQuestionBankList)
                {
                    if (!AppCommon.CheckIfStringIsEmptyOrNull(docProxyQuestionBank.ParentReferenceGuid))
                    {
                        Question questionFromSkillSet = _questionBankService.GetQuestion(docProxyQuestionBank.Url);
                        questionFromSkillSet.SequenceNumber = docProxyQuestionBank.OrderSequenceNumber;
                        questionFromSkillSet.CreatedBy = docProxyQuestionBank.CreatedBy;
                        questionFromSkillSet.CreatedTimeStamp = docProxyQuestionBank.CreatedTimeStamp;
                        skillSetToSave.Questions.Add(docProxyQuestionBank.UniqueIdentifier, questionFromSkillSet);
                    }
                    else
                    {
                        Question question = _questionBankService.GetQuestion(docProxyQuestionBank.Url);
                        string newGuidQuestionBank = question.GetNewGuidValue();
                        question.Url = uniqueIdentifierUrl + "/Questions/" + newGuidQuestionBank;
                        question.ParentReferenceGuid = docProxyQuestionBank.Url;
                        question.CreatedBy = docProxyQuestionBank.CreatedBy;
                        question.CreatedTimeStamp = docProxyQuestionBank.CreatedTimeStamp;
                        skillSetToSave.Questions.Add(newGuidQuestionBank, question);
                    }
                }
                foreach (DocumentProxy docProxyQuestionTemplate in selectQuestionTemplateList)
                {
                    if (!AppCommon.CheckIfStringIsEmptyOrNull(docProxyQuestionTemplate.Url))
                    {
                        //  ----------------- step 2 already saved question  --------------
                        Question docQusTempExist = _questionBankService.GetQuestion(docProxyQuestionTemplate.Url);
                        docQusTempExist.SequenceNumber = docProxyQuestionTemplate.OrderSequenceNumber;
                        docQusTempExist.CreatedBy = docProxyQuestionTemplate.CreatedBy;
                        docQusTempExist.CreatedTimeStamp = docProxyQuestionTemplate.CreatedTimeStamp;
                        docQusTempExist.IsAutoSave = docProxyQuestionTemplate.IsAutoSave;
                        docQusTempExist.IsActive = docProxyQuestionTemplate.IsActive;
                        string uniqueIdentifierAlreadySavedQuestionBank = (!String.IsNullOrEmpty(docProxyQuestionTemplate.Url) ? docProxyQuestionTemplate.Url.Split('/').Last() : String.Empty);
                        skillSetToSave.Questions.Add(uniqueIdentifierAlreadySavedQuestionBank, docQusTempExist);
                    }
                    else
                    {
                        //  ----------------- step 2 new saved question  --------------
                        Question questionTemplate = new Question();
                        questionTemplate.QuestionText = docProxyQuestionTemplate.Text;
                        questionTemplate.QuestionType = docProxyQuestionTemplate.TypeOfQuestion;
                        questionTemplate.SequenceNumber = docProxyQuestionTemplate.OrderSequenceNumber;
                        questionTemplate.IsQuestionFromTemplate = docProxyQuestionTemplate.IsQuestionFromTemplate;
                        questionTemplate.TemplateSequenceNumber = docProxyQuestionTemplate.TemplateSequenceNumber;
                        questionTemplate.CreatedBy = docProxyQuestionTemplate.CreatedBy;
                        questionTemplate.CreatedTimeStamp = docProxyQuestionTemplate.CreatedTimeStamp;
                        string newGuidQuestion = questionTemplate.GetNewGuidValue();
                        questionTemplate.Url = uniqueIdentifierUrl + "/Questions/" + newGuidQuestion;
                        skillSetToSave.Questions.Add(newGuidQuestion, questionTemplate);
                    }
                }
                identifierUrl = uniqueIdentifierUrl + "/Questions";
                _questionDocument.SaveOrUpdate(identifierUrl, skillSetToSave.Questions);
                return true;
            }
            catch (Exception ex )
            {
                return false;
            }           
            
        }
    }
}
