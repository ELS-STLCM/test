using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.Competency;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;
using SimChartMedicalOffice.Core.DataInterfaces.SkillSetBuilder;
using SimChartMedicalOffice.Core.Json;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.SkillSetBuilder;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.AssignmentBuilder;
using SimChartMedicalOffice.ApplicationServices.Builder;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder;
using SimChartMedicalOffice.Common.Extensions;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class AssignmentBuilderController : BaseController
    {
        private readonly IAssignmentService _assignmentService;
        private readonly IQuestionBankDocument _questionBankDocument;
        private readonly ICompetencyService _competencyService;
        private readonly IQuestionBankService _questionBankService;
        private readonly ISkillSetDocument _skillSetDocument;
        private readonly ISkillSetService _skillSetService;

        public AssignmentBuilderController(IAssignmentService assignmentService, IQuestionBankDocument questionBankDocument, ICompetencyService competencyService, IQuestionBankService questionBankService, ISkillSetDocument skillSetDocument,ISkillSetService skillSetService)
        {
            this._assignmentService = assignmentService;
            this._questionBankDocument = questionBankDocument;
            this._competencyService = competencyService;
            this._questionBankService = questionBankService;
            this._skillSetDocument = skillSetDocument;
            this._skillSetService = skillSetService;
        }
        /// <summary>
        /// Method to set viewbg values to populate question type flexbox
        /// </summary>
        /// <returns></returns>
        public ActionResult AddFromQuestionBank()
        {
            ViewBag.FilterByQuestionTypeToAdd = GetQuestionTypeFlexBoxList();
            return View("../../Views/Builder/Assignment/_Step3AddFromQuestionBank");
        }


        /// <summary>
        /// Method to save the assignment metadata step 1
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public ActionResult SaveAssignmentMetadata(string folderUrl,bool isEditMode)
        {
            string result = "";
            string assignmentId="";
            try
            {
                //bool isEditMode = false;
                AssignmentProxySave assignmentProxyObject = DeSerialize<AssignmentProxySave>();
                assignmentProxyObject.IsActive = true;
                SetAuditFields(assignmentProxyObject, false);
                assignmentId = _assignmentService.SaveAssignmentMetaData(assignmentProxyObject, folderUrl, GetLoginUserCourse() + "/" + GetLoginUserRole(),isEditMode);
            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType.Error, ex.ToString(), ""));
            }
            return Json(new { assignmentUrl = assignmentId });
        }
        
        public ActionResult SaveAssignment()
        {
            string assignmentJson = "";
            string result = "";
            bool isEditMode = false;
            string assignmentUrl = "";
            Assignment assignmentObject = new Assignment();
            Assignment assignment = new Assignment();
            IList<SkillSetProxy> skillSetList = new List<SkillSetProxy>();
            IList<Question> questionList = new List<Question>();
            try
            {
                assignmentJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                assignmentObject = JsonSerializer.DeserializeObject<Assignment>(assignmentJson);                
                Type assignmentType = assignmentObject.GetType();
                //foreach (System.Reflection.PropertyInfo objProp in assignmentType.GetProperties())
                //{
                //    if (objProp.CanWrite && objProp.Name.ToUpper() != "ID")
                //    {
                //        objProp.SetValue(assignment, assignmentType.GetProperty(objProp.Name).GetValue(assignmentObject, null), null);
                //    }
                //}
                assignment = _assignmentService.GetAssignment(assignmentObject.Url);
                assignment.NoOfAttemptsAllowed = assignmentObject.NoOfAttemptsAllowed;
                assignment.AssignmentPassRate = assignmentObject.AssignmentPassRate;
                foreach (var question in assignmentObject.Questions)
                {
                    assignment.Questions.Add(question.Key, question.Value);
                }
                if (assignment.Url != null && assignment.Url != "")
                {
                    isEditMode = true;
                    assignmentUrl = assignment.Url;
                }
                SetAuditFields(assignment, isEditMode);
                _assignmentService.SaveAssignment(assignment, GetLoginUserCourse() + "/" + GetLoginUserRole(), isEditMode, assignmentUrl);
                skillSetList = GetSkillSetsForAnAssignment(assignmentUrl);
                questionList = GetQuestionsForAssignment(assignmentUrl);
                //questionList = ((assignment.Questions != null) ? (assignment.Questions.Select(questionItem => questionItem.Value).ToList()) : new List<Question>());
                //skillSetList = ((assignment.SkillSets != null) ? (assignment.SkillSets.Select(skillSetItem => skillSetItem.Value).ToList()) : new List<SkillSet>());

            }
            catch (Exception ex)
            {                
                result = AjaxCallResult(new AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType.Error, ex.ToString(), ""));
            }
            return Json(new { AssignmentUrl = assignment.Url, skillSetList = skillSetList, questionList = questionList,AjaxResult=result });

        }

        /// <summary>
        /// Save an assignment object for Orientation Step
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveOrientation()
        {
            string result = "";
            Assignment assignmentStream;
            Assignment assignment = new Assignment();
            try
            {
                assignmentStream = new Assignment();                                
                assignmentStream = DeSerialize<Assignment>();
                assignment = _assignmentService.GetAssignment(assignmentStream.Url);
                assignment.Orientation = assignmentStream.Orientation;
                assignment.PatientImageReferance = assignmentStream.PatientImageReferance;
                assignment.PatientImageDescription = assignmentStream.PatientImageDescription;
                assignment.VideoReferance = assignmentStream.VideoReferance;
                SetAuditFields(assignment, true);
                _assignmentService.SaveAssignment(assignment, GetLoginCourseAndRole(), true, assignmentStream.Url);
            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType.Error, ex.ToString(), ""));
            }
            return Json(new { AssignmentUrl = assignment.Url, AjaxResult = result });
        }
        /// <summary>
        /// Method to get the question objects from the guids
        /// </summary>
        /// <param name="questionGuids"></param>
        /// <returns></returns>
        public ActionResult GetSelectedQuestionObjectsFromGuid(string[] questionGuids, string assignmentUrl)
        {
            int nextSequenceNumber = 1;
            string[] questionGuidList = questionGuids[0].Split(',');
            Dictionary<string, Question> questionList = new Dictionary<string, Question>();
            IList<Question> questionListForAssignment = GetQuestionsForAssignment(assignmentUrl);
            if (questionListForAssignment.Count > 0)
            {
                nextSequenceNumber = questionListForAssignment.Last().SequenceNumber;
            }
            foreach (string questionGuid in questionGuidList)
            {
                string urlOfQuestion = _questionBankService.GetQuestionUrlToUpdate(questionGuid);
                Question questionObjectFromQuestionBank = _questionBankService.GetQuestion(urlOfQuestion);
                questionObjectFromQuestionBank.ParentReferenceGuid = questionGuid;
                string newGuidValue = questionObjectFromQuestionBank.GetNewGuidValue();
                questionObjectFromQuestionBank.Url = assignmentUrl + "/Questions/" + newGuidValue;
                questionObjectFromQuestionBank.SequenceNumber = nextSequenceNumber;
                questionList.Add(newGuidValue, questionObjectFromQuestionBank);
                nextSequenceNumber = nextSequenceNumber + 1;
            }
            return Json(new { QuestionObjects = questionList }); 
        }

        /// <summary>
        /// Get the Assignment for perticular Unique Identifier
        /// </summary>
        /// <param name="assignmentUrl"></param>
        /// <returns></returns>
        public JsonResult GetAssignment(string assignmentUniqueIdentifier)
        {
            Assignment assignmentObj = new Assignment();
            string result = "";
            try
            {                
                assignmentObj = _assignmentService.GetAssignment(assignmentUniqueIdentifier);
            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType.Error, ex.ToString(), ""));
            }
            return Json(new { Result = assignmentObj });
        }

        /// <summary>
        /// Redirect to Orientation page
        /// </summary>
        /// <returns></returns>
        public ActionResult ReditectToOrientation()
        {
            return View("../../Views/Builder/Assignment/Orientation");
        }
        /// <summary>
        /// Redirect to step 1 Assignment Metadata
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadAssignmentMetaData(string assignmentUniqueIdentifier)
        {
            ViewData["AssignmentDurationHrs"] = new SelectList(AppCommon.AssignmentDurationHrs.ToList(), "AssignmentDurationHrs");
            ViewData["AssignmentDurationMns"] = new SelectList(AppCommon.AssignmentDurationMns.ToList(), "AssignmentDurationMns");
            ViewData["officeType"] = new SelectList(AppCommon.officeTypeOptions, "officeType");
            ViewData["provider"] = new SelectList(AppCommon.providerOptions, "provider");
            List<SkillSetProxy> skillList = new List<SkillSetProxy>();
            List<SkillSetProxy> savedSkillList = new List<SkillSetProxy>();

            AssignmentProxySave assignmentToEdit=null;

            skillList = _skillSetService.GetAllSkillSetsInSkillSetRepository();
            skillList.RemoveAll(skillset => skillset.Status == null);
            skillList = (from skillSet in skillList where skillSet.Status.ToLower().Equals("published") select skillSet).ToList();
            // if edit mode, get saved and formatted competency list for the assignment
            if (!string.IsNullOrEmpty(assignmentUniqueIdentifier))
            {
                Assignment assignmentObj = _assignmentService.GetAssignment(assignmentUniqueIdentifier);
                // if the assignment object has skillset dict, get skillSetProxy list for this dictionary
                if (assignmentObj != null && assignmentObj.SkillSets!=null && assignmentObj.SkillSets.Count>0)
                {
                    savedSkillList = _skillSetService.TransformSkillSetsToSkillSetProxy(assignmentObj.SkillSets.Select(skillDict=>skillDict.Value).ToList(), null);

                    // if savedSkillList has items remove them from skillList
                    if (savedSkillList!=null && savedSkillList.Count>0)
                    {
                        skillList.RemoveAll(unsaved => savedSkillList.Any(saved => saved.ParentReferenceGuid.Split('/').Last() == unsaved.UniqueIdentifier));
                    }
                }

                // assigning other assignment attributes to assignmentToEdit proxy object
                if (assignmentObj != null)
                {
                    assignmentToEdit= new AssignmentProxySave();
                    assignmentToEdit.Title = assignmentObj.Title;
                    assignmentToEdit.Module = assignmentObj.Module;
                    assignmentToEdit.Keywords = assignmentObj.Keywords;
                    assignmentToEdit.Duration = assignmentObj.Duration;
                    if (assignmentObj.Patients != null)
                    {
                        var patientObj =
                            assignmentObj.Patients.Select(x => x.Value).Where(x => x.IsAssignmentPatient == true).ToList();
                        if (patientObj.Count > 0)
                        {
                            assignmentToEdit.Patient = patientObj[0];
                            assignmentToEdit.Patient.UniqueIdentifier =
                                assignmentObj.Patients.Select(x => x.Key).ToList().First();
                        }
                    }
                    if (assignmentObj.Resources != null && assignmentObj.Resources.Count > 0)
                    {
                        assignmentToEdit.Resources = assignmentObj.Resources.Select(x => x.Value).OrderBy(res=>Convert.ToInt32(res.SequenceNumber)).ToList();
                    }
                }
            }
            ViewBag.ListOfSelectedSkillsets = skillList;
            ViewBag.ListOfSavedSkillSets = savedSkillList;
            ViewBag.MRNnumber = AppCommon.GenerateRandomNumber();
            ViewBag.AssignmentToEdit = assignmentToEdit;
            return View("../../Views/Builder/Assignment/AssignmentMetaData");
        }

        public ActionResult LoadAssignmentOfficeSettings()
        {
            return View("../../Views/Builder/Assignment/Step4OfficeSetting");
        }

        public ActionResult LoadAssignmentPreviewAndPublish()
        {
            return View("../../Views/Builder/Assignment/Step5_PreviewAndPublish");
        }

        /// <summary>
        /// method to load the Assignments in the datatable
        /// </summary>
        /// <param name="param"></param>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="filterByModule"></param>
        /// <param name="selectedAssignmentList"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public ActionResult GetAssignmentList(jQueryDataTableParamModel param, string parentFolderIdentifier, int folderType, string filterByModule, string selectedAssignmentList, string folderUrl)
        {
            int assignmentListCount = 0;
            string[] gridColumnList = { "Title", "Module", "LinkedCompetencies", "Duration", "CreatedTimeStamp", "Status" };
            List<Assignment> assignmentList = (List<Assignment>)_assignmentService.GetAssignmentItems(parentFolderIdentifier, folderType,
                                                                                     GetLoginUserCourse() + "/" +
                                                                                     GetLoginUserRole(), folderUrl);
            if (filterByModule != "")
            {
                assignmentList.RemoveAll(assignment => assignment.Module == null);
                assignmentList = (from assignment in assignmentList where assignment.Module.Select(x => x.ToLower()).Contains(filterByModule.ToLower()) select assignment).ToList();
            }

            int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnOrder = Request["sSortDir_0"];
            string sortColumnName = gridColumnList[sortColumnIndex - 1];

            switch (gridColumnList[sortColumnIndex - 1])
            {
                case "Module":
                    if (sortColumnOrder == "asc")
                    {
                        assignmentList = (from assignmentItem in assignmentList
                                          let module = ((assignmentItem.Module) != null && (assignmentItem.Module.Count) > 0) ? assignmentItem.Module.Aggregate((first, next) => first + ", " + next) : ""
                                          orderby module
                                          select assignmentItem).ToList();
                    }
                    else
                    {
                        assignmentList = (from assignmentItem in assignmentList
                                          let module = ((assignmentItem.Module) != null && (assignmentItem.Module.Count) > 0) ? assignmentItem.Module.Aggregate((first, next) => first + ", " + next) : ""
                                          orderby module descending 
                                          select assignmentItem).ToList();
                    }
                    break;
                case "LinkedCompetencies":
                    if (sortColumnOrder == "asc")
                    {
                        assignmentList = (from assignmentItem in assignmentList
                                          let competency = ((assignmentItem.SkillSets) != null && (assignmentItem.SkillSets.Count) > 0) ? _competencyService.GetCompetencyNameForListofCompetencies(_assignmentService.ConvertSkillsetToCompetencies(assignmentItem.SkillSets)) : ""
                                          orderby competency
                                          select assignmentItem).ToList();
                    }
                    else
                    {
                        assignmentList = (from assignmentItem in assignmentList
                                          let competency = ((assignmentItem.SkillSets) != null && (assignmentItem.SkillSets.Count) > 0) ? _competencyService.GetCompetencyNameForListofCompetencies(_assignmentService.ConvertSkillsetToCompetencies(assignmentItem.SkillSets)) : ""
                                          orderby competency descending
                                          select assignmentItem).ToList();
                    }
                    break;
                default:
                    var sortableList = assignmentList.AsQueryable();
                    assignmentList = sortableList.OrderBy<Assignment>(sortColumnName, sortColumnOrder).ToList<Assignment>();
                    break;
            }

            IList<Assignment> assignmentListToRender = assignmentList.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            assignmentListCount = assignmentList.Count;
            string[] strArray = AppCommon.GetStringArrayAfterSplitting(selectedAssignmentList);
            var data = (from assignmentItem in assignmentListToRender
                        select new[]
                                   {
                                       "<input type='checkbox' id='" + assignmentItem.UniqueIdentifier + "' onClick='assignBuilder.commonFunctions.gridOperations.AssignmentItemChanged(this)'" + AppCommon.CheckForFlagAndReturnValue(strArray, assignmentItem.UniqueIdentifier) + "/>",
                                      !string.IsNullOrEmpty(assignmentItem.Title) ? "<a href='#' onclick='assignBuilder.commonFunctions.LoadStep1OrStep5(\""+(!string.IsNullOrEmpty(assignmentItem.Url)?assignmentItem.Url:"")+"\","+(!string.IsNullOrEmpty(assignmentItem.Status)?(assignmentItem.Status.ToLower()=="published"?"true":"false"):"false") +")' class=\"link select-hand\">" + assignmentItem.Title + "</a>" : "",
                                      ((assignmentItem.Module) !=null &&(assignmentItem.Module.Count) > 0) ? assignmentItem.Module.Aggregate((first, next) => first + ", " + next) : "",
                                       ((assignmentItem.SkillSets) !=null &&(assignmentItem.SkillSets.Count) > 0) ? _competencyService.GetCompetencyNameForListofCompetencies(_assignmentService.ConvertSkillsetToCompetencies(assignmentItem.SkillSets)) : "",
                                       !string.IsNullOrEmpty(assignmentItem.Duration) ? assignmentItem.Duration : "",
                                       !string.IsNullOrEmpty(assignmentItem.CreatedTimeStamp.ToString("MM/dd/yyyy")) ? assignmentItem.CreatedTimeStamp.ToString("MM/dd/yyyy") : "",
                                       !string.IsNullOrEmpty(assignmentItem.Status) ? assignmentItem.Status : ""
                                   }).ToArray();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = assignmentListCount,
                iTotalDisplayRecords = assignmentListCount,
                aaData = data
            },
            JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Method to get the competency values associated with the assignment to populate the flexbox
        /// </summary>
        /// <param name="assignmentUrl"></param>
        /// <returns></returns>
        public ActionResult GetCompetenciesForFlexBox(string assignmentUrl)
        {
            IList<AutoCompleteProxy> competenciesToReturn = GetCompetenciesForAnAssignment(assignmentUrl);
            return Json(new { Result = competenciesToReturn });
        }
        /// <summary>
        /// Method to get the competency values associated with the assignment
        /// </summary>
        /// <param name="assignmentUrl"></param>
        /// <returns></returns>
        public IList<AutoCompleteProxy> GetCompetenciesForAnAssignment(string assignmentUrl)
        {
            Assignment assignmentObject = _assignmentService.GetAssignment(assignmentUrl);
            Dictionary<string, SkillSet> skillSetsForTheAssignment = assignmentObject.SkillSets;
            IList<AutoCompleteProxy> competencyListForSkillSets = new List<AutoCompleteProxy>();
            foreach (var skillSetItem in skillSetsForTheAssignment)
            {
                foreach (var competencyItem in skillSetItem.Value.Competencies)
                {
                    AutoCompleteProxy competencyItemToAdd = new AutoCompleteProxy();
                    competencyItemToAdd.id = competencyItem;
                    competencyItemToAdd.name = GetLinkedCompetencyForAGuid(competencyItem);
                    competencyListForSkillSets.Add(competencyItemToAdd);
                }
            }
            return competencyListForSkillSets;
        }
        /// <summary>
        /// Method to filter the questions from question bank based on filter
        /// </summary>
        /// <param name="selectedCompetency"></param>
        /// <param name="selectedFilter"></param>
        /// <param name="assignmentUrl"></param>
        /// <returns></returns>
        public ActionResult GetQuestionItemsBasedOnCompetencyAndFilter(string selectedCompetency, string selectedFilter, string assignmentUrl)
        {
            IList<DocumentProxy> questionList = new List<DocumentProxy>();
            questionList = _questionBankDocument.GetAllQuestionsInAQuestionBank();
            //Assignment assignmentObject = _assignmentService.GetAssignment(assignmentUrl);
            //Dictionary<string, SkillSet> skillSetsForTheAssignment = assignmentObject.SkillSets;
            IList<AutoCompleteProxy> competencyListForSkillSets = GetCompetenciesForAnAssignment(assignmentUrl);
            questionList =
                (from qus in questionList join compGuid in competencyListForSkillSets on qus.LinkedItemReference equals compGuid.id.ToString() select qus).ToList();
            if (!string.IsNullOrEmpty(selectedCompetency))
            {
                questionList = (from questionListItem in questionList
                                where
                                    GetLinkedCompetencyForAGuid(questionListItem.LinkedItemReference) ==
                                    selectedCompetency
                                select questionListItem).ToList();
            }
            if (!string.IsNullOrEmpty(selectedFilter))
            {
                questionList = (from questionListItem in questionList
                                where questionListItem.TypeOfQuestion == selectedFilter
                                select questionListItem).ToList();
            }
            return Json(new {Result = questionList});
        }
        /// <summary>
        /// Method to get the competency value with its reference
        /// </summary>
        /// <param name="guidOfLinkedCompetency"></param>
        /// <returns></returns>
        public string GetLinkedCompetencyForAGuid(string guidOfLinkedCompetency)
        {
            List<AutoCompleteProxy> lstOfAutoComplete = _competencyService.GetAllCompetencyListForDropDown();
            return (from lstCompetency in lstOfAutoComplete where lstCompetency.id == guidOfLinkedCompetency select lstCompetency.name).SingleOrDefault();
        }
        /// <summary>
        /// Method to filter skill sets based on competency value
        /// </summary>
        /// <param name="guidOfCompetency"></param>
        /// <returns></returns>
        public ActionResult FilterSkillSetsBasedOnCompetencies(string guidOfCompetency)
        {
            List<SkillSetProxy> skillSetProxyList = (List<SkillSetProxy>) _skillSetService.GetFilteredSkillSetsBasedOnCompetency(guidOfCompetency);
            skillSetProxyList.RemoveAll(skillset => skillset.Status == null);
            skillSetProxyList = (from skillSet in skillSetProxyList where skillSet.Status.ToLower().Equals("published") select skillSet).ToList();
            return Json(new { Result = skillSetProxyList });
            //_skillSetService.GetFilteredSkillSetsBasedOnCompetency(guidOfCompetency);
        }
        /// <summary>
        /// Method to get competencies associated with the given skill set
        /// </summary>
        /// <param name="skillsetUniqueIdentifier"></param>
        /// <returns></returns>
        public ActionResult GetCompetenciesForASkillSet(string skillsetUniqueIdentifier)
        {
            SkillSet skillset = _skillSetService.GetSkillSet(skillsetUniqueIdentifier);
            string competencyList = _competencyService.GetCompetencyNameListInNumbers(skillset.Competencies);
            //IList<AutoCompleteProxy> competencyList = _skillSetService.GetFormattedCompetenciesForSkillSet(skillset);
            return Json(new { Result = competencyList });
        }

        /// <summary>
        /// To load upload control to view
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadCompetencyList()
        {
            return View("../Builder/Assignment/LoadCompetencyList");
        }

        public ActionResult AssignmentBuilder()
        {
            //ViewData["NoOfAttempts"] = new SelectList(AppCommon.NoOfAttempts.ToList(), "NoOfAttempts");                     
            return View("../Builder/Assignment/AssignmentBuilder");
        }
        /// <summary>
        /// Method to load Step 3 of assignment builder
        /// </summary>
        /// <param name="assignmentUrl"></param>
        /// <returns></returns>
        public ActionResult LoadAssignmentStep3(string assignmentUrl, string selectedAttempts, string selectedPassRate)
        {
            ViewData["NoOfAttempts"] = new SelectList(AppCommon.NoOfAttempts.ToList(), "NoOfAttempts");
            Assignment assignmentObject = _assignmentService.GetAssignment(assignmentUrl);
            if (selectedAttempts == "")
            {
                selectedAttempts = assignmentObject.NoOfAttemptsAllowed ?? "";
                selectedPassRate = assignmentObject.AssignmentPassRate ?? "";
            }
            ViewBag.SelectedAttempts = selectedAttempts;
            ViewBag.SelectedPassRate = selectedPassRate;
            IList<SkillSetProxy> skillSetList = GetSkillSetsForAnAssignment(assignmentUrl);
            ViewBag.SkillSetList = skillSetList;
            IList<Question> questionList = GetQuestionsForAssignment(assignmentUrl);
            ViewBag.QuestionList = questionList;
            int skillSetCount = 0;
            foreach (var skillSet in skillSetList)
            {
                skillSetCount = skillSetCount + skillSet.Questions.Count();
            }
            double noOfQuestions = skillSetCount + questionList.Count();
            List<string> percentages = AppCommon.CalculatePassRate(noOfQuestions);
            ViewData["PassRate"] = new SelectList(percentages, "PassRate");
            return View("../Builder/Assignment/_Step3SkillSets");
            //return Json(new { Result = "Success" });
        }
        /// <summary>
        /// Method to save step 3 of assignment builder
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveAssignmentStep3()
        {
            string assignmentJson = "";
            string result = "";
            bool isEditMode = false;
            string assignmentUrl = "";
            Assignment assignmentObject = new Assignment();
            Assignment assignment = new Assignment();
            IList<SkillSetProxy> skillSetList = new List<SkillSetProxy>();
            IList<Question> questionList = new List<Question>();
            try
            {
                assignmentJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                assignmentObject = JsonSerializer.DeserializeObject<Assignment>(assignmentJson);
                Type assignmentType = assignmentObject.GetType();
                assignment = _assignmentService.GetAssignment(assignmentObject.Url);
                assignment.NoOfAttemptsAllowed = assignmentObject.NoOfAttemptsAllowed;
                assignment.AssignmentPassRate = assignmentObject.AssignmentPassRate;
                if (assignment.Questions != null)
                {
                    if (assignmentObject.Questions != null)
                    {
                        foreach (var question in assignmentObject.Questions)
                        {
                            if (!assignment.Questions.ContainsKey(question.Key))
                            {
                                assignment.Questions.Add(question.Key, question.Value);
                            }
                        }
                    }
                }
                else
                {
                    assignment.Questions = assignmentObject.Questions;
                }
                if (assignment.Url != null && assignment.Url != "")
                {
                    isEditMode = true;
                    assignmentUrl = assignment.Url;
                }
                SetAuditFields(assignment, isEditMode);
                _assignmentService.SaveAssignment(assignment, GetLoginUserCourse() + "/" + GetLoginUserRole(), isEditMode, assignmentUrl);
                skillSetList = GetSkillSetsForAnAssignment(assignmentUrl);
                questionList = GetQuestionsForAssignment(assignmentUrl);
                //questionList = ((assignment.Questions != null) ? (assignment.Questions.Select(questionItem => questionItem.Value).ToList()) : new List<Question>());
                //skillSetList = ((assignment.SkillSets != null) ? (assignment.SkillSets.Select(skillSetItem => skillSetItem.Value).ToList()) : new List<SkillSet>());

            }
            catch (Exception ex)
            {
                result = "A problem was encountered preventing" + ex;
            }
            return Json(new { AssignmentUrl = assignment.Url, skillSetList = skillSetList, questionList = questionList });

        }
        /// <summary>
        /// Method to get the skill sets and questions associated with it for a given assignment
        /// </summary>
        /// <param name="assignmentUrl"></param>
        /// <returns></returns>
        public IList<SkillSetProxy> GetSkillSetsForAnAssignment(string assignmentUrl)
        {
            Assignment assignmentObject = _assignmentService.GetAssignment(assignmentUrl);
            Dictionary<string, SkillSet> skillSetsForTheAssignment = new Dictionary<string, SkillSet>();
            if (assignmentObject != null && assignmentObject.SkillSets != null)
            {
                skillSetsForTheAssignment = assignmentObject.SkillSets;
            }
            IList<string> competencyListForSkillSets = new List<string>();
            if (skillSetsForTheAssignment != null)
            {
                foreach (var skillSetItem in skillSetsForTheAssignment)
                {
                    foreach (var competencyItem in skillSetItem.Value.Competencies)
                    {
                        competencyListForSkillSets.Add(GetLinkedCompetencyForAGuid(competencyItem));
                    }
                    skillSetItem.Value.UniqueIdentifier = skillSetItem.Key;
                    Dictionary<string, Question> questionsForSkillSet = skillSetItem.Value.Questions;
                    if (questionsForSkillSet != null)
                    {
                        foreach (var questionItem in questionsForSkillSet)
                        {
                            questionItem.Value.UniqueIdentifier = questionItem.Key;
                        }
                    }
                }
            }
            IList<SkillSetProxy> skillSetProxyObject = new List<SkillSetProxy>();
            IList<SkillSet> skillSetList = ((skillSetsForTheAssignment != null) ? (skillSetsForTheAssignment.Select(assignmentItem => assignmentItem.Value).ToList()).OrderBy(x => x.SequenceNumber).ToList() : new List<SkillSet>());
            if (skillSetList != null)
            {
                foreach (SkillSet skillSet in skillSetList)
                {
                    SkillSetProxy skillSetProxy = new SkillSetProxy();
                    skillSetProxy.UniqueIdentifier = skillSet.UniqueIdentifier;
                    skillSetProxy.SkillSetTitle = skillSet.SkillSetTitle;
                    skillSetProxy.Questions = ((skillSet.Questions != null) ? (skillSet.Questions.Select(questionItem => questionItem.Value).ToList()).OrderBy(x => x.SequenceNumber).ToList() : new List<Question>());
                    skillSetProxy.Guid = skillSet.UniqueIdentifier;
                    skillSetProxy.Url = skillSet.Url;
                    skillSetProxyObject.Add(skillSetProxy);
                }
            }
            //ViewBag.CompetencyListForSelectedSkillSet = competencyListForSkillSets;
            return skillSetProxyObject;
        }
        /// <summary>
        /// Method to get the list of questions in a skill set
        /// </summary>
        /// <param name="assignmentUrl"></param>
        /// <param name="skillSetGuid"></param>
        /// <returns></returns>
        public ActionResult GetSkillSetsForAssignmentTree(string assignmentUrl)
        {
            IList<SkillSetProxy> skillSetsForTheAssignment = GetSkillSetsForAnAssignment(assignmentUrl);
            return Json(new { SkillSetList = skillSetsForTheAssignment });
        }

        public ActionResult GetQuestionsForAssignmentTree(string assignmentUrl)
        {
            IList<Question> questionsForTheAssignment = GetQuestionsForAssignment(assignmentUrl);
            return Json(new { QuestionList = questionsForTheAssignment });
        }
        /// <summary>
        /// Method to swap questions inside a skill set
        /// </summary>
        /// <param name="sourceUrl"></param>
        /// <param name="destinationUrl"></param>
        /// <param name="assignmentUrl"></param>
        /// <returns></returns>
        public ActionResult SwapSkillSets(string sourceUrl, string destinationUrl, string assignmentUrl)
        {
            SkillSet skillSetSource = new SkillSet();
            SkillSet skillSetDestination = new SkillSet();
            string skillSetIdentifierSource = string.Empty;
            string skillSetIdentifierDestination = string.Empty;
            skillSetSource = _skillSetService.GetSkillSet(sourceUrl);
            skillSetDestination = _skillSetService.GetSkillSet(destinationUrl);
            int tempSequenceNumber = 0;
            tempSequenceNumber = (skillSetSource != null) ? skillSetSource.SequenceNumber : 1;
            skillSetSource.SequenceNumber = (skillSetDestination != null) ? skillSetDestination.SequenceNumber : 1;
            skillSetDestination.SequenceNumber = (tempSequenceNumber != 0) ? tempSequenceNumber : 1;
            _skillSetService.SwapSkillSetSave(skillSetSource, sourceUrl);
            _skillSetService.SwapSkillSetSave(skillSetDestination, destinationUrl);

            //Question questionSource = new Question();
            //Question questionDestination = new Question();
            //questionSource = _questionBankService.GetQuestion(sourceUrl);
            //questionDestination = _questionBankService.GetQuestion(destinationUrl);
            //int tempSequenceNumber = 0;
            //tempSequenceNumber = (questionSource != null) ? questionSource.SequenceNumber : 1;
            //questionSource.SequenceNumber = (questionDestination != null) ? questionDestination.SequenceNumber : 1;
            //questionDestination.SequenceNumber = (tempSequenceNumber != 0) ? tempSequenceNumber : 1;
            //_questionBankService.SaveQuestion(questionSource, "", sourceUrl, "", true);
            //_questionBankService.SaveQuestion(questionDestination, "", destinationUrl, "", true);
            //IList<Question> questionsForSkillSet = GetQuestionsForAssignment(assignmentUrl);
            //IList<SkillSetProxy> skillSetListForAssignment = GetSkillSetsForAnAssignment(assignmentUrl);
            //questionsForSkillSet = questionsForSkillSet.OrderBy(x => x.SequenceNumber).ToList();
            return Json(new { Result = string.Empty, strSourceUrl = sourceUrl });

        }

        public ActionResult SwapQuestions(string sourceUrl, string destinationUrl, string assignmentUrl)
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
            return Json(new { Result = string.Empty, strSourceUrl = sourceUrl });
        }
        /// <summary>
        /// Method to fetch the questions associated with an assignment
        /// </summary>
        /// <param name="assignmentUrl"></param>
        /// <returns></returns>
        public IList<Question> GetQuestionsForAssignment(string assignmentUrl)
        {
            Assignment assignmentObject = _assignmentService.GetAssignment(assignmentUrl);
            Dictionary<string, Question> questionListObject = new Dictionary<string, Question>();
            if (assignmentObject != null && assignmentObject.Questions != null)
            {
                questionListObject = assignmentObject.Questions;
            }
            if (questionListObject != null)
            {
                foreach (var question in questionListObject)
                {
                    question.Value.UniqueIdentifier = question.Key;
                }
            }
            IList<Question> questionList = ((questionListObject != null) ? (questionListObject.Select(questionItem => questionItem.Value).ToList()) : new List<Question>());
            questionList = questionList.OrderBy(x => x.SequenceNumber).ToList();
            return questionList;
        }
        /// <summary>
        /// Method to revert the selected question to the original vaersion as in question bank
        /// </summary>
        /// <param name="questionUrl"></param>
        /// <param name="questionGuid"></param>
        /// <param name="skillSetGuid"></param>
        public void RevertQuestionToOriginal(string questionUrl, string questionGuid, string skillSetGuid, bool isSelectedItemQuestion)
        {
            Question questionItemSelected = _questionBankService.GetQuestion(questionUrl);
            string questionReferenceGuid = "";
            if (isSelectedItemQuestion)
            {
                questionReferenceGuid = questionItemSelected.SkillSetReferenceUrlOfQuestion;
            }
            else
            {
                questionReferenceGuid = questionItemSelected.ParentReferenceGuid;
            }
            //Question questionObjectFromSkillSet = _questionBankService.GetQuestion(questionUrl);
            Question questionFromQuestionBank =
                _questionBankService.GetQuestion(questionReferenceGuid);
            //string questionUrlToSave = _questionBankService.FormUrlForSkillSetQuestionToQuestionBank(GetLoginUserCourse() + "/" + GetLoginUserRole(), questionGuid);
            _questionBankService.SaveQuestion(questionFromQuestionBank, GetLoginUserCourse() + "/" + GetLoginUserRole(), questionUrl, "", true);
            //_assignmentService.DeleteAssignmentQuestion(questionUrl);
        }
        /// <summary>
        /// Method to delete a specific question from the assignment
        /// </summary>
        /// <param name="questionUrl"></param>
        /// <returns></returns>
        public ActionResult DeleteQuestionFromAssignment(string questionUrl)
        {
            if (questionUrl != "")
            {
                _assignmentService.DeleteAssignmentQuestion(questionUrl);
            }
            return Json(new {Result = "Success"});
        }

        /// <summary>
        /// To refresh the grid on search
        /// </summary>
        /// <param name="param"></param>
        /// <param name="strSearchText"></param>
        /// <returns></returns>
        public ActionResult GetAssignmentSearchList(jQueryDataTableParamModel param, string strSearchText, string strModule)
        {
            IList<AssignmentProxy> lstAssignmentSearchResult = new List<AssignmentProxy>();
            int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnOrder = Request["sSortDir_0"];
            try
            {
                IList<AssignmentProxy> lstAssignmentSearchResultTemp = _assignmentService.GetSearchResultsForAssignment(strSearchText, sortColumnIndex, sortColumnOrder, strModule);
                lstAssignmentSearchResult = lstAssignmentSearchResultTemp.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                var data = (from assignmentItem in lstAssignmentSearchResult
                            select new[]
                                   {
                                       (!String.IsNullOrEmpty(assignmentItem.Patients)) ? assignmentItem.Patients :String.Empty,
                                       !string.IsNullOrEmpty(assignmentItem.AssignmentTitle) ? "<a href='#' onclick='assignBuilder.commonFunctions.LoadStep1OrStep5(\""+(!string.IsNullOrEmpty(assignmentItem.Url)?assignmentItem.Url:"")+"\","+(!string.IsNullOrEmpty(assignmentItem.Status)?(assignmentItem.Status.ToLower()=="published"?"true":"false"):"false") +")' class=\"link select-hand\">" + assignmentItem.AssignmentTitle + "</a>" : "",
                                       (!String.IsNullOrEmpty(assignmentItem.ModuleText)) ? assignmentItem.ModuleText :String.Empty,
                                       (!String.IsNullOrEmpty(assignmentItem.LinkedCompetencies)) ? assignmentItem.LinkedCompetencies :String.Empty,
                                       (!String.IsNullOrEmpty(assignmentItem.Duration)) ? assignmentItem.Duration :String.Empty,
                                       (!String.IsNullOrEmpty(assignmentItem.FolderName)) ? assignmentItem.FolderName :String.Empty,
                                       !string.IsNullOrEmpty(assignmentItem.CreatedTimeStamp.ToString("MM/dd/yyyy")) ? assignmentItem.CreatedTimeStamp.ToString("MM/dd/yyyy") : "",
                                       (!String.IsNullOrEmpty(assignmentItem.Status)) ? assignmentItem.Status :String.Empty
                                   }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = lstAssignmentSearchResultTemp.Count,
                    iTotalDisplayRecords = lstAssignmentSearchResultTemp.Count,
                    aaData = data
                },
            JsonRequestBehavior.AllowGet);

            }
            catch
            {
                //To-Do
            }
            return Json(new { Result = string.Empty });
        }

        /// <summary>
        /// To render search page on click of search
        /// </summary>
        /// <param name="strSearchText"></param>
        /// /// <param name="strModule"></param>
        /// <returns></returns>
        public ActionResult GiveSearchResults(string strSearchText, string strModule)
        {            
            ViewBag.SearchText = strSearchText;
            //ViewBag.SearchModule = strModule;
            //ViewBag.SearchQuestionType = !String.IsNullOrEmpty(strQuestionType) ? (AppCommon.QuestionTypeOptions.Single(x => x.Key == Convert.ToInt32(strQuestionType)).Value.ToString()) : String.Empty;
            //ViewBag.filterByQuestionTypeSearch = GetQuestionTypeFlexBoxList();
            IList<ApplicationModules> competencyModuleList = _competencyService.GetAllApplicationModules();
            int index = 0;
            ViewBag.ModuleList = competencyModuleList.Select(cat => new { id = index++, name = cat.Name.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.SearchModule = strModule;
            return View("../Builder/Assignment/AssignmentSearchResults");
        }

        public ActionResult GetPatientList(string assignmentUrl)
        {
            List<Patient> patientList;            
            patientList = _assignmentService.GetPatientListOfAssignment(assignmentUrl);
            return Json(new { Result = patientList });
        }

        /// <summary>
        /// To get the name of the patient saved in step 1 of assignment.
        /// </summary>
        /// <param name="assignmentObj"></param>
        /// <returns></returns>
        private string GetAssignmentPatientName(Assignment assignmentObj)
        {
            Patient patient = new Patient();
            string nameOfPatient = String.Empty;
            if (assignmentObj != null && assignmentObj.Patients != null && assignmentObj.Patients.Count > 0)
            {
                IList<Patient> patientList= assignmentObj.Patients.Select(s => s.Value).ToList();
                patient = (from lstpat in patientList where lstpat.IsAssignmentPatient == true select lstpat).FirstOrDefault();
                if (patient != null)
                {
                    nameOfPatient = patient.LastName + ", " + patient.FirstName + ", " + patient.MiddleInitial;
                }
            }
            return nameOfPatient;
        }

        /// <summary>
        /// To get patient Name list for step 1 Assignment Builder
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetAllPatientsName()
        {
            List<Patient> patientsList = new List<Patient>();
            int patientindex = 0;
            patientsList = _assignmentService.GetAllPatientName();
            var patientName = patientsList.Select(patient => new { id = patient.Url, name = (patient.LastName.ToString() + ", " + patient.FirstName.ToString() + " " + patient.MiddleInitial.ToString()) }).ToList();
            return Json(new { PatientList = JsonSerializer.SerializeObject(patientName) });

        }

        /// <summary>
        /// Step 4 of skillset builder
        /// </summary>
        /// <param name="skillSetUrl"></param>
        /// <returns></returns>
        public ActionResult PreviewAndPublishStep5(string assignmentUrl)
        {
            try
            {
                IList<DocumentProxy> questionsForSkillSetProxy = _assignmentService.GetAssignmentInfos(assignmentUrl);
                IList<Resource> resourceListForAssignment = _assignmentService.GetResourcesForAnAssignment(assignmentUrl);
                Assignment assignmentObj = _assignmentService.GetAssignment(assignmentUrl);
                ViewBag.assignmentName = (assignmentObj!=null && !string.IsNullOrEmpty(assignmentObj.Title))?assignmentObj.Title:String.Empty;
                ViewBag.Modules = (assignmentObj.Module != null && assignmentObj.Module.Count > 0) ? String.Join(", ", assignmentObj.Module.Select(m=>m)) : String.Empty;
                ViewBag.Keywords = (assignmentObj != null && !string.IsNullOrEmpty(assignmentObj.Keywords)) ? assignmentObj.Keywords : String.Empty;
                ViewBag.Duration = (assignmentObj != null && !string.IsNullOrEmpty(assignmentObj.Duration)) ? assignmentObj.Duration : String.Empty;
                ViewBag.Patient = GetAssignmentPatientName(assignmentObj); 
                ViewBag.assignmentInfos = questionsForSkillSetProxy;
                ViewBag.assignmentResources = resourceListForAssignment;
                ViewBag.assignmentOrientation = (assignmentObj != null && !string.IsNullOrEmpty(assignmentObj.Orientation)) ? assignmentObj.Orientation : String.Empty;
                ViewBag.assignmentStatus = (assignmentObj != null && !string.IsNullOrEmpty(assignmentObj.Status)) ? AppCommon.CheckIfPublished(assignmentObj.Status): false;
            }
            catch
            {

                //TO-dO
            }
            return View("../Builder/Assignment/Step5_PreviewAndPublish");
        }


        /// <summary>
        /// To publish a in progress assignment
        /// </summary>
        /// <param name="skillSetUrl"></param>
        /// <returns></returns>
        public JsonResult PublishAnAssignment(string assignmentUrl)
        {
            bool isPublishedSuccessfully = false;
            try
            {
                isPublishedSuccessfully = _assignmentService.PublishAnAssignment(assignmentUrl);
            }
            catch
            {
                isPublishedSuccessfully = false;
                //To-Do
            }
            return Json(new { Result = isPublishedSuccessfully });
        }

    }
}
