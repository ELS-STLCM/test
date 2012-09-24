using System;
using System.Collections.Generic;
using System.Linq;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.FrontOffice;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Common;
using System.Web.Mvc;
using SimChartMedicalOffice.Core.ProxyObjects;
using System.Web;
using SimChartMedicalOffice.Core.Patient;
//using System.Web.Mvc;
using SimChartMedicalOffice.Core;
using System.IO;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface;
using SimChartMedicalOffice.Core.Json;
using SimChartMedicalOffice.Core.DropBox;
using SimChartMedicalOffice.Common.Logging;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class AppointmentController : BaseController
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IMasterService _masterService;
        public AppointmentController(IAppointmentService appointmentService, IMasterService masterService)
        {
            _appointmentService = appointmentService;
            _masterService = masterService;
        }
        public void SavePatientVisit()
        {
            PatientVisit appointment = GetSamplePatientVisitData();
            //_appointmentService.SavePatientVisitAppointment(appointment, "CourseId2", AppEnum.ApplicationRole.Student, "123");
        }
        private PatientVisit GetSamplePatientVisitData()
        {
            PatientVisit visit = new PatientVisit
                                     {
                                         StartDateTime = DateTime.Now,
                                         EndDateTime = DateTime.Now.AddMinutes(30),
                                         Description = "Description",
                                         Type = "PatientVisit",
                                         Recurrence =
                                             new RecurrenceGroup
                                                 {NumberOfOccurences = 4, Pattern = AppEnum.RecurrencePattern.Daily}
                                     };
            return visit;
        }

        /// <summary>
        /// This method is used to save patient in assignment for a New Appointment
        /// </summary>
        /// <param name="assignmentUniqueIdentifier"></param>
        /// <returns></returns>
        public ActionResult SaveNewPatientForAppointment(string assignmentUniqueIdentifier)
        {
            try
            {
                Patient patient = DeSerialize<Patient>();
                patient.MedicalRecordNumber = AppCommon.GenerateRandomNumber().ToString();
                bool patientValid = _appointmentService.IsPatientValid(GetDropBoxFromCookie(), patient);
                if (patientValid)
                {
                    patient = _appointmentService.SaveNewAppointmentPatient(assignmentUniqueIdentifier, patient,GetDropBoxFromCookie());                    
                }
                IList<Patient> apponitmentPatients = _appointmentService.GetAppointmentPatientList(GetDropBoxFromCookie());                    
                var appointmentPatientList = apponitmentPatients.Select(pat => new { id = pat.UniqueIdentifier, name = pat.LastName + ", " + pat.FirstName + " " + pat.MiddleInitial }).ToList();
                return Json(new { PatientPresent = patientValid, Result = patient, SearchPatientList = appointmentPatientList });
            }
            catch (Exception ex)
            {
                string result = AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                return Json(new { Result = result });
            }
        }

        /// <summary>
        /// Load the appoinment in both create new and edit mode
        /// </summary>
        /// <param name="appointmentDate"></param>
        /// <param name="appointmentUniqueIdentifierUrl"></param>
        /// <param name="appointmentType"> </param>
        /// <param name="occurenceType"> </param>
        /// <returns></returns>
        public ActionResult LoadAppointment(string appointmentDate, string appointmentUniqueIdentifierUrl, string appointmentType, string occurenceType)
        {
            try
            {
                if (string.IsNullOrEmpty(appointmentUniqueIdentifierUrl))
                {
                    SetMasterForAllTypes();
                    LoadPatientVisitAppointmentMaster();
                    LoadBlockAppointmentMaster();
                    LoadOtherAppointmentMaster();
                }
                AppEnum.EditStatus occuranceEdit = !AppCommon.CheckIfStringIsEmptyOrNull(occurenceType) ? (AppEnum.EditStatus)Enum.Parse(typeof(AppEnum.EditStatus), occurenceType) : AppEnum.EditStatus.None;
                
                if (!string.IsNullOrEmpty(appointmentUniqueIdentifierUrl))
                { //edit mode
                    if (appointmentType == AppEnum.AppointmentTypes.PatientVisit.ToString())
                    {
                        SetMasterForAllTypes();
                        LoadPatientVisitAppointmentMaster();
                        PatientVisit editAppointment = _appointmentService.GetPatientVisitAppointment(appointmentUniqueIdentifierUrl, GetDropBoxFromCookie());
                        if (occuranceEdit == AppEnum.EditStatus.All && editAppointment.IsRecurrence)
                        {
                            editAppointment.StartDateTime = editAppointment.Recurrence.StartDateTime;
                            editAppointment.EndDateTime = editAppointment.Recurrence.EndDateTime;
                        }
                        ViewBag.EditAppointment = editAppointment;
                        SetViewBagsForPatientVisitAppointment(editAppointment);
                    }
                    else if (appointmentType == AppEnum.AppointmentTypes.Block.ToString())
                    {
                        SetMasterForAllTypes();
                        LoadBlockAppointmentMaster();
                        BlockAppointment editAppointment = _appointmentService.GetBlockAppointment(appointmentUniqueIdentifierUrl,GetDropBoxFromCookie());

                        if (occuranceEdit == AppEnum.EditStatus.All && editAppointment.IsRecurrence)
                        {
                            editAppointment.StartDateTime = editAppointment.Recurrence.StartDateTime;
                            editAppointment.EndDateTime = editAppointment.Recurrence.EndDateTime;
                        }
                        ViewBag.EditAppointment = editAppointment;
                        SetViewBagsForBlockAppointment(editAppointment);
                    }
                    else if(appointmentType == AppEnum.AppointmentTypes.Other.ToString())
                    {
                        SetMasterForAllTypes();
                        LoadOtherAppointmentMaster();
                        OtherAppointment editAppointment =
                            _appointmentService.GetOtherAppointment(appointmentUniqueIdentifierUrl,
                                                                    GetDropBoxFromCookie());
                        if(occuranceEdit == AppEnum.EditStatus.All && editAppointment.IsRecurrence)
                        {
                            editAppointment.StartDateTime = editAppointment.Recurrence.StartDateTime;
                            editAppointment.EndDateTime = editAppointment.Recurrence.EndDateTime;
                        }
                        ViewBag.EditAppointment = editAppointment;
                        SetViewBagsForOtherAppointment(editAppointment);
                    }
                }
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
            }
            return View("../../Views/SimOfficeCalendar/_LoadAppointment");
        }

        public ActionResult GetPatientVisitAppointment(string appointmentUniqueIdentifierUrl)
        {
            PatientVisit editAppointment = _appointmentService.GetPatientVisitAppointment(appointmentUniqueIdentifierUrl, GetDropBoxFromCookie());
            return Json(new { PatientVisitPresent = editAppointment, AppointmentDate = editAppointment.StartDateTime.ToString("MM/dd/yyyy") });
        }


        /// <summary>
        /// Loading the Cancel Popup with default values from the sender object
        /// </summary>
        /// <param name="cancelValue"></param>
        /// <returns></returns>
        public ActionResult DeleteCancelAppointment(string cancelValue)
        {
            if (cancelValue != "Edit" || cancelValue != "Delete")
            {
                ViewData["CancelValue"] = cancelValue;
            }
            else
            {
                int cancelVal = int.Parse(cancelValue);
                AppEnum.AppointmentStatus appointmentStatus = (AppEnum.AppointmentStatus)cancelVal;
                if (appointmentStatus == AppEnum.AppointmentStatus.Canceled)
                {
                    ViewData["CancelValue"] = "Cancel";
                }
                else
                {
                    ViewData["CancelValue"] = "";
                }
            }
            return View("../../Views/SimOfficeCalendar/_CancelAppointment");
        }


        private void LoadPatientVisitAppointmentMaster()
        {
            ViewData["VisitType"] = new SelectList(_masterService.GetAppointmentVisitType(), "VisitType");
            Dictionary<int, string> patientProviderValues = _masterService.GetPatientProviderValues();
            var patientProviderList = (from item in patientProviderValues select new { Id = item.Key, Name = item.Value }).ToList();
            ViewData["ProviderList"] = new SelectList(patientProviderList, "Id", "Name", 2);
            //ViewData["ProviderList"] = new SelectList(_masterService.GetPatientProviderValues(), "ProviderList");
            ViewData["ExamRoom"] = new SelectList(_masterService.GetExamRooms(), "ExamRoom");
            Dictionary<int, string> appointmentStatus = AppCommon.AppointmentStatus;
            var appointmentStatusList = (from item in appointmentStatus select new { Id = item.Key, Name = item.Value }).ToList();
            ViewData["AppointmentStatus"] = new SelectList(appointmentStatusList, "Id", "Name");
            Dictionary<int, string> statusLocation = AppCommon.StatusLocation;
            var statusLocationList = (from item in statusLocation select new { Id = item.Key, Name = item.Value }).ToList();
            ViewData["StatusLocationList"] = new SelectList(statusLocationList, "Id", "Name");
        }

        private void LoadBlockAppointmentMaster()
        {
            ViewData["BlockType"] = new SelectList(_masterService.GetBlockType(), "BlockType");
            Dictionary<int, string> patientProviderValues = _masterService.GetPatientProviderValuesBlock();
            //patientProviderValues.Add(patientProviderValues.Count+1,"All Staff");
            //List<KeyValuePair<int, string>> providerSortedList = patientProviderValues.ToList();
            //providerSortedList.Sort(
            //    delegate(KeyValuePair<int, string> firstPair,
            //    KeyValuePair<int, string> nextPair)
            //        {
            //            if (!nextPair.Value.Equals(AppConstants.Select_DropDown))
            //            {
            //                return firstPair.Value.CompareTo(nextPair.Value);
            //            }
            //            else
            //            {
            //                return 0;
            //            }
            //        }

            //    );
            var patientProviderList = (from item in patientProviderValues select new { Id = item.Key, Name = item.Value }).ToList();
            ViewData["BlockFor"] = new SelectList(patientProviderList, "Id", "Name", 1);
            //blockfor.Insert(1, "All Staff");
            List<string> strLocation = _masterService.GetExamRooms();
            strLocation.Insert(strLocation.Count, "Meeting Room");
            ViewData["BlockLocation"] = new SelectList(strLocation, "BlockLocation");
        }

        private void LoadOtherAppointmentMaster()
        {
            Dictionary<int, string> patientAttendeesValues = _masterService.GetPatientProviderValuesBlock();
            var patientAttendeesList = (from item in patientAttendeesValues select new { Id = item.Key, Name = item.Value }).ToList();
            ViewData["otherAttendees"] = new SelectList(patientAttendeesList, "Id", "Name", 1);
            List<string> strLocation = _masterService.GetExamRooms();
            strLocation.Insert(strLocation.Count, "Meeting Room");
            ViewData["LocationOther"] = new SelectList(strLocation, "LocationOther");
            ViewData["OtherType"] = new SelectList(_masterService.GetOtherType(),"OtherType");
        }

        public void SetViewBagsForPatientVisitAppointment(PatientVisit appointment)
        {
            try
            {
                if (appointment != null)
                {
                    ViewBag.PatientName = appointment.LastName + ", " + appointment.FirstName + " " +
                                          appointment.MiddleInitial;
                    ViewBag.EditVisitType = appointment.Type;
                    if (appointment.ProviderId !=null && appointment.ProviderId.Count > 0)
                    {
                     ViewBag.Provider = appointment.ProviderId[0];
                    }
                    ViewBag.EditExamRoom = appointment.ExamRoomIdentifier;
                    ViewBag.AppointmentDate = appointment.StartDateTime.ToString("MM/dd/yyyy");
                    ViewBag.EditStartTime = appointment.StartDateTime.ToString("hh:mm tt");
                    ViewBag.EditEndTime = appointment.EndDateTime.ToString("hh:mm tt");
                    ViewBag.EditDescription = appointment.Description;
                    ViewBag.AppointmentType = AppEnum.AppointmentTypes.PatientVisit.ToString();
                    ViewBag.AppointmentObject = appointment;
                    ViewBag.AppointmentURL = appointment.Url;
                    ViewBag.AppointmentDuration = appointment.Duration;
                    if (appointment.IsRecurrence)
                    {
                        List<string> visitTypeList = _masterService.GetAppointmentVisitType();
                        if (visitTypeList.Contains("New Patient Visit"))
                        {
                          visitTypeList.Remove("New Patient Visit");
                        }
                        ViewData["VisitType"] = new SelectList(visitTypeList, "VisitType");
                    }
                } 
                
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: Controller: Appointment, MethodName: SetViewBagsForPatientVisitAppointment", ex);
            }
        }

        public void SetViewBagsForBlockAppointment(BlockAppointment appointment)
        {
            try
            {
                if (appointment != null)
                {
                    ViewBag.PatientName = appointment.LastName + ", " + appointment.FirstName + " " +
                                          appointment.MiddleInitial;
                    ViewBag.EditBlockType = appointment.Type;
                    if (appointment.IsAllStaffSelected)
                    {
                        ViewBag.For = AppCommon.AllStaffId;
                    }
                    else
                    {
                        if (appointment.ProviderId != null && appointment.ProviderId.Count > 0)
                        {
                         ViewBag.For = appointment.ProviderId[0];
                        }
                    }
                    ViewBag.Location = appointment.ExamRoomIdentifier;
                    ViewBag.AppointmentDate = appointment.StartDateTime.ToString("MM/dd/yyyy");
                    ViewBag.EditStartTime = appointment.StartDateTime.ToString("hh:mm tt");
                    ViewBag.EditEndTime = appointment.EndDateTime.ToString("hh:mm tt");
                    ViewBag.EditDescription = appointment.Description;
                    ViewBag.AppointmentType = AppEnum.AppointmentTypes.Block.ToString();
                    ViewBag.AppointmentObject = appointment;
                    ViewBag.AppointmentURL = appointment.Url;
                    ViewBag.OtherText = appointment.OtherText;
                    ViewBag.IsRecurrence = appointment.IsRecurrence;
                    ViewBag.AppointmentDuration = appointment.Duration;
                }
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: Controller: Appointment, MethodName: SetViewBagsForBlockAppointment", ex);
            }
        }

        public void SetViewBagsForOtherAppointment(OtherAppointment appointment)
        {
            try
            {
                if (appointment != null)
                {
                    ViewBag.PatientName = appointment.LastName + ", " + appointment.FirstName + " " +
                                          appointment.MiddleInitial;
                    ViewBag.EditOtherType = appointment.Type;
                    ViewBag.ProviderListOther = appointment.ProviderId;
                    ViewBag.Location = appointment.ExamRoomIdentifier;
                    ViewBag.AppointmentDate = appointment.StartDateTime.ToString("MM/dd/yyyy");
                    ViewBag.EditStartTime = appointment.StartDateTime.ToString("hh:mm tt");
                    ViewBag.EditEndTime = appointment.EndDateTime.ToString("hh:mm tt");
                    ViewBag.EditDescription = appointment.Description;
                    ViewBag.AppointmentType = AppEnum.AppointmentTypes.Other.ToString();
                    ViewBag.AppointmentObject = appointment;
                    ViewBag.AppointmentURL = appointment.Url;
                    ViewBag.OtherText = appointment.OtherText;
                    ViewBag.IsRecurrence = appointment.IsRecurrence;
                    ViewBag.IsAllStaffSelected = appointment.IsAllStaffSelected;
                }
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Error: Controller: Appointment, MethodName: SetViewBagsForOtherAppointment", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appointmentUrl"></param>
        /// <param name="recurrenceStatus"></param>
        /// <param name="appointmentType"></param>
        /// <returns></returns>
        public ActionResult DeleteAppointment(string appointmentUrl, AppEnum.EditStatus recurrenceStatus, string appointmentType)
        {
            string result;
            try
            {
                Appointment appointmentToDelete = null;
                if (appointmentType == AppEnum.AppointmentTypes.PatientVisit.ToString())
                {
                    appointmentToDelete = DeSerialize<PatientVisit>();
                }
                else if (appointmentType == AppEnum.AppointmentTypes.Block.ToString())
                {
                    appointmentToDelete = DeSerialize<BlockAppointment>();
                }
                else if (appointmentType == AppEnum.AppointmentTypes.Other.ToString())
                {
                    appointmentToDelete = DeSerialize<OtherAppointment>();
                }
                if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentUrl))
                {
                    _appointmentService.DeleteAppointmentType(appointmentToDelete, GetDropBoxFromCookie(), recurrenceStatus, _appointmentService.GetAppointment(appointmentUrl, appointmentToDelete, GetDropBoxFromCookie()));
                }
                result = AjaxCallResult(new AjaxResult(AppEnum.ResultType.Success, AppConstants.Deleted, ""));
            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Url:" + appointmentUrl + " ControllerName: Appointment, MethodName:DeleteAppointment", ex);
            }
            return Json(new { Result = result });
        }

        //public ActionResult CancelAppointment(string appointmentUrl, bool cancelAllAppintment, string appointmentType)
        //{
        //    string result = "";
        //    string errorMessage = "";
        //    bool isSaved = false;
        //    try
        //    {
        //        if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentUrl))
        //        {
        //            _appointmentService.CancelAppointment(appointmentUrl, appointmentType, cancelAllAppintment,GetDropBoxFromCookie());
        //        }
        //        result = AppConstants.Save;
        //    }
        //    catch (Exception ex)
        //    {
        //        result = AjaxCallResult(new AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType.Error, ex.ToString(), ""));
        //        result = AppConstants.Error;
        //    }
        //    return Json(new { Result = result, ErrorMessage = errorMessage });
        //}
        public ActionResult SaveAppointment(string appointmentType, string appointmentGuid, string occurenceType, bool isSaveConflict)
        {
            //PatientVisit patientVisitAppointmentToSave;
            //BlockAppointment blockAppointmentToSave;
            Appointment appointmentToSave=null;
            string result;
            try
            {
                if (appointmentType == AppEnum.AppointmentTypes.PatientVisit.ToString())
                {
                    appointmentToSave = DeSerialize<PatientVisit>();
                }
                else if (appointmentType == AppEnum.AppointmentTypes.Block.ToString())
                {
                    appointmentToSave = DeSerialize<BlockAppointment>();
                }
                else if (appointmentType == AppEnum.AppointmentTypes.Other.ToString())
                {
                    appointmentToSave = DeSerialize<OtherAppointment>();
                }
                //if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid))
                //{// edit mode 
                //    //SetAuditFields(appointmentToSave, true);
                //    SetClientAuditFields(appointmentToSave, true);
                //    if (appointmentToSave != null && appointmentToSave.Recurrence != null)
                //    {
                //        //SetAuditFields(appointmentToSave.Recurrence, true);
                //        SetClientAuditFields(appointmentToSave.Recurrence, true);                        
                //    }
                //}
                //else
                //{ // create mode
                //    //SetAuditFields(appointmentToSave, false);
                //    SetClientAuditFields(appointmentToSave, false);
                //    if (appointmentToSave != null && appointmentToSave.Recurrence != null)
                //    {
                //        //SetAuditFields(appointmentToSave.Recurrence, false);
                //        SetClientAuditFields(appointmentToSave.Recurrence, false);
                //    }
                //}

                if (_appointmentService.IsStartEndTimeSame(appointmentToSave.StartDateTime, appointmentToSave.EndDateTime))
                {
                    result = AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, AppConstants.StartEndTimeSameWarningMessage, ""));
                }
                else if (!_appointmentService.IsFifteenMinutesAppointment(appointmentToSave.StartDateTime, appointmentToSave.EndDateTime))
                {
                    result = AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, AppConstants.AppointmentDurationValidationMessage, ""));
                }
                // ProviderId changed from int to list - need to be change - as of now i have given  ProviderId as ProviderId[0]
                    //else if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid) && _appointmentService.IsAppointmentExists(appointmentToSave.StartDateTime, appointmentToSave.ProviderId, GetDropBoxFromCookie()) &&(!isSaveConflict))
                else if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid) && _appointmentService.IsAppointmentExists(appointmentToSave.StartDateTime, appointmentToSave.ProviderId, GetDropBoxFromCookie()) && (!isSaveConflict))
                {
                    result = AjaxCallResult(new AjaxResult(AppEnum.ResultType.Validation, AppConstants.AppointmentExistsValidationMessage , ""));
                }
                else
                {
                    result = _appointmentService.SaveAppointmentType(appointmentGuid, appointmentToSave, GetDropBoxFromCookie(), !AppCommon.CheckIfStringIsEmptyOrNull(occurenceType) ? (AppEnum.EditStatus)Enum.Parse(typeof(AppEnum.EditStatus), occurenceType) : AppEnum.EditStatus.None);
                    result = AjaxCallResult(new AjaxResult(AppEnum.ResultType.Success, result, ""));
                }

                //if (appointmentType == AppEnum.AppointmentTypes.PatientVisit.ToString())
                //{
                   
                //    if (!_appointmentService.IsFifteenMinutesAppointment(patientVisitAppointmentToSave.StartDateTime, patientVisitAppointmentToSave.EndDateTime))
                //    {
                //        errorMessage = AppConstants.AppointmentDurationValidationMessage;
                //    }
                //    else if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid) && _appointmentService.IsAppointmentExists(patientVisitAppointmentToSave.StartDateTime, patientVisitAppointmentToSave.ProviderId, GetDropBoxFromCookie()))
                //    {
                //        errorMessage = AppConstants.AppointmentExistsValidationMessage;
                //    }
                //    else
                //    {
                //        _appointmentService.SaveAppointment(appointmentGuid, patientVisitAppointmentToSave, GetDropBoxFromCookie());
                //    }
                //}
                //else if (appointmentType == AppEnum.AppointmentTypes.Block.ToString())
                //{
                //    //blockAppointmentToSave = DeSerialize<BlockAppointment>();
                //    //if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid))
                //    //{// edit mode 
                //    //    SetAuditFields(blockAppointmentToSave, true);
                //    //    if (blockAppointmentToSave.Recurrence != null)
                //    //    {
                //    //        SetAuditFields(blockAppointmentToSave.Recurrence, true);
                //    //    }
                //    //}
                //    //else
                //    //{ // create mode
                //    //    SetAuditFields(blockAppointmentToSave, false);
                //    //    if (blockAppointmentToSave.Recurrence != null)
                //    //    {
                //    //        SetAuditFields(blockAppointmentToSave.Recurrence, false);
                //    //    }
                //    //}
                //    if (!_appointmentService.IsFifteenMinutesAppointment(blockAppointmentToSave.StartDateTime, blockAppointmentToSave.EndDateTime))
                //    {
                //        errorMessage = AppConstants.AppointmentDurationValidationMessage;
                //    }
                //    else if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid) && _appointmentService.IsAppointmentExists(blockAppointmentToSave.StartDateTime, blockAppointmentToSave.ProviderId, GetDropBoxFromCookie()))
                //    {
                //        errorMessage = AppConstants.AppointmentExistsValidationMessage;
                //    }
                //    else
                //    {
                //        _appointmentService.SaveAppointment(appointmentGuid, blockAppointmentToSave, GetDropBoxFromCookie());
                //    }
                //}
                
            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                ExceptionManager.Error("Url:" + appointmentGuid + " ControllerName: Appointment, MethodName: SaveAppointment", ex);
                //errorMessage = AppConstants.Error;
            }
            return Json(new { Result = result });
        }

        /// <summary>
        /// Load Calendar for FrontOffice
        /// </summary>
        /// <returns></returns>
        public ActionResult FrontOfficeCalendar()
        {            
            var patientProviderValues = _masterService.GetPatientProviderValues();
            var patientProviderList = (from item in patientProviderValues select new { Id = item.Key, Name = item.Value }).ToList();
            ViewData["ProviderCalendar"] = new SelectList(patientProviderList, "Id", "Name", 2);
            ViewData["ExamRoomCalendar"] = new SelectList(_masterService.GetExamRooms(), "ExamRoom");
            ViewBag.ExamRoomViewResourceList = _masterService.GetExamRoomViewResourceList();
            IList<Patient> apponitmentPatients = _appointmentService.GetAppointmentPatientList(GetDropBoxFromCookie());
            ViewBag.AppointmentPatientList = apponitmentPatients.Select(pat => new { id = pat.UniqueIdentifier, name = pat.LastName + ", "+ pat.FirstName + " " + pat.MiddleInitial }).ToList();
            ViewBag.AssignmentUniquePath = "SimApp/Courses/ELSEVIER_CID/Admin/AssignmentRepository/Assignments/143a6002-eef3-4ad8-adca-f5d272e164cc";
            return View("../../Views/SimOfficeCalendar/FullCalendar");
        }

        public JsonResult GetAppointments(AppEnum.CalendarFilterTypes filterType)
        {
            string filterJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
            CalendarFilterProxy calendarFilter = JsonSerializer.DeserializeObject<CalendarFilterProxy>(filterJson);
            calendarFilter = SetDropboxDetails(calendarFilter);
            IList<CalendarEventProxy> appointmentList = _appointmentService.GetAppointmentsForCalendar(calendarFilter, filterType);
            return Json(new { eventList = appointmentList });
        }


        public JsonResult GetAppointmentsForExamView()
        {
            string filterJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
            CalendarFilterProxy calendarFilter = JsonSerializer.DeserializeObject<CalendarFilterProxy>(filterJson);
            calendarFilter = SetDropboxDetails(calendarFilter);
            IList<CalendarEventProxy> appointmentList = _appointmentService.GetExamRoomViewFilter(calendarFilter);
            return Json(new { eventList = appointmentList });
        }
        private CalendarFilterProxy SetDropboxDetails(CalendarFilterProxy calendarFilter)
        {
            DropBoxLink dropboxObject = GetDropBoxFromCookie();
            calendarFilter.ScenarioId = (dropboxObject != null) ? dropboxObject.Sid : "InvalidSid";
            calendarFilter.CourseId = (dropboxObject != null) ? dropboxObject.Cid : "InvalidCid";
            calendarFilter.UserId = (dropboxObject != null) ? dropboxObject.Uid : "InvalidUid";
            if (dropboxObject != null) calendarFilter.Role = AppCommon.GetCurrentUserRole(dropboxObject.UserRole);
            return calendarFilter;
        }

        public ActionResult GetViewMoreAppointments(DateTime dateString)
        {
            CalendarFilterProxy calendarFilter = new CalendarFilterProxy
                                                     {
                                                         StartDate = dateString,
                                                         EndDate = dateString,
                                                         CalendarView = AppEnum.CalendarViewTypes.agendaDay.ToString()
                                                     };
            calendarFilter = SetDropboxDetails(calendarFilter);
            IList<CalendarEventProxy> appointmentList = _appointmentService.GetAppointmentsForCalendar(calendarFilter, AppEnum.CalendarFilterTypes.None);
            ViewBag.calendarEvents = appointmentList;
            return View("../../Views/SimOfficeCalendar/ViewMore");
        }

        /// <summary>
        /// To populate the search patient Grid on Calender Landing page.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="patientUniqueIdentifier"></param>
        /// <param name="filterDate"> </param>
        /// <returns></returns>
        public ActionResult GetAppointmentPatientSearchList(JQueryDataTableParamModel param, string patientUniqueIdentifier, string filterDate)
        {
            int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnOrder = Request["sSortDir_0"];
            try
            {

                CalendarFilterProxy calendarFilter = new CalendarFilterProxy
                                                         {
                                                             CalendarView = AppEnum.CalendarViewTypes.None.ToString(),
                                                             PatientGuid = patientUniqueIdentifier
                                                         };
                calendarFilter = SetDropboxDetails(calendarFilter);
                IList<Appointment> appointmentList = _appointmentService.GetAppointmentsForPatientSearch(calendarFilter, sortColumnIndex, sortColumnOrder);
                IList<Appointment> appointmentListToRender = appointmentList.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                int appointmentListCount = appointmentList.Count;
                var data = (from appointmentItem in appointmentListToRender
                            select new[]
                                   {
                                       "<input type='radio' name='appointmentpatientSearch' id='" + appointmentItem.Url + "' />",
                                       (!String.IsNullOrEmpty(appointmentItem.StartDateTime.ToString())) ? String.Format("{0:MM/dd/yyyy hh:mm tt}", appointmentItem.StartDateTime) :String.Empty,                                       
                                       (!String.IsNullOrEmpty(appointmentItem.Type)) ? appointmentItem.Type :String.Empty,
                                       ((appointmentItem.Status != 0)) ? AppCommon.AppointmentStatus[appointmentItem.Status] :String.Empty
                                       // ProviderId changed from int to list - need to be change
                                       //,(!String.IsNullOrEmpty(appointmentItem.ProviderId.ToString())) ? AppCommon.providerList[appointmentItem.ProviderId] :String.Empty
                                       }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = appointmentListCount,
                    iTotalDisplayRecords = appointmentListCount,
                    aaData = data
                },
            JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string result = AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                return Json(new{Result= result});
            }
        }

        /// <summary>
        /// load the search patient for appointment page
        /// </summary>
        /// <param name="searchPatientUniqueIndefier"></param>
        /// <param name="filteredDate"> </param>
        /// <param name="patientName"> </param>
        /// <returns></returns>
        public ActionResult SearchPatientInAppointment(string searchPatientUniqueIndefier, string filteredDate, string patientName)
        {
            try
            {
                ViewBag.FilterDate = filteredDate; 
                ViewBag.PatientUniqueIdentifier = searchPatientUniqueIndefier;
                ViewBag.PatientName = patientName;
                return View("../../Views/SimOfficeCalendar/_AppointmentPatientsGrid");
            }
            catch (Exception ex)
            {
                string result = AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
                return Json(new { Result = result });
            }
        }

        public ActionResult LoadAppointmentInViewMode(string appointmentDate, string appointmentUniqueIdentifierUrl, string appointmentType)
         {
            try
            {
                if (!string.IsNullOrEmpty(appointmentUniqueIdentifierUrl))
                { //edit mode
                    if (appointmentType == AppEnum.AppointmentTypes.PatientVisit.ToString())
                    {
                        PatientVisit editAppointment = _appointmentService.GetPatientVisitAppointment(appointmentUniqueIdentifierUrl, GetDropBoxFromCookie());
                        ViewBag.EditAppointment = editAppointment;
                        SetViewBagsForPatientVisitAppointment(editAppointment);
                        // ProviderId changed from int to list - need to be change
                        if (editAppointment.IsAllStaffSelected)
                        {
                            ViewBag.Provider = AppCommon.ProviderList[AppCommon.AllStaffId];
                        }
                        else
                        {
                            if(editAppointment.ProviderId.Count > 0 )
                            {
                             ViewBag.Provider = AppCommon.ProviderList[editAppointment.ProviderId[0]];
                            }
                        }
                    }
                    else if (appointmentType == AppEnum.AppointmentTypes.Block.ToString())
                    {
                        BlockAppointment editAppointment =
                            _appointmentService.GetBlockAppointment(appointmentUniqueIdentifierUrl, GetDropBoxFromCookie());
                        ViewBag.EditAppointment = editAppointment;
                        SetViewBagsForBlockAppointment(editAppointment);
                    }
                    else if (appointmentType == AppEnum.AppointmentTypes.Other.ToString())
                    {
                        OtherAppointment editAppointment =
                            _appointmentService.GetOtherAppointment(appointmentUniqueIdentifierUrl,
                                                                    GetDropBoxFromCookie());
                        ViewBag.EditAppointment = editAppointment;
                        SetViewBagsForOtherAppointment(editAppointment);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                AjaxCallResult(new AjaxResult(AppEnum.ResultType.Error, ex.ToString(), ""));
            }
            return View("../../Views/SimOfficeCalendar/_ViewAppointment");
         }

        private void SetMasterForAllTypes()
        {
            List<string> timeList = Enumerable.Range(0, 41).Select(i => DateTime.Today.AddHours(8).AddMinutes(i * 15).ToString("hh:mm tt")).ToList();
            timeList.Insert(0, "-Select-");
            ViewData["StartTime"] = new SelectList(timeList, "StartTime");
            ViewData["EndTime"] = new SelectList(timeList, "EndTime");
            List<string> emptyList = new List<string> {"-Select-"};
            Dictionary<int, string> emptyDictionary = new Dictionary<int, string> {{0, "Select"}};
            ViewData["VisitType"] = new SelectList(emptyList, "VisitType");
            ViewData["ProviderList"] = new SelectList(emptyDictionary);
            ViewData["ExamRoom"] = new SelectList(emptyList, "ExamRoom");
            ViewData["AppointmentStatus"] = new SelectList(emptyDictionary);
            ViewData["StatusLocationList"] = new SelectList(emptyDictionary);
            ViewData["BlockType"] = new SelectList(emptyList, "BlockType");
            ViewData["BlockFor"] = new SelectList(emptyDictionary);
            ViewData["BlockLocation"] = new SelectList(emptyList, "BlockLocation");
            ViewData["otherAttendees"] = new SelectList(emptyDictionary);
            ViewData["LocationOther"] = new SelectList(emptyList, "LocationOther");
            ViewData["OtherType"] = new SelectList(emptyList, "OtherType");
        }
    }
}
