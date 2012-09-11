using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Common.Utility;
using System.Web;
using System.IO;
using SimChartMedicalOffice.ApplicationServices.Builder;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder;
using SimChartMedicalOffice.Core.Json;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface;
namespace SimChartMedicalOffice.Web.Controllers
{
    public class PatientController : BaseController
    {
        private readonly IPatientService _patientService;
        private readonly IMasterService _masterService;
        public PatientController(IPatientService patientService, IMasterService masterService)
        {
            this._patientService = patientService;
            this._masterService = masterService;
        }

        /// <summary>
        /// Leads to Patient step1
        /// </summary>
        /// <returns></returns>
        public ActionResult Patient()
        {
            try
            {
                ViewData["officeType"] = new SelectList(AppCommon.officeTypeOptions, "officeType");
                Dictionary<int, string> patientProviderValues = _masterService.GetPatientProviderValues();
                var patientProviderList = (from item in patientProviderValues select new { Id = item.Key, Name = item.Value }).ToList();
                ViewData["provider"] = new SelectList(patientProviderList, "Id", "Name", 2);
                //ViewData["provider"] = new SelectList(AppCommon.providerOptions, "provider");
                //ViewBag.PatientList = GetPatientForGuid();
                ViewBag.MrnNumber = AppCommon.GenerateRandomNumber();
            }
            catch
            {
                //to do
            }
            return View("../Builder/Patient/PatientProfileSetUp");
        }

        public List<Patient> GetAllPatients()
        {
            List<Patient> patientsList = new List<Patient>();
            patientsList = _patientService.GetAllPatients();
            return patientsList;
        }

        public ActionResult GetPatientForGuid(string patientUrl)
        {
            Patient patientForGuid = new Patient();
            patientForGuid = _patientService.GetPatientForGuid(patientUrl);
            return Json(new { Result = patientForGuid, PatientAge = patientForGuid.FormAgeText()});
        }
        /// <summary>
        /// To save the patient
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SavePatient(string patientUrlReference, string folderIdentifier, bool isEditMode)
        {
            string patientJson = "";
            string result = "";
            Patient patientObject = new Patient();
            Patient patient = new Patient();
            try
            {
                patientJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                patientObject = JsonSerializer.DeserializeObject<Patient>(patientJson);
                patientObject.Status = "Unpublished";
                patientObject.IsActive = true;
                SetAuditFields(patientObject, isEditMode);
                Type patientType = patientObject.GetType();
                foreach (System.Reflection.PropertyInfo objProp in patientType.GetProperties())
                {
                    if (objProp.CanWrite && objProp.Name.ToUpper() != "ID")
                    {
                        objProp.SetValue(patient, patientType.GetProperty(objProp.Name).GetValue(patientObject, null), null);
                    }
                }                
                _patientService.SavePatient(patient, GetLoginUserCourse() + "/" + GetLoginUserRole(), patientUrlReference, folderIdentifier, isEditMode);
            }
            catch (Exception ex)
            {
                result = "A problem was encountered preventing" + ex;
            }

            return Json(new { Result = "success" });
        }

        /// <summary>
        /// To populate the Patient Grid on patient builder
        /// </summary>
        /// <param name="param"></param>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="filterByAge"></param>
        /// <param name="filterBySearch"></param>
        /// <returns></returns>
        public ActionResult GetPatientList(jQueryDataTableParamModel param, string parentFolderIdentifier, int folderType, string filterByAge, string filterBySearch, string selectedPatientList, string folderUrl)
        {
            var result = "";
            try
            {
                int patientListCount = 0;                
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortColumnOrder = Request["sSortDir_0"];   
                IList<Patient> patientList = _patientService.GetPatientItems(parentFolderIdentifier, folderType, sortColumnIndex, sortColumnOrder,
                                                                             GetLoginUserCourse() + "/" + GetLoginUserRole(), folderUrl);
               
                             
                IList<Patient> patientListToRender =
                    patientList.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                string[] strArray = AppCommon.GetStringArrayAfterSplitting(selectedPatientList);
                patientListCount = patientList.Count;
                var data = (from patientItem in patientListToRender
                            select new[]
                                   {                                       
                                       "<input type='checkbox' id='" + patientItem.UniqueIdentifier + "' onClick='patient.gridOperations.patientItemChanged(this)'" + AppCommon.CheckForFlagAndReturnValue(strArray, patientItem.UniqueIdentifier) + "/>",
                                       !string.IsNullOrEmpty(patientItem.FirstName) ? patientItem.FirstName : "",
                                       !string.IsNullOrEmpty(patientItem.LastName)
                                           ? "<a href='#' onclick=\"patient.pageOperations.loadPatientInEditMode('"+patientItem.Url+"')\" class=\"link select-hand\">" + patientItem.LastName +
                                             "</a>"
                                           : "",
                                       !string.IsNullOrEmpty(patientItem.Sex) ? ((patientItem.Sex == "Male")?"M":"F") : "",
                                       !string.IsNullOrEmpty(patientItem.DateOfBirth) ? patientItem.DateOfBirth : "",
                                       //!string.IsNullOrEmpty(patientItem.AgeInYears.ToString()) ? patientItem.AgeInYears.ToString() : "",
                                       patientItem.FormAgeText(),
                                       !string.IsNullOrEmpty(patientItem.CreatedTimeStamp.ToString("MM/dd/yyyy"))
                                           ? patientItem.CreatedTimeStamp.ToString("MM/dd/yyyy")
                                           : "",
                                       !string.IsNullOrEmpty(patientItem.Status) ? patientItem.Status : "",
                                   }).ToArray();
                var jsonData = Json(new
                                        {
                                            sEcho = param.sEcho,
                                            iTotalRecords = patientListCount,
                                            iTotalDisplayRecords = patientListCount,
                                            aaData = data
                                        },
                                    JsonRequestBehavior.AllowGet);
                return jsonData;
            }
            catch (Exception patientGrid)
            {
                result = AjaxCallResult(new AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType.Error, patientGrid.ToString(), ""));
                return Json(new { Result = result });
            }
        }

        public ActionResult GiveSearchResults(string strSearchText)
        {
            ViewBag.SearchText = strSearchText;
            return View("../Builder/Patient/PatientSearchResults");
        }

        /// <summary>
        /// To populate the patient search grid
        /// </summary>
        /// <param name="param"></param>
        /// <param name="strSearchText"></param>
        /// <returns></returns>
        public ActionResult GetPatientSearchList(jQueryDataTableParamModel param, string strSearchText)
        {

            try
            {
                int patientCount = 0;
                IList<PatientProxy> lstPatientSearchResult = new List<PatientProxy>();
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortColumnOrder = Request["sSortDir_0"];
                lstPatientSearchResult = _patientService.GetSearchResultsForPatient(strSearchText, sortColumnIndex, sortColumnOrder, GetLoginUserCourse(), GetLoginUserRole());               
                IList<PatientProxy> patientSearchListToRender = lstPatientSearchResult.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                patientCount = lstPatientSearchResult.Count;
                var data = (from patientItem in patientSearchListToRender
                            select new[]
                                   {
                                       !string.IsNullOrEmpty(patientItem.FirstName) ? patientItem.FirstName : "",
                                       !string.IsNullOrEmpty(patientItem.LastName)
                                           ? "<a href='#' onclick=\"patient.pageOperations.loadPatientInEditMode('"+patientItem.Url+"')\" class=\"link select-hand\">" + patientItem.LastName +
                                             "</a>"
                                           : "",
                                       !string.IsNullOrEmpty(patientItem.Sex) ? ((patientItem.Sex == "Male")?"M":"F") : "",
                                       !string.IsNullOrEmpty(patientItem.DateOfBirth) ? patientItem.DateOfBirth : "",
                                       !string.IsNullOrEmpty(patientItem.Age.ToString()) ? patientItem.Age.ToString() : "",
                                       !string.IsNullOrEmpty(patientItem.CreatedTimeStamp.ToString("MM/dd/yyyy"))
                                           ? patientItem.CreatedTimeStamp.ToString("MM/dd/yyyy")
                                           : "",
                                       !string.IsNullOrEmpty(patientItem.Status) ? patientItem.Status : "",
                                   }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = patientCount,
                    iTotalDisplayRecords = patientCount,
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
        /// To render patient in edit mode
        /// </summary>
        /// <param name="patientid">Url of patient to load</param>
        /// <returns></returns>
        public ActionResult RenderPatientInEditMode(string patientUrl)
        {

            try
            {
                Patient patientObject = new Patient();
                patientObject = _patientService.GetPatientForGuid(patientUrl);
                SetViewBagsForEditMode(patientObject);
                ViewBag.PatientUrl = patientUrl;
            }
            catch
            {
                //To-Do
            }
            ViewData["officeType"] = new SelectList(AppCommon.officeTypeOptions, "officeType");
            ViewData["provider"] = new SelectList(AppCommon.providerOptions, "provider");
            return View("../Builder/Patient/PatientProfileSetUp");
        }

        /// <summary>
        /// set values for patient object
        /// </summary>
        /// <param name="patientObject"></param>
        public void SetViewBagsForEditMode(Patient patientObject)
        {
            try
            {
                if (patientObject != null)
                {
                    ViewBag.FirstName = patientObject.FirstName;
                    ViewBag.LastName = patientObject.LastName;
                    ViewBag.MiddleInitial = patientObject.MiddleInitial;
                    ViewBag.AgeInYears = patientObject.AgeInYears.ToString();
                    ViewBag.AgeInMonths = patientObject.AgeInMonths.ToString();
                    ViewBag.AgeInDays = patientObject.AgeInDays.ToString();
                    ViewBag.DateOfBirth = patientObject.DateOfBirth;
                    ViewBag.MedicalRecordNumber = patientObject.MedicalRecordNumber;
                    ViewBag.OfficeTypetoload = patientObject.OfficeType;
                    ViewBag.Providertoload = patientObject.Provider;
                    ViewBag.UploadImage = patientObject.UploadImage;
                    ViewBag.Sex = patientObject.Sex;
                    ViewBag.CreatedTime = patientObject.CreatedTimeStamp.ToString();
                    ViewBag.IsEditMode = true;
                }

            }
            catch
            {
                //To-Do
            }
        }
    }
}