using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.DataInterfaces;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency;
using SimChartMedicalOffice.Common.Extensions;

namespace SimChartMedicalOffice.ApplicationServices.Builder
{
    public class QuestionBankService : IQuestionBankService
    {
        private readonly IQuestionDocument _questionDocument;
        private readonly IAttachmentDocument _attachmentDocument;
        private readonly IFolderDocument _folderDocument;
        private readonly IQuestionBankDocument _questionBankDocument;
        private readonly ICompetencyService _competencyService;

        /// <summary>
        /// initalising the question, attachment, questionbank and folder document
        /// </summary>
        /// <param name="questionDocument"></param>
        /// <param name="attachmentDocument"></param>
        /// <param name="folderDocumentInstance"></param>
        /// <param name="questionBankDocumentInstance"></param>
        public QuestionBankService(IQuestionDocument questionDocument, IAttachmentDocument attachmentDocument, IFolderDocument folderDocumentInstance, IQuestionBankDocument questionBankDocumentInstance, ICompetencyService competencyService)
        {
            this._questionDocument = questionDocument;
            this._attachmentDocument = attachmentDocument;
            this._folderDocument = folderDocumentInstance;
            this._questionBankDocument = questionBankDocumentInstance;
            this._competencyService = competencyService;
        }

        /// <summary>
        /// method to save a question
        /// </summary>
        /// <param name="questionObjectFromUI"></param>
        /// <param name="courseId"></param>
        /// <param name="questionUrl"></param>
        /// <param name="folderIdentifier"></param>
        /// <param name="isEditMode"></param>
        /// <returns></returns>
        public bool SaveQuestion(Question questionObjectFromUi, string courseId, string questionUrl, string folderIdentifier, bool isEditMode)
        {
            try
            {
                IList<AnswerOption> answerOptionList = new List<AnswerOption>();
                Question questionObject = new Question();
                IList<string> answerKeys = new List<string>();
                questionObject = questionObjectFromUi;
                //A) Set Guid referncce and Correct Url
                string strUrlToSave = FormAndSetUrlForQuestion(questionObject, courseId, questionUrl, isEditMode, folderIdentifier);
                SavePersistentImage(isEditMode, questionObject, strUrlToSave);
                _questionDocument.SaveOrUpdate(strUrlToSave, questionObject);
            }
            catch
            {
                //To-Do
            }
            return true;
        }

        /// <summary>
        /// To save Persistent image under SimApp/Attachment/Persistent
        /// </summary>
        /// <param name="isEditMode"></param>
        /// <param name="questionEntity"></param>
        /// <param name="questionUrlReference"></param>
        private void SavePersistentImage(bool isEditMode, Question questionEntity, string questionUrlReference)
        {
            string transientImage = String.Empty;
            string persistentImage = String.Empty;
            string imgUrl;
            if (!isEditMode)
            {
                transientImage = questionEntity.QuestionImageReference;
                if (!String.IsNullOrEmpty(transientImage))
                {
                    MoveTransientToPersistentAttachment(transientImage, out persistentImage);
                    questionEntity.QuestionImageReference = persistentImage;
                }
                if (questionEntity.AnswerOptions != null)
                {
                    foreach (AnswerOption item in questionEntity.AnswerOptions)
                    {
                        if (!String.IsNullOrEmpty(item.AnswerImageReference))
                        {
                            string imgAnswerPersistentImage = string.Empty;
                            MoveTransientToPersistentAttachment(item.AnswerImageReference, out imgAnswerPersistentImage);
                            item.AnswerImageReference = imgAnswerPersistentImage;
                        }
                    }
                }
            }
            else
            {
                Question qnFromDb = GetQuestion(questionUrlReference);
                if (qnFromDb != null)
                {
                    imgUrl = checkIfTransientImageExistsAndCreatePersistent(qnFromDb, questionEntity);
                    questionEntity.QuestionImageReference = imgUrl;
                    SavePersistentImageForAnswers(qnFromDb, questionEntity);
                }
            }
        }
        /// <summary>
        /// To save persistent images for answers
        /// </summary>
        /// <param name="qnFromDb"></param>
        /// <param name="questionEntity"></param>
        private void SavePersistentImageForAnswers(Question qnFromDb, Question questionEntity)
        {
            string imgReference = String.Empty;

            if (questionEntity.AnswerOptions != null && questionEntity.AnswerOptions.Count > 0)
            {
                foreach (var item in questionEntity.AnswerOptions)
                {
                    if (!String.IsNullOrEmpty(item.AnswerImageReference))
                    {
                        CheckAndMoveTransientImages(AppEnum.AttachmentActions.CreatePersistent, string.Empty, item.AnswerImageReference, out imgReference);
                        item.AnswerImageReference = imgReference;
                    }

                }
            }
            if (qnFromDb.AnswerOptions != null && qnFromDb.AnswerOptions.Count > 0)
            {
                foreach (var item in qnFromDb.AnswerOptions)
                {
                    if (!String.IsNullOrEmpty(item.AnswerImageReference))
                    {
                        CheckAndMoveTransientImages(AppEnum.AttachmentActions.RemovePersistent, item.AnswerImageReference, string.Empty, out imgReference);
                    }

                }
            }
        }
        /// <summary>
        /// To check image status from Db and UI
        /// </summary>
        /// <param name="isImageExistsInDb"></param>
        /// <param name="isImageExistsInUi"></param>
        /// <returns></returns>
        private AppEnum.AttachmentFlagsForStatus GetAttachmentStatus(bool isImageExistsInDb, bool isImageExistsInUi)
        {
            if (!isImageExistsInUi && !isImageExistsInDb)
            {
                return AppEnum.AttachmentFlagsForStatus.NotExistsInUiAndDb;
            }
            else if (isImageExistsInUi && isImageExistsInDb)
            {
                return AppEnum.AttachmentFlagsForStatus.ExistsInUiAndDb;
            }
            else
            {
                if (isImageExistsInDb & !isImageExistsInUi)
                {
                    return AppEnum.AttachmentFlagsForStatus.ExistsInDbNotInUI;
                }
            }
            return AppEnum.AttachmentFlagsForStatus.ExistsInUiNotInDb;
        }
        /// <summary>
        /// To get the action to perform from the flags
        /// </summary>
        /// <param name="statusOfAttachment"></param>
        /// <param name="persistentImage"></param>
        /// <param name="transientImage"></param>
        /// <returns></returns>
        private AppEnum.AttachmentActions GetActionToPerformForAttachment(AppEnum.AttachmentFlagsForStatus statusOfAttachment, string persistentImage, string transientImage)
        {
            switch (statusOfAttachment)
            {
                case AppEnum.AttachmentFlagsForStatus.ExistsInDbNotInUI:
                    return AppEnum.AttachmentActions.RemovePersistent;
                case AppEnum.AttachmentFlagsForStatus.ExistsInUiNotInDb:
                    return AppEnum.AttachmentActions.CreatePersistent;
                case AppEnum.AttachmentFlagsForStatus.NotExistsInUiAndDb:
                    return AppEnum.AttachmentActions.None;
                case AppEnum.AttachmentFlagsForStatus.ExistsInUiAndDb:
                    {
                        if (persistentImage.Equals(transientImage))
                        {
                            return AppEnum.AttachmentActions.None;
                        }
                        else
                        {
                            return AppEnum.AttachmentActions.RemoveTransientPersistentAndCreatePersistent;
                        }
                    }
            }
            return AppEnum.AttachmentActions.None;
        }

        /// <summary>
        /// Attachment objects handler
        /// </summary>
        /// <param name="questionFromDb"></param>
        /// <param name="questionFromUi"></param>
        /// <returns></returns>
        private string checkIfTransientImageExistsAndCreatePersistent(Question questionFromDb, Question questionFromUi)
        {
            string persistentImage = questionFromDb.QuestionImageReference;
            string transientImage = questionFromUi.QuestionImageReference;
            string imgUrl = string.Empty;
            bool isImageExistsInDb = String.IsNullOrEmpty(questionFromDb.QuestionImageReference) ? false : true;
            bool isImageExistsInUi = String.IsNullOrEmpty(questionFromUi.QuestionImageReference) ? false : true;
            AppEnum.AttachmentFlagsForStatus statusOfAttachment = GetAttachmentStatus(isImageExistsInDb, isImageExistsInUi);
            AppEnum.AttachmentActions attachmentActions = GetActionToPerformForAttachment(statusOfAttachment, persistentImage, transientImage);
            CheckAndMoveTransientImages(attachmentActions, persistentImage, transientImage, out imgUrl);
            return imgUrl;
        }

        /// <summary>
        /// To remove the Transient attachment and move to persistent.
        /// </summary>
        /// <param name="transientImage"></param>
        /// <param name="persistentImageTemp"></param>
        public void MoveTransientToPersistentAttachment(string transientImage, out string persistentImageTemp)
        {
            Attachment transientAttachment = GetAttachment(transientImage);
            SaveAttachment(String.Empty, transientAttachment, false, out persistentImageTemp);
            RemoveAttachment(transientImage);
        }
        /// <summary>
        /// To move transient images from Attachment/Transient to Attachment/Persistent
        /// </summary>
        /// <param name="attachmentActions"></param>
        /// <param name="persistentImage"></param>
        /// <param name="transientImage"></param>
        /// <param name="imgReference"></param>
        private void CheckAndMoveTransientImages(AppEnum.AttachmentActions attachmentActions, string persistentImage, string transientImage, out string imgReference)
        {
            string persistentImageTemp = String.Empty;
            switch (attachmentActions)
            {
                case AppEnum.AttachmentActions.RemovePersistent:
                    RemoveAttachment(persistentImage);
                    persistentImageTemp = String.Empty;
                    break;
                case AppEnum.AttachmentActions.RemoveTransientPersistentAndCreatePersistent:
                    {
                        RemoveAttachment(persistentImage);
                        MoveTransientToPersistentAttachment(transientImage, out persistentImageTemp);
                    }
                    break;
                case AppEnum.AttachmentActions.CreatePersistent:
                    {
                        MoveTransientToPersistentAttachment(transientImage, out persistentImageTemp);
                    }
                    break;
                case AppEnum.AttachmentActions.None:
                    imgReference = persistentImage;
                    return;
                default:
                    break;
            }
            imgReference = persistentImageTemp;
        }

        /// <summary>
        /// to set and get dynamic url for questions
        /// </summary>
        /// <param name="questionObjFromUI"></param>
        /// <param name="courseId"></param>
        /// <param name="questionUrl"></param>
        /// <param name="isEditMode"></param>
        /// <param name="folderIdentifier"></param>
        /// <returns></returns>
        private string FormAndSetUrlForQuestion(Question questionObjFromUI, string courseId, string questionUrl, bool isEditMode, string folderIdentifier)
        {
            try
            {
                if (isEditMode)
                {
                    return questionUrl;
                }
                else
                {
                    if (String.IsNullOrEmpty(questionUrl))
                    {
                        return string.Format(_questionDocument.Url, courseId, questionUrl, questionObjFromUI.GetNewGuidValue());
                    }
                    return string.Concat(questionUrl, '/', folderIdentifier, "/QuestionItems/", questionObjFromUI.GetNewGuidValue());
                }
            }
            catch
            {
                //To-Do
            }
            return String.Empty;

        }

        /// <summary>
        /// to get a particular question for passing specific guid
        /// </summary>
        /// <param name="strGuidURlOfQuestion"></param>
        /// <returns></returns>
        public Question GetQuestion(string strGuidURlOfQuestion)
        {
            return _questionDocument.Get(strGuidURlOfQuestion);
        }
        /// <summary>
        /// to get question with guid
        /// </summary>
        /// <param name="strGuid"></param>
        /// <returns></returns>
        public Question GetQuestionWithGuid(string strGuid)
        {
            string _questionDocumentUrl = "";
            try
            {
                _questionDocumentUrl = string.Format(_questionDocument.Url, strGuid);
            }
            catch
            {
                //To-Do
            }
            return _questionDocument.Get(_questionDocumentUrl);
        }

        /// <summary>
        /// to save the image 
        /// </summary>
        /// <param name="attachmentGuid"></param>
        /// <param name="attachmentObject"></param>
        /// <param name="attachmentUrl"></param>
        /// <returns></returns>
        public bool SaveAttachment(string attachmentGuid, Attachment attachmentObject, bool isTransient, out string attachmentUrl)
        {
            if (isTransient)
            {
                attachmentUrl = _attachmentDocument.GetAttachementTransientUrl();
            }
            else
            {
                attachmentUrl = _attachmentDocument.Url;
            }
            attachmentUrl = _attachmentDocument.SaveOrUpdate(attachmentUrl, attachmentObject);
            return true;
        }

        /// <summary>
        /// to get the saved image 
        /// </summary>
        /// <param name="attachmentGuid"></param>
        /// <returns></returns>
        public Attachment GetAttachment(string attachmentGuid)
        {
            return _attachmentDocument.Get(attachmentGuid);
        }

        /// <summary>
        /// to remove the saved image 
        /// </summary>
        /// <param name="attachmentGuid"></param>
        /// <returns></returns>
        public bool RemoveAttachment(string attachmentGuid)
        {
            try
            {
                string result;
                _attachmentDocument.Delete(attachmentGuid, out result);
            }
            catch
            {

                //To-Do
            }

            return true;
        }

        /// <summary>
        /// to delete a particular question by passing a quid
        /// </summary>
        /// <param name="questionGuid"></param>
        /// <returns></returns>
        public bool DeleteQuestion(string questionGuid)
        {
            try
            {
                string result;
                string questionUrl = string.Format(_questionDocument.Url, questionGuid);
                _questionDocument.Delete(questionUrl, out result);
            }
            catch
            {
                //To-Do
            }
            return true;
        }

        #region Folder Functionalities

        /// <summary>
        /// To get folders inside a folder or tab given the url of the parent and type of folder(QuestionBank, PatientBuilder, etc...)
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public IList<Folder> GetSubfolders(string parentFolderIdentifier, string courseId, int folderType, string folderUrl, out IList<BreadCrumbProxy> breadCrumbFolders, bool breadCrumbNeeded)
        {
            return _folderDocument.GetSubfolders(parentFolderIdentifier, courseId, folderType, folderUrl, out breadCrumbFolders, breadCrumbNeeded);
        }
        /// <summary>
        /// Function to retrieve a folder data using its GuidValue
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public Folder GetFolder(string folderPath)
        {
            return _folderDocument.Get(folderPath);
        }
        /// <summary>
        /// This function is to delete a folder using its folder url
        /// </summary>
        /// <param name="folderIdentifier"></param>
        /// <returns></returns>
        public bool DeleteFolder(string folderIdentifier)
        {
            try
            {
                string result;
                _folderDocument.Delete(folderIdentifier, out result);
            }
            catch
            {
                //To-Do
            }
            return true;
        }

        /// <summary>
        /// To save a new folder in the appropriate urls
        /// </summary>
        /// <param name="folderObject"></param>
        /// <param name="folderType"></param>
        /// <param name="courseId"></param>
        /// <param name="folderUrl"></param>
        /// <param name="folderGuid"></param>
        /// <returns></returns>
        public string SaveFolder(Folder folderObject, int folderType, string courseId, string folderUrl, string folderGuid)
        {
            string newFolderGUID = "";
            try
            {
                if (folderObject != null)
                {
                    newFolderGUID = folderObject.GetNewGuidValue();
                    if (folderUrl == "")
                    {
                        folderUrl = (string.Format(_folderDocument.Url, courseId, AppCommon.GetFolderType(folderType), "/" + newFolderGUID));
                    }
                    else
                    {
                        folderUrl = _folderDocument.GetCorrectFolderUrl(courseId, folderType, folderUrl, folderGuid);
                        folderUrl = (folderUrl + "/" + newFolderGUID);
                    }
                    _folderDocument.SaveOrUpdate(folderUrl, folderObject);
                }
            }
            catch
            {
                //To-Do

            }

            return newFolderGUID;
        }

        /// <summary>
        /// To update a edited folders
        /// </summary>
        /// <param name="folderType"></param>
        /// <param name="folderUrl"></param>
        /// <param name="folderGuid"></param>
        /// <param name="courseId"></param>
        /// <param name="folderFromForm"></param>
        /// <returns></returns>
        public bool UpdateFolder(int folderType, string folderUrl, string folderGuid, string courseId, Folder folderFromForm)
        {
            try
            {
                string urlofFolder = "";
                urlofFolder = folderUrl + '/' + folderGuid;
                _folderDocument.SaveOrUpdate(urlofFolder, folderFromForm);
            }
            catch
            {
                //To-Do
            }
            return true;
        }

        /// <summary>
        /// To update a dictionary of subfolders for a folder
        /// </summary>
        /// <param name="folderType"></param>
        /// <param name="folderUrl"></param>
        /// <param name="folderGuid"></param>
        /// <param name="courseId"></param>
        /// <param name="subFolderDictToSave"></param>
        /// <returns></returns>
        public bool UpdateSubFoldersForFolder(int folderType, string folderUrl, string folderGuid, string courseId, Dictionary<string, Folder> subFolderDictToSave)
        {
            try
            {
                _folderDocument.SaveOrUpdate(_folderDocument.GetCorrectFolderUrl(courseId, folderType, folderUrl, folderGuid), subFolderDictToSave);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion Folder Functionalities


        /// <summary>
        /// to get list of questions
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="courseId"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public IList<Question> GetQuestionItems(string parentFolderIdentifier, int folderType, string courseId, string folderUrl)
        {
            try
            {
                if (AppCommon.CheckIfStringIsEmptyOrNull(parentFolderIdentifier))
                {
                    return _questionDocument.GetQuestionItems(parentFolderIdentifier, folderType, courseId);
                }
                return _folderDocument.GetQuestionItems(parentFolderIdentifier, courseId, folderUrl);
            }
            catch
            {
                //To-Do                
            }
            return new List<Question>();
        }

        /// <summary>
        /// To get list of all questions from a question bank
        /// </summary>
        /// <returns></returns>
        public List<DocumentProxy> GetAllQuestions()
        {
            List<DocumentProxy> lstAllQuestions = new List<DocumentProxy>();
            lstAllQuestions = _questionBankDocument.GetAllQuestionsInAQuestionBank();
            return lstAllQuestions;
        }
        /// <summary>
        /// To get search results from question bank
        /// </summary>
        /// <param name="strSearchText"></param>
        /// <param name="sortColumnIndex"></param>
        /// <param name="sortColumnOrder"></param>
        /// <returns></returns>
        public List<DocumentProxy> GetSearchResultsForQuestionBank(string strSearchText, int sortColumnIndex, string sortColumnOrder, string strQuestionType)
        {
            IList<DocumentProxy> lstQuestionSearchResult = new List<DocumentProxy>();
            IList<DocumentProxy> lstQuestionSearchResultTemp = new List<DocumentProxy>();
            lstQuestionSearchResult = _questionBankDocument.GetAllQuestionsInAQuestionBank();
            lstQuestionSearchResult.ToList().ForEach(question => _competencyService.SetLinkedCompetencyTextForAQuestions(question.LinkedItemReference, question));
            if (!String.IsNullOrEmpty(strSearchText))
            {
                lstQuestionSearchResultTemp = GetQuestionsMatchingText(strSearchText, lstQuestionSearchResult);
            }
            else
            {
                lstQuestionSearchResultTemp = lstQuestionSearchResult;
            }
            if (!String.IsNullOrEmpty(strQuestionType))
            {
                lstQuestionSearchResultTemp = GetQuestionsMatchingQuestionType(strQuestionType, lstQuestionSearchResultTemp);
            }
            string sortColumnName = AppCommon.gridColumnForQuestionSearchList[sortColumnIndex];
            var sortableList = lstQuestionSearchResultTemp.AsQueryable();
            lstQuestionSearchResultTemp = sortableList.OrderBy<DocumentProxy>(sortColumnName, sortColumnOrder).ToList<DocumentProxy>();
            return lstQuestionSearchResultTemp.ToList();
        }
        /// <summary>
        /// To get the search text matching questions , search for question text and competencies
        /// </summary>
        /// <param name="strSearchText"></param>
        /// <param name="lstQuestionSearchResult"></param>
        /// <returns></returns>
        private List<DocumentProxy> GetQuestionsMatchingText(string strSearchText, IList<DocumentProxy> lstQuestionSearchResult)
        {
            var lstQuestionSearchResultTemp = (from lstSearch in lstQuestionSearchResult
                                               where (
                                                   !String.IsNullOrEmpty(lstSearch.Text) && lstSearch.Text.ToLower().Contains(strSearchText.ToLower()))
                                                   || (!String.IsNullOrEmpty(lstSearch.LinkedItemReference) && lstSearch.LinkedItemReference.ToLower().Contains(strSearchText.ToLower()))
                                                   || (!String.IsNullOrEmpty(lstSearch.Rationale) && lstSearch.Rationale.ToLower().Contains(strSearchText.ToLower()))
                                                   || (lstSearch.AnswerTexts != null && lstSearch.AnswerTexts.Count(F => (F != null && F.ToLower().Contains(strSearchText.ToLower()))) > 0)
                                               select lstSearch).ToList();
            return lstQuestionSearchResultTemp.ToList();
        }

        /// <summary>
        /// To get the search text matching questions , search for question text and competencies
        /// </summary>
        /// <param name="strQuestionType"></param>
        /// <param name="lstQuestionSearchResult"></param>
        /// <returns></returns>
        private List<DocumentProxy> GetQuestionsMatchingQuestionType(string strQuestionType, IList<DocumentProxy> lstQuestionSearchResult)
        {
            var lstQuestionSearchResultTemp = (from lstSearch in lstQuestionSearchResult
                                               where (
                                                   !String.IsNullOrEmpty(lstSearch.TypeOfQuestion) && lstSearch.TypeOfQuestion.ToLower().Contains(strQuestionType.ToLower()))
                                               select lstSearch).ToList();
            return lstQuestionSearchResultTemp.ToList();
        }

        public string GetQuestionUrlToUpdate(string strQuestionGuid)
        {
            List<DocumentProxy> QuestionList = GetAllQuestions();
            DocumentProxy lstQuestionSearchResultTemp = (from lstSearch in QuestionList
                                               where (
                                                   !String.IsNullOrEmpty(lstSearch.UniqueIdentifier) && lstSearch.UniqueIdentifier.ToLower().Contains(strQuestionGuid.ToLower()))
                                               select lstSearch).SingleOrDefault();
            return (lstQuestionSearchResultTemp.Url != null)?lstQuestionSearchResultTemp.Url.ToString():"";
        }

        public bool IsQuestionNameExists(string strQuestionText)
        {
            List<DocumentProxy> QuestionList = GetAllQuestions();
            List<DocumentProxy> lstQuestionSearchResultTemp = (from lstSearch in QuestionList
                                                         where (
                                                             !String.IsNullOrEmpty(lstSearch.Text) && lstSearch.Text == strQuestionText)
                                                         select lstSearch).ToList();
            if (lstQuestionSearchResultTemp.Count > 0)
            {
                return true;
            }
            return false;
        }

        public Dictionary<string, int> GetQuestionType()
        {
            Dictionary<string, int> questionType = new Dictionary<string, int>();
            questionType = _questionBankDocument.GetQuestionType();
            return questionType;
        }

        public string FormUrlForSkillSetQuestionToQuestionBank(Question questionItemToEdit, string courseId)
        {
            return string.Format(_questionDocument.Url, courseId, "", questionItemToEdit.GetNewGuidValue());
        }


    }
}
