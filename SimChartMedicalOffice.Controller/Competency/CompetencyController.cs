using System.Xml;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SimChartMedicalOffice.Common.Logging;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.Competency;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using System.IO;
using SimChartMedicalOffice.Core.Json;
using System;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency;
using SimChartMedicalOffice.Common.Extensions;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class CompetencyController : BaseController
    {
        private readonly ICompetencyService _competencyService;

        public CompetencyController(ICompetencyService competencyService)
        {
            _competencyService = competencyService;
        }

        public ActionResult LoadMaster(int iReferenceOfMasterToLoad)
        {
            ViewBag.MasterName = (int)AppEnum.Master.Competency;
            SetViewBagValuesForCompetency();
            return View();
        }

        public void SetViewBagValuesForCompetency()
        {
            IList<string> competencyCategoryList = _competencyService.GetAllCategories();
            IList<CompetencySources> competencySourceList = _competencyService.GetAllCompetecnySources();
            IList<ApplicationModules> competencyMainFocusList = new List<ApplicationModules>();
            IList<ApplicationModules> competencyFocusList = _competencyService.GetAllApplicationModules();
            ApplicationModules app = new ApplicationModules {Name = "-Select-"};
            competencyMainFocusList.Add(app);
            competencyMainFocusList = competencyMainFocusList.Concat(competencyFocusList).ToList();
            int index = 0;
            var strSourceList = competencySourceList.Select(source => source.Name.ToString()).ToList();
            var strCategory = competencyCategoryList.Select(cat => new { id = index++, name = cat.ToString() }).ToList();
            var strFocus = competencyMainFocusList.Select(source => source.Name.ToString()).ToList();
            ViewBag.CompetencyCategory = strCategory;
            ViewBag.CompetencySource = strSourceList;
            ViewData["CompetencyFocus"] = new SelectList(strFocus, "CompetencyFocus");
        }
        public void ExportCompetencyToXmlDocument()
        {
            List<Competency> competencyList = _competencyService.GetAllCompetencies().ToList();

            var competencyData = (from competency in competencyList
                                  select new
                                             {
                                                 CompetencyCategory = competency.Category,
                                                 CompetencyName = competency.Name,
                                                 CAAHEP = ((competency.Sources) != null && (competency.Sources.Count) > 0) ? competency.Sources.OrderBy(source => source.Name).Where(source => source.Name.Equals("CAAHEP")).Select(source => source.Number).FirstOrDefault() : "",
                                                 ABHES = ((competency.Sources) != null && (competency.Sources.Count) > 0) ? competency.Sources.OrderBy(source => source.Name).Where(source => source.Name.Equals("ABHES")).Select(source => source.Number).FirstOrDefault() : "",
                                                 Source = ((competency.Sources) != null && (competency.Sources.Count) > 0) ? (string.Join(",", competency.Sources.OrderBy(source => source.Name).Select(source => source.Name.ToString()).ToArray())) : "",
                                                 Focus = competency.Focus,
                                                 Status = (competency.IsActive) ? "Active" : "Deleted",
                                             }).ToList();

            XmlDocument competencyXmlDocument = JsonSerializer.DeserializeXmlObject(
                JsonSerializer.SerializeObject(competencyData));

            MemoryStream ms = new MemoryStream();
            competencyXmlDocument.Save(ms);
            byte[] btFile = ms.ToArray();

            Response.Buffer = true;
            Response.Expires = 0;
            Response.ContentType = "text/xml";
            Response.AddHeader("Content-Type", "text/xml");
            Response.AddHeader("Content-Length", btFile.Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment;filename=Competency.xml");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(btFile);
            Response.End();

        }

        /// <summary>
        /// method to save competency
        /// </summary>
        /// <param name="isEditMode"></param>
        /// <param name="competencyUrlReference"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveCompetency(string competencyUrlReference, bool isEditMode)
        {
            string competencyObjJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
            Competency competencyObject = JsonSerializer.DeserializeObject<Competency>(competencyObjJson);
            SetAuditFields(competencyObject, isEditMode);
            _competencyService.SaveCompetency(competencyObject, competencyUrlReference, isEditMode);
            return Json(new { Success = "" });
        }

        /// <summary>
        /// method to save competency
        /// </summary>
        /// <param name="param"></param>
        /// <param name="searchByText"></param>
        /// <param name="selectedCompetencyList"></param>
        /// <param name="isCompetencySave"></param>
        /// <param name="isCompetencyDelete"></param>
        /// <returns></returns>
        public ActionResult GetCompetencyList(JQueryDataTableParamModel param, string searchByText, string selectedCompetencyList, bool isCompetencySave, bool isCompetencyDelete)
        {
           // int competencyCount=0;

            IList<Competency> competencyList = _competencyService.GetAllCompetencies();
            if (searchByText != "")
            {
                competencyList = (from com in competencyList where com.Category.ToLower().StartsWith(searchByText.ToLower()) || com.Name.ToLower().StartsWith(searchByText.ToLower()) select com).Distinct().ToList();

            }

            int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnOrder = Request["sSortDir_0"];
            string[] gridColumnList = { "Category", "Name", "Source", "Focus", "IsActive" };
            string sortColumnName = gridColumnList[sortColumnIndex - 1];
            switch (sortColumnName)
            {

                case "Source":
                    if (sortColumnOrder == "asc")
                    {
                        competencyList = (from com in competencyList
                                          let sourceNumber = ((com.Sources) !=null &&(com.Sources.Count) > 0) ? (string.Join(", ", com.Sources.OrderBy(sour => sour.Name).Select(sour => sour.Name.ToString()).ToArray())) : ""
                                          orderby sourceNumber
                                          select com).ToList();
                    }
                    else
                    {
                        competencyList = (from com in competencyList
                                          let sourceNumber   = ((com.Sources) != null && (com.Sources.Count) > 0) ? (string.Join(", ", com.Sources.OrderBy(sour => sour.Name).Select(sour => sour.Name.ToString()).ToArray())) : ""
                                          orderby sourceNumber descending
                                          select com).ToList();
                    }
                    break;

                default:
                    var sortableList = competencyList.AsQueryable();
                    if (isCompetencySave && !isCompetencyDelete)
                    {
                        competencyList = sortableList.OrderByDescending(com => (!(com.CreatedTimeStamp.Equals(DateTime.MinValue))?com.CreatedTimeStamp:com.ModifiedTimeStamp)).ThenByDescending(comp=>comp.IsActive).ToList();
                    }
                    else if (!isCompetencySave && isCompetencyDelete)
                    {
                        competencyList = sortableList.OrderByDescending(com => com.DeletedTimeStamp).ThenByDescending(comp => comp.IsActive).ToList();
                    }
                    else
                    {
                        competencyList = sortableList.OrderBy(sortColumnName, sortColumnOrder).ToList();
                    }                    
                    break;
            }
            IList<Competency> competencyListToRender = competencyList.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
          int  competencyCount = competencyList.Count;
            string[] strArray = AppCommon.GetStringArrayAfterSplitting(selectedCompetencyList);
            var data = (from competencyItem in competencyListToRender
                        select new[]
                                   {(competencyItem.IsActive)? "<input type='checkbox' id='" + competencyItem.UniqueIdentifier + "' onClick='competency.commonFunctions.competencyItemChanged(this)'" + AppCommon.CheckForFlagAndReturnValue(strArray, competencyItem.UniqueIdentifier) + "/>": "",
                                       !string.IsNullOrEmpty(competencyItem.Category) ? competencyItem.Category : "",
                                       !string.IsNullOrEmpty(competencyItem.Name) ? ( (competencyItem.IsActive)?"<a href='#' onclick=\"competency.commonFunctions.loadCompetency('"+competencyItem.UniqueIdentifier+"')\" class=\"link select-hand\">" + competencyItem.Name + "</a>" :competencyItem.Name) : "",
                                       ((competencyItem.Sources) !=null &&(competencyItem.Sources.Count) > 0) ? ( string.Join(", ",competencyItem.Sources.OrderBy(sour=>sour.Name).Select(sour=> sour.Name.ToString()).ToArray())) : "",
                                       !string.IsNullOrEmpty(competencyItem.Focus) ? competencyItem.Focus : "",
                                       (competencyItem.IsActive) ? "Active" : "Deleted"
                                       }).ToArray();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = competencyCount,
                iTotalDisplayRecords = competencyCount,
                aaData = data
            },
            JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// method to save competency sources 
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveCompetencySources()
        {
            string competencySourceObjJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
            CompetencySources sourceObject = JsonSerializer.DeserializeObject<CompetencySources>(competencySourceObjJson);
            _competencyService.SaveCompetencySource(sourceObject, "", false);
            IList<CompetencySources> competencySourceList = _competencyService.GetAllCompetecnySources();
            var strSourceList = competencySourceList.Select(source => source.Name.ToString()).ToList();
            return Json(new { Success = "", sourceList = strSourceList });
        }
        public ActionResult LoadCompetency(string competencyGuid)
        {
            if (!AppCommon.CheckIfStringIsEmptyOrNull(competencyGuid))
            {
                Competency competencyObject = _competencyService.GetCompetency(competencyGuid);
                SetViewBagsForCompetencyEditMode(competencyObject);
            }
            else
            {
                SetViewBagValuesForCompetency();
            }
            return View("../Competency/_CreateNewCompetency");
        }
        public void SetViewBagsForCompetencyEditMode(Competency competencyObject)
        {
            try
            {
                if (competencyObject != null)
                {
                    SetViewBagValuesForCompetency();
                    ViewBag.Category = competencyObject.Category;
                    ViewBag.Name = competencyObject.Name;
                    ViewBag.Focus = competencyObject.Focus;
                    ViewBag.Notes = competencyObject.Notes;
                    ViewBag.Sources = competencyObject.Sources;
                    ViewBag.Url = competencyObject.Url;
                    ViewBag.UniqueIdentifier = competencyObject.UniqueIdentifier;
                    ViewBag.IsEditMode = true;
                }

            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: Controller: Competency, MethodName: SetViewBagsForCompetencyEditMode", ex);
            }

        }
        /// <summary>
        /// to delete competencies
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteCompetency()
        {
            string competencyListOfIdsJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
            string competencyListOfIds = JsonSerializer.DeserializeObject<string>(competencyListOfIdsJson);
            string[] strArray = AppCommon.GetStringArrayAfterSplitting(competencyListOfIds);
            foreach (string t in strArray)
            {
                Competency deleteCompetecny = _competencyService.GetCompetency(t);
                deleteCompetecny.IsActive = false;
                SetAuditFields(deleteCompetecny, true);
                _competencyService.DeleteCompetency(deleteCompetecny);
            }
            return Json(new { Success = "" });
        }

    }
}
