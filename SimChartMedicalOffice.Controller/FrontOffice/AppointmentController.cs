using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.FrontOffice;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Common;
using System.Web.Mvc;
using SimChartMedicalOffice.Core.ProxyObjects;
using System.Web;
using SimChartMedicalOffice.Core.Patient;
using System.Web.Mvc;
using SimChartMedicalOffice.Core;
using System.IO;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface;
using SimChartMedicalOffice.Core.Json;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class AppointmentController : BaseController
    {
        private IAppointmentService _appointmentService;
        private readonly IMasterService _masterService;
        public AppointmentController(IAppointmentService appointmentService, IMasterService masterService)
        {
            _appointmentService = appointmentService;
            this._masterService = masterService;
        }
        public void SavePatientVisit()
        {
            PatientVisit appointment = GetSamplePatientVisitData();
            //_appointmentService.SavePatientVisitAppointment(appointment, "CourseId2", AppEnum.ApplicationRole.Student, "123");
        }
        private PatientVisit GetSamplePatientVisitData()
        {
            PatientVisit visit = new PatientVisit();
            visit.StartDateTime = DateTime.Now;
            visit.EndDateTime = DateTime.Now.AddMinutes(30);
            visit.Description = "Description";
            visit.Type = "PatientVisit";
            visit.Recurrence = new RecurrenceGroup();
            visit.Recurrence.NumberOfOccurences = 4;
            visit.Recurrence.Pattern = Common.AppEnum.RecurrencePattern.Daily;
            return visit;
        }

        /// <summary>
        /// This method is used to save patient in assignment for a New Appointment
        /// </summary>
        /// <param name="assignmentUniqueIdentifier"></param>
        /// <returns></returns>
        public ActionResult SaveNewPatientForAppointment(string assignmentUniqueIdentifier)
        {
            Patient patient = new Patient();
            string result;
            bool patientValid = true;
            try
            {
                patient = DeSerialize<Patient>();
                patient.MedicalRecordNumber = AppCommon.GenerateRandomNumber().ToString();
                patientValid = _appointmentService.IsPatientValid(assignmentUniqueIdentifier, patient);
                if (patientValid)
                {
                    patient = _appointmentService.SaveNewAppointmentPatient(assignmentUniqueIdentifier, patient);
                }
            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType.Error, ex.ToString(), ""));
            }
            return Json(new { PatientPresent = patientValid, Result = patient });
        }

        /// <summary>
        /// Load the appoinment in both create new and edit mode
        /// </summary>
        /// <param name="appointmentDate"></param>
        /// <param name="appointmentUniqueIdentifierUrl"></param>
        /// <returns></returns>
        public ActionResult LoadAppointment(string appointmentDate, string appointmentUniqueIdentifierUrl, string appointmentType)
        {
            string result;
            try
            {
                LoadPatientVisitAppointment();
                LoadBlockAppointment();
                LoadOtherAppointment();
                
                if (!string.IsNullOrEmpty(appointmentUniqueIdentifierUrl))
                { //edit mode
                    if (appointmentType == AppEnum.AppointmentTypes.PatientVisit.ToString())
                    {
                        PatientVisit editAppointment = _appointmentService.GetPatientVisitAppointment(appointmentUniqueIdentifierUrl, GetDropBoxFromCookie());
                        ViewBag.EditAppointment = editAppointment;
                        SetViewBagsForPatientVisitAppointment(editAppointment);
                    }
                    else if (appointmentType == AppEnum.AppointmentTypes.Block.ToString())
                    {
                        BlockAppointment editAppointment =
                            _appointmentService.GetBlockAppointment(appointmentUniqueIdentifierUrl,GetDropBoxFromCookie());
                        ViewBag.EditAppointment = editAppointment;
                        SetViewBagsForBlockAppointment(editAppointment);
                    }
                }
                else
                {// create mode 
                    //SavePatientVisit();

                }

            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType.Error, ex.ToString(), ""));
            }
            return View("../../Views/SimOfficeCalendar/_LoadAppointment");
        }


        /// <summary>
        /// Loading the Cancel Popup with default values from the sender object
        /// </summary>
        /// <param name="cancelValue"></param>
        /// <returns></returns>
        public ActionResult DeleteCancelAppointment(string cancelValue)
        {
            ViewData["CancelValue"] = cancelValue;
            return View("../../Views/SimOfficeCalendar/_CancelAppointment");
        }


        private void LoadPatientVisitAppointment()
        {
            List<string> timeList = Enumerable.Range(0, 41).Select(i => DateTime.Today.AddHours(8).AddMinutes(i * 15).ToString("hh:mm tt")).ToList();
            timeList.Insert(0, "-Select-");
            ViewData["StartTime"] = new SelectList(timeList, "StartTime");
            ViewData["EndTime"] = new SelectList(timeList, "EndTime");
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

        private void LoadBlockAppointment()
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

        private void LoadOtherAppointment()
        {
            ViewData["LocationOther"] = new SelectList(_masterService.GetExamRooms(), "LocationOther");
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
                    ViewBag.Provider = appointment.ProviderId;
                    ViewBag.EditExamRoom = appointment.ExamRoomIdentifier;
                    ViewBag.AppointmentDate = appointment.StartDateTime.ToString("MM/dd/yyyy");
                    ViewBag.EditStartTime = appointment.StartDateTime.ToString("hh:mm tt");
                    ViewBag.EditEndTime = appointment.EndDateTime.ToString("hh:mm tt");
                    ViewBag.EditDescription = appointment.Description;
                    ViewBag.AppointmentType = AppEnum.AppointmentTypes.PatientVisit.ToString();
                    ViewBag.AppointmentObject = appointment;
                    ViewBag.AppointmentURL = appointment.Url;
                }
            }
            catch
            {
                //To-Do
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
                    ViewBag.For = appointment.ProviderId;
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
                }
            }
            catch
            {
                //To-Do
            }
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
        public ActionResult SaveAppointment(string appointmentType, string appointmentGuid)
        {
            //PatientVisit patientVisitAppointmentToSave;
            //BlockAppointment blockAppointmentToSave;
            Appointment appointmentToSave=null;
            string result = "";
            string errorMessage = "";            
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
                if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid))
                {// edit mode 
                    SetAuditFields(appointmentToSave, true);
                    if (appointmentToSave.Recurrence != null)
                    {
                        SetAuditFields(appointmentToSave.Recurrence, true);
                    }
                }
                else
                { // create mode
                    SetAuditFields(appointmentToSave, false);
                    if (appointmentToSave.Recurrence != null)
                    {
                        SetAuditFields(appointmentToSave.Recurrence, false);
                    }
                }

                if (!_appointmentService.IsFifteenMinutesAppointment(appointmentToSave.StartDateTime, appointmentToSave.EndDateTime))
                {
                    errorMessage = AppConstants.AppointmentDurationValidationMessage;
                }
                else if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid) && _appointmentService.IsAppointmentExists(appointmentToSave.StartDateTime, appointmentToSave.ProviderId, GetDropBoxFromCookie()))
                {
                    errorMessage = AppConstants.AppointmentExistsValidationMessage;
                }
                else
                {

                    _appointmentService.SaveAppointmentType(appointmentGuid, appointmentToSave, GetDropBoxFromCookie(),AppEnum.EditStatus.All);
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
                result = AppConstants.Save;
            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType.Error, ex.ToString(), ""));
                result = AppConstants.Error;
            }
            return Json(new { Result = result, ErrorMessage = errorMessage });
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
            IList<Patient> ApponitmentPatients = _appointmentService.GetAppointmentPatientList("SimApp/Courses/ELSEVIER_CID/Admin/AssignmentRepository/Assignments/143a6002-eef3-4ad8-adca-f5d272e164cc");
            ViewBag.AppointmentPatientList = ApponitmentPatients.Select(pat => new { id = pat.UniqueIdentifier, name = pat.LastName + ","+ pat.FirstName + " " + pat.MiddleInitial }).ToList();
            ViewBag.AssignmentUniquePath = "SimApp/Courses/ELSEVIER_CID/Admin/AssignmentRepository/Assignments/143a6002-eef3-4ad8-adca-f5d272e164cc";
            return View("../../Views/SimOfficeCalendar/FullCalendar");
        }

        public JsonResult GetAppointments(AppEnum.CalendarFilterTypes filterType)
        {
            string filterJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
            string parentIdentifier = String.Empty;
            CalendarFilterProxy calendarFilter=null;
            calendarFilter = JsonSerializer.DeserializeObject<CalendarFilterProxy>(filterJson);
            calendarFilter = SetDropboxDetails(calendarFilter);
            IList<CalendarEventProxy> appointmentList = _appointmentService.GetAppointmentsForCalendar(calendarFilter, filterType);
            return Json(new { eventList = appointmentList });
        }


        public JsonResult GetAppointmentsForExamView()
        {
            string filterJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
            string parentIdentifier = String.Empty;
            CalendarFilterProxy calendarFilter=null;// = new CalendarFilterProxy();
            calendarFilter = JsonSerializer.DeserializeObject<CalendarFilterProxy>(filterJson);
            calendarFilter = SetDropboxDetails(calendarFilter);
            IList<CalendarEventProxy> appointmentList = _appointmentService.GetExamRoomViewFilter(calendarFilter);
            return Json(new { eventList = appointmentList });
        }
        private CalendarFilterProxy SetDropboxDetails(CalendarFilterProxy calendarFilter)
        {
            DropBoxLink dropboxObject;
            dropboxObject = GetDropBoxFromCookie();
            calendarFilter.ScenarioId = (dropboxObject != null) ? dropboxObject.SID : "InvalidSid";
            calendarFilter.CourseId = (dropboxObject != null) ? dropboxObject.CID : "InvalidCid";
            calendarFilter.UserId = (dropboxObject != null) ? dropboxObject.UID : "InvalidUid";
            calendarFilter.Role = AppCommon.GetCurrentUserRole(dropboxObject.UserRole);
            return calendarFilter;
        }

        public ActionResult GetViewMoreAppointments(DateTime dateString)
        {
            string filterJson = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
            string parentIdentifier = String.Empty;
            CalendarFilterProxy calendarFilter = new CalendarFilterProxy();
            calendarFilter.StartDate = dateString;
            calendarFilter.EndDate = dateString;
            calendarFilter.CalendarView = AppEnum.CalendarViewTypes.agendaDay.ToString();
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
        /// <returns></returns>
        public ActionResult GetAppointmentPatientSearchList(jQueryDataTableParamModel param, string patientUniqueIdentifier, string filterDate)
        {
            string result = "";
            int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnOrder = Request["sSortDir_0"];
            int appointmentListCount = 0;
            try
            {
                
                CalendarFilterProxy calendarFilter = new CalendarFilterProxy();
                calendarFilter.CalendarView = AppEnum.CalendarViewTypes.month.ToString();
                calendarFilter.StartDate = Convert.ToDateTime(filterDate);
                calendarFilter = SetDropboxDetails(calendarFilter);
                IList<Appointment> appointmentList = _appointmentService.GetAppointmentsForPatientSearch(calendarFilter, patientUniqueIdentifier, sortColumnIndex, sortColumnOrder);
                IList<Appointment> appointmentListToRender = appointmentList.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                appointmentListCount = appointmentList.Count;
                var data = (from appointmentItem in appointmentListToRender
                            select new[]
                                   {
                                       "<input type='radio' name='patientSearchResult' id='" + appointmentItem.UniqueIdentifier + "' onClick=''/>",
                                       (!String.IsNullOrEmpty(appointmentItem.StartDateTime.ToString())) ? String.Format("{0:MM/dd/yyyy hh:mm tt}", appointmentItem.StartDateTime) :String.Empty,                                       
                                       (!String.IsNullOrEmpty(appointmentItem.Type)) ? appointmentItem.Type :String.Empty,
                                       ((appointmentItem.Status != 0)) ? AppCommon.AppointmentStatus[appointmentItem.Status] :String.Empty,
                                       (!String.IsNullOrEmpty(appointmentItem.ProviderId.ToString())) ? AppCommon.providerList[appointmentItem.ProviderId] :String.Empty
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
               result = AjaxCallResult(new AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType.Error, ex.ToString(), ""));
               return Json(new{Result= result});
            }
        }

        /// <summary>
        /// load the search patient for appointment page
        /// </summary>
        /// <param name="searchPatientUniqueIndefier"></param>
        /// <returns></returns>
        public ActionResult SearchPatientInAppointment(string searchPatientUniqueIndefier, string filteredDate, string patientName)
        {
            string result = "";
            try
            {
                ViewBag.FilterDate = filteredDate; 
                ViewBag.PatientUniqueIdentifier = searchPatientUniqueIndefier;
                ViewBag.PatientName = patientName;
                return View("../../Views/SimOfficeCalendar/_AppointmentPatientsGrid");
            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType.Error, ex.ToString(), ""));
                return Json(new { Result = result });
            }           
        }

        public ActionResult LoadAppointmentInViewMode(string appointmentDate, string appointmentUniqueIdentifierUrl, string appointmentType)
         {
            string result;
            try
            {
                if (!string.IsNullOrEmpty(appointmentUniqueIdentifierUrl))
                { //edit mode
                    if (appointmentType == AppEnum.AppointmentTypes.PatientVisit.ToString())
                    {
                        PatientVisit editAppointment = _appointmentService.GetPatientVisitAppointment(appointmentUniqueIdentifierUrl, GetDropBoxFromCookie());
                        ViewBag.EditAppointment = editAppointment;
                        SetViewBagsForPatientVisitAppointment(editAppointment);
                        ViewBag.Provider = AppCommon.providerList[editAppointment.ProviderId];
                    }
                    else if (appointmentType == AppEnum.AppointmentTypes.Block.ToString())
                    {
                        BlockAppointment editAppointment =
                            _appointmentService.GetBlockAppointment(appointmentUniqueIdentifierUrl, GetDropBoxFromCookie());
                        ViewBag.EditAppointment = editAppointment;
                        SetViewBagsForBlockAppointment(editAppointment);
                    }
                    
                }
                else
                {// create mode 
                    //SavePatientVisit();
                }
            }
            catch (Exception ex)
            {
                result = AjaxCallResult(new AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType.Error, ex.ToString(), ""));
            }
            return View("../../Views/SimOfficeCalendar/_ViewAppointment");
         }
    }
}
