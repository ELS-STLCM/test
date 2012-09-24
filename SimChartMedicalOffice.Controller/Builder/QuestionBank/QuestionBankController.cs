using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder;
using SimChartMedicalOffice.Common.Logging;
using SimChartMedicalOffice.Core;
using System.Configuration;
using SimChartMedicalOffice.Common;
using System.IO;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.Json;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency;
using SimChartMedicalOffice.Common.Extensions;
using SimChartMedicalOffice.Core.SkillSetBuilder;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class QuestionBankController : BaseController
    {
        private readonly IQuestionBankService _questionBankService;
        private readonly ICompetencyService _competencyService;
        private readonly ISkillSetService _skillSetService;

        public QuestionBankController(IQuestionBankService questionBankService, ICompetencyService competencyService, ISkillSetService skillSetService)
        {
            _questionBankService = questionBankService;
            _competencyService = competencyService;
            _skillSetService = skillSetService;
        }

        /// <summary>
        /// To redirect to View with set ViewData
        /// </summary>
        public ActionResult QuestionBank()
        {
            try
            {
                ViewData["QuestionBankList"] = new SelectList(AppCommon.QuestionTypeOptions, "Key", "Value", 1);
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: QuestionBank", ex);
            }
            return View("../Builder/QuestionBank/QuestionBank");
        }

        /// <summary>
        /// To load Question Type
        /// </summary>
        /// <param name="strQuestionType"></param>
        /// <returns></returns>
        public ActionResult LoadQuestionType(int strQuestionType)
        {
            try
            {
                ViewData["BlankOrientation"] = new SelectList(AppCommon.BlankOrientation, "BlankOrientation");
                ViewData["NoOfLabels"] = new SelectList(AppCommon.NoOfLabels, "NoOfLabels");
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: LoadQuestionType", ex);
            }
            return View("../Builder/QuestionBank/" + AppCommon.GetKeyBasedOnQuestionType(strQuestionType));
        }
        #region Get List Of competencies and filter methods
        /// <summary>
        /// To get list of competencies based on the filter string
        /// </summary>
        /// <param name="strFilterText"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetCompetenciesForDropDown(string strFilterText)
        {
            List<AutoCompleteProxy> competencyStringListTemp = new List<AutoCompleteProxy>();
            try
            {
                competencyStringListTemp = !String.IsNullOrEmpty(strFilterText) ? _competencyService.GetAllCompetencyListForDropDown() : _competencyService.GetFilteredCompetenciesBasedOnString(strFilterText);
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: GetCompetenciesForDropDown", ex);
            }
            return Json(new { competencyStringListTemp = JsonSerializer.SerializeObject(competencyStringListTemp), competencyArray = competencyStringListTemp.Select(s => s.name).ToArray() });
        }
        #endregion

        #region To Load Question in Edit mode

        /// <summary>
        /// To render question in edit mode
        /// </summary>
        /// <param name="questionQuid">Url of question to load</param>
        /// <param name="iQuestionType">ApCommon dictionary key of the question type to be loaded</param>
        /// <param name="folderType"> </param>
        /// <returns></returns>
        public ActionResult RenderQuestionInEditMode(string questionQuid, int iQuestionType, int? folderType)
        {

            try
            {
                //IList<string> AnswerOptionOrderedList = new List<string>();
                ViewBag.IsQuestionBank = !String.IsNullOrEmpty(folderType.ToString()) && folderType.ToString().Equals("1");
                if (folderType.ToString().Equals("2"))
                {
                    ViewBag.IsAssignmentQuestion = true;
                }
                Question questionObject = _questionBankService.GetQuestion(questionQuid);
                SetViewBagsForEditMode(questionObject, iQuestionType);
                ViewBag.Url = questionObject.Url;
                ViewData["BlankOrientation"] = new SelectList(AppCommon.BlankOrientation, "BlankOrientation");
                ViewData["NoOfLabels"] = new SelectList(AppCommon.NoOfLabels, "NoOfLabels");
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: RenderQuestionInEditMode", ex);
            }
            return View("../Builder/QuestionBank/" + AppCommon.GetKeyBasedOnQuestionType(iQuestionType));
        }

        /// <summary>
        /// To get a competency string based on its Guid
        /// </summary>
        /// <param name="guidOfLinkedCompetency"></param>
        /// <returns></returns>
        public string GetLinkedCompetencyForAGuid(string guidOfLinkedCompetency)
        {
            List<AutoCompleteProxy> lstOfAutoComplete = _competencyService.GetAllCompetencyListForDropDown();
            return (from lstCompetency in lstOfAutoComplete where lstCompetency.id == guidOfLinkedCompetency select lstCompetency.name).SingleOrDefault();
        }

        /// <summary>
        /// To set view bags for question types based on question type.
        /// </summary>
        /// <param name="questionObject"></param>
        /// <param name="iQuestionType"></param>
        public void SetViewBagsForEditMode(Question questionObject, int iQuestionType)
        {
            try
            {
                AppEnum.QuestionTypes questioType = (AppEnum.QuestionTypes)iQuestionType;
                ViewBag.QuestionText = questionObject.QuestionText;
                ViewBag.QuestionImage = String.IsNullOrEmpty(questionObject.QuestionImageReference) ? String.Empty : questionObject.QuestionImageReference;
                ViewBag.CorrectAnswerRationale = questionObject.Rationale;
                ViewBag.LinkedCompetency = GetLinkedCompetencyForAGuid(questionObject.CompetencyReferenceGuid);
                ViewBag.LinkedCompetencyGuid = questionObject.CompetencyReferenceGuid;
                ViewBag.IsEditMode = true;
                ViewBag.questionTypeLoadedFlag = questionObject.QuestionType;
                ViewBag.BlankOrientationToLoad = questionObject.BlankOrientation;
                ViewBag.NoOfLabelsToLoad = questionObject.NoOfLabels;
                if (questionObject.AnswerOptions != null)
                {
                    SetAnswerOptionsViewBagsForQuestions(questionObject.AnswerOptions);
                }
                if (questionObject.CorrectOrder != null && questionObject.CorrectOrder.Count > 0)
                {
                    SetCorrectOrderViewBagsForQuestion(questionObject.CorrectOrder);
                }
                switch (questioType)
                {
                    case AppEnum.QuestionTypes.CorrectOrder:
                        break;
                    case AppEnum.QuestionTypes.FillInTheBlank:
                        break;
                    case AppEnum.QuestionTypes.MultipleChoice:
                        break;
                    case AppEnum.QuestionTypes.ShortAnswer:
                        break;
                    case AppEnum.QuestionTypes.TrueFalseQuestionSetUp:
                        break;
                }
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: SetViewBagsForEditMode", ex);
            }

        }
        /// <summary>
        /// To set view datas for the questions in edit mode.
        /// </summary>
        /// <param name="answerOptionList"></param>
        public void SetAnswerOptionsViewBagsForQuestions(List<AnswerOption> answerOptionList)
        {
            try
            {
                int iIndexOfAnswer = 1;
                List<string> answerList = new List<string>();
                foreach (AnswerOption answerOptionItem in answerOptionList)
                {
                    answerList.Add(answerOptionItem.AnswerText);
                    ViewData["AnswerOption_" + iIndexOfAnswer] = answerOptionItem.AnswerText;
                    ViewData["AnswerMatch_" + iIndexOfAnswer] = answerOptionItem.MachingText;
                    if (answerOptionItem.IsCorrectAnswer)
                        ViewData["CorrectAnswer"] = "Answer" + iIndexOfAnswer;
                    if (!String.IsNullOrEmpty(answerOptionItem.AnswerImageReference))
                        ViewData["AnsImage_" + iIndexOfAnswer] = answerOptionItem.AnswerImageReference;
                    iIndexOfAnswer++;
                }
                ViewBag.AnswerListToLoad = answerList;
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: SetAnswerOptionsViewBagsForQuestions", ex);
            }

        }
        /// <summary>
        /// To set view datas for correct order
        /// </summary>
        /// <param name="correctOrderList"></param>
        public void SetCorrectOrderViewBagsForQuestion(List<string> correctOrderList)
        {
            try
            {
                int iIndexOfOrder = 1;
                foreach (string correctOrderItem in correctOrderList)
                {
                    ViewData["AnswerOption_" + iIndexOfOrder] = correctOrderItem;
                    iIndexOfOrder++;
                }
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: SetCorrectOrderViewBagsForQuestion", ex);
            }

        }
        #endregion

        /// <summary>
        /// To get all the questions
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetAllQuestions()
        {
            _questionBankService.GetAllQuestions();
            return Json(new { Result = "success" });
        }

        /// <summary>
        /// To load upload control to view
        /// </summary>
        /// <returns></returns>
        public ActionResult SimOfficeImageUpload()
        {
            return View("../Builder/QuestionBank/SimOfficeImageUpload");
        }

        /// <summary>
        /// To upload file in ajax
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public FileUploadJsonResult AjaxUploadAFile(HttpPostedFileBase file)
        {

            string strMessage = AppConstants.FileUploadSuccess;
            bool isFileUploaded = true;
            string attachmentUrl = String.Empty;
            if (file == null)
            {
                return new FileUploadJsonResult { Data = new { strUploadMessage = AppConstants.FileUploadNoFileSelected, isSuccessfulUpload = false } };
            }
            //To validate for standard image formats  .pdf .jpeg .png .gif .bmp
            if (AppCommon.IsValidImageFormatForSimOffice(file.ContentType))
            {
                int fileSize = Convert.ToInt32(ConfigurationManager.AppSettings["SimOfficeMaxFileSize"]);
                try
                {
                    //To validate for standard image size - 3 MB
                    if (file.ContentLength <= fileSize)
                    {
                        byte[] fileByteToBeSaved = new byte[file.ContentLength];
                        file.InputStream.Read(fileByteToBeSaved, 0, file.ContentLength);
                        Attachment uploadedFile = new Attachment
                                                      {
                                                          FileType = file.ContentType,
                                                          FileName =
                                                              String.IsNullOrEmpty(file.FileName)
                                                                  ? AppConstants.StandardFileName
                                                                  : file.FileName.Split('\\').Last(),
                                                          FileContent = fileByteToBeSaved,
                                                          IsActive = true,
                                                          IsAutoSave = true
                                                      };
                        SetAuditFields(uploadedFile, false);
                        _questionBankService.SaveAttachment(string.Empty, uploadedFile, true, out attachmentUrl);
                    }
                    else
                    {
                        strMessage = AppConstants.FileUploadError;
                        isFileUploaded = false;
                    }
                }
                catch
                {
                    strMessage = AppConstants.FileUploadFailure;
                    isFileUploaded = false;
                }
            }
            else
            {
                strMessage = AppConstants.FileUploadError;
                isFileUploaded = false;
            }
            return new FileUploadJsonResult { Data = new { strUploadMessage = strMessage, isSuccessfulUpload = isFileUploaded, imageRefId = Convert.ToString(attachmentUrl) } };
        }

        /// <summary>
        /// To load the uploaded file from firebase
        /// </summary>
        /// <param name="strGuId"></param>
        /// <param name="nocache"></param>
        public void GetAttachment(string strGuId, string nocache)
        {
            try
            {
                if (!String.IsNullOrEmpty(strGuId))
                {
                    Attachment attachment = _questionBankService.GetAttachment(strGuId);
                    var response = HttpContext.Response;
                    response.Clear();
                    response.Cache.SetCacheability(HttpCacheability.NoCache);
                    response.ContentType = attachment.FileType;
                    var stream = new MemoryStream(attachment.FileContent);
                    stream.WriteTo(response.OutputStream);
                }
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: GetAttachment", ex);
            }

        }
        /// <summary>
        /// to remove the image uploaded
        /// </summary>
        /// <param name="strGuId"></param>
        /// <returns></returns>
        public ActionResult RemoveAttachment(string strGuId)
        {
            bool isAttachmentRemoved = _questionBankService.RemoveAttachment(strGuId);
            return Json(new { Result = isAttachmentRemoved });
        }
        #region Folder Functionalities

        /// <summary>
        /// Method to create a new folder 
        /// </summary>
        /// <param name="folderType"></param>
        /// <param name="folderUrl"></param>
        /// <param name="folderGuid"></param>
        /// <param name="parentFolderName"> </param>
        /// <param name="currentTab"> </param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateFolder(int folderType, string folderUrl, string folderGuid, string parentFolderName, string currentTab)
        {
            string message = "";
            string savedFolderId = "";
            try
            {
                IList<BreadCrumbProxy> breadCrumbFolders;
                string folderJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                Folder folderToCreate = JsonSerializer.DeserializeObject<Folder>(folderJson);
                IList<Folder> subFolderList = _questionBankService.GetSubfolders(folderGuid, GetLoginUserCourse() + "/" + GetLoginUserRole(),
                                                                                 folderType, folderUrl, out breadCrumbFolders, false);
                if (subFolderList.Any(folderItem => folderItem.Name == folderToCreate.Name))
                {
                    message = "Folder name already exists";
                    return Json(new { folderGuid = folderGuid, folderUrl = folderUrl, messageToReturn = message });
                }
                SetAuditFields(folderToCreate, false);
                savedFolderId = _questionBankService.SaveFolder(folderToCreate, folderType, GetLoginUserCourse() + "/" + GetLoginUserRole(), folderUrl, folderGuid);
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: CreateFolder", ex);
            }
            return Json(new { folderGuid = folderGuid, folderUrl = folderUrl, messageToReturn = message, newFolderId = savedFolderId });
        }

        /// <summary>
        /// Method to rename a  folder 
        /// </summary>
        /// <param name="folderType"></param>
        /// <param name="folderUrl"></param>
        /// <param name="folderGuid"></param>
        /// <param name="parentFolderName"> </param>
        /// <param name="currentTab"> </param>
        /// <param name="folderUrlOfParent"> </param>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RenameFolder(int folderType, string folderUrl, string folderGuid, string parentFolderName, string currentTab, string folderUrlOfParent)
        {
            string message = "";
            try
            {
                IList<BreadCrumbProxy> breadCrumbFolders;
                string folderJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                Folder folderFromForm = JsonSerializer.DeserializeObject<Folder>(folderJson);
                Folder folderObjectToUpdate = _questionBankService.GetFolder(folderUrl + '/' + folderGuid);
                IList<Folder> subFolderList = _questionBankService.GetSubfolders(parentFolderName, GetLoginUserCourse() + "/" + GetLoginUserRole(),
                                                                                 folderType, folderUrlOfParent, out breadCrumbFolders, false);
                if (subFolderList.Any(folderItem => folderItem.Name == folderFromForm.Name))
                {
                    message = "Folder name already exists";
                }
                folderObjectToUpdate.Name = folderFromForm.Name;
                _questionBankService.UpdateFolder(folderType, folderUrl, folderGuid, GetLoginUserCourse() + "/" + GetLoginUserRole(), folderObjectToUpdate);

            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: RenameFolder", ex);
            }
            return Json(new { folderGuid = folderGuid, folderUrl = folderUrl, messageToReturn = message });
        }

        /// <summary>
        /// method to delete a folder
        /// </summary>
        /// <param name="folderType"></param>
        /// <param name="folderUrl"></param>
        /// <param name="folderGuid"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteFolder(int folderType, string folderUrl, string folderGuid)
        {

            try
            {
                _questionBankService.DeleteFolder(folderUrl + '/' + folderGuid);
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: DeleteFolder", ex);
            }
            return Json(new { folderGuid = folderGuid, folderUrl = folderUrl });
        }

        /// <summary>
        /// Get list of folders for the selected folder
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetSubFolders(string parentFolderIdentifier, int folderType, string folderUrl)
        {
            IList<BreadCrumbProxy> breadCrumbFolders;
            IList<Folder> subFolderList = _questionBankService.GetSubfolders(parentFolderIdentifier, GetLoginUserCourse() + "/" + GetLoginUserRole(), folderType, folderUrl, out breadCrumbFolders, true);
            return Json(new { Result = subFolderList.OrderByDescending(x => Convert.ToInt32(x.SequenceNumber)), BreadCrumbFolders = breadCrumbFolders });
        }

        /// <summary>
        /// Get list of sub-folders for the selected folder for rearranging
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public ActionResult GetSubFoldersForSwap(string parentFolderIdentifier, int folderType, string folderUrl)
        {
            IList<BreadCrumbProxy> breadCrumbFolders;
            IList<Folder> subFolderList = _questionBankService.GetSubfolders(parentFolderIdentifier, GetLoginUserCourse() + "/" + GetLoginUserRole(), folderType, folderUrl, out breadCrumbFolders, false);
            ViewData["MaxSequenceNumber"] = 10;
            ViewData["folderCount"] = subFolderList.Count;
            return View("../../Views/Builder/QuestionBank/SwapPopUp", subFolderList.OrderByDescending(x => Convert.ToInt32(x.SequenceNumber)).ToList());
        }

        /// <summary>
        /// Get list of sub-folders for the selected folder to update their sequence number
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RearrangeSubFolderSequenceNumber(string parentFolderIdentifier, int folderType, string folderUrl)
        {
            string message = "";
            try
            {
                IList<BreadCrumbProxy> breadCrumbFolders;
                Dictionary<string, Folder> subFolderDicToSave = new Dictionary<string, Folder>();

                string folderJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                IList<Folder> subFolderListFromForm = JsonSerializer.DeserializeObject<IList<Folder>>(folderJson);
                IList<Folder> subFolderList = _questionBankService.GetSubfolders(parentFolderIdentifier, GetLoginUserCourse() + "/" + GetLoginUserRole(), folderType, folderUrl, out breadCrumbFolders, false);
                //replace each subFolder's sequence number with those from Form
                foreach (Folder folderFromForm in subFolderListFromForm)
                {
                    foreach (Folder folder in subFolderList)
                    {
                        if (folder.UniqueIdentifier == folderFromForm.UniqueIdentifier)
                        {
                            //update new sequence number, remove its uniqueIdentifier, add it to dictionary
                            folder.SequenceNumber = folderFromForm.SequenceNumber;
                            folder.UniqueIdentifier = null;
                            subFolderDicToSave[folderFromForm.UniqueIdentifier] = folder;
                            break;
                        }
                    }
                }

                //updating this dictionary of subFolders for the parent folder
                if (subFolderDicToSave.Count > 0)
                {
                    bool result = _questionBankService.UpdateSubFoldersForFolder(folderType, folderUrl,
                                                                                 parentFolderIdentifier,
                                                                                 GetLoginUserCourse() + "/" +
                                                                                 GetLoginUserRole(), subFolderDicToSave);

                    message = result ? "Success" : "";
                }
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: RearrangeSubFolderSequenceNumber", ex);
            }
            return Json(new { folderGuid = parentFolderIdentifier, folderUrl = folderUrl, messageToReturn = message });
        }

        #endregion Folder Functionalities


        /// <summary>
        /// method to load the questions in the datatable
        /// </summary>
        /// <param name="param"></param>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="filterByType"></param>
        /// <param name="selectedQuestionList"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public ActionResult GetQuestionBankList(JQueryDataTableParamModel param, string parentFolderIdentifier, int folderType, string filterByType, string selectedQuestionList, string folderUrl)
        {
            string[] gridColumnList = { "QuestionText", "Competency", "QuestionType", "CreatedTimeStamp" };
            IList<Question> questionBankList = _questionBankService.GetQuestionItems(parentFolderIdentifier, folderType, GetDropBoxFromCookie(), folderUrl);
            if (filterByType != "")
            {
                string filterValue = AppCommon.QuestionTypeOptionsForLanding.Single(x => x.Value == filterByType).Key.ToString(CultureInfo.InvariantCulture);
                questionBankList = (from qb in questionBankList where qb.QuestionType == filterValue select qb).ToList();
            }
            int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnOrder = Request["sSortDir_0"];
            string sortColumnName = gridColumnList[sortColumnIndex - 1];
            switch (gridColumnList[sortColumnIndex - 1])
            {

                case "Competency":
                    if (sortColumnOrder == "asc")
                    {
                        questionBankList = (from question in questionBankList
                                            let competency =
                                                GetLinkedCompetencyForAGuid(question.CompetencyReferenceGuid)
                                            orderby competency
                                            select question).ToList();
                    }
                    else
                    {

                        questionBankList = (from question in questionBankList
                                            let competency =
                                                GetLinkedCompetencyForAGuid(question.CompetencyReferenceGuid)
                                            orderby competency descending
                                            select question).ToList();
                    }
                    break;
                case "QuestionType":
                    {
                        if (sortColumnOrder == "asc")
                        {
                            questionBankList = (from question in questionBankList
                                                let questionType =
                                                    AppCommon.GetKeyBasedOnQuestionType(
                                                        Convert.ToInt32(question.QuestionType))
                                                orderby questionType
                                                select question).ToList();
                        }
                        else
                        {
                            questionBankList = (from question in questionBankList
                                                let questionType =
                                                    AppCommon.GetKeyBasedOnQuestionType(
                                                        Convert.ToInt32(question.QuestionType))
                                                orderby questionType descending
                                                select question).ToList();
                        }
                    }
                    break;
                default:
                    var sortableList = questionBankList.AsQueryable();
                    questionBankList = sortableList.OrderBy(sortColumnName, sortColumnOrder).ToList();
                    break;
            }
            IList<Question> questionBankListToRender = questionBankList.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            int questionBankCount = questionBankList.Count;
            string[] strArray = AppCommon.GetStringArrayAfterSplitting(selectedQuestionList);
            var data = (from questionBankItem in questionBankListToRender
                        select new[]
                                   {
                                       "<input type='checkbox' id='" + questionBankItem.UniqueIdentifier + "' onClick='questionBank.commonFunctions.gridOperations.questionItemChanged(this)'" + AppCommon.CheckForFlagAndReturnValue(strArray, questionBankItem.UniqueIdentifier) + "/>",
                                       !string.IsNullOrEmpty(questionBankItem.QuestionText) ? "<a href='#' onclick=\"questionBank.commonFunctions.loadQuestionInEditMode('"+questionBankItem.Url+"','"+questionBankItem.QuestionType+"')\" class=\"link select-hand\">" + AppCommon.BreakWord(AppCommon.ReplaceEscapeCharacterWithHtmlForReports(questionBankItem.QuestionText), 20) + "</a>" : "",
                                       !string.IsNullOrEmpty(questionBankItem.CompetencyReferenceGuid) ? GetLinkedCompetencyForAGuid(questionBankItem.CompetencyReferenceGuid) : "",
                                       !string.IsNullOrEmpty(questionBankItem.QuestionType) && questionBankItem.QuestionType != "-1" ? ((AppCommon.QuestionTypeOptions.Single(x => x.Key == Convert.ToInt32(questionBankItem.QuestionType))).Value).ToString(CultureInfo.InvariantCulture) : "",
                                       !string.IsNullOrEmpty(questionBankItem.CreatedTimeStamp.ToString("MM/dd/yyyy")) ? questionBankItem.CreatedTimeStamp.ToString("MM/dd/yyyy") : ""
                                   }).ToArray();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = questionBankCount,
                iTotalDisplayRecords = questionBankCount,
                aaData = data
            },
            JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// To save a question
        /// </summary>
        /// <param name="questionUrlReference"></param>
        /// <param name="folderIdentifier"></param>
        /// <param name="isEditMode"></param>
        /// <param name="isNewQuestion"> </param>
        /// <param name="isExistingQuestion"> </param>
        /// <param name="questionGuid"> </param>
        /// <param name="authoringType"> </param>
        /// <param name="authoringUrl"> </param>
        /// <param name="questionNewTextToSave"> </param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveQuestion(string questionUrlReference, string folderIdentifier, bool isEditMode, bool isNewQuestion, bool isExistingQuestion, string questionGuid, int authoringType, string authoringUrl, string questionNewTextToSave)
        {
            Question questionItemToEdit = new Question();
            bool isProceedToStep4Valid = false;
            bool isQuestionSavedToQuestionBankValue = false;
            try
            {
                if ((isNewQuestion || isExistingQuestion) && questionNewTextToSave != "")
                {
                    bool isQuestionNameExists = _questionBankService.IsQuestionNameExists(questionNewTextToSave);
                    if (isQuestionNameExists)
                    {
                        return Json(new { isQuestionNameAlreadyExists = true });
                    }
                }
                string questionObjJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                string parentIdentifier = String.Empty;
                Question questionObject = JsonSerializer.DeserializeObject<Question>(questionObjJson);
                questionItemToEdit = isEditMode ? _questionBankService.GetQuestion(questionUrlReference) : questionObject;
                questionItemToEdit.AnswerOptions = questionObject.AnswerOptions;
                questionItemToEdit.BlankOrientation = questionObject.BlankOrientation;
                questionItemToEdit.CompetencyReferenceGuid = questionObject.CompetencyReferenceGuid;
                questionItemToEdit.CorrectOrder = questionObject.CorrectOrder;
                questionItemToEdit.NoOfLabels = questionObject.NoOfLabels;
                questionItemToEdit.QuestionImageReference = questionObject.QuestionImageReference;
                questionItemToEdit.QuestionText = questionObject.QuestionText;
                questionItemToEdit.QuestionType = questionObject.QuestionType;
                questionItemToEdit.Rationale = questionObject.Rationale;
                questionItemToEdit.IsActive = questionObject.IsActive;
                questionItemToEdit.IsAutoSave = questionObject.IsAutoSave;
                SetAuditFields(questionItemToEdit, isEditMode);

                if (authoringType == (int)AppCommon.AuthoringType.SkillSet || authoringType == (int)AppCommon.AuthoringType.AssignmentBuilder)
                {
                    Question questionItemForAuthoring = questionItemToEdit.Clone();

                    if (isNewQuestion && !String.IsNullOrEmpty(questionNewTextToSave))
                    {
                        questionItemToEdit.QuestionText = questionNewTextToSave;
                        questionItemForAuthoring.QuestionText = questionNewTextToSave;
                    }
                    parentIdentifier = SaveQuestionsForSkillSet(questionItemForAuthoring, isNewQuestion,
                                                                isExistingQuestion, questionGuid);
                    if (isNewQuestion || isExistingQuestion)
                    {
                        isQuestionSavedToQuestionBankValue = true;
                    }
                }
                questionItemToEdit.ParentReferenceGuid = parentIdentifier;
                _questionBankService.SaveQuestion(questionItemToEdit, GetDropBoxFromCookie(), questionUrlReference, folderIdentifier, isEditMode, false);
                if (!String.IsNullOrEmpty(authoringUrl))
                {
                    isProceedToStep4Valid = CheckIfAllQuestionsTemplatesAreConfigured(authoringUrl, authoringType);
                }
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: SaveQuestion", ex);
            }
            return Json(new { Success = "", questionResult = questionItemToEdit, isProceedToStep4Valid = isProceedToStep4Valid, isQuestionSavedToQuestionBank = isQuestionSavedToQuestionBankValue, isQuestionNameAlreadyExists = false });
        }

        private bool CheckIfAllQuestionsTemplatesAreConfigured(string urlOfAuthoring, int authoringType)
        {
            AppCommon.AuthoringType authoringTypeVal = (AppCommon.AuthoringType)authoringType;
            int countOfConfiguredQuestions = 0;
            switch (authoringTypeVal)
            {
                case AppCommon.AuthoringType.SkillSet:
                    SkillSet skillSetObj = _skillSetService.GetSkillSet(urlOfAuthoring);
                    if (skillSetObj != null && skillSetObj.Questions != null)
                    {
                        IList<Question> questionList = skillSetObj.Questions.Select(qn => qn.Value).ToList();
                        countOfConfiguredQuestions = (from skillSetlst in questionList where string.IsNullOrEmpty(skillSetlst.CompetencyReferenceGuid) select skillSetlst).Count();
                    }
                    break;
                case AppCommon.AuthoringType.AssignmentBuilder:
                    break;
            }
            if (countOfConfiguredQuestions > 0)
                return false;
            return true;
        }

        private string SaveQuestionsForSkillSet(Question questionItemToEdit, bool isNewQuestion, bool isExistingQuestion, string questionGuid)
        {
            if (isNewQuestion)
            {

                string questionUrlToSave = _questionBankService.FormUrlForSkillSetQuestionToQuestionBank(questionItemToEdit, GetDropBoxFromCookie());
                if (questionUrlToSave != "")
                {
                    _questionBankService.CloneImagesForQuestion(questionItemToEdit);
                    SetAuditFields(questionItemToEdit, false);
                    _questionBankService.SaveQuestion(questionItemToEdit, GetDropBoxFromCookie(), questionUrlToSave, "", true, true);
                }
                return questionUrlToSave;
            }
            if (isExistingQuestion)
            {
                if (questionItemToEdit.ParentReferenceGuid != "")
                {
                    SetAuditFields(questionItemToEdit, true);
                    Question questionItemFromQuestionBank =
                        _questionBankService.GetQuestion(questionItemToEdit.ParentReferenceGuid);
                    _questionBankService.CloneImagesForQuestion(questionItemToEdit);
                    _questionBankService.DeleteImagesForQuestion(questionItemFromQuestionBank);
                    questionItemToEdit.CreatedBy = questionItemFromQuestionBank.CreatedBy;
                    questionItemToEdit.CreatedTimeStamp = questionItemFromQuestionBank.CreatedTimeStamp;
                    _questionBankService.SaveQuestion(questionItemToEdit,GetDropBoxFromCookie(), questionItemToEdit.ParentReferenceGuid, "", true, true);
                }
                return questionItemToEdit.ParentReferenceGuid;
            }
            return questionItemToEdit.ParentReferenceGuid;
        }

        /// <summary>
        /// To render search page on click of search
        /// </summary>
        /// <param name="strSearchText"></param>
        /// <param name="strQuestionType"> </param>
        /// <returns></returns>
        public ActionResult GiveSearchResults(string strSearchText, string strQuestionType)
        {
            if (strQuestionType == "undefined")
            {
                strQuestionType = "";
            }
            ViewBag.SearchText = strSearchText;
            ViewBag.SearchQuestionType = !String.IsNullOrEmpty(strQuestionType) ? (AppCommon.QuestionTypeOptions.Single(x => x.Key == Convert.ToInt32(strQuestionType)).Value) : String.Empty;
            ViewBag.filterByQuestionTypeSearch = GetQuestionTypeFlexBoxList();
            ViewBag.SearchQuestionTypeText = strQuestionType;
            return View("../Builder/QuestionBank/QuestionSearchResults");
        }

        /// <summary>
        /// To refresh the grid on search
        /// </summary>
        /// <param name="param"></param>
        /// <param name="strSearchText"></param>
        /// <param name="strQuestionType"> </param>
        /// <returns></returns>
        public ActionResult GetQuestionBankSearchList(JQueryDataTableParamModel param, string strSearchText, string strQuestionType)
        {
            int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnOrder = Request["sSortDir_0"];
            try
            {
                IList<DocumentProxy> lstQuestionSearchResultTemp = _questionBankService.GetSearchResultsForQuestionBank(strSearchText, sortColumnIndex, sortColumnOrder, strQuestionType);
                IList<DocumentProxy> lstQuestionSearchResult = lstQuestionSearchResultTemp.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                var data = (from questionBankItem in lstQuestionSearchResult
                            select new[]
                                   {
                                       !string.IsNullOrEmpty(questionBankItem.Text) ? "<a href='#' onclick=\"questionBank.commonFunctions.loadQuestionInEditMode('"+questionBankItem.Url+"','"+Convert.ToInt32(((AppCommon.QuestionTypeOptionsForLanding.Single(x => x.Value == questionBankItem.TypeOfQuestion)).Key).ToString(CultureInfo.InvariantCulture))+"')\" class=\"link select-hand\">" + AppCommon.BreakWord(AppCommon.ReplaceEscapeCharacterWithHtmlForReports(questionBankItem.Text), 20) + "</a>" : "",
                                       !string.IsNullOrEmpty(questionBankItem.LinkedItemReference) ? questionBankItem.LinkedItemReference : String.Empty,
                                        "<a href='#' onclick=\"questionBank.commonFunctions.loadFolder('"+questionBankItem.FolderUrl+"','"+questionBankItem.FolderIdentifier+"')\" class=\"link select-hand\">" + (!String.IsNullOrEmpty(questionBankItem.FolderName)? questionBankItem.FolderName: AppCommon.QuestionBankLandingPageFolderName) + "</a>",
                                        (!String.IsNullOrEmpty(questionBankItem.TypeOfQuestion)) ? questionBankItem.TypeOfQuestion :String.Empty,
                                       !string.IsNullOrEmpty(questionBankItem.CreatedTimeStamp.ToString("MM/dd/yyyy")) ? questionBankItem.CreatedTimeStamp.ToString("MM/dd/yyyy") : ""
                                   }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = lstQuestionSearchResultTemp.Count,
                    iTotalDisplayRecords = lstQuestionSearchResultTemp.Count,
                    aaData = data
                },
            JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: ControllerName: QuestionBank, MethodName: GetQuestionBankSearchList", ex);
            }
            return Json(new { Result = string.Empty });
        }


        /// <summary>
        /// Get list of question type with id.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetQuestionType()
        {
            var varQuestionType = GetQuestionTypeFlexBoxList();
            return Json(new { ResultList = varQuestionType, questionList = JsonSerializer.SerializeObject(GetQuestionTypeFlexBoxList()), questionNameArray = GetQuestionTypeFlexBoxList().Select(s => s.name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        public Dictionary<string, int> GetGetQuestionTypeList()
        {
            return _questionBankService.GetQuestionType();
        }

    }
}
