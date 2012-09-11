using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Extensions;
using SimChartMedicalOffice.Common.Logging;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.Competency;
using SimChartMedicalOffice.Core.Json;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.SkillSetBuilder;
using SimChartMedicalOffice.Core;
namespace SimChartMedicalOffice.Web.Controllers
{
    public class SkillSetController : BaseController
    {
        private readonly ISkillSetService _skillSetService;
        private readonly ICompetencyService _competencyService;
        private readonly IQuestionBankService _questionBankService;

        public SkillSetController(ISkillSetService skillSetService, ICompetencyService competencyService, IQuestionBankService questionBankService)
        {
            this._skillSetService = skillSetService;
            this._competencyService = competencyService;
            this._questionBankService = questionBankService;
        }

        public ActionResult LoadQuestionsForSkillSet(string skillSetUrl)
        {
            ViewBag.SkillSetUrl = skillSetUrl;
            IList<Question> questionsForSkillSet = _skillSetService.GetQuestionsForSkillSet(skillSetUrl);
            ViewBag.QuestionList = questionsForSkillSet.OrderBy(x => x.SequenceNumber).ToList();
            ViewBag.isProceedToStep4Valid=CheckIfAllQuestionsTemplatesAreConfigured(skillSetUrl, 1);
            return View("../Builder/SkillSet/ConfigureQuestionsAnswersStep3");
        }

        public ActionResult SkillSetBuilderLanding()
        {
            IList<CompetencySources> SourceList = _competencyService.GetAllCompetecnySources();
            int index = 0;
            ViewBag.FilterBySource = SourceList.Select(cat => new { id = index++, name = cat.Name.ToString(CultureInfo.InvariantCulture) }).ToList();
            return View("../Builder/SkillSet/SkillSetBuilderLanding");
        }

        public ActionResult SkillSetBuilder()
        {
            IList<CompetencySources> SourceList = _competencyService.GetAllCompetecnySources();
            int index = 0;
            ViewBag.FilterBySource = SourceList.Select(cat => new { id = index++, name = cat.Name.ToString(CultureInfo.InvariantCulture) }).ToList();
            return View("../Builder/SkillSet/SkillSetBuilder");
        }
        /// <summary>
        /// method to load the SkillSets in the datatable
        /// </summary>
        /// <param name="param"></param>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="filterBySource"></param>
        /// <param name="selectedSkillSetList"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetSkillSetList(jQueryDataTableParamModel param, string parentFolderIdentifier, int folderType, string filterBySource, string selectedSkillSetList, string folderUrl)
        {
            int skillSetCount = 0;
            string[] gridColumnList = { "SkillSetTitle", "Competency", "Source", "CreatedTimeStamp", "Status" };
            IList<SkillSet> skillSetList = _skillSetService.GetSkillSetItems(parentFolderIdentifier, folderType, GetLoginUserCourse() + "/" + GetLoginUserRole(), folderUrl);
            IList<Competency> competencyAll = _competencyService.GetAllCompetencies();
            if (filterBySource != "")
            {
                IList<SkillSet> skillSetLocal = new List<SkillSet>();

                skillSetLocal = (from skillSet in skillSetList
                                 where
                                     skillSet.Competencies != null && skillSet.Competencies.Count > 0 &&
                                     (_skillSetService.GetLinkedCompetencySources(skillSet.Competencies, competencyAll).
                                         Any(x => x.ToLower().Contains(filterBySource.ToLower())))
                                 select skillSet).ToList();
                skillSetList = skillSetLocal;
            }

            int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnOrder = Request["sSortDir_0"];
            string sortColumnName = gridColumnList[sortColumnIndex - 1];
            switch (gridColumnList[sortColumnIndex - 1])
            {
                case "Competency":
                    if (sortColumnOrder == "asc")
                    {
                        skillSetList = (from skillSetItem in skillSetList
                                        let competency = ((skillSetItem.Competencies) != null && (skillSetItem.Competencies.Count) > 0) ? _competencyService.GetCompetencyNameForListofCompetencies(skillSetItem.Competencies) : ""
                                        orderby competency
                                        select skillSetItem).ToList();
                    }
                    else
                    {
                        skillSetList = (from skillSetItem in skillSetList
                                        let competency = ((skillSetItem.Competencies) != null && (skillSetItem.Competencies.Count) > 0) ? _competencyService.GetCompetencyNameForListofCompetencies(skillSetItem.Competencies) : ""
                                        orderby competency descending
                                        select skillSetItem).ToList();
                    }
                    break;

                case "Source":
                    if (sortColumnOrder == "asc")
                    {
                        skillSetList = (from skillSetItem in skillSetList
                                        let sources = ((skillSetItem.Competencies) != null && (skillSetItem.Competencies.Count) > 0) ? String.Join(",<br/> ", _skillSetService.GetLinkedCompetencySources(skillSetItem.Competencies, _skillSetService.GetAllCompetenciesForASkillSet(skillSetItem))) : ""
                                        orderby sources
                                        select skillSetItem).ToList();
                    }
                    else
                    {
                        skillSetList = (from skillSetItem in skillSetList
                                        let sources = ((skillSetItem.Competencies) != null && (skillSetItem.Competencies.Count) > 0) ? String.Join(",<br/> ", _skillSetService.GetLinkedCompetencySources(skillSetItem.Competencies, _skillSetService.GetAllCompetenciesForASkillSet(skillSetItem))) : ""
                                        orderby sources descending
                                        select skillSetItem).ToList();
                    }
                    break;
                default:
                    var sortableList = skillSetList.AsQueryable();
                    skillSetList = sortableList.OrderBy<SkillSet>(sortColumnName, sortColumnOrder).ToList<SkillSet>();
                    break;
            };
            IList<SkillSet> skillSetListToRender = skillSetList.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            IList<Competency> skillSetCompetencies = _skillSetService.GetAllCompetenciesForSkillSets(skillSetListToRender);
            skillSetCount = skillSetList.Count;
            string[] strArray = AppCommon.GetStringArrayAfterSplitting(selectedSkillSetList);
            var data = (from skillSetItem in skillSetListToRender
                        select new[]
                                   {
                                       "<input type='checkbox' id='" + skillSetItem.UniqueIdentifier + "' onClick='skillSet.commonFunctions.gridOperations.skillSetItemChanged(this)'" + AppCommon.CheckForFlagAndReturnValue(strArray, skillSetItem.UniqueIdentifier) + "/>",
                                       !string.IsNullOrEmpty(skillSetItem.SkillSetTitle) ? "<a href='#' onclick=\""+(AppCommon.CheckIfPublished(skillSetItem.Status)?"skillSet.commonFunctions.loadStep4('"+skillSetItem.Url+"')":"skillSet.commonFunctions.loadStep1('"+skillSetItem.Url+"')") +"\" class=\"link select-hand\">" + skillSetItem.SkillSetTitle + "</a>" : "", 
                                       (skillSetItem.Competencies!=null && skillSetItem.Competencies.Count>0 ? _competencyService.GetCompetencyNameForListofCompetencies(skillSetItem.Competencies):""),
                                       (skillSetItem.Competencies!=null && skillSetItem.Competencies.Count>0 ? String.Join(",<br/> ",_skillSetService.GetLinkedCompetencySources(skillSetItem.Competencies,skillSetCompetencies)):"") ,
                                       !string.IsNullOrEmpty(skillSetItem.CreatedTimeStamp.ToString("MM/dd/yyyy")) ? skillSetItem.CreatedTimeStamp.ToString("MM/dd/yyyy") : "",
                                       (!String.IsNullOrEmpty(skillSetItem.Status)) ? skillSetItem.Status :String.Empty
                                      }).ToArray();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = skillSetCount,
                iTotalDisplayRecords = skillSetCount,
                aaData = data
            },
            JsonRequestBehavior.AllowGet);
        }
        public IList<string> GetLinkedCompetencySources(IList<string> competencyGuids, IList<Competency> competencyList)
        {
            IList<string> sourceListBasic = AppCommon.SourceOptions.Select(x => x.Value).ToList();
            IList<string> sourceListReqd = new List<string>();
            foreach (string competencyGuid in competencyGuids)
            {
                Competency competencyObj = (from competency in competencyList where competency.UniqueIdentifier == competencyGuid select competency).SingleOrDefault();
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
            return sourceListReqd;
        }

        public IList<Competency> GetAllCompetenciesForSkillSets(IList<SkillSet> skillSets)
        {
            IList<Competency> competenciesForSkillSet = new List<Competency>();
            foreach (SkillSet skillSet in skillSets)
            {
                if (skillSet.Competencies != null && skillSet.Competencies.Count > 0)
                {
                    foreach (string competencyGuid in skillSet.Competencies)
                    {
                        Competency competency = _competencyService.GetCompetency(competencyGuid);
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
        /// To refresh the grid on search
        /// </summary>
        /// <param name="param"></param>
        /// <param name="strSearchText"></param>
        /// <returns></returns>
        public ActionResult GetSkillSetSearchList(jQueryDataTableParamModel param, string strSearchText, string strSource)
        {
            try
            {
                IList<SkillSetProxy> lstSkillSetSearchResult = new List<SkillSetProxy>();
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortColumnOrder = Request["sSortDir_0"];
                IList<SkillSetProxy> lstSkillSetSearchResultTemp = _skillSetService.GetSearchResultsForSkillSet(strSearchText, sortColumnIndex, sortColumnOrder, strSource);
                lstSkillSetSearchResult = lstSkillSetSearchResultTemp.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                var data = (from skillSetItem in lstSkillSetSearchResult
                            select new[]
                                   {
                                       !string.IsNullOrEmpty(skillSetItem.SkillSetTitle) ? "<a href='#' onclick=\""+(AppCommon.CheckIfPublished(skillSetItem.Status)?"skillSet.commonFunctions.loadStep4('"+skillSetItem.Url+"')":"skillSet.commonFunctions.loadStep1('"+skillSetItem.Url+"')") +"\" class=\"link select-hand\">" + skillSetItem.SkillSetTitle + "</a>" : "", 
                                       (skillSetItem.LinkedCompetencies!=null && skillSetItem.LinkedCompetencies.Count>0 ? _competencyService.GetCompetencyNameForListofCompetencies(skillSetItem.LinkedCompetencies):""),
                                       (skillSetItem.Source!=null&&skillSetItem.Source.Count>0)?String.Join(", ",skillSetItem.Source.Select(s=>s)):String.Empty,
                                       !string.IsNullOrEmpty(skillSetItem.CreatedTimeStamp.ToString("MM/dd/yyyy")) ? skillSetItem.CreatedTimeStamp.ToString("MM/dd/yyyy") : "",
                                       (!String.IsNullOrEmpty(skillSetItem.Status)) ? skillSetItem.Status :String.Empty
                                   }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = lstSkillSetSearchResultTemp.Count,
                    iTotalDisplayRecords = lstSkillSetSearchResultTemp.Count,
                    aaData = data
                },
            JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ExceptionManager.Error("Page Name : SkillSet - Search Results, Controller Method : GetSkillSetSearchList, Service Method : _skillSetService.GetSearchResultsForSkillSet", ex);
            }
            return Json(new { Result = string.Empty });
        }
        /// <summary>
        /// Step 4 of skillset builder
        /// </summary>
        /// <param name="skillSetUrl"></param>
        /// <returns></returns>
        public ActionResult PreviewAndPublishStep4(string skillSetUrl)
        {
            try
            {
                IList<DocumentProxy> questionsForSkillSetProxy = _skillSetService.GetQuestionsForPreview(skillSetUrl);
                ViewBag.SkillSetQuestions = questionsForSkillSetProxy;
                ViewBag.SkillSetName = _skillSetService.GetSkillSet(skillSetUrl).SkillSetTitle;
                ViewBag.SkillFocus = _skillSetService.GetFocusForSkillSet(skillSetUrl);
                ViewBag.SkillSetStatus = AppCommon.CheckIfPublished(_skillSetService.GetSkillSet(skillSetUrl).Status);
            }
            catch
            {

                //TO-dO
            }
            return View("../Builder/SkillSet/PreviewAndPublishStep4");
        }

        /// <summary>
        /// To render search page on click of search
        /// </summary>
        /// <param name="strSearchText"></param>
        /// /// <param name="strSource"></param>
        /// <returns></returns>
        public ActionResult GiveSearchResults(string strSearchText, string strSource)
        {
            ViewBag.SearchText = strSearchText;
            ViewBag.searchSource = strSource;
            //ViewBag.filterBySourceSearch = GetQuestionTypeFlexBoxList();
            IList<CompetencySources> SourceList = _competencyService.GetAllCompetecnySources();
            int index = 0;
            ViewBag.FilterBySource = SourceList.Select(cat => new { id = index++, name = cat.Name.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.searchSource = strSource;
            return View("../Builder/SkillSet/SKillSetSearchResults");
        }

        public ActionResult LoadSkillSetStepTwo(string skillSetUrl)
        {
            Dictionary<string, int> questionTypeList = _questionBankService.GetQuestionType();
            if (questionTypeList != null)
            {
                ViewData["questionTemplateList"] = new SelectList(questionTypeList.Select(qt => qt.Key.ToString()).ToList());
            }
            else
            {
                ViewData["questionTemplateList"] = null;
            }
            Dictionary<string, int> questionTypeFromData = _questionBankService.GetQuestionType();
            ViewBag.FilterByQuestionType = GetQuestionTypeFlexBoxList(questionTypeFromData);

            IList<Question> questionsForSkillSet = _skillSetService.GetQuestionsForSkillSet(skillSetUrl);

            if (questionsForSkillSet != null)
            {
                var selectedQuestionList = (from lstquestionItem in questionsForSkillSet
                                            select new
                                            {
                                                Text = lstquestionItem.QuestionText,
                                                QuestionTypeId = Convert.ToInt32(lstquestionItem.QuestionType),
                                                IsQuestionFromTemplate = lstquestionItem.IsQuestionFromTemplate,
                                                TypeOfQuestion = (AppCommon.QuestionTypeOptionsForLanding.Single(x => x.Key == Convert.ToInt32(lstquestionItem.QuestionType)).Value.ToString()),
                                                OrderSequenceNumber = lstquestionItem.SequenceNumber,
                                                Url = lstquestionItem.Url,
                                                ParentReferenceGuid = lstquestionItem.ParentReferenceGuid,
                                                TemplateSequenceNumber = lstquestionItem.TemplateSequenceNumber,
                                                UniqueIdentifier = (!String.IsNullOrEmpty(lstquestionItem.Url) ? lstquestionItem.Url.Split('/').Last() : String.Empty)
                                            });
                selectedQuestionList.OrderBy(x => x.OrderSequenceNumber).ToList();
                ViewBag.selectedQuestionOrderList = selectedQuestionList;
                ViewBag.IsSkillStructureEditMode = true;
            }
            else
            {
                ViewBag.selectedQuestionOrderList = null;
                ViewBag.IsSkillStructureEditMode = false;
            }

            return View("../Builder/SkillSet/SkillStructureStepTwo");
        }
        public ActionResult AddQuestionList(string UniqueIdentifier, string skillSetUrl)
        {
            IList<DocumentProxy> competencyQuestionList = _skillSetService.GetCompetencyQuestionListInSkillSet(skillSetUrl, "", "");
            competencyQuestionList = (from comp in competencyQuestionList where comp.UniqueIdentifier.ToString() != UniqueIdentifier select comp).ToList();
            return Json(new { CompetencyQuestionList = competencyQuestionList });
        }

        public IList<DocumentProxy> GetCompetencyQuestionListInSkillSet(string skillSetUniqueIdentifier, string competencySearchText, string filterQuestionType)
        {
            IList<DocumentProxy> competencyQuestionList = _skillSetService.GetCompetencyQuestionListInSkillSet(skillSetUniqueIdentifier, competencySearchText, filterQuestionType);
            return competencyQuestionList;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetJsonCompetencyQuestionListInSkillSet()
        {
            string skillSetUniqueIdentifier = "";
            string competencySearchText = "";
            string filterQuestionType = "";

            IList<Question> selectQuestionOrderList = new List<Question>();
            SkillSetProxy competencyQuestionFilterObject;
            string competencyQuestionListJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
            competencyQuestionFilterObject = JsonSerializer.DeserializeObject<SkillSetProxy>(competencyQuestionListJson);
            // getting the required Filters
            if (competencyQuestionFilterObject != null)
            {
                skillSetUniqueIdentifier = competencyQuestionFilterObject.UniqueIdentifier;
                competencySearchText = competencyQuestionFilterObject.competencyText;
                filterQuestionType = competencyQuestionFilterObject.FilterQuestionType;
                selectQuestionOrderList = competencyQuestionFilterObject.Questions;
            }
            IList<DocumentProxy> competencyQuestionList = GetCompetencyQuestionListInSkillSet(skillSetUniqueIdentifier, competencySearchText, filterQuestionType);
            //competencyQuestionList = ( from comQues in competencyQuestionList join selectQues in selectQuestionOrderList on (comQues.UniqueIdentifier.Equals(selectQues.UniqueIdentifier) select comQues).ToList();
            competencyQuestionList = (from comQues in competencyQuestionList where !(selectQuestionOrderList.Any(selQues => selQues.ParentReferenceGuid == comQues.Url)) select comQues).ToList();
            IList<Question> SkillQuestionList = new List<Question>();
            //return Json(new { CompetencyQuestionList = competencyQuestionList, SkillQuestionList = SkillQuestionList });
            return Json(new { CompetencyQuestionList = competencyQuestionList });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveSkillStructure(string uniqueIdentifierUrl)
        {
            string result = AppConstants.FileUploadSuccess;
            bool isSaved = false;
            try
            {
                List<DocumentProxy> selectQuestionOrderList = new List<DocumentProxy>();
                string selectQuestionOrderListJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                selectQuestionOrderList = JsonSerializer.DeserializeObject<List<DocumentProxy>>(selectQuestionOrderListJson);
                //selectQuestionOrderList = selectQuestionOrderList.Select(proxyQues => SetAuditFields(proxyQues, false));
                //selectQuestionOrderList = selectQuestionOrderList.ForEach(docProxy => SetAuditFields(docProxy, false)).se.ToList<DocumentProxy>();    
                foreach (var item in selectQuestionOrderList)
                {
                    item.IsActive = true;
                    if (String.IsNullOrEmpty(item.Url))
                    {
                        SetAuditFields(item, false);
                    }
                    else
                    {
                        if (item.Url.IndexOf("SkillSets") < 0)
                        {
                            SetAuditFields(item, false);
                        }
                        else
                        {
                            SetAuditFields(item, true);
                        }
                    } 
                }
                isSaved = _skillSetService.SaveSkillStructure(uniqueIdentifierUrl, selectQuestionOrderList);
                if(isSaved) {
                    result = AppConstants.Save;
                }
                else{
                    result = AppConstants.Error;
                }
            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType.Error, ex.ToString(), ""));
                result = "Error";
            }
            return Json(new { Result = result , SkillSetUrl = uniqueIdentifierUrl });

        }



        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetCompetencyForSkillSetFlexBox(string UniqueIdentifier)
        {
            List<AutoCompleteProxy> competencyStringListTemp = new List<AutoCompleteProxy>();
            try
            {
                if (!String.IsNullOrEmpty(UniqueIdentifier))
                {
                    competencyStringListTemp = _skillSetService.GetCompetencyGuidListInSkillSetForFlexBox(UniqueIdentifier);
                }
            }
            catch
            {
                //To-Do
            }
            return Json(new { competencyStringListTemp = JsonSerializer.SerializeObject(competencyStringListTemp), competencyArray = competencyStringListTemp.Select(S => S.name).ToArray() });
        }

        /// <summary>
        /// To publish a in progress skill set
        /// </summary>
        /// <param name="skillSetUrl"></param>
        /// <returns></returns>
        public JsonResult PublishASkillSet(string skillSetUrl)
        {
            bool isPublishedSuccessfully = false;
            try
            {
                isPublishedSuccessfully = _skillSetService.PublishSkillSet(skillSetUrl);
            }
            catch
            {
                isPublishedSuccessfully = false;
                //To-Do
            }
            return Json(new { Result = isPublishedSuccessfully });
        }

        /// <summary>
        /// to swap the questions inside skillset in step 3
        /// </summary>
        /// <param name="sourceUrl"></param>
        /// <param name="destinationUrl"></param>
        /// <param name="SkillSetUrl"></param>
        /// <returns></returns>
        public ActionResult SwapQuestionsForSkillSet(string sourceUrl, string destinationUrl, string SkillSetUrl)
        {
            Question questionSource = new Question();
            Question questionDestination = new Question();
            questionSource = _questionBankService.GetQuestion(sourceUrl);
            questionDestination = _questionBankService.GetQuestion(destinationUrl);
            int tempSequenceNumber = 0;
            tempSequenceNumber = (questionSource != null) ? questionSource.SequenceNumber : 1;
            questionSource.SequenceNumber = (questionDestination != null) ? questionDestination.SequenceNumber : 1;
            questionDestination.SequenceNumber = (tempSequenceNumber != 0) ? tempSequenceNumber : 1;
            _questionBankService.SaveQuestion(questionSource, "", sourceUrl, "", true);
            _questionBankService.SaveQuestion(questionDestination, "", destinationUrl, "", true);
            IList<Question> questionsForSkillSet = _skillSetService.GetQuestionsForSkillSet(SkillSetUrl);
            questionsForSkillSet = questionsForSkillSet.OrderBy(x => x.SequenceNumber).ToList();
            return Json(new { Result = string.Empty, questionList = questionsForSkillSet, strSourceUrl = sourceUrl });

        }
        /// <summary>
        /// To load all competencies for the skillset metadata step
        /// </summary>
        /// <param name="skillSetUrl"></param>
        /// <returns></returns>
        public ActionResult LoadCompetenciesForSkillSet(string skillSetUrl)
        {
            List<AutoCompleteProxy> competencyStringListTemp = new List<AutoCompleteProxy>();
            List<AutoCompleteProxy> savedCompetencyList = new List<AutoCompleteProxy>();
            competencyStringListTemp = _competencyService.GetAllCompetencyListForDropDown();
            if (!string.IsNullOrEmpty(skillSetUrl))
            {
                // if edit mode, get saved and formatted competency list for the skillset
                SkillSet skillSetObj = _skillSetService.GetSkillSet(skillSetUrl);
                if (skillSetObj != null)
                {
                    savedCompetencyList = _skillSetService.GetFormattedCompetenciesForSkillSet(skillSetObj).ToList();
                    ViewBag.SkillSetTitle = skillSetObj.SkillSetTitle;
                    ViewBag.Focus = skillSetObj.Focus;
                }
            }
            // to remove saved competencies from competencies all list
            competencyStringListTemp.RemoveAll(unsaved => savedCompetencyList.Any(saved => saved.id == unsaved.id));
            ViewBag.UnselectedCompetencies = competencyStringListTemp;
            ViewBag.SavedCompetencies = savedCompetencyList;
            ViewBag.SkillSetUrl = skillSetUrl;

            return View("../Builder/SkillSet/SkillSetMetadata");
        }
        /// <summary>
        /// To save/update a skill set object
        /// </summary>
        /// <param name="skillSetUrl"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveSkillSet(string skillSetUrl, bool isEditMode, string folderIdentifier, string folderUrl)
        {
            string message = "";
            string skillsetIdentifier = string.Empty;
            try
            {
                string skillSetJson;
                SkillSet skillSetToSave;
                skillSetJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                skillSetToSave = JsonSerializer.DeserializeObject<SkillSet>(skillSetJson);
                if (skillSetToSave != null)
                {
                    skillSetToSave.IsActive = true;
                    skillSetToSave.Status = AppCommon.Status_InProgress;
                    SetAuditFields(skillSetToSave, false);
                    bool result = _skillSetService.SaveSkillSet(skillSetToSave, GetLoginUserCourse() + "/" + GetLoginUserRole(), skillSetUrl, folderIdentifier, isEditMode, out skillsetIdentifier);
                    if (result)
                    {
                        message = "Success";
                    }
                }

            }
            catch
            {
                //To-Do
            }
            return Json(new { messageToReturn = message, uniqueIdentifier = skillsetIdentifier });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetFilteredCompetencyList()
        {
            List<AutoCompleteProxy> competencyStringListTemp = new List<AutoCompleteProxy>();
            try
            {
                string skillSetJson;
                SkillSetProxy skillSetFilterObject;
                skillSetJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                skillSetFilterObject = JsonSerializer.DeserializeObject<SkillSetProxy>(skillSetJson);

                IList<string> savedCompetencyList = new List<string>();
                IList<string> filterSourceList = new List<string>();
                string competencyFilterText = "";

                // getting the required Filters
                if (skillSetFilterObject != null)
                {
                    savedCompetencyList = skillSetFilterObject.SelectedCompetencyList;
                    filterSourceList = skillSetFilterObject.FilterSourceList;
                    competencyFilterText = skillSetFilterObject.competencyText;
                }
                competencyStringListTemp = _competencyService.GetAllCompetencyListForDropDown();

                // remove from competencyAll list all SelectedCompetencyList
                if (skillSetFilterObject != null && savedCompetencyList.Count > 0)
                {
                    competencyStringListTemp.RemoveAll(unsaved => savedCompetencyList.Any(saved => saved == unsaved.id));
                }

                competencyStringListTemp = (from lst in competencyStringListTemp
                                            where lst.name.ToLower().Contains(competencyFilterText.ToLower()) && (filterSourceList.Count > 0 ? filterSourceList.Any(s => lst.name.ToLower().Contains(s.ToLower())) : true)
                                            select lst).ToList();
            }
            catch
            {

            }
            return Json(new { filteredCompetencyList = competencyStringListTemp });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skillSetUrl"></param>
        /// <returns></returns>
        public ActionResult ViewSeletedCompetencies(string skillSetUrl)
        {
            List<AutoCompleteProxy> competencyStringListTemp = new List<AutoCompleteProxy>();
            List<AutoCompleteProxy> CompetencyList = new List<AutoCompleteProxy>();
            competencyStringListTemp = _competencyService.GetAllCompetencyListForDropDown();
            // if edit mode, get saved and formatted competency list for the skillset
            if (!string.IsNullOrEmpty(skillSetUrl))
            {
                SkillSet skillSetObj = _skillSetService.GetSkillSet(skillSetUrl);
                if (skillSetObj != null)
                {
                    CompetencyList = _skillSetService.GetFormattedCompetenciesForSkillSet(skillSetObj).ToList();
                }
            }
            // to remove saved competencies from competencies all list
            ViewBag.CompetencyList = CompetencyList;
            return View("../Builder/SkillSet/_ViewSeletedCompetenciesInStep2");
        }


        /// <summary>
        /// To check if questions exists on click of remove competencies in Skill set step 1 edit mode
        /// </summary>
        /// <param name="skillSetIdentifier"></param>
        /// <returns></returns>
        public ActionResult CheckIfQuestionsPresentForListOfCompetencies(string skillSetIdentifier)
        {
            string skillSetCompetencyJson;
            bool isQuestionsExist = false;
            string strQuestionText = String.Empty;
            IList<Question> lstOfSkillSetQuestion = new List<Question>();
            IList<string> lstOfSkillSetQuestionText = new List<string>();
            skillSetCompetencyJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
            List<string> competencylstOfGuids = JsonSerializer.DeserializeObject<List<string>>(skillSetCompetencyJson);
            SkillSet skillSetObj = _skillSetService.GetSkillSet(skillSetIdentifier);
            if (skillSetObj != null && skillSetObj.Questions != null)
            {
                lstOfSkillSetQuestion = skillSetObj.Questions.Select(q => q.Value).ToList();
                lstOfSkillSetQuestionText = (from lstQns in lstOfSkillSetQuestion join compIdentifier in competencylstOfGuids on lstQns.CompetencyReferenceGUID equals compIdentifier select lstQns.QuestionText).ToList();

                strQuestionText = AppCommon.QuestionsForCOmpetencies_Confirmation + "</br>";
                strQuestionText += "<UL>";
                foreach (var item in lstOfSkillSetQuestionText)
                {
                    strQuestionText = strQuestionText + "<LI>" + item + "</LI>";

                }
                strQuestionText += "</UL>";
            }

            return Json(new { lstOfSkillSetQuestion = lstOfSkillSetQuestionText, stringOfQuestions = strQuestionText });
        }


        /// <summary>
        /// To check if templates are configured
        /// </summary>
        /// <param name="urlOfAuthoring"></param>
        /// <param name="authoringType"></param>
        /// <returns></returns>
        public bool CheckIfAllQuestionsTemplatesAreConfigured(string urlOfAuthoring, int authoringType)
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
                        countOfConfiguredQuestions = (from skillSetlst in questionList where (skillSetlst.CompetencyReferenceGUID == null || skillSetlst.CompetencyReferenceGUID == String.Empty) select skillSetlst).Count();
                    }
                    break;
                case AppCommon.AuthoringType.AssignmentBuilder:
                    break;
                default:
                    break;
            }
            if (countOfConfiguredQuestions > 0)
                return false;
            else
                return true;
        }
    }
}
