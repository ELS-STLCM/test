using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.DataInterfaces.FrontOffice;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.ApplicationServices.FrontOffice.AppointmentPatterns;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.FrontOffice;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.DropBox;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface;
using SimChartMedicalOffice.Common.Extensions;


namespace SimChartMedicalOffice.ApplicationServices.FrontOffice
{

    public class AppointmentService : BaseService, IAppointmentService
    {
        private IPatientVisitAppointmentDocument _patientVisitAppointmentDocument;
        private IBlockAppointmentDocument _blockAppointmentDocument;
        private IOtherAppointmentDocument _otherAppointmentDocument;
        private IRecurrenceGroupDocument _recurrenceDocument;
        private IPatientDocument _patientDocument;
        private IAppointmentDocument _appointmentDocument;
        private IMasterService _masterService;
        public AppointmentService(IPatientVisitAppointmentDocument patientVisitAppointmentDocument,
                                    IBlockAppointmentDocument blockAppointmentDocument,
                                    IOtherAppointmentDocument otherAppointmentDocument,
                                    IRecurrenceGroupDocument recurrenceDocument,
                                    IPatientDocument patientDocument, IAppointmentDocument appointmentDocument, IMasterService masterService)
        {
            this._blockAppointmentDocument = blockAppointmentDocument;
            this._otherAppointmentDocument = otherAppointmentDocument;
            this._patientVisitAppointmentDocument = patientVisitAppointmentDocument;
            this._recurrenceDocument = recurrenceDocument;
            this._patientDocument = patientDocument;
            this._appointmentDocument = appointmentDocument;
            this._masterService = masterService;
        }

        private string FormatAppointmentUrl(string Url, string courseId, AppEnum.ApplicationRole role, string assignmentScenarioId, DateTime appointmentDate, string providerId, string appointmentGuid, string uid)
        {
            return string.Format(Url, courseId, AppCommon.GetRoleDescription(role), uid, assignmentScenarioId, string.Format("{0:yyyyMM}", appointmentDate), "Day" + string.Format("{0:dd}", appointmentDate), providerId, ((AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid)) ? "{0}" : appointmentGuid));
        }
        private string FormatRecurrenceGroupUrl(string Url, string courseId, AppEnum.ApplicationRole role, string assignmentScenarioId, string appointmentGuid, string uid)
        {
            return string.Format(Url, courseId, AppCommon.GetRoleDescription(role), uid, assignmentScenarioId, ((AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid)) ? "{0}" : appointmentGuid));
        }
        private string GetIdentifierFromUrl(string responseUrl)
        {
            return responseUrl.Substring(responseUrl.LastIndexOf('/') + 1);
        }
        private string GetAppointments(Appointment appointmentDocument, AppEnum.AppointmentTypes typeOfAppointment)
        {

            IAppointmentTypeStrategy appointmentType = null;
            if (typeOfAppointment == AppEnum.AppointmentTypes.PatientVisit)
            {
                //appointmentType = new PatientVisitStrategy((PatientVisit)appointmentDocument);
                appointmentType = new AppointmentStrategy<PatientVisit>((PatientVisit)appointmentDocument);
            }
            else if (typeOfAppointment == AppEnum.AppointmentTypes.Block)
            {
                //appointmentType = new BlockStrategy((BlockAppointment)appointmentDocument);
                appointmentType = new AppointmentStrategy<BlockAppointment>((BlockAppointment)appointmentDocument);
            }
            else if (typeOfAppointment == AppEnum.AppointmentTypes.Other)
            {
                //appointmentType = new BlockStrategy((BlockAppointment)appointmentDocument);
                appointmentType = new AppointmentStrategy<OtherAppointment>((OtherAppointment)appointmentDocument);
            }
            return appointmentType.GenerateAppointments();
        }

        private void CreateRecurrenceAppointments(DateTime startDateTime, DateTime endDateTime, DateTime endBy)
        {
            IList<Appointment> appointmentList = new List<Appointment>();
        }
        //public void SavePatientVisitAppointment(PatientVisit patientVisitAppointment, string courseId, AppEnum.ApplicationRole role, string assignmentScenarioId)
        //{
        //    //patientVisitAppointment.RecurrenceGroup = GetIdentifierFromUrl(_recurrenceDocument.SaveOrUpdate(FormatRecurrenceGroupUrl(_recurrenceDocument.Url, courseId, role, assignmentScenarioId), patientVisitAppointment.Recurrence));
        //    //IList<PatientVisit> appointmentsListToSchedule = JsonSerializer.DeserializeObject<IList<PatientVisit>>(GetAppointments(patientVisitAppointment, AppEnum.AppointmentTypes.PatientVisit));
        //    //patientVisitAppointment.ClearRecurrenceGroup();
        //    // _patientVisitAppointmentDocument.SaveOrUpdate(FormatAppointmentUrl(_patientVisitAppointmentDocument.Url, courseId, role, assignmentScenarioId, patientVisitAppointment.StartDateTime), patientVisitAppointment);
        //    if (patientVisitAppointment.ProviderId != null)
        //    {
        //        foreach (int item in patientVisitAppointment.ProviderId)
        //        {
        //            _patientVisitAppointmentDocument.SaveOrUpdate(FormatAppointmentUrl(_patientVisitAppointmentDocument.Url, courseId, role, assignmentScenarioId, patientVisitAppointment.StartDateTime, item.ToString()), patientVisitAppointment);
        //        }
        //    }
        //}

        private bool CheckIsDataChange(Appointment patientVisitExist, Appointment toSaveAppointment)
        {
            bool isDateChanged = false;
            if (!patientVisitExist.StartDateTime.Date.Equals(toSaveAppointment.StartDateTime.Date))
            {
                return true;
            }
            if (patientVisitExist.Recurrence != null && toSaveAppointment.Recurrence != null)
            {
                if (!patientVisitExist.Recurrence.Pattern.ToString().Equals(toSaveAppointment.Recurrence.Pattern.ToString()))
                {
                    return true;
                }
                if (!patientVisitExist.Recurrence.NumberOfOccurences.Equals(toSaveAppointment.Recurrence.NumberOfOccurences))
                {
                    return true;
                }
            }
            return isDateChanged;
        }

        /// <summary>
        /// this method will be invoked by appointment controller. Base method 
        /// </summary>
        /// <param name="appointmentGuid"></param>
        /// <param name="toSaveAppointment"></param>
        /// <param name="dropBoxLink"></param>
        public void SaveAppointmentType(string appointmentGuid, Appointment toSaveAppointment, DropBoxLink dropBoxLink,AppEnum.EditStatus occurrenceStatus)
        {
            bool isDateChanged = false;
            Appointment appointmentExist = null;


            if (toSaveAppointment is PatientVisit)
            {
                if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid))
                {
                    appointmentExist = GetPatientVisitAppointment(appointmentGuid, dropBoxLink);
                    isDateChanged = CheckIsDataChange(appointmentExist, toSaveAppointment);
                }

            }
            else if (toSaveAppointment is BlockAppointment)
            {
                if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid))
                {
                    appointmentExist = GetBlockAppointment(appointmentGuid, dropBoxLink);
                    isDateChanged = CheckIsDataChange(appointmentExist, toSaveAppointment);
                }
            }

            if (toSaveAppointment.Status == (int)AppEnum.AppointmentStatus.Canceled)
            {
                CancelAppointment(appointmentGuid, appointmentExist, dropBoxLink, occurrenceStatus);
            }
            else
            {
                SaveAppointment(appointmentGuid, toSaveAppointment, dropBoxLink, isDateChanged, appointmentExist);
            }
        }


        private string SaveActualAppointment(string appointmentGuidUrl, Appointment appointmentToSave, DropBoxLink dropBoxLink)
        {
            AppEnum.ApplicationRole dropBoxLinkUserRole = AppCommon.GetCurrentUserRole(dropBoxLink.UserRole);
            if (appointmentToSave is PatientVisit)
            {
                if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuidUrl))
                {
                    return _patientVisitAppointmentDocument.SaveOrUpdate(FormatAppointmentUrl(_patientVisitAppointmentDocument.Url, dropBoxLink.CID, dropBoxLinkUserRole, dropBoxLink.SID, appointmentToSave.StartDateTime, appointmentToSave.ProviderId.ToString(), appointmentGuidUrl, dropBoxLink.UID), (PatientVisit)appointmentToSave);
                }
                else
                {
                    return _patientVisitAppointmentDocument.SaveOrUpdate(appointmentGuidUrl, (PatientVisit)appointmentToSave);
                }
            }
            else if (appointmentToSave is BlockAppointment)
            {
                if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuidUrl))
                {
                    return _blockAppointmentDocument.SaveOrUpdate(FormatAppointmentUrl(_blockAppointmentDocument.Url, dropBoxLink.CID, dropBoxLinkUserRole, dropBoxLink.SID, appointmentToSave.StartDateTime, appointmentToSave.ProviderId.ToString(), appointmentGuidUrl, dropBoxLink.UID), (BlockAppointment)appointmentToSave);
                }
                else
                {
                    return _blockAppointmentDocument.SaveOrUpdate(appointmentGuidUrl, (BlockAppointment)appointmentToSave);
                }
            }
            return "";
        }

        private string SaveRecurrenceGroup(DropBoxLink dropBoxLink, RecurrenceGroup recurrenceGroup, string recurrenceGroupGuid)
        {
            AppEnum.ApplicationRole dropBoxLinkUserRole = AppCommon.GetCurrentUserRole(dropBoxLink.UserRole);
            string str = _recurrenceDocument.SaveOrUpdate(FormatRecurrenceGroupUrl(_recurrenceDocument.Url, dropBoxLink.CID, dropBoxLinkUserRole, dropBoxLink.SID, recurrenceGroupGuid, dropBoxLink.UID), recurrenceGroup);
            return GetIdentifierFromUrl(str);
        }

        private void SaveActualAppointmentRecurrence(string appointmentGuidUrl, Appointment appointmentToSave, DropBoxLink dropBoxLink)
        {
            appointmentToSave.RecurrenceGroup = SaveRecurrenceGroup(dropBoxLink, appointmentToSave.Recurrence, appointmentToSave.RecurrenceGroup);
            IList<Appointment> appointmentsListToSchedule = new List<Appointment>();
            if (appointmentToSave is PatientVisit)
            {
                IList<PatientVisit> patientVisitAppointmentsListToSchedule = JsonSerializer.DeserializeObject<IList<PatientVisit>>(GetAppointments((PatientVisit)appointmentToSave, AppEnum.AppointmentTypes.PatientVisit));
                appointmentsListToSchedule = patientVisitAppointmentsListToSchedule.Cast<Appointment>().ToList<Appointment>();
            }

            else if (appointmentToSave is BlockAppointment)
            {
                IList<BlockAppointment> blockAppointmentsListToSchedule = JsonSerializer.DeserializeObject<IList<BlockAppointment>>(GetAppointments((BlockAppointment)appointmentToSave, AppEnum.AppointmentTypes.Block));
                appointmentsListToSchedule = blockAppointmentsListToSchedule.Cast<Appointment>().ToList<Appointment>();
            }
            List<string> patientVisitSavedUrl = new List<string>();
            if (appointmentsListToSchedule != null && appointmentsListToSchedule.Count > 0)
            {
                foreach (Appointment appointmentItem in appointmentsListToSchedule)
                {
                    ClearAppointmentRecurrenceGroup(appointmentItem);
                    patientVisitSavedUrl.Add(SaveActualAppointment(appointmentGuidUrl, appointmentItem, dropBoxLink));
                }
            }
            SaveAppointmentUrlRecurrenceGroup(patientVisitSavedUrl, dropBoxLink, appointmentToSave.RecurrenceGroup);
        }

        private void SaveAppointmentUrlRecurrenceGroup(List<string> appointmentSavedUrl, DropBoxLink dropBoxLink, string recurrenceGroupGuid)
        {
            if (appointmentSavedUrl != null && appointmentSavedUrl.Count > 0)
            {
                AppEnum.ApplicationRole dropBoxLinkUserRole = AppCommon.GetCurrentUserRole(dropBoxLink.UserRole);
                string AppointmentUrlRecurrenceGroupUrl = FormatRecurrenceGroupUrl(_recurrenceDocument.Url, dropBoxLink.CID, dropBoxLinkUserRole, dropBoxLink.SID, "", dropBoxLink.UID);
                AppointmentUrlRecurrenceGroupUrl = string.Format(AppointmentUrlRecurrenceGroupUrl, recurrenceGroupGuid);
                _recurrenceDocument.SaveOrUpdate(AppointmentUrlRecurrenceGroupUrl + "/RecurrenceList", JsonSerializer.SerializeObject(appointmentSavedUrl));
            }
        }

        private void ClearAppointmentRecurrenceGroup(Appointment appointment)
        {
            appointment.ClearRecurrenceGroup();
        }

        private void SaveAppointment(string appointmentGuidUrl, Appointment toSaveAppointment, DropBoxLink dropBoxLink, bool isDateChanged, Appointment appointmentAlreadyExist)
        {
            //  appointmentGuidUrl null 
            if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuidUrl))
            {
                if (toSaveAppointment.Recurrence == null)
                { // save non Recurrence patient Visit Appointment 
                    SaveActualAppointment("", toSaveAppointment, dropBoxLink);
                }
                else
                {// save Recurrence patient Visit Appointment 
                    SaveActualAppointmentRecurrence("", toSaveAppointment, dropBoxLink);
                }
            }
            else if (isDateChanged)
            { // if date changed in edit mode delete  all Recurrence and patient Visit Appointment list  and create new appointment 
                DeleteAndCreateNewAppointment(appointmentGuidUrl, toSaveAppointment, dropBoxLink, appointmentAlreadyExist);
            }
            else
            { // if date not change then change need to be done only on current & fucutre date appointmnet. 

                // Patient Visit from DB and Patient Visit from edited  both has Recurrence then  chnage is for Recurrence To Recurrence 
                if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentAlreadyExist.RecurrenceGroup) && (toSaveAppointment.Recurrence != null))
                {
                    UpdateSaveAppointmentRecurrenceToRecurrence(appointmentGuidUrl, toSaveAppointment, dropBoxLink, appointmentAlreadyExist);
                }
                //  Patient Visit from DB has recurrence  but Patient Visit from edited has no recurrence then  chnage is for Recurrence To non Recurrence 
                else if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentAlreadyExist.RecurrenceGroup) && (toSaveAppointment.Recurrence == null))
                {
                    UpdateSaveAppointmentRecurrenceToNonRecurrence(toSaveAppointment, dropBoxLink, appointmentAlreadyExist);
                }
                //  Patient Visit from DB has no recurrence  but Patient Visit from edited has recurrence then  change is for Non Recurrence To Recurrence 
                else if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentAlreadyExist.RecurrenceGroup) && (toSaveAppointment.Recurrence != null))
                {
                    UpdateSaveAppointmentNonRecurrenceToRecurrence(appointmentGuidUrl, toSaveAppointment, dropBoxLink, appointmentAlreadyExist);
                }
                //  Patient Visit from DB has no recurrence  but Patient Visit from edited has no recurrence then  change is for Non Recurrence To non Recurrence 
                else if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentAlreadyExist.RecurrenceGroup) && (toSaveAppointment.Recurrence == null))
                {
                    toSaveAppointment.CreatedBy = appointmentAlreadyExist.CreatedBy;
                    toSaveAppointment.CreatedTimeStamp = appointmentAlreadyExist.CreatedTimeStamp;
                    SaveActualAppointment(appointmentGuidUrl, toSaveAppointment, dropBoxLink);
                }
            }
        }

        /// <summary>
        /// Non Recurrence To Recurrence
        /// </summary>
        /// <param name="appointmentGuidUrl"></param>
        /// <param name="toSaveAppointment"></param>
        /// <param name="dropBoxLink"></param>
        private void UpdateSaveAppointmentNonRecurrenceToRecurrence(string appointmentGuidUrl, Appointment toSaveAppointment, DropBoxLink dropBoxLink, Appointment appointmentAlreadyExist)
        {
            if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuidUrl))
            {
                DeleteAnAppointment(appointmentGuidUrl);
                SetAuditFields(toSaveAppointment, true, dropBoxLink);
                toSaveAppointment.CreatedBy = appointmentAlreadyExist.CreatedBy;
                toSaveAppointment.CreatedTimeStamp = appointmentAlreadyExist.CreatedTimeStamp;
                SaveActualAppointmentRecurrence("", toSaveAppointment, dropBoxLink);
            }
        }

        /// <summary>
        /// Recurrence To Non Recurrence
        /// </summary>
        /// <param name="appointmentGuidUrl"></param>
        /// <param name="toSaveAppointment"></param>
        /// <param name="dropBoxLink"></param>
        private void UpdateSaveAppointmentRecurrenceToNonRecurrence(Appointment toSaveAppointment, DropBoxLink dropBoxLink, Appointment appointmentAlreadyExist)
        {
            if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentAlreadyExist.RecurrenceGroup))
            {
                appointmentAlreadyExist = DeleteRecurrenceAppointment(appointmentAlreadyExist, dropBoxLink, AppEnum.EditStatus.All);
            }
            SetAuditFields(toSaveAppointment, true, dropBoxLink);
            toSaveAppointment.CreatedBy = appointmentAlreadyExist.CreatedBy;
            toSaveAppointment.CreatedTimeStamp = appointmentAlreadyExist.CreatedTimeStamp;
            SaveActualAppointment("", toSaveAppointment, dropBoxLink);
        }
        /// <summary>
        /// Recurrence To Recurrence Patient Visit
        /// </summary>
        /// <param name="appointmentGuidUrl"></param>
        /// <param name="toSaveAppointment"></param>
        /// <param name="dropBoxLink"></param>
        private void UpdateSaveAppointmentRecurrenceToRecurrence(string appointmentGuidUrl, Appointment toSaveAppointment, DropBoxLink dropBoxLink, Appointment appointmentAlreadyExist)
        {
            AppEnum.ApplicationRole dropBoxLinkUserRole = AppCommon.GetCurrentUserRole(dropBoxLink.UserRole);
            IList<string> appointmentExistRecurrenceList = new List<string>();
            Appointment eachRecurrenceListAppointmentDBItem = null;
            if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuidUrl))
            {

                DateTime startDateTime = (appointmentAlreadyExist.StartDateTime.Date).Add(toSaveAppointment.StartDateTime.TimeOfDay);
                DateTime endDateTime = (appointmentAlreadyExist.EndDateTime.Date).Add(toSaveAppointment.EndDateTime.TimeOfDay);
                toSaveAppointment.StartDateTime = startDateTime;
                toSaveAppointment.EndDateTime = endDateTime;
                IList<Appointment> appointmentsListToSchedule = new List<Appointment>();
                if (toSaveAppointment is PatientVisit)
                {
                    IList<PatientVisit> patientVisitAppointmentsListToSchedule = JsonSerializer.DeserializeObject<IList<PatientVisit>>(GetAppointments((PatientVisit)toSaveAppointment, AppEnum.AppointmentTypes.PatientVisit));
                    appointmentsListToSchedule = patientVisitAppointmentsListToSchedule.Cast<Appointment>().ToList<Appointment>();
                }
                else if (toSaveAppointment is BlockAppointment)
                {
                    IList<BlockAppointment> blockAppointmentsListToSchedule = JsonSerializer.DeserializeObject<IList<BlockAppointment>>(GetAppointments((BlockAppointment)toSaveAppointment, AppEnum.AppointmentTypes.Block));
                    appointmentsListToSchedule = blockAppointmentsListToSchedule.Cast<Appointment>().ToList<Appointment>();
                }
                foreach (string appointmentItem in appointmentAlreadyExist.Recurrence.RecurrenceList)
                {
                    if (toSaveAppointment is PatientVisit)
                    {
                        eachRecurrenceListAppointmentDBItem = _patientVisitAppointmentDocument.Get(appointmentItem);
                    }
                    else if (toSaveAppointment is PatientVisit)
                    {
                        eachRecurrenceListAppointmentDBItem = _blockAppointmentDocument.Get(appointmentItem);
                    }
                    Appointment toSaveAppointmentType = appointmentsListToSchedule.SingleOrDefault(pv => pv.StartDateTime.Date.Equals(eachRecurrenceListAppointmentDBItem.StartDateTime.Date));
                    if (toSaveAppointmentType != null)
                    {
                        toSaveAppointmentType.CreatedBy = appointmentAlreadyExist.CreatedBy;
                        toSaveAppointmentType.CreatedTimeStamp = appointmentAlreadyExist.CreatedTimeStamp;
                        SetAuditFields(toSaveAppointmentType, true, dropBoxLink);
                        ClearAppointmentRecurrenceGroup(toSaveAppointmentType);
                        toSaveAppointmentType.RecurrenceGroup = appointmentAlreadyExist.RecurrenceGroup;
                        SaveActualAppointment(appointmentItem, toSaveAppointmentType, dropBoxLink);
                    }
                }
            }
        }

        /// <summary>
        /// delete  patient Visit Appointment   
        /// </summary>
        /// <param name="appointmentGuidUrl"></param>
        /// <param name="dropBoxLink"></param>
        /// <param name="recurrenceEditStatus"></param>
        /// <param name="isCancelStatusToCheck"></param>
        /// <returns></returns>
        private Appointment DeleteAppointmentType(DropBoxLink dropBoxLink, AppEnum.EditStatus recurrenceEditStatus, Appointment appointmentAlreadyExist)
        {
            if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentAlreadyExist.Url))
            {
                if (AppEnum.EditStatus.All == recurrenceEditStatus) // check for all( current / fucture ) 
                {
                    if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentAlreadyExist.RecurrenceGroup))
                    {// delete the single appointment 
                        DeleteAnAppointment(appointmentAlreadyExist.Url);
                    }
                    else
                    {// delete the Recurrence Appointment 
                        appointmentAlreadyExist = DeleteRecurrenceAppointment(appointmentAlreadyExist, dropBoxLink, recurrenceEditStatus);
                    }
                }
            }
            return appointmentAlreadyExist;
        }

        /// <summary>
        /// delete Recurrence Appointment 
        /// </summary>
        /// <param name="appointmentExist"></param>
        /// <param name="dropBoxLink"></param>
        /// <param name="recurrenceEditStatus"></param>
        /// <param name="isCancelStatusToCheck"></param>
        /// <returns></returns>
        private Appointment DeleteRecurrenceAppointment(Appointment appointmentExist, DropBoxLink dropBoxLink, AppEnum.EditStatus recurrenceEditStatus)
        {
            AppEnum.ApplicationRole dropBoxLinkUserRole = AppCommon.GetCurrentUserRole(dropBoxLink.UserRole);
            // get the Url of Recurrence Appointment
            if (appointmentExist.Recurrence.RecurrenceList != null && appointmentExist.Recurrence.RecurrenceList.Count > 0)
            {
                DeleteAppointmentList(appointmentExist);
                string deleteRecurrenceGroup;
                _recurrenceDocument.Delete(appointmentExist.Url, out deleteRecurrenceGroup);
            }
            return appointmentExist;
        }

        /// <summary>
        /// to delete the list appointment for Patient Visit  & Block Type
        /// </summary>
        /// <param name="appointmentUrlList"></param>
        private void DeleteAppointmentList(Appointment appointmentExist)
        {
            if (appointmentExist.Recurrence.RecurrenceList != null && appointmentExist.Recurrence.RecurrenceList.Count > 0)
            {
                foreach (string appItem in appointmentExist.Recurrence.RecurrenceList)
                {
                    Appointment eachAppointment = _appointmentDocument.Get(appItem);
                    DeleteAnAppointment(appItem);
                }
            }
        }

        /// <summary>
        /// to delete the appointment
        /// </summary>
        /// <param name="appointmentUrl"></param>
        private void DeleteAnAppointment(string appointmentUrl)
        {
            string deleteAppointmentUrl;
            _appointmentDocument.Delete(appointmentUrl, out deleteAppointmentUrl);
        }

        private void DeleteAndCreateNewAppointment(string appointmentGuidUrl, Appointment toSaveAppointment, DropBoxLink dropBoxLink, Appointment appointmentAlreadyExist)
        {
            Appointment deletedAppointmentExist = DeleteAppointmentType(dropBoxLink, AppEnum.EditStatus.All, appointmentAlreadyExist);
            SetAuditFields(toSaveAppointment, true, dropBoxLink); //  set audit fields for modify...
            toSaveAppointment.CreatedBy = deletedAppointmentExist.CreatedBy;
            toSaveAppointment.CreatedTimeStamp = deletedAppointmentExist.CreatedTimeStamp;
            if (toSaveAppointment.Recurrence == null)
            {// set audit fields for create ...
                SaveActualAppointment("", toSaveAppointment, dropBoxLink);
            }
            else
            {   // set audit fields for create ...
                if (deletedAppointmentExist.Recurrence != null)
                {// set audit fields for P.V  recurrence create ...
                    toSaveAppointment.Recurrence.CreatedBy = deletedAppointmentExist.Recurrence.CreatedBy;
                    toSaveAppointment.Recurrence.CreatedTimeStamp = deletedAppointmentExist.Recurrence.CreatedTimeStamp;
                }
                else
                {// set audit fields for P.V  recurrence create .. for new 
                    SetAuditFields(toSaveAppointment.Recurrence, false, dropBoxLink);
                }
                SaveActualAppointmentRecurrence("", toSaveAppointment, dropBoxLink);
            }
        }
        /// <summary>
        /// This method is used to save patient in assignment for a New Appointment
        /// </summary>
        /// <param name="assignmentUniqueIdentifier"></param>
        /// <param name="patientItem"></param>
        /// <returns></returns>
        public Patient SaveNewAppointmentPatient(string assignmentUniqueIdentifier, Patient patientItem)
        {
            string patientUniqueIdentifier;
            string patientAssignmentUrl = assignmentUniqueIdentifier + "/Patients/" + patientItem.GetNewGuidValue();
            _patientDocument.SaveOrUpdate(patientAssignmentUrl, patientItem);
            patientItem = _patientDocument.Get(patientAssignmentUrl);
            patientUniqueIdentifier = patientItem.Url.Split('/').Last();
            patientItem.UniqueIdentifier = patientUniqueIdentifier;
            return patientItem;
        }

        private string GetCalendarClassForAppointmentType(Appointment appointment)
        {
            if (appointment is PatientVisit)
            {
                return "patient-visit";
            }
            else if (appointment is BlockAppointment)
            {
                return "blocked-appointment";
            }
            return "other-appointment";

        }
        private string GetCalendarTitleForAppointmentType(Appointment appointment)
        {
            string titleOfAppointment = string.Empty;
            if (appointment is PatientVisit)
            {
                titleOfAppointment = string.Format("{1},{0} {2}", appointment.FirstName, appointment.LastName, appointment.MiddleInitial);
            }
            else if (appointment is BlockAppointment)
            {
                titleOfAppointment = appointment.Type;
            }
            else if (appointment is OtherAppointment)
            {
                titleOfAppointment = appointment.Type;
            }
            //if (appointment.StartDateTime.Subtract(appointment.EndDateTime).Minutes <= 15)
            //{
            //    titleOfAppointment = titleOfAppointment + " " + _masterService.GetProviderName(appointment.ProviderId) + " " + appointment.Status;
            //}
            //else
            //{
            titleOfAppointment = titleOfAppointment + "\n" + _masterService.GetProviderName(appointment.ProviderId) + "\n" + ((AppEnum.AppointmentStatus)appointment.Status).ToString() + "\n " + AppCommon.GetStatusLocationString(appointment.StatusLocation); 
            //}
            return titleOfAppointment;
        }
        private bool checkIfRecurrenceExists(Appointment appointment)
        {
            if (!String.IsNullOrEmpty(appointment.RecurrenceGroup))
            {
                return true;
            }
            return false;
        }
        private string GetCalendarTooltipForAppointmentType(Appointment appointment)
        {
            string tooltip = String.Empty;
            if (appointment is PatientVisit)
            {
                tooltip = string.Format("{1},{0} {2}", appointment.FirstName, appointment.LastName, appointment.MiddleInitial);
            }
            else if (appointment is BlockAppointment)
            {
                tooltip = appointment.Type;
            }
            else if (appointment is OtherAppointment)
            {
                tooltip = appointment.Type;
            }
            tooltip = tooltip + "\n" + "<span class='header-text'>" + _masterService.GetProviderName(appointment.ProviderId) + "</span>" + "\n" + String.Format("{0:h:mm tt}", appointment.StartDateTime) + "-" + @String.Format("{0:h:mm tt}", appointment.EndDateTime);
            return tooltip;
        }

        private string GetAppointmentType(Appointment appointment)
        {
            if (appointment is PatientVisit)
            {
                return AppEnum.AppointmentTypes.PatientVisit.ToString();
            }
            else if (appointment is BlockAppointment)
            {
                return AppEnum.AppointmentTypes.Block.ToString();
            }
            return AppEnum.AppointmentTypes.Other.ToString();
        }

        public IList<CalendarEventProxy> GetExamRoomViewFilter(CalendarFilterProxy CalendarFilterObject)
        {
            IList<CalendarEventProxy> calendarEvents = new List<CalendarEventProxy>();
            CalendarEventProxy calenderEvent = null;
            CalendarFilterObject.CalendarView = AppEnum.CalendarViewTypes.agendaDay.ToString();
            IList<Appointment> appointmentsList = _appointmentDocument.GetPatientVisitAppointments(CalendarFilterObject, AppEnum.CalendarFilterTypes.None);
            foreach (var item in appointmentsList)
            {
                calenderEvent = new CalendarEventProxy();
                calenderEvent.title = GetCalendarTitleForAppointmentType(item);
                calenderEvent.tooltip = GetCalendarTooltipForAppointmentType(item);
                calenderEvent.className = GetCalendarClassForAppointmentType(item);
                calenderEvent.isRecurrence = checkIfRecurrenceExists(item);
                calenderEvent.editable = false;
                calenderEvent.start = item.StartDateTime.ToString();
                calenderEvent.end = item.EndDateTime.ToString();
                calenderEvent.allDay = false;
                calenderEvent.Url = item.Url;
                calenderEvent.PatientName = string.Format("{1},{0} {2}", item.FirstName, item.LastName, item.MiddleInitial);
                calenderEvent.resourceId = item.ExamRoomIdentifier;
                calendarEvents.Add(calenderEvent);
            }
            return calendarEvents;
        }

        private IList<CalendarEventProxy> GetAppointmentsForCalendar(List<Appointment> appointments, string calenderView)
        {
            IList<CalendarEventProxy> calendarEvents = new List<CalendarEventProxy>();
            var appointmentsForEachDay = appointments.GroupBy(s => s.StartDateTime.Date);
            List<Appointment> appointmentsTemp = new List<Appointment>();
            CalendarEventProxy calenderEvent = null;
            if (calenderView == "month")
            {
                foreach (var item in appointmentsForEachDay)
                {
                    if (item.Count<Appointment>() > 3)
                    {
                        appointmentsTemp.AddRange(item.Cast<Appointment>().OrderBy(f => f.StartDateTime).Take(3).ToList());
                        PatientVisit patientVisit = new PatientVisit();
                        patientVisit.Type = AppCommon.VIEW_MORE;
                        patientVisit.StartDateTime = item.Key;
                        appointmentsTemp.Add(patientVisit);
                    }
                    else
                    {
                        appointmentsTemp.AddRange(item.Cast<Appointment>().ToList());
                    }
                }
            }
            else
            {
                appointmentsTemp = appointments;
            }

            foreach (var item in appointmentsTemp)
            {
                calenderEvent = new CalendarEventProxy();
                calenderEvent.allDay = false;
                calenderEvent.editable = false;

                if (item.Type != AppCommon.VIEW_MORE)
                {
                    calenderEvent.title = GetCalendarTitleForAppointmentType(item);
                    calenderEvent.tooltip = GetCalendarTooltipForAppointmentType(item);
                    calenderEvent.AppointmentType = GetAppointmentType(item);
                    calenderEvent.className = GetCalendarClassForAppointmentType(item);
                    calenderEvent.start = item.StartDateTime.ToString();
                    calenderEvent.isRecurrence = checkIfRecurrenceExists(item);
                    calenderEvent.end = item.EndDateTime.ToString();
                    calenderEvent.Url = item.Url;
                    calenderEvent.PatientName = string.Format("{1},{0} {2}", item.FirstName, item.LastName, item.MiddleInitial);
                    calenderEvent.Status = item.Status;
                }
                else
                {
                    calenderEvent.title = item.Type;
                    calenderEvent.className = "view-more";
                    calenderEvent.start = new DateTime(item.StartDateTime.Year, item.StartDateTime.Month, item.StartDateTime.Day, 23, 59, 59).ToString();
                }
                calendarEvents.Add(calenderEvent);
            }
            return calendarEvents;
        }

        public IList<CalendarEventProxy> GetAppointmentsForCalendar(CalendarFilterProxy CalendarFilterObject, AppEnum.CalendarFilterTypes filterType)
        {
            IList<Appointment> appointmentsList = _appointmentDocument.GetAppointments(CalendarFilterObject, filterType);
            IList<CalendarEventProxy> calendarEvents = new List<CalendarEventProxy>();
            calendarEvents = GetAppointmentsForCalendar(appointmentsList.ToList(), CalendarFilterObject.CalendarView);
            return calendarEvents;
        }

        public bool IsPatientValid(string assignmentUniqueIdentifier, Patient patientItem)
        {
            bool isPatientValid = true;
            IList<Patient> patientList = new List<Patient>();
            IList<Patient> duplicatePatientList = new List<Patient>();
            patientList = _patientDocument.GetAllPatientForAssignment(assignmentUniqueIdentifier);
            if (patientList != null && patientList.Count > 0)
            {
                duplicatePatientList =
                    patientList.Where(x => ((x.LastName.ToLower() == patientItem.LastName.ToLower()) && (x.FirstName.ToLower() == patientItem.FirstName.ToLower()) && (x.MiddleInitial.ToLower() == patientItem.MiddleInitial.ToLower()) &&
                        (x.DateOfBirth.ToLower() == patientItem.DateOfBirth.ToLower()))).ToList();
            }
            if (duplicatePatientList.Count() != 0)
            {
                isPatientValid = false;
            }
            return isPatientValid;
        }

        public bool IsFifteenMinutesAppointment(DateTime appointmentStartTime, DateTime appointmentEndTime)
        {
            TimeSpan appointmentDifferenceTime = appointmentEndTime - appointmentStartTime;
            if (appointmentDifferenceTime.TotalMinutes >= 15)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsAppointmentExists(DateTime appointmentStartTime, int lstProviderId, DropBoxLink dropBoxLink)
        {
            CalendarFilterProxy calendarFilterObject = new CalendarFilterProxy();
            calendarFilterObject.CalendarView = AppEnum.CalendarViewTypes.agendaDay.ToString();
            calendarFilterObject.StartDate = appointmentStartTime;
            calendarFilterObject.EndDate = appointmentStartTime;
            calendarFilterObject.ScenarioId = (dropBoxLink != null) ? dropBoxLink.SID : "SID";
            calendarFilterObject.CourseId = (dropBoxLink != null) ? dropBoxLink.CID : "Course";
            calendarFilterObject.Role = AppEnum.ApplicationRole.Student;
            IList<Appointment> appointmentsList = _appointmentDocument.GetAppointments(calendarFilterObject, AppEnum.CalendarFilterTypes.None);
            appointmentsList =
                (from appointment in appointmentsList
                 where appointment.StartDateTime == appointmentStartTime
                 select appointment).ToList();
            appointmentsList =
                (from appointment in appointmentsList
                 where appointment.ProviderId == lstProviderId
                 select appointment).ToList();
            if (appointmentsList.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public PatientVisit GetPatientVisitAppointment(string appointmentUrl, DropBoxLink dropBoxLink)
        {
            PatientVisit patientVisitEdit = new PatientVisit();
            patientVisitEdit = _patientVisitAppointmentDocument.Get(appointmentUrl);
            if (!AppCommon.CheckIfStringIsEmptyOrNull(patientVisitEdit.RecurrenceGroup))
            {
                AppEnum.ApplicationRole dropBoxLinkUserRole = AppCommon.GetCurrentUserRole(dropBoxLink.UserRole);
                RecurrenceGroup patientVisitRecurrenceExist = _recurrenceDocument.Get(FormatRecurrenceGroupUrl(_recurrenceDocument.Url, dropBoxLink.CID, dropBoxLinkUserRole, dropBoxLink.SID, patientVisitEdit.RecurrenceGroup, dropBoxLink.UID));
                patientVisitEdit.Recurrence = patientVisitRecurrenceExist;
            }
            return patientVisitEdit;
        }

        public BlockAppointment GetBlockAppointment(string appointmentUrl, DropBoxLink dropBoxLink)
        {
            //return _blockAppointmentDocument.Get(appointmentUrl);
            BlockAppointment blockAppointmentEdit = new BlockAppointment();
            blockAppointmentEdit = _blockAppointmentDocument.Get(appointmentUrl);
            if (!AppCommon.CheckIfStringIsEmptyOrNull(blockAppointmentEdit.RecurrenceGroup))
            {
                AppEnum.ApplicationRole dropBoxLinkUserRole = AppCommon.GetCurrentUserRole(dropBoxLink.UserRole);
                RecurrenceGroup blockRecurrenceExist = _recurrenceDocument.Get(FormatRecurrenceGroupUrl(_recurrenceDocument.Url, dropBoxLink.CID, dropBoxLinkUserRole, dropBoxLink.SID, blockAppointmentEdit.RecurrenceGroup, dropBoxLink.UID));
                blockAppointmentEdit.Recurrence = blockRecurrenceExist;
            }
            return blockAppointmentEdit;
        }

        /// <summary>
        /// Get all the patients in Patient Repository and given Assignment
        /// </summary>
        /// <returns></returns>
        public IList<Patient> GetAppointmentPatientList(string assignmentUniqueIdentifier)
        {
            return _patientDocument.GetAllPatientForAssignment(assignmentUniqueIdentifier);
        }


        /// <summary>
        /// Get appointment list for a perticular patient
        /// </summary>
        /// <param name="calendarFilter"></param>
        /// <param name="patientUniqueIdentifier"></param>
        /// <param name="sortColumnIndex"></param>
        /// <param name="sortColumnOrder"></param>
        /// <returns></returns>
        public IList<Appointment> GetAppointmentsForPatientSearch(CalendarFilterProxy calendarFilter, string patientUniqueIdentifier, int sortColumnIndex, string sortColumnOrder)
        {
            IList<Appointment> appointmentsForPatient = new List<Appointment>();
            IList<Appointment> appointments = _appointmentDocument.GetAppointments(calendarFilter, AppEnum.CalendarFilterTypes.None);
            appointmentsForPatient = appointments.Where(x => x.PatientIdentifier == patientUniqueIdentifier).ToList();
            string sortColumnName = AppCommon.gridColumnForAppointmentPatientList[sortColumnIndex - 1];
            var sortablePatientList = appointmentsForPatient.AsQueryable();
            appointmentsForPatient = sortablePatientList.OrderBy<Appointment>(sortColumnName, sortColumnOrder).ToList<Appointment>();
            return appointmentsForPatient;
        }


        private bool CancelAppointment(string appointmentUrl, Appointment appointmentToCancel, DropBoxLink dropBoxLink, AppEnum.EditStatus occurrenceStatus)
        {
            switch (occurrenceStatus)
            {
                case AppEnum.EditStatus.All:
                    CancelRecurringAppointment(appointmentUrl, appointmentToCancel, dropBoxLink);
                    break;
                case AppEnum.EditStatus.Current:
                    CancelActualAppointment(appointmentUrl, appointmentToCancel, dropBoxLink);
                    break;
                default:
                    break;
            }
            return true;
        }

        private void CancelActualAppointment(string appointmentUrl, Appointment appointmentToCancel, DropBoxLink dropBoxLink)
        {
            appointmentToCancel.Status = (int)AppEnum.AppointmentStatus.Canceled;
            SetAuditFields(appointmentToCancel, true, dropBoxLink);
            SaveActualAppointment(appointmentUrl, appointmentToCancel, dropBoxLink);

        }

        private void CancelRecurringAppointment(string appointmentUrl, Appointment appointmentExist, DropBoxLink dropBoxLink)
        {
            // get the Url of Recurrence Appointment
            Appointment appointmentToCancel = null;
            if (appointmentExist.Recurrence.RecurrenceList != null && appointmentExist.Recurrence.RecurrenceList.Count > 0)
            {
                foreach (string cancelItem in appointmentExist.Recurrence.RecurrenceList)
                {
                    if (appointmentExist is PatientVisit)
                    {
                        appointmentToCancel = _patientVisitAppointmentDocument.Get(cancelItem);
                    }
                    else if (appointmentExist is BlockAppointment)
                    {
                        appointmentToCancel = _blockAppointmentDocument.Get(cancelItem);
                    }
                    CancelActualAppointment(appointmentUrl, appointmentToCancel, dropBoxLink);
                }
            }
        }
    }
}
