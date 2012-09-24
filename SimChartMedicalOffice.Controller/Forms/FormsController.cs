using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Logging;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Forms;
using SimChartMedicalOffice.Core.Json;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class FormsController : BaseController
    {
        private readonly IFormsService _formsService;

        public FormsController(IFormsService formsService)
        {
            _formsService = formsService;
        }

        public ActionResult PatientInformation(string patientInformationSelected)
        {
            ViewBag.patientInformationSelected = patientInformationSelected.Trim();
            return View();
        }

        public ActionResult PatientRecordsAccessForm(string patientRecordsAccessSelected)
        {
            //ViewBag.patientRecordsAccessSelected = patientRecordsAccessSelected.Trim();
            ViewBag.Title = "Patient Records Access Form";
            return View();
        }
        public ActionResult MedicalRecordsRelease(string medicalRecordsReleaseSelected)
        {
            //ViewBag.patientRecordsAccessSelected = patientRecordsAccessSelected.Trim();
            ViewBag.Title = "Medical Records Release Form";
            return View();
        }

        public ActionResult PriorAuthorizationRequestForm(string priorAuthorizationSelected)
        {
            ViewBag.priorAuthorizationSelected = priorAuthorizationSelected.Trim();
            ViewBag.Title = "Prior Authorization Request Form";
            return View();
        }

        public ActionResult HIPAANoticeForm(string HIPAANoticeFormSelected)
        {
            //ViewBag.HIPAANoticeFormSelected = HIPAANoticeFormSelected.Trim();
            ViewBag.Title = "HIPAA Notice of Privacy Practice";

            return View();
        }

        public ActionResult Referral(string pageSelected)
        {
            ViewBag.pageSelected = pageSelected.Trim();
            ViewBag.Title = "Medical Referral";
            return View();
        }

        public ActionResult PatientBillofRights(string patientbillofrightsSelected)
        {
            ViewBag.patientbillofrightsSelected = patientbillofrightsSelected.Trim();
            return View();
        }

        public ActionResult FormsRepository(int iReferenceOfFormToLoad, string formName, string patientName)
        {
            iReferenceOfFormToLoad = iReferenceOfFormToLoad == 0 ? 6 : iReferenceOfFormToLoad;
            if (iReferenceOfFormToLoad == Convert.ToInt32(AppEnum.FormsRepository.Confirmation))
            {
                @ViewBag.FormName = AppCommon.GetFormName(Convert.ToInt32(formName));
                @ViewBag.PatientName = patientName;
            }
            AppEnum.FormsRepository formsRepository = (AppEnum.FormsRepository)iReferenceOfFormToLoad;
            ViewBag.FormValue = (int)formsRepository;
            ViewBag.NoticeFormPdfid = AppConstants.NoticePrivacyPractice;
            ViewBag.BillOfRightsFormPdfid = AppConstants.BillofRights;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SavePriorAuthorizationRequestForm()
        {
            string result;
            JavaScriptSerializer js = new JavaScriptSerializer();

            PriorAuthorizationRequestForm priorAuthorizationRequestForm = new PriorAuthorizationRequestForm();
            try
            {
                //List<Patient> patient = new List<Patient>();
                //patient = _formsService.GetAllPatient().ToList();
                string priorAuthorizationRequestFormObjectJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                PriorAuthorizationRequestForm priorAuthorizationRequestFormObject = js.Deserialize<PriorAuthorizationRequestForm>(priorAuthorizationRequestFormObjectJson);
                Type priorAuthorizationRequestFormType = priorAuthorizationRequestFormObject.GetType();
                foreach (var objProp in priorAuthorizationRequestFormType.GetProperties())
                {
                    if (objProp.CanWrite && objProp.Name.ToUpper() != "ID")
                    {
                        objProp.SetValue(priorAuthorizationRequestForm, priorAuthorizationRequestFormType.GetProperty(objProp.Name).GetValue(priorAuthorizationRequestFormObject, null), null);
                    }
                }
                //priorAuthorizationRequestForm.PatientReferenceId = patient[0].UniqueIdentifier;
                priorAuthorizationRequestForm.CreatedTimeStamp = DateTime.Now;

                // to check if save/update based on whether UniqueIdentifier is set in View
                if (priorAuthorizationRequestForm.UniqueIdentifier == null)
                {
                    priorAuthorizationRequestForm.UniqueIdentifier = DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                // temporary fix for saving int uniqueIdentifier
                //priorAuthorizationRequestForm.UniqueIdentifier=AppendKeyToFormId(priorAuthorizationRequestForm.UniqueIdentifier);

                _formsService.SavePriorAuthorizationRequestForm(priorAuthorizationRequestForm, GetDropBoxFromCookie());
                result = "success";
            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("error: ControllerName: Forms, MethodName: SavePriorAuthorizationRequestForm", ex);
                //errorMessage = AppConstants.Error;
            }
            return Json(new { Result = result });

        }

        private string AppendKeyToFormId(string formId)
        {
            if (formId != "")
            {
                return formId + "_Key";
            }
            return formId;
        }

        private string RemoveKeyFromId(object formId)
        {
            if (formId != null)
            {
                return formId.ToString().Split('_')[0];
            }
            return "";
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LoadPatientInfo(string patientGUID)
        {
            Patient patientObj = null;
            string result;
            try
            {
                // should get patientInfo from Current PAtient assignment when Assignment Copy is implemtented
                //patientObj = _formsService.GetPatient(GetLoginUserCourse(),GetLoginUserRole(),GetLoginUserId(),GetLoginScenarioId(),patientGUID);

                // till Assignment Copy is implemtented, get PatientInfo from AssignmentRepository
                patientObj = _formsService.GetPatientFromAssignmentRepository(patientGUID,GetDropBoxFromCookie());
                // extra uniqueIdenitfier check as object is getting returned as new Patient() and not null when the patient is not there 
                if (!(patientObj != null && patientObj.UniqueIdentifier != null))
                {
                    patientObj = _formsService.GetPatientFromPatientRepository(patientGUID);
                }
                result = "Fetch successful";
            }
            catch (Exception ex)
            {
                result = "A problem was encountered preventing" + ex;
            }
            return Json(new { Result = result, PatientInfo = patientObj });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SavePatientRecordsAccessForm()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();

            PatientRecordsAccessForm patientRecordsAccessForm = new PatientRecordsAccessForm();
            try
            {

                //List<Patient> patinet = new List<Patient>();
                //patinet = _formsService.GetAllPatient().ToList();
                string patientRecordsAccessFormObjectJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                PatientRecordsAccessForm patientRecordsAccessFormObject = js.Deserialize<PatientRecordsAccessForm>(patientRecordsAccessFormObjectJson);
                patientRecordsAccessForm.Address = patientRecordsAccessFormObject.Address;
                patientRecordsAccessForm.Charge = patientRecordsAccessFormObject.Charge;
                patientRecordsAccessForm.CompletedBy = patientRecordsAccessFormObject.CompletedBy;
                patientRecordsAccessForm.DateCompleted = patientRecordsAccessFormObject.DateCompleted;
                patientRecordsAccessForm.EmergencyPatientCity = patientRecordsAccessFormObject.EmergencyPatientCity;
                patientRecordsAccessForm.EmergencyPatientDob = patientRecordsAccessFormObject.EmergencyPatientDob;
                patientRecordsAccessForm.EmergencyPatientName = patientRecordsAccessFormObject.EmergencyPatientName;
                patientRecordsAccessForm.EmergencyPatientState = patientRecordsAccessFormObject.EmergencyPatientState;
                patientRecordsAccessForm.EmergencyPatientZipCode = patientRecordsAccessFormObject.EmergencyPatientZipCode;
                patientRecordsAccessForm.EmergencyPatientAddress = patientRecordsAccessFormObject.EmergencyPatientAddress;
                patientRecordsAccessForm.MedicalRecordPeriodFrom = patientRecordsAccessFormObject.MedicalRecordPeriodFrom;
                patientRecordsAccessForm.MedicalRecordPeriodTo = patientRecordsAccessFormObject.MedicalRecordPeriodTo;
                //List<PatientMedicalRecordRequest> patientMedicalRecordRequestlist = new List<PatientMedicalRecordRequest>();
                //PatientMedicalRecordRequest patientMedicalRecord = new PatientMedicalRecordRequest();
                //foreach (PatientMedicalRecordRequest patientMedicalRecordRequest in patientRecordsAccessFormObject.PatientMedicalRecordRequest)
                //{
                //    patientMedicalRecord.Value = patientMedicalRecordRequest.Value;
                //    patientMedicalRecordRequestlist.Add(patientMedicalRecord);
                //}

                patientRecordsAccessForm.PatientMedicalRecordRequest = patientRecordsAccessFormObject.PatientMedicalRecordRequest;
                patientRecordsAccessForm.Phone = patientRecordsAccessFormObject.Phone;
                patientRecordsAccessForm.ReasonforDisclosure = patientRecordsAccessFormObject.ReasonforDisclosure;
                patientRecordsAccessForm.ReleasingTo = patientRecordsAccessFormObject.ReleasingTo;
                patientRecordsAccessForm.RequestExpiryDate = patientRecordsAccessFormObject.RequestExpiryDate;
                patientRecordsAccessForm.Signature = patientRecordsAccessFormObject.Signature;
                patientRecordsAccessForm.SignatureDate = patientRecordsAccessFormObject.SignatureDate;
                patientRecordsAccessForm.WitnessSignature = patientRecordsAccessFormObject.WitnessSignature;
                patientRecordsAccessForm.WitnessSignatureDate = patientRecordsAccessFormObject.WitnessSignatureDate;
                patientRecordsAccessForm.CreatedTimeStamp = DateTime.Now;

                // new code
                patientRecordsAccessForm.PatientReferenceId = patientRecordsAccessFormObject.PatientReferenceId;
                // to check if save/update based on whether UniqueIdentifier is set in View
                if (patientRecordsAccessFormObject.UniqueIdentifier != null)
                {
                    patientRecordsAccessForm.UniqueIdentifier = patientRecordsAccessFormObject.UniqueIdentifier;
                }
                else
                {
                    patientRecordsAccessForm.UniqueIdentifier = DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                // temporary fix for saving int uniqueIdentifier
                //patientRecordsAccessForm.UniqueIdentifier=AppendKeyToFormId(patientRecordsAccessForm.UniqueIdentifier);

                _formsService.SavePatientRecordsAccessForm(patientRecordsAccessForm, GetDropBoxFromCookie());
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: Controller: Forms, MethodName: SavePatientRecordsAccessForm", ex);
            }
            return Json(new { Result = "success" });

        }

        /// <summary>
        /// to save/update MedicalRecordsReleaseForm
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveMedicalRecordsReleaseForm()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();

            MedicalRecordsRelease medicalRecordsReleaseForm = new MedicalRecordsRelease();
            try
            {

                //List<Patient> patinet = new List<Patient>();
                //patinet = _formsService.GetAllPatient().ToList();
                string medicalRecordsReleaseFormObjectJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                MedicalRecordsRelease medicalRecordsReleaseFormObject = js.Deserialize<MedicalRecordsRelease>(medicalRecordsReleaseFormObjectJson);
                medicalRecordsReleaseForm.Address = medicalRecordsReleaseFormObject.Address;
                medicalRecordsReleaseForm.PatientName = medicalRecordsReleaseFormObject.PatientName;
                medicalRecordsReleaseForm.Ssn = medicalRecordsReleaseFormObject.Ssn;
                medicalRecordsReleaseForm.DateOfBirth = medicalRecordsReleaseFormObject.DateOfBirth;
                medicalRecordsReleaseForm.DiffName = medicalRecordsReleaseFormObject.DiffName;
                medicalRecordsReleaseForm.Authorize = medicalRecordsReleaseFormObject.Authorize;
                medicalRecordsReleaseForm.Date = medicalRecordsReleaseFormObject.Date;
                medicalRecordsReleaseForm.Name = medicalRecordsReleaseFormObject.Name;
                medicalRecordsReleaseForm.DiffAddress = medicalRecordsReleaseFormObject.DiffAddress;
                medicalRecordsReleaseForm.Other = medicalRecordsReleaseFormObject.Other;
                medicalRecordsReleaseForm.DiffOther = medicalRecordsReleaseFormObject.DiffOther;
                medicalRecordsReleaseForm.DiffPhone = medicalRecordsReleaseFormObject.DiffPhone;
                medicalRecordsReleaseForm.DoctorName = medicalRecordsReleaseForm.DoctorName;
                medicalRecordsReleaseForm.Fax = medicalRecordsReleaseFormObject.Fax;
                medicalRecordsReleaseForm.Phone = medicalRecordsReleaseFormObject.Phone;
                medicalRecordsReleaseForm.DiffDate = medicalRecordsReleaseFormObject.DiffDate;
                medicalRecordsReleaseForm.PrintedDate = medicalRecordsReleaseFormObject.PrintedDate;
                medicalRecordsReleaseForm.AuthoritySign = medicalRecordsReleaseFormObject.AuthoritySign;
                medicalRecordsReleaseForm.Signature = medicalRecordsReleaseFormObject.Signature;
                medicalRecordsReleaseForm.LaterThan = medicalRecordsReleaseFormObject.LaterThan;
                medicalRecordsReleaseForm.Event = medicalRecordsReleaseFormObject.Event;
                medicalRecordsReleaseForm.PrintedName = medicalRecordsReleaseForm.PrintedName;
               medicalRecordsReleaseForm.MedicalRecordsReleaseForm = medicalRecordsReleaseFormObject.MedicalRecordsReleaseForm;
               medicalRecordsReleaseForm.MedicalRecordsReleaseFormTwo = medicalRecordsReleaseFormObject.MedicalRecordsReleaseFormTwo;

                medicalRecordsReleaseForm.CreatedTimeStamp = DateTime.Now;

                // new code
                medicalRecordsReleaseForm.PatientReferenceId = medicalRecordsReleaseFormObject.PatientReferenceId;
                // to check if save/update based on whether UniqueIdentifier is set in View
                if (medicalRecordsReleaseFormObject.UniqueIdentifier != null)
                {
                    medicalRecordsReleaseForm.UniqueIdentifier = medicalRecordsReleaseFormObject.UniqueIdentifier;
                }
                else
                {
                    medicalRecordsReleaseForm.UniqueIdentifier = DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                // temporary fix for saving int uniqueIdentifier
                //patientRecordsAccessForm.UniqueIdentifier=AppendKeyToFormId(patientRecordsAccessForm.UniqueIdentifier);

                _formsService.SaveMedicalRecordsReleaseForm(medicalRecordsReleaseForm, GetDropBoxFromCookie());
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: Controller: Forms, MethodName: SaveMedicalRecordsReleaseForm", ex);
            }
            return Json(new { Result = "success" });

        }



        /// <summary>
        /// To save/update a Notice of Privacy Practice Form
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveNoticeOfPrivacyPractice()
        {
            string result;
            try
            {
                NoticeOfPrivacyPractice noticeOfPrivacyPracticeForm = _formsService.GetNoticeOfPrivacyPracticeDocument("b326d8a2-e2cf-438d-8df8-742090611f0d", GetDropBoxFromCookie());
                if (noticeOfPrivacyPracticeForm == null)
                {
                    noticeOfPrivacyPracticeForm = new NoticeOfPrivacyPractice();
                }
                string noticeOfPrivacyPracticeObjectJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                NoticeOfPrivacyPractice noticeOfPrivacyPracticeObject = JsonSerializer.DeserializeObject<NoticeOfPrivacyPractice>(noticeOfPrivacyPracticeObjectJson);
                noticeOfPrivacyPracticeForm.FormName = noticeOfPrivacyPracticeObject.FormName;
                noticeOfPrivacyPracticeForm.PatientReferenceId = noticeOfPrivacyPracticeObject.PatientReferenceId;
                SetAuditFields(noticeOfPrivacyPracticeForm, false);
                _formsService.SaveNoticeOfPrivacyPractice(noticeOfPrivacyPracticeForm, "b326d8a2-e2cf-438d-8df8-742090611f0d", GetDropBoxFromCookie());
                result = "Notice Of Privacy Practice Form is Saved in Patient Record!";
            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
            }
            return Json(new { Success = result });
        }

        /// <summary>
        /// To save/update a Patient Bill of Rights Form
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SavePatientBillofRights()
        {
            string result;
            try
            {
                BillOfRights patientBillofRightsForm = _formsService.GetBillOfRightsDocument("b326d8a2-e2cf-438d-8df8-742090611f0d", GetDropBoxFromCookie());
                if (patientBillofRightsForm == null)
                {
                    patientBillofRightsForm = new BillOfRights();
                }
                string patientBillofRightsJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
                BillOfRights patientBillofRightsObject = JsonSerializer.DeserializeObject<BillOfRights>(patientBillofRightsJson);
                patientBillofRightsForm.FormName = patientBillofRightsObject.FormName;
                patientBillofRightsForm.PatientReferenceId = patientBillofRightsObject.PatientReferenceId;
                SetAuditFields(patientBillofRightsForm, false);
                _formsService.SavePatientBillOfRights(patientBillofRightsForm, "b326d8a2-e2cf-438d-8df8-742090611f0d", GetDropBoxFromCookie());
                result = "Patient Bill of Rights Form is Saved in Patient Record!";
            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
            }
            return Json(new { Success = result });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LoadPatientRecordsAccessForm(string patientGUID)
        {
            PatientRecordsAccessForm patientRecordsAccessFormObj = null;
            string result;
            try
            {
                IList<PatientRecordsAccessForm> patientRecordsAccessFormsAll = _formsService.GetAllPatientRecordsAccessFormsForPatient(GetDropBoxFromCookie(), patientGUID, "");
                if (patientRecordsAccessFormsAll.Count > 0)
                {
                    patientRecordsAccessFormObj = patientRecordsAccessFormsAll[0];
                    patientRecordsAccessFormObj.UniqueIdentifier = RemoveKeyFromId(patientRecordsAccessFormObj.UniqueIdentifier);
                    result = "Fetch successful";
                }
                else
                {
                    result = AppConstants.FormNotFound;
                }
            }
            catch (Exception ex)
            {
                result = "A problem was encountered preventing" + ex;
            }
            return Json((new { Result = result, PatientRecordsAccess = patientRecordsAccessFormObj }));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LoadPriorAuthorizationRequestForm(string patientGUID)
        {
            PriorAuthorizationRequestForm priorAuthorizationRequestFormObj = null;
            string result;
            try
            {
                IList<PriorAuthorizationRequestForm> priorAuthorizationRequestFormsAll = _formsService.GetAllPriorAuthorizationRequestFormsForPatient(GetDropBoxFromCookie(), patientGUID, "");
                if (priorAuthorizationRequestFormsAll.Count > 0)
                {
                    priorAuthorizationRequestFormObj = priorAuthorizationRequestFormsAll[0];

                    // temporary fix for Saved int form uniqueIdentifier with _Key
                    priorAuthorizationRequestFormObj.UniqueIdentifier =
                        RemoveKeyFromId(priorAuthorizationRequestFormObj.UniqueIdentifier);
                    result = "Fetch successful";
                }
                else
                {
                    result = AppConstants.FormNotFound;
                }
            }
            catch (Exception ex)
            {
                result = "A problem was encountered preventing" + ex;
            }
            return Json((new { Result = result, PriorAuthorizationRequest = priorAuthorizationRequestFormObj }));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LoadMedicalRecordsReleaseForm(string patientGUID)
        {
            MedicalRecordsRelease medicalRecordsReleaseFormObj = null;
            string result;
            try
            {
                IList<MedicalRecordsRelease> medicalRecordsReleaseFormsAll = _formsService.GetAllMedicalRecordsReleaseFormsForPatient(GetDropBoxFromCookie(), patientGUID, "");
                if (medicalRecordsReleaseFormsAll.Count > 0)
                {
                    medicalRecordsReleaseFormObj = medicalRecordsReleaseFormsAll[0];

                    // temporary fix for Saved int form uniqueIdentifier with _Key
                    medicalRecordsReleaseFormObj.UniqueIdentifier =
                        RemoveKeyFromId(medicalRecordsReleaseFormObj.UniqueIdentifier);
                    result = "Fetch successful";
                }
                else
                {
                    result = AppConstants.FormNotFound;
                }
            }
            catch (Exception ex)
            {
                result = "A problem was encountered preventing" + ex;
            }
            return Json((new { Result = result, MedicalRecordsRelease = medicalRecordsReleaseFormObj }));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LoadReferralForm(string patientGUID)
        {
            ReferralForm referralFormObj = null;
            string result;
            try
            {
                IList<ReferralForm> referralFormsAll = _formsService.GetAllReferralFormsForPatient(GetDropBoxFromCookie(), patientGUID, "");
                if (referralFormsAll.Count > 0)
                {
                    referralFormObj = referralFormsAll[0];
                    // temporary fix for Saved int form uniqueIdentifier with _Key
                    referralFormObj.UniqueIdentifier = RemoveKeyFromId(referralFormObj.UniqueIdentifier);
                    result = "Fetch successful";
                }
                else
                {
                    result = AppConstants.FormNotFound;
                }
            }
            catch (Exception ex)
            {
                result = "A problem was encountered preventing" + ex;
            }
            return Json((new { Result = result, ReferralFormObj = referralFormObj }));
        }


        //public ActionResult DeletePatientRecordAccessForm()
        //{
        //    List<PatientRecordsAccessForm> patientRecordsAccessFormlst;
        //    patientRecordsAccessFormlst = _formsService.GetAllPatientRecordsAccessForm().ToList();
        //    _formsService.DeletePatientRecordsAccessForm(patientRecordsAccessFormlst[0].UniqueIdentifier);
        //    return Json(new { Result = "success" });
        //}


        //public JsonResult GetAllPriorAuthorizationRequest()
        //{
        //    return Json(new { Result = JsonSerializer.SerializeObject(_formsService.GetAllPriorAuthorizationRequest()) });
        //}


        public ActionResult GetPatientListForGrid(JQueryDataTableParamModel param, string MRNNumber, string firstName, string lastName, string DOB)
        {
            IList<Patient> patientListForGrid = _formsService.GetAllPatient(GetDropBoxFromCookie());
            patientListForGrid = (from patient in patientListForGrid
                                  where
                                      (patient.FirstName != null ? patient.FirstName.ToLower().Contains(firstName.ToLower()) : (firstName == "")) && (patient.LastName != null ? patient.LastName.ToLower().Contains(lastName.ToLower()) : (lastName == "")) && (patient.MedicalRecordNumber != null ? patient.MedicalRecordNumber.ToLower().Contains(MRNNumber.ToLower()) : (MRNNumber == "")) && (patient.DateOfBirth != null ? patient.DateOfBirth.ToLower().Contains(DOB.ToLower()) : (DOB == ""))
                                  select patient).ToList();

            string[] gridColumnList = { "FirstName", "LastName", "Sex", "DOB" };
            int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnOrder = Request["sSortDir_0"];
            int startIndex = param.iDisplayStart;
            int lengthIndex = param.iDisplayLength;

            switch (gridColumnList[sortColumnIndex - 1])
            {
                case "FirstName":
                    patientListForGrid = sortColumnOrder == "asc" ? patientListForGrid.OrderBy(x => x.FirstName).ToList() : patientListForGrid.OrderByDescending(x => x.FirstName).ToList();
                    break;
                case "LastName":
                    patientListForGrid = sortColumnOrder == "asc" ? patientListForGrid.OrderBy(x => x.LastName).ToList() : patientListForGrid.OrderByDescending(x => x.LastName).ToList();
                    break;
                case "Sex":
                    patientListForGrid = sortColumnOrder == "asc" ? patientListForGrid.OrderBy(x => x.Sex).ToList() : patientListForGrid.OrderByDescending(x => x.Sex).ToList();
                    break;
                case "DOB":
                    patientListForGrid = sortColumnOrder == "asc"
                                             ? patientListForGrid.OrderBy(x => Convert.ToDateTime(x.DateOfBirth)).ToList
                                                   ()
                                             : patientListForGrid.OrderByDescending(
                                                 x => Convert.ToDateTime(x.DateOfBirth)).ToList();
                    break;
                default:
                    var sortableList = patientListForGrid.AsQueryable();
                    patientListForGrid = sortableList.OrderBy(x => x.LastName).ToList();
                    break;

            }
            IList<Patient> indexedPatientListForGrid = patientListForGrid.Skip(startIndex).Take(lengthIndex).ToList();
            var data = (from patient in indexedPatientListForGrid
                        select new[]
                                   {
                                       "<input type='radio' name='selectPatient' id='"+patient.UniqueIdentifier+"'/>",
                                       !string.IsNullOrEmpty(patient.FirstName) ? patient.FirstName : "",
                                       !string.IsNullOrEmpty(patient.LastName) ? patient.LastName : "",
                                       !string.IsNullOrEmpty(patient.Sex) ? (patient.Sex.ToLower()=="male" ? "M":"F") : "",
                                       !string.IsNullOrEmpty(patient.DateOfBirth) ? patient.DateOfBirth : "",
                                       !string.IsNullOrEmpty(patient.MedicalRecordNumber) ? patient.MedicalRecordNumber : "",
                                       !string.IsNullOrEmpty(patient.Phone) ? patient.Phone : "",
                                       !string.IsNullOrEmpty(patient.Address) ? patient.Address : ""
                                   }).ToArray();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = patientListForGrid.Count,
                iTotalDisplayRecords = patientListForGrid.Count,
                aaData = data
            },
            JsonRequestBehavior.AllowGet);
        }

        public ActionResult FormsPatientSearch()
        {
            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveReferralForm()
        {
            ReferralForm referralFormOjectToSave = new ReferralForm();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string referralFormJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
            ReferralForm referralFormOjectFromPage = js.Deserialize<ReferralForm>(referralFormJson);
            Type priorAuthorizationRequestFormType = referralFormOjectFromPage.GetType();
            foreach (var objProp in priorAuthorizationRequestFormType.GetProperties())
            {
                if (objProp.CanWrite && objProp.Name.ToUpper() != "ID")
                {
                    objProp.SetValue(referralFormOjectToSave, priorAuthorizationRequestFormType.GetProperty(objProp.Name).GetValue(referralFormOjectFromPage, null), null);
                }
            }
            referralFormOjectToSave.CreatedTimeStamp = DateTime.Now;
            // to check if save/update based on whether UniqueIdentifier is set in View
            if (referralFormOjectToSave.UniqueIdentifier == null)
            {
                referralFormOjectToSave.UniqueIdentifier = DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            // temporary fix for saving int uniqueIdentifier
            //referralFormOjectToSave.UniqueIdentifier=AppendKeyToFormId(referralFormOjectToSave.UniqueIdentifier);

            _formsService.SaveReferralForm(referralFormOjectToSave,GetDropBoxFromCookie());
            return Json(new { Result = "success" });
        }
        //public ActionResult DeletePriorAuthorizationRequestForm()
        //{
        //    List<PriorAuthorizationRequestForm> priorAuthorizationRequestFormlst;
        //    priorAuthorizationRequestFormlst = _formsService.GetAllPriorAuthorizationRequest().ToList();
        //    _formsService.DeletePriorAuthorizationRequestForm(priorAuthorizationRequestFormlst[0].UniqueIdentifier);
        //    return Json(new { Result = "success" });
        //}

        /// <summary>
        /// to get the paitentRecordsRequest saved object from db and send it to print page
        /// </summary>
        /// <param name="patientGUID"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        public ActionResult FilledPatientRecordsAccessFormPrint(string patientGUID, string formId)
        {
            //string patientRecordsAccessFormPrintJson="";
            PatientRecordsAccessForm patientRecordsFormObject;
            try
            {
                patientRecordsFormObject = _formsService.GetPatientRecordsAccessForm(GetDropBoxFromCookie(), patientGUID, formId);
            }
            catch
            {
                patientRecordsFormObject = null;
            }
            ViewBag.Signature = GetLoginUserId();
            ViewBag.patientRecordsAccess = patientRecordsFormObject;
            return View("../../Views/Forms/PatientRecordsAccessFormPrint");
        }

        /// <summary>
        /// to open the print page wihtout sending any object
        /// </summary>
        /// <returns></returns>
        public ActionResult EmptyPatientRecordsAccessFormPrint()
        {
            ViewBag.Signature = GetLoginUserId();
            return View("../../Views/Forms/PatientRecordsAccessFormPrint");
        }

        /// <summary>
        /// to open the print page wihtout sending any object
        /// </summary>
        /// <returns></returns>
        public ActionResult EmptyPriorAuthorizationRequestPrint()
        {
            ViewBag.Signature = GetLoginUserId();
            return View("../../Views/Forms/PriorAuthorizationRequestFormPrint");
        }

        /// <summary>
        /// to get the paitentRecordsRequest saved object from db and send it to print page
        /// </summary>
        /// <param name="patientGUID"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        public ActionResult FilledPriorAuthorizationRequestPrint(string patientGUID, string formId)
        {
            //string patientRecordsAccessFormPrintJson="";
            PriorAuthorizationRequestForm priorAuthReqObject;
            try
            {
                priorAuthReqObject = _formsService.GetPriorAuthorizationRequestForm(GetDropBoxFromCookie(), patientGUID, formId);
            }
            catch
            {
                priorAuthReqObject = null;
            }
            ViewBag.Signature = GetLoginUserId();
            ViewBag.PriorAuthorityRequestObj = priorAuthReqObject;
            return View("../../Views/Forms/PriorAuthorizationRequestFormPrint");
        }
        /// <summary>
        /// to open the print page wihtout sending any object
        /// </summary>
        /// <returns></returns>
        public ActionResult EmptyMedicalRecordsReleasePrint()
        {
            ViewBag.Signature = GetLoginUserId();
            return View("../../Views/Forms/MedicalRecordsReleaseFormPrint");
        }

        /// <summary>
        /// to get the paitentRecordsRequest saved object from db and send it to print page
        /// </summary>
        /// <param name="patientGUID"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        public ActionResult FilledMedicalRecordsReleasePrint(string patientGUID, string formId)
        {
            //string patientRecordsAccessFormPrintJson="";
            MedicalRecordsRelease medicalRecordsReleaseObject;
            try
            {
                medicalRecordsReleaseObject = _formsService.GetMedicalRecordsReleaseForm(GetDropBoxFromCookie(), patientGUID, formId);
            }
            catch
            {
                medicalRecordsReleaseObject = null;
            }
            ViewBag.Signature = GetLoginUserId();
            ViewBag.MedicalRecordsReleaseFormObj = medicalRecordsReleaseObject;
            return View("../../Views/Forms/MedicalRecordsReleaseFormPrint");
        }

      

        /// <summary>
        /// to open the print page wihtout sending any object
        /// </summary>
        /// <returns></returns>
        public ActionResult EmptyReferralFormPrint()
        {
            ViewBag.Signature = GetLoginUserId();
            return View("../../Views/Forms/ReferralPrint");
        }


        /// <summary>
        /// to get the paitentRecordsRequest saved object from db and send it to print page
        /// </summary>
        /// <param name="patientGUID"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        public ActionResult FilledReferralFormPrint(string patientGUID, string formId)
        {
            //string patientRecordsAccessFormPrintJson="";
            ReferralForm referralFormObject;
            try
            {
                referralFormObject = _formsService.GetReferralForm(GetDropBoxFromCookie(), patientGUID, formId);
            }
            catch
            {
                referralFormObject = null;
            }
            ViewBag.Signature = GetLoginUserId();
            ViewBag.ReferralFormObj = referralFormObject;
            return View("../../Views/Forms/ReferralPrint");
        }

        ///// <summary>
        ///// To load the uploaded file from firebase
        ///// </summary>
        ///// <param name="strGuId"></param>
        ///// <param name="nocache"></param>
        //public void GetPdfAttachment(string strGuId, string nocache)
        //{
        //    if (!String.IsNullOrEmpty(strGuId))
        //    {
        //        Attachment attachment = _formsService.GetPdfAttachment(strGuId);
        //        var response = HttpContext.Response;
        //        response.Clear();
        //        response.Cache.SetCacheability(HttpCacheability.NoCache);
        //        response.ContentType = attachment.FileType;
        //        var stream = new MemoryStream(attachment.FileContent);
        //        stream.WriteTo(response.OutputStream);
        //    }
        //}

        /// <summary>
        /// To load the uploaded file from firebase
        /// </summary>
        /// <param name="strFormName"></param>
        /// <param name="nocache"></param>
        public void GetPdfAttachment(string strFormName, string nocache)
        {
            Attachment attachment = new Attachment();
            if (!String.IsNullOrEmpty(strFormName))
            {
                if (strFormName == AppConstants.BillofRights)
                {
                    attachment = _formsService.GetBillOfRightsPdfData();
                }
                else if (strFormName.Equals(AppConstants.NoticePrivacyPractice))
                {
                    attachment = _formsService.GetNoticeOfPrivacyPracticePdfData();
                }
                var response = HttpContext.Response;
                response.Clear();
                response.Cache.SetCacheability(HttpCacheability.NoCache);
                response.ContentType = attachment.FileType;
                var stream = new MemoryStream(attachment.FileContent);
                stream.WriteTo(response.OutputStream);
            }
        }
    }
}
