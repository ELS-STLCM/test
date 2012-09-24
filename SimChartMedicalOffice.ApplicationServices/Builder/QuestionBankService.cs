using System;
using System.Collections.Generic;
using System.Linq;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Extensions;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DataInterfaces;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.DropBox;
using SimChartMedicalOffice.Data;

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
        /// <param name="competencyService"> </param>
        public QuestionBankService(IQuestionDocument questionDocument, IAttachmentDocument attachmentDocument, IFolderDocument folderDocumentInstance, IQuestionBankDocument questionBankDocumentInstance, ICompetencyService competencyService)
        {
            _questionDocument = questionDocument;
            _attachmentDocument = attachmentDocument;
            _folderDocument = folderDocumentInstance;
            _questionBankDocument = questionBankDocumentInstance;
            _competencyService = competencyService;

        }
        public string CloneAttachmentToPersistent(string attachmentGuid)
        {
            string attachementPersistent = String.Empty;
            if (!String.IsNullOrEmpty(attachmentGuid))
            {
                Attachment attachment = _attachmentDocument.Get(attachmentGuid);
                if (attachment != null)
                {
                    attachementPersistent = _attachmentDocument.SaveOrUpdate(_attachmentDocument.GetAssignmentUrl(DocumentPath.Module.Attachments), attachment);
                }
            }
            return attachementPersistent;
        }

        /// <summary>
        /// method to save a question
        /// </summary>
        /// <param name="questionObjectFromUi"> </param>
        /// <param name="dropBox"> </param>
        /// <param name="questionUrl"></param>
        /// <param name="folderIdentifier"></param>
        /// <param name="isEditMode"></param>
        /// <param name="isImageMovedToPersistent"> </param>
        /// <returns></returns>
        public bool SaveQuestion(Question questionObjectFromUi, DropBoxLink dropBox, string questionUrl, string folderIdentifier, bool isEditMode, bool isImageMovedToPersistent)
        {
            Question questionObject = questionObjectFromUi;
            //A) Set Guid referncce and Correct Url
            string strUrlToSave = FormAndSetUrlForQuestion(questionObject, dropBox, questionUrl, isEditMode, folderIdentifier);
            if (!isImageMovedToPersistent)
            {
                SavePersistentImage(isEditMode, questionObject, strUrlToSave);
            }
            _questionDocument.SaveOrUpdate(strUrlToSave, questionObject);
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
            if (!isEditMode)
            {
                string transientImage = questionEntity.QuestionImageReference;
                if (!String.IsNullOrEmpty(transientImage))
                {
                    string persistentImage;
                    MoveTransientToPersistentAttachment(transientImage, out persistentImage);
                    questionEntity.QuestionImageReference = persistentImage;
                }
                if (questionEntity.AnswerOptions != null)
                {
                    foreach (AnswerOption item in questionEntity.AnswerOptions)
                    {
                        if (!String.IsNullOrEmpty(item.AnswerImageReference))
                        {
                            string imgAnswerPersistentImage;
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
                    string imgUrl = CheckIfTransientImageExistsAndCreatePersistent(qnFromDb, questionEntity);
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
            string imgReference;

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
                    else
                    {
                        item.AnswerImageReference = String.Empty;
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
            if (isImageExistsInUi && isImageExistsInDb)
            {
                return AppEnum.AttachmentFlagsForStatus.ExistsInUiAndDb;
            }
            if (isImageExistsInDb & !isImageExistsInUi)
            {
                return AppEnum.AttachmentFlagsForStatus.ExistsInDbNotInUi;
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
                case AppEnum.AttachmentFlagsForStatus.ExistsInDbNotInUi:
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
                        return AppEnum.AttachmentActions.RemoveTransientPersistentAndCreatePersistent;
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
        private string CheckIfTransientImageExistsAndCreatePersistent(Question questionFromDb, Question questionFromUi)
        {
            string persistentImage = questionFromDb.QuestionImageReference;
            string transientImage = questionFromUi.QuestionImageReference;
            string imgUrl;
            bool isImageExistsInDb = !String.IsNullOrEmpty(questionFromDb.QuestionImageReference);
            bool isImageExistsInUi = !String.IsNullOrEmpty(questionFromUi.QuestionImageReference);
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
            if (transientAttachment != null)
            {
                SaveAttachment(String.Empty, transientAttachment, false, out persistentImageTemp);
                RemoveAttachment(transientImage);
            }
            else
            {
                persistentImageTemp = String.Empty;
            }
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
            }
            imgReference = persistentImageTemp;
        }

        /// <summary>
        /// to set and get dynamic url for questions
        /// </summary>
        /// <param name="questionObjFromUi"></param>
        /// <param name="dropBox"> </param>
        /// <param name="questionUrl"></param>
        /// <param name="isEditMode"></param>
        /// <param name="folderIdentifier"></param>
        /// <returns></returns>
        private string FormAndSetUrlForQuestion(Question questionObjFromUi, DropBoxLink dropBox, string questionUrl, bool isEditMode, string folderIdentifier)
        {
            if (isEditMode)
            {
                return questionUrl;
            }
            if (String.IsNullOrEmpty(questionUrl))
            {
                return string.Format(_questionDocument.GetAssignmentUrl(dropBox,DocumentPath.Module.QuestionBank,AppConstants.Create),  questionUrl, questionObjFromUi.GetNewGuidValue());
            }
            return string.Concat(questionUrl, '/', folderIdentifier, "/" + Respository.QuestionItems + "/", questionObjFromUi.GetNewGuidValue());
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

        //public Question GetQuestionWithGuid(string strGuid)
        //{
        //    string _questionDocumentUrl = "";
        //    _questionDocumentUrl = string.Format(_questionDocument.GetAssignmentUrl(), strGuid);
        //    return _questionDocument.Get(_questionDocumentUrl);
        //}
        /// <summary>
        /// to get question with guid
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// to save the image 
        /// </summary>
        /// <param name="attachmentGuid"></param>
        /// <param name="attachmentObject"></param>
        /// <param name="isTransient"> </param>
        /// <param name="attachmentUrl"></param>
        /// <returns></returns>
        public bool SaveAttachment(string attachmentGuid, Attachment attachmentObject, bool isTransient, out string attachmentUrl)
        {
            attachmentUrl = isTransient ? _attachmentDocument.GetAssignmentUrl(DocumentPath.Module.Attachments,AppConstants.TransientAttachment) : _attachmentDocument.GetAssignmentUrl(DocumentPath.Module.Attachments);
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
            string result;
            _attachmentDocument.Delete(attachmentGuid, out result);
            return true;
        }

        //public bool DeleteQuestion(string questionGuid)
        //{
        //    string result;
        //    string questionUrl = string.Format(_questionDocument.GetAssignmentUrl(), questionGuid);
        //    _questionDocument.Delete(questionUrl, out result);
        //    return true;
        //}

        #region Folder Functionalities

        /// <summary>
        /// to delete a particular question by passing a quid
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// To get folders inside a folder or tab given the url of the parent and type of folder(QuestionBank, PatientBuilder, etc...)
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="courseId"></param>
        /// <param name="folderUrl"> </param>
        /// <param name="breadCrumbFolders"> </param>
        /// <param name="breadCrumbNeeded"> </param>
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
            string result;
            _folderDocument.Delete(folderIdentifier, out result);
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
            string newFolderGuid = "";
            if (folderObject != null)
            {
                newFolderGuid = folderObject.GetNewGuidValue();
                if (folderUrl == "")
                {
                    folderUrl = (string.Format(_folderDocument.GetAssignmentUrl(DocumentPath.Module.LCMFolders), AppCommon.GetFolderType(folderType), "/" + newFolderGuid));
                }
                else
                {
                        folderUrl = _folderDocument.GetCorrectFolderUrl(null, folderType, folderUrl, folderGuid);
                    folderUrl = (folderUrl + "/" + newFolderGuid);
                }
                _folderDocument.SaveOrUpdate(folderUrl, folderObject);
            }
            return newFolderGuid;
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
            string urlofFolder = folderUrl + '/' + folderGuid;
            _folderDocument.SaveOrUpdate(urlofFolder, folderFromForm);
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
                _folderDocument.SaveOrUpdate(_folderDocument.GetCorrectFolderUrl(null, folderType, folderUrl, folderGuid), subFolderDictToSave);
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
        /// <param name="dropBox"> </param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public IList<Question> GetQuestionItems(string parentFolderIdentifier, int folderType, DropBoxLink dropBox, string folderUrl)
        {
            if (AppCommon.CheckIfStringIsEmptyOrNull(parentFolderIdentifier))
            {
                return _questionDocument.GetQuestionItems(parentFolderIdentifier, folderType, dropBox);
            }
            return _folderDocument.GetQuestionItems(parentFolderIdentifier, dropBox, folderUrl);
        }

        /// <summary>
        /// To get list of all questions from a question bank
        /// </summary>
        /// <returns></returns>
        public List<DocumentProxy> GetAllQuestions()
        {
            List<DocumentProxy> lstAllQuestions = _questionBankDocument.GetAllQuestionsInAQuestionBank();
            return lstAllQuestions;
        }

        /// <summary>
        /// To get search results from question bank
        /// </summary>
        /// <param name="strSearchText"></param>
        /// <param name="sortColumnIndex"></param>
        /// <param name="sortColumnOrder"></param>
        /// <param name="strQuestionType"> </param>
        /// <returns></returns>
        public List<DocumentProxy> GetSearchResultsForQuestionBank(string strSearchText, int sortColumnIndex, string sortColumnOrder, string strQuestionType)
        {
            IList<DocumentProxy> lstQuestionSearchResult = _questionBankDocument.GetAllQuestionsInAQuestionBank();
            lstQuestionSearchResult.ToList().ForEach(question => _competencyService.SetLinkedCompetencyTextForAQuestions(question.LinkedItemReference, question));
            IList<DocumentProxy> lstQuestionSearchResultTemp = !String.IsNullOrEmpty(strSearchText) ? GetQuestionsMatchingText(strSearchText, lstQuestionSearchResult) : lstQuestionSearchResult;
            if (!String.IsNullOrEmpty(strQuestionType))
            {
                lstQuestionSearchResultTemp = GetQuestionsMatchingQuestionType(strQuestionType, lstQuestionSearchResultTemp);
            }
            string sortColumnName = AppCommon.GridColumnForQuestionSearchList[sortColumnIndex];
            var sortableList = lstQuestionSearchResultTemp.AsQueryable();
            lstQuestionSearchResultTemp = sortableList.OrderBy(sortColumnName, sortColumnOrder).ToList();
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
                                                   || (lstSearch.AnswerTexts != null && lstSearch.AnswerTexts.Count(f => (f != null && f.ToLower().Contains(strSearchText.ToLower()))) > 0)
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
            List<DocumentProxy> questionList = GetAllQuestions();
            DocumentProxy lstQuestionSearchResultTemp = (from lstSearch in questionList
                                                         where (
                                                             !String.IsNullOrEmpty(lstSearch.UniqueIdentifier) && lstSearch.UniqueIdentifier.ToLower().Contains(strQuestionGuid.ToLower()))
                                                         select lstSearch).SingleOrDefault();
            return lstQuestionSearchResultTemp != null && (lstQuestionSearchResultTemp.Url != null) ? lstQuestionSearchResultTemp.Url : "";
        }

        public bool IsQuestionNameExists(string strQuestionText)
        {
            List<DocumentProxy> questionList = GetAllQuestions();
            List<DocumentProxy> lstQuestionSearchResultTemp = (from lstSearch in questionList
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
            Dictionary<string, int> questionType = _questionBankDocument.GetQuestionType();
            return questionType;
        }

        public string FormUrlForSkillSetQuestionToQuestionBank(Question questionItemToEdit, DropBoxLink dropBox)
        {
            return string.Format(_questionDocument.GetAssignmentUrl(dropBox,DocumentPath.Module.QuestionBank,AppConstants.Create),  "", questionItemToEdit.GetNewGuidValue());
        }

        /// <summary>
        /// To clone all the images in a question.
        /// </summary>
        /// <param name="questionObj"></param>
        public void CloneImagesForQuestion(Question questionObj)
        {
            if (!String.IsNullOrEmpty(questionObj.QuestionImageReference))
            {
                questionObj.QuestionImageReference = CloneAttachmentToPersistent(questionObj.QuestionImageReference);
            }
            if (questionObj.AnswerOptions != null && questionObj.AnswerOptions.Count > 0)
            {
                foreach (AnswerOption item in questionObj.AnswerOptions)
                {
                    if (!String.IsNullOrEmpty(item.AnswerImageReference))
                    {
                        item.AnswerImageReference = CloneAttachmentToPersistent(item.AnswerImageReference);
                    }
                }
            }
        }

        /// <summary>
        /// To delete all the images in a Question
        /// </summary>
        /// <param name="questionObj"></param>
        public void DeleteImagesForQuestion(Question questionObj)
        {
            if (!String.IsNullOrEmpty(questionObj.QuestionImageReference))
            {
                RemoveAttachment(questionObj.QuestionImageReference);
                questionObj.QuestionImageReference = String.Empty;
            }
            if (questionObj.AnswerOptions != null && questionObj.AnswerOptions.Count > 0)
            {
                foreach (AnswerOption item in questionObj.AnswerOptions)
                {
                    if (!String.IsNullOrEmpty(item.AnswerImageReference))
                    {
                        RemoveAttachment(item.AnswerImageReference);
                        item.AnswerImageReference = String.Empty;
                    }
                }
            }
        }



    }
}
