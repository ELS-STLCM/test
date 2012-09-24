using System;
using System.Collections.Generic;
using System.Linq;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.FrontOffice;
using SimChartMedicalOffice.ApplicationServices.FrontOffice.AppointmentPatterns;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Extensions;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DataInterfaces.FrontOffice;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using SimChartMedicalOffice.Core.DropBox;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.ProxyObjects;


namespace SimChartMedicalOffice.ApplicationServices.FrontOffice
{

    public class AppointmentService : BaseService, IAppointmentService
    {
        private readonly IPatientVisitAppointmentDocument _patientVisitAppointmentDocument;
        private readonly IBlockAppointmentDocument _blockAppointmentDocument;
        private readonly IOtherAppointmentDocument _otherAppointmentDocument;
        private readonly IRecurrenceGroupDocument _recurrenceDocument;
        private readonly IPatientDocument _patientDocument;
        private readonly IAppointmentDocument _appointmentDocument;
        private readonly IMasterService _masterService;
        public AppointmentService(IPatientVisitAppointmentDocument patientVisitAppointmentDocument,
                                    IBlockAppointmentDocument blockAppointmentDocument,
                                    IOtherAppointmentDocument otherAppointmentDocument,
                                    IRecurrenceGroupDocument recurrenceDocument,
                                    IPatientDocument patientDocument, IAppointmentDocument appointmentDocument, IMasterService masterService)
        {
            _blockAppointmentDocument = blockAppointmentDocument;
            _otherAppointmentDocument = otherAppointmentDocument;
            _patientVisitAppointmentDocument = patientVisitAppointmentDocument;
            _recurrenceDocument = recurrenceDocument;
            _patientDocument = patientDocument;
            _appointmentDocument = appointmentDocument;
            _masterService = masterService;
        }

        private string FormatAppointmentUrl(string url, DateTime appointmentDate, string providerId, string appointmentGuid, AppEnum.ProviderType providerType)
        {
            string appointmentUrl = "";
            switch (providerType)
            {
                case AppEnum.ProviderType.SingleProvider:
                    appointmentUrl = string.Format(url, string.Format(AppCommon.CalendarYearMonthNode, appointmentDate), AppCommon.GetWeekNode(appointmentDate), AppCommon.GetDayNode(appointmentDate), providerId, ((AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid)) ? "{0}" : appointmentGuid));
                    break;
                case AppEnum.ProviderType.MultiProvider:
                    appointmentUrl = string.Format(url, string.Format(AppCommon.CalendarYearMonthNode, appointmentDate), AppCommon.GetWeekNode(appointmentDate), AppCommon.GetDayNode(appointmentDate), ((AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid)) ? "{0}" : appointmentGuid));
                    break;
            }
            return appointmentUrl;
        }
        private string FormatRecurrenceGroupUrl(string url, string appointmentGuid)
        {
            return string.Format(url, ((AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid)) ? "{0}" : appointmentGuid));
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
            if (appointmentType != null)
            {
                return appointmentType.GenerateAppointments();
            }
            return "";
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
            const bool isDataChanged = false;
            if (!patientVisitExist.StartDateTime.Equals(toSaveAppointment.StartDateTime))
            {
                return true;
            }
            if (!patientVisitExist.EndDateTime.Equals(toSaveAppointment.EndDateTime))
            {
                return true;
            }
            if (!AppCommon.IsListSame(patientVisitExist.ProviderId, toSaveAppointment.ProviderId))
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
            return isDataChanged;
        }

        /// <summary>
        /// Method to fetch the Appointment based on Dropbox and url
        /// </summary>
        /// <param name="appointmentUrl"></param>
        /// <param name="appointmentToGet"></param>
        /// <param name="dropBoxLink"></param>
        /// <returns></returns>
        public Appointment GetAppointment(string appointmentUrl, Appointment appointmentToGet, DropBoxLink dropBoxLink)
        {
            Appointment appointmentExist = null;
            if (appointmentToGet is PatientVisit)
            {
                if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentUrl))
                {
                    appointmentExist = GetPatientVisitAppointment(appointmentUrl, dropBoxLink);
                }

            }
            else if (appointmentToGet is BlockAppointment)
            {
                if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentUrl))
                {
                    appointmentExist = GetBlockAppointment(appointmentUrl, dropBoxLink);
                }
            }
            return appointmentExist;
        }

        /// <summary>
        /// this method will be invoked by appointment controller. Base method 
        /// </summary>
        /// <param name="appointmentGuid"></param>
        /// <param name="toSaveAppointment"></param>
        /// <param name="dropBoxLink"></param>
        /// <param name="occurrenceStatus"></param>
        public string SaveAppointmentType(string appointmentGuid, Appointment toSaveAppointment, DropBoxLink dropBoxLink, AppEnum.EditStatus occurrenceStatus)
        {
            bool isDateChanged = false;
            Appointment appointmentExist = null;
            string returnString = "";


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
            else if (toSaveAppointment is OtherAppointment)
            {
                if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid))
                {
                    appointmentExist = GetOtherAppointment(appointmentGuid, dropBoxLink);
                    isDateChanged = CheckIsDataChange(appointmentExist, toSaveAppointment);
                }
            }

            if (toSaveAppointment.Status == (int)AppEnum.AppointmentStatus.Canceled)
            {
                if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid))
                {
                    if (appointmentExist != null)
                    {
                        appointmentExist.ReasonForCancellation = toSaveAppointment.ReasonForCancellation;
                        CancelAppointment(appointmentGuid, appointmentExist, dropBoxLink, occurrenceStatus);
                    }
                    returnString = AppConstants.Cancelled;
                }
            }
            else
            {
                SetAuditFieldsForAppointment(appointmentGuid, toSaveAppointment, dropBoxLink);
                SaveAppointment(appointmentGuid, toSaveAppointment, dropBoxLink, isDateChanged, appointmentExist, occurrenceStatus);
                returnString = AppConstants.AppointmentSave;
            }

            return returnString;
        }

        private void SetAuditFieldsForAppointment(string appointmentGuid, Appointment toSaveAppointment, DropBoxLink dropBoxLink)
        {
            if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuid))
            {// edit mode 
                //SetAuditFields(appointmentToSave, true);
                SetClientAuditFields(toSaveAppointment, true, dropBoxLink);
                if (toSaveAppointment != null && toSaveAppointment.Recurrence != null)
                {
                    //SetAuditFields(appointmentToSave.Recurrence, true);
                    SetClientAuditFields(toSaveAppointment.Recurrence, true, dropBoxLink);
                }
            }
            else
            { // create mode
                //SetAuditFields(appointmentToSave, false);
                SetClientAuditFields(toSaveAppointment, false, dropBoxLink);
                if (toSaveAppointment != null && toSaveAppointment.Recurrence != null)
                {
                    //SetAuditFields(appointmentToSave.Recurrence, false);
                    SetClientAuditFields(toSaveAppointment.Recurrence, false, dropBoxLink);
                }
            }
        }

/*
        private string GetWeekOfYear(DateTime appointmentDateTime)
        {
            return "W" + AppCommon.GetWeekOfYear(appointmentDateTime);
        }
*/

        private AppEnum.ProviderType GetProviderType(Appointment appointmentToSave)
        {
            AppEnum.ProviderType providerType = AppEnum.ProviderType.SingleProvider;
            if (appointmentToSave.ProviderId.Count > 1)
            {
                providerType = AppEnum.ProviderType.MultiProvider;
            }
            else if (appointmentToSave.ProviderId.Count == 1)
            {
                providerType = AppEnum.ProviderType.SingleProvider;
            }
            return providerType;
        }
        private string SaveActualAppointment(string appointmentGuidUrl, Appointment appointmentToSave, DropBoxLink dropBoxLink)
        {
            AppEnum.ProviderType providerType = GetProviderType(appointmentToSave);
            string providerIntId = "";
            if (providerType == AppEnum.ProviderType.SingleProvider)
            {
                providerIntId = appointmentToSave.ProviderId[0].ToString();
            }
            if (appointmentToSave is PatientVisit)
            {
                if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuidUrl))
                {

                    return _patientVisitAppointmentDocument.SaveOrUpdate(FormatAppointmentUrl(_patientVisitAppointmentDocument.GetAssignmentUrl(dropBoxLink, DocumentPath.Module.PatientVisitAppointment, providerType.ToString()),
                                appointmentToSave.StartDateTime, providerIntId, appointmentGuidUrl, providerType), (PatientVisit)appointmentToSave);
                }
                return _patientVisitAppointmentDocument.SaveOrUpdate(appointmentGuidUrl, (PatientVisit)appointmentToSave);
            }
            if (appointmentToSave is BlockAppointment)
            {
                if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuidUrl))
                {
                    return _blockAppointmentDocument.SaveOrUpdate(FormatAppointmentUrl(_blockAppointmentDocument.GetAssignmentUrl(dropBoxLink, DocumentPath.Module.BlockAppointment, providerType.ToString()), appointmentToSave.StartDateTime, providerIntId, appointmentGuidUrl, providerType), (BlockAppointment)appointmentToSave);
                }
                return _blockAppointmentDocument.SaveOrUpdate(appointmentGuidUrl, (BlockAppointment)appointmentToSave);
            }
            if (appointmentToSave is OtherAppointment)
            {
                if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuidUrl))
                {
                    return _otherAppointmentDocument.SaveOrUpdate(FormatAppointmentUrl(_otherAppointmentDocument.GetAssignmentUrl(dropBoxLink, DocumentPath.Module.OtherAppointment, providerType.ToString()), appointmentToSave.StartDateTime, providerIntId, appointmentGuidUrl, providerType), (OtherAppointment)appointmentToSave);
                }
                return _otherAppointmentDocument.SaveOrUpdate(appointmentGuidUrl, (OtherAppointment)appointmentToSave);
            }
            return "";
        }

        private string SaveRecurrenceGroup(DropBoxLink dropBoxLink, RecurrenceGroup recurrenceGroup, string recurrenceGroupGuid)
        {
            string str = _recurrenceDocument.SaveOrUpdate(FormatRecurrenceGroupUrl(_recurrenceDocument.GetAssignmentUrl(dropBoxLink, DocumentPath.Module.RecurrenceGroup), recurrenceGroupGuid), recurrenceGroup);
            return GetIdentifierFromUrl(str);
        }

        private void SaveActualAppointmentRecurrence(string appointmentGuidUrl, Appointment appointmentToSave, DropBoxLink dropBoxLink)
        {
            appointmentToSave.RecurrenceGroup = SaveRecurrenceGroup(dropBoxLink, appointmentToSave.Recurrence, appointmentToSave.RecurrenceGroup);
            IList<Appointment> appointmentsListToSchedule = new List<Appointment>();
            if (appointmentToSave is PatientVisit)
            {
                IList<PatientVisit> patientVisitAppointmentsListToSchedule = JsonSerializer.DeserializeObject<IList<PatientVisit>>(GetAppointments(appointmentToSave, AppEnum.AppointmentTypes.PatientVisit));
                appointmentsListToSchedule = patientVisitAppointmentsListToSchedule.ToList<Appointment>();
            }

            else if (appointmentToSave is BlockAppointment)
            {
                IList<BlockAppointment> blockAppointmentsListToSchedule = JsonSerializer.DeserializeObject<IList<BlockAppointment>>(GetAppointments(appointmentToSave, AppEnum.AppointmentTypes.Block));
                appointmentsListToSchedule = blockAppointmentsListToSchedule.ToList<Appointment>();
            }
            else if (appointmentToSave is OtherAppointment)
            {
                IList<OtherAppointment> otherAppointmentsListToSchedule = JsonSerializer.DeserializeObject<IList<OtherAppointment>>(GetAppointments(appointmentToSave, AppEnum.AppointmentTypes.Other));
                appointmentsListToSchedule = otherAppointmentsListToSchedule.ToList<Appointment>();
            }
            List<string> patientVisitSavedUrl = new List<string>();
            if (appointmentsListToSchedule.Count > 0)
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
                string appointmentUrlRecurrenceGroupUrl = _recurrenceDocument.GetAssignmentUrl(dropBoxLink, DocumentPath.Module.RecurrenceGroup);
                appointmentUrlRecurrenceGroupUrl = string.Format(appointmentUrlRecurrenceGroupUrl, recurrenceGroupGuid);
                _recurrenceDocument.SaveOrUpdate(appointmentUrlRecurrenceGroupUrl + "/RecurrenceList", JsonSerializer.SerializeObject(appointmentSavedUrl));
            }
        }

        private void ClearAppointmentRecurrenceGroup(Appointment appointment)
        {
            appointment.ClearRecurrenceGroup();
        }


        private void SetAppointmentAduitFieldsValues(Appointment toSaveAppointment, Appointment appointmentAlreadyExist)
        {
            toSaveAppointment.CreatedBy = appointmentAlreadyExist.CreatedBy;
            toSaveAppointment.CreatedTimeStamp = appointmentAlreadyExist.CreatedTimeStamp;
            toSaveAppointment.ChartTimeStamp = appointmentAlreadyExist.ChartTimeStamp;
            toSaveAppointment.Signature = appointmentAlreadyExist.Signature;
        }

        private void SaveAppointment(string appointmentGuidUrl, Appointment toSaveAppointment, DropBoxLink dropBoxLink, bool isDateChanged, Appointment appointmentAlreadyExist, AppEnum.EditStatus occurenceStatus)
        {
            //  appointmentGuidUrl null 
            if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuidUrl))
            { // create mode 
                if (toSaveAppointment.Recurrence == null)
                { // save non Recurrence patient Visit Appointment 
                    SaveActualAppointment("", toSaveAppointment, dropBoxLink);
                }
                else
                {// save Recurrence patient Visit Appointment 
                    SaveActualAppointmentRecurrence("", toSaveAppointment, dropBoxLink);
                }
            }
            else
            {    // edit mode 

                switch (occurenceStatus)
                {
                    case AppEnum.EditStatus.All:
                        DeleteAndCreateNewAppointment(toSaveAppointment, dropBoxLink, appointmentAlreadyExist);
                        break;
                    case AppEnum.EditStatus.Current:
                        if (isDateChanged)
                        {
                            // delete current appoint from reccurence & create new non recurring appointment 
                            // from recurrence to new appointment (non-recurrence)
                            DeleteFromRecurrenceCreateNewNonRecurrenceAppointment(toSaveAppointment, dropBoxLink, appointmentAlreadyExist, occurenceStatus);
                        }
                        else
                        { // Update an appointment  from recurrence list 
                            //toSaveAppointment.CreatedBy = appointmentAlreadyExist.CreatedBy;
                            //toSaveAppointment.CreatedTimeStamp = appointmentAlreadyExist.CreatedTimeStamp;
                            //toSaveAppointment.ChartTimeStamp = appointmentAlreadyExist.ChartTimeStamp;
                            //toSaveAppointment.Signature = appointmentAlreadyExist.Signature;
                            SetAppointmentAduitFieldsValues(toSaveAppointment, appointmentAlreadyExist);
                            toSaveAppointment.Recurrence = null;
                            toSaveAppointment.RecurrenceGroup = appointmentAlreadyExist.RecurrenceGroup;
                            SaveActualAppointment(appointmentGuidUrl, toSaveAppointment, dropBoxLink);
                        }
                        break;
                    case AppEnum.EditStatus.None:
                        //  Appointment from DB has no recurrence  but  Appointment  from edited has recurrence then  change is for Non Recurrence To Recurrence 
                        if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentAlreadyExist.RecurrenceGroup) && (toSaveAppointment.Recurrence != null))
                        {
                            UpdateSaveAppointmentNonRecurrenceToRecurrence(appointmentGuidUrl, toSaveAppointment, dropBoxLink, appointmentAlreadyExist);
                        }
                        //  Patient Visit from DB has no recurrence  but Patient Visit from edited has no recurrence then  change is for Non Recurrence To non Recurrence 
                        else if (AppCommon.CheckIfStringIsEmptyOrNull(appointmentAlreadyExist.RecurrenceGroup) && (toSaveAppointment.Recurrence == null))
                        {
                            //toSaveAppointment.CreatedBy = appointmentAlreadyExist.CreatedBy;
                            //toSaveAppointment.CreatedTimeStamp = appointmentAlreadyExist.CreatedTimeStamp;
                            //toSaveAppointment.ChartTimeStamp = appointmentAlreadyExist.ChartTimeStamp;
                            //toSaveAppointment.Signature = appointmentAlreadyExist.Signature;

                            SetAppointmentAduitFieldsValues(toSaveAppointment, appointmentAlreadyExist);

                            if (isDateChanged)
                            {
                                DeleteAndCreateNewAppointment(toSaveAppointment, dropBoxLink, appointmentAlreadyExist);
                            }
                            else
                            {
                                SaveActualAppointment(appointmentGuidUrl, toSaveAppointment, dropBoxLink);
                            }
                        }
                        break;
                }

                //if (isDateChanged)
                //{ // if date changed in edit mode delete  all Recurrence and patient Visit Appointment list  and create new appointment 
                //    DeleteAndCreateNewAppointment(appointmentGuidUrl, toSaveAppointment, dropBoxLink, appointmentAlreadyExist);
                //}
                //else
                //{ // if date not change then change need to be done only on current & fucutre date appointmnet. 

                //    // Patient Visit from DB and Patient Visit from edited  both has Recurrence then  chnage is for Recurrence To Recurrence 
                //    if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentAlreadyExist.RecurrenceGroup) && (toSaveAppointment.Recurrence != null))
                //    {
                //        UpdateSaveAppointmentRecurrenceToRecurrence(appointmentGuidUrl, toSaveAppointment, dropBoxLink, appointmentAlreadyExist);
                //    }
                //    //  Patient Visit from DB has recurrence  but Patient Visit from edited has no recurrence then  chnage is for Recurrence To non Recurrence 
                //    else if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentAlreadyExist.RecurrenceGroup) && (toSaveAppointment.Recurrence == null))
                //    {
                //        UpdateSaveAppointmentRecurrenceToNonRecurrence(toSaveAppointment, dropBoxLink, appointmentAlreadyExist);
                //    }

                //}
            }
        }


        private void DeleteFromRecurrenceCreateNewNonRecurrenceAppointment(Appointment toSaveAppointment, DropBoxLink dropBoxLink, Appointment appointmentAlreadyExist, AppEnum.EditStatus occurenceStatus)
        {
            //Appointment deletedAppointmentExist = DeleteAppointmentType(toSaveAppointment, dropBoxLink, occurenceStatus, appointmentAlreadyExist);
            DeleteAppointmentType(toSaveAppointment, dropBoxLink, occurenceStatus, appointmentAlreadyExist);
            //toSaveAppointment.CreatedBy = appointmentAlreadyExist.CreatedBy;
            //toSaveAppointment.CreatedTimeStamp = appointmentAlreadyExist.CreatedTimeStamp;
            //toSaveAppointment.ChartTimeStamp = appointmentAlreadyExist.ChartTimeStamp;
            //toSaveAppointment.Signature = appointmentAlreadyExist.Signature;
            SetAppointmentAduitFieldsValues(toSaveAppointment, appointmentAlreadyExist);
            toSaveAppointment.Recurrence = null;
            SaveActualAppointment("", toSaveAppointment, dropBoxLink);
        }

        /// <summary>
        /// Non Recurrence To Recurrence
        /// </summary>
        /// <param name="appointmentGuidUrl"></param>
        /// <param name="toSaveAppointment"></param>
        /// <param name="dropBoxLink"></param>
        /// <param name="appointmentAlreadyExist"> </param>
        private void UpdateSaveAppointmentNonRecurrenceToRecurrence(string appointmentGuidUrl, Appointment toSaveAppointment, DropBoxLink dropBoxLink, Appointment appointmentAlreadyExist)
        {
            if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuidUrl))
            {
                DeleteAnAppointment(appointmentGuidUrl);
                //SetAuditFields(toSaveAppointment, true, dropBoxLink);
                SetClientAuditFields(toSaveAppointment, true, dropBoxLink);
                //toSaveAppointment.CreatedBy = appointmentAlreadyExist.CreatedBy;
                //toSaveAppointment.CreatedTimeStamp = appointmentAlreadyExist.CreatedTimeStamp;
                //toSaveAppointment.ChartTimeStamp = appointmentAlreadyExist.ChartTimeStamp;
                //toSaveAppointment.Signature = appointmentAlreadyExist.Signature;
                SetAppointmentAduitFieldsValues(toSaveAppointment, appointmentAlreadyExist);
                SaveActualAppointmentRecurrence("", toSaveAppointment, dropBoxLink);
            }
        }

        //private void UpdateSaveAppointmentRecurrenceToNonRecurrence(Appointment toSaveAppointment, DropBoxLink dropBoxLink, Appointment appointmentAlreadyExist)
        //{
        //    if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentAlreadyExist.RecurrenceGroup))
        //    {
        //        appointmentAlreadyExist = DeleteRecurrenceAppointment(toSaveAppointment, appointmentAlreadyExist, dropBoxLink, AppEnum.EditStatus.All);
        //    }
        //    SetAuditFields(toSaveAppointment, true, dropBoxLink);
        //    toSaveAppointment.CreatedBy = appointmentAlreadyExist.CreatedBy;
        //    toSaveAppointment.CreatedTimeStamp = appointmentAlreadyExist.CreatedTimeStamp;
        //    SaveActualAppointment("", toSaveAppointment, dropBoxLink);
        //}
        //private void UpdateSaveAppointmentRecurrenceToRecurrence(string appointmentGuidUrl, Appointment toSaveAppointment, DropBoxLink dropBoxLink, Appointment appointmentAlreadyExist)
        //{
        //    AppEnum.ApplicationRole dropBoxLinkUserRole = AppCommon.GetCurrentUserRole(dropBoxLink.UserRole);
        //    IList<string> appointmentExistRecurrenceList = new List<string>();
        //    Appointment eachRecurrenceListAppointmentDbItem = null;
        //    if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentGuidUrl))
        //    {

        //        DateTime startDateTime = (appointmentAlreadyExist.StartDateTime.Date).Add(toSaveAppointment.StartDateTime.TimeOfDay);
        //        DateTime endDateTime = (appointmentAlreadyExist.EndDateTime.Date).Add(toSaveAppointment.EndDateTime.TimeOfDay);
        //        toSaveAppointment.StartDateTime = startDateTime;
        //        toSaveAppointment.EndDateTime = endDateTime;
        //        IList<Appointment> appointmentsListToSchedule = new List<Appointment>();
        //        if (toSaveAppointment is PatientVisit)
        //        {
        //            IList<PatientVisit> patientVisitAppointmentsListToSchedule = JsonSerializer.DeserializeObject<IList<PatientVisit>>(GetAppointments((PatientVisit)toSaveAppointment, AppEnum.AppointmentTypes.PatientVisit));
        //            appointmentsListToSchedule = patientVisitAppointmentsListToSchedule.Cast<Appointment>().ToList<Appointment>();
        //        }
        //        else if (toSaveAppointment is BlockAppointment)
        //        {
        //            IList<BlockAppointment> blockAppointmentsListToSchedule = JsonSerializer.DeserializeObject<IList<BlockAppointment>>(GetAppointments((BlockAppointment)toSaveAppointment, AppEnum.AppointmentTypes.Block));
        //            appointmentsListToSchedule = blockAppointmentsListToSchedule.Cast<Appointment>().ToList<Appointment>();
        //        }
        //        foreach (string appointmentItem in appointmentAlreadyExist.Recurrence.RecurrenceList)
        //        {
        //            if (toSaveAppointment is PatientVisit)
        //            {
        //                eachRecurrenceListAppointmentDbItem = _patientVisitAppointmentDocument.Get(appointmentItem);
        //            }
        //            else if (toSaveAppointment is PatientVisit)
        //            {
        //                eachRecurrenceListAppointmentDbItem = _blockAppointmentDocument.Get(appointmentItem);
        //            }
        //            Appointment toSaveAppointmentType = appointmentsListToSchedule.SingleOrDefault(pv => pv.StartDateTime.Date.Equals(eachRecurrenceListAppointmentDbItem.StartDateTime.Date));
        //            if (toSaveAppointmentType != null)
        //            {
        //                toSaveAppointmentType.CreatedBy = appointmentAlreadyExist.CreatedBy;
        //                toSaveAppointmentType.CreatedTimeStamp = appointmentAlreadyExist.CreatedTimeStamp;
        //                SetAuditFields(toSaveAppointmentType, true, dropBoxLink);
        //                ClearAppointmentRecurrenceGroup(toSaveAppointmentType);
        //                toSaveAppointmentType.RecurrenceGroup = appointmentAlreadyExist.RecurrenceGroup;
        //                SaveActualAppointment(appointmentItem, toSaveAppointmentType, dropBoxLink);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// delete  patient Visit Appointment
        /// </summary>
        /// <param name="appointmentToSave"></param>
        /// <param name="dropBoxLink"> </param>
        /// <param name="recurrenceEditStatus"></param>
        /// <param name="appointmentAlreadyExist"> </param>
        /// <returns></returns>
        public Appointment DeleteAppointmentType(Appointment appointmentToSave, DropBoxLink dropBoxLink, AppEnum.EditStatus recurrenceEditStatus, Appointment appointmentAlreadyExist)
        {
            if (!AppCommon.CheckIfStringIsEmptyOrNull(appointmentAlreadyExist.Url))
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
            return appointmentAlreadyExist;
        }

        /// <summary>
        /// delete Recurrence Appointment 
        /// </summary>
        /// <param name="appointmentExist"></param>
        /// <param name="dropBoxLink"></param>
        /// <param name="recurrenceEditStatus"></param>
        /// <returns></returns>
        private Appointment DeleteRecurrenceAppointment(Appointment appointmentExist, DropBoxLink dropBoxLink, AppEnum.EditStatus recurrenceEditStatus)
        {
            // get the Url of Recurrence Appointment
            if (appointmentExist.Recurrence.RecurrenceList != null && appointmentExist.Recurrence.RecurrenceList.Count > 0)
            {
                string recurrenceGroupUrl = FormatRecurrenceGroupUrl(_recurrenceDocument.GetAssignmentUrl(dropBoxLink, DocumentPath.Module.RecurrenceGroup), appointmentExist.RecurrenceGroup);
                recurrenceGroupUrl = string.Format(recurrenceGroupUrl, appointmentExist.RecurrenceGroup);
                switch (recurrenceEditStatus)
                {
                    case AppEnum.EditStatus.All:
                        DeleteAppointmentList(appointmentExist);
                        string deleteRecurrenceGroup;
                        _recurrenceDocument.Delete(recurrenceGroupUrl, out deleteRecurrenceGroup);
                        break;
                    case AppEnum.EditStatus.Current:
                        DeleteAnAppointment(appointmentExist.Url);
                        List<string> appointmentSavedUrl = appointmentExist.Recurrence.RecurrenceList.Where(app => !(app.Equals(appointmentExist.Url))).ToList();
                        SaveAppointmentUrlRecurrenceGroup(appointmentSavedUrl, dropBoxLink, appointmentExist.RecurrenceGroup);
                        break;
                }
            }
            return appointmentExist;
        }

        /// <summary>
        /// to delete the list appointment for Patient Visit  & Block Type
        /// </summary>
        /// <param name="appointmentExist"></param>
        private void DeleteAppointmentList(Appointment appointmentExist)
        {
            if (appointmentExist.Recurrence.RecurrenceList != null && appointmentExist.Recurrence.RecurrenceList.Count > 0)
            {
                foreach (string appItem in appointmentExist.Recurrence.RecurrenceList)
                {
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

        private void DeleteAndCreateNewAppointment(Appointment toSaveAppointment, DropBoxLink dropBoxLink, Appointment appointmentAlreadyExist)
        {
            Appointment deletedAppointmentExist = DeleteAppointmentType(toSaveAppointment, dropBoxLink, AppEnum.EditStatus.All, appointmentAlreadyExist);
            //SetAuditFields(toSaveAppointment, true, dropBoxLink); //  set audit fields for modify...
            SetClientAuditFields(toSaveAppointment, true, dropBoxLink);
            //toSaveAppointment.CreatedBy = deletedAppointmentExist.CreatedBy;
            //toSaveAppointment.CreatedTimeStamp = deletedAppointmentExist.CreatedTimeStamp;
            //toSaveAppointment.ChartTimeStamp = deletedAppointmentExist.ChartTimeStamp;
            //toSaveAppointment.Signature = deletedAppointmentExist.Signature;
            SetAppointmentAduitFieldsValues(toSaveAppointment, appointmentAlreadyExist);
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
                    toSaveAppointment.Recurrence.ChartTimeStamp = deletedAppointmentExist.Recurrence.ChartTimeStamp;
                    toSaveAppointment.Recurrence.Signature = deletedAppointmentExist.Recurrence.Signature;
                    //toSaveAppointment.StartDateTime = deletedAppointmentExist.Recurrence.StartDateTime;
                    //toSaveAppointment.EndDateTime = deletedAppointmentExist.Recurrence.EndDateTime;
                }
                else
                {// set audit fields for P.V  recurrence create .. for new 
                    //SetAuditFields(toSaveAppointment.Recurrence, false, dropBoxLink);
                    SetClientAuditFields(toSaveAppointment.Recurrence, false, dropBoxLink);

                }
                SaveActualAppointmentRecurrence("", toSaveAppointment, dropBoxLink);
            }
        }

        /// <summary>
        /// This method is used to save patient in assignment for a New Appointment
        /// </summary>
        /// <param name="assignmentUniqueIdentifier"></param>
        /// <param name="patientItem"></param>
        /// <param name="dropBox"> </param>
        /// <returns></returns>
        public Patient SaveNewAppointmentPatient(string assignmentUniqueIdentifier, Patient patientItem, DropBoxLink dropBox)
        {
            string patientAssignmentUrl = _patientDocument.GetAssignmentUrl(dropBox, DocumentPath.Module.Patients, AppConstants.Create);
            patientAssignmentUrl = _patientDocument.SaveOrUpdate(patientAssignmentUrl, patientItem);
            //patientItem = _patientDocument.Get(patientAssignmentUrl);
            string patientUniqueIdentifier = patientAssignmentUrl.Split('/').Last();
            patientItem.UniqueIdentifier = patientUniqueIdentifier;
            return patientItem;
        }

        /// <summary>
        /// Get class names for calendar Events
        /// </summary>
        /// <param name="appointment"></param>
        /// <returns></returns>
        private string GetCalendarClassForAppointmentType(Appointment appointment)
        {
            if (appointment is PatientVisit)
            {
                return AppCommon.PatientVisitType;
            }
            if (appointment is BlockAppointment)
            {
                return AppCommon.BlockType;
            }
            return AppCommon.OtherType;
        }
        /// <summary>
        /// Get Provider name based on All staff selection.
        /// </summary>
        /// <param name="appointment"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        private string GetProviderName(Appointment appointment, string delimiter)
        {
            if (appointment != null)
            {
                if (appointment.IsAllStaffSelected)
                {
                    return AppCommon.ProviderList.Where(s => s.Key.Equals(AppCommon.AllStaffIdentifier)).Select(s => s.Value.ToString()).SingleOrDefault();
                }
                return _masterService.GetProviderName(appointment.ProviderId, delimiter);
            }
            return String.Empty;
        }

        /// <summary>
        /// Get Title for Calendar Tool tips
        /// </summary>
        /// <param name="appointment"></param>
        /// <param name="calenderView"></param>
        /// <returns></returns>
        private string GetCalendarTitleForAppointmentType(Appointment appointment, string calenderView)
        {
            string titleOfAppointment = string.Empty;
            if (appointment is PatientVisit)
            {
                titleOfAppointment = AppCommon.GetPatientName(appointment.FirstName, appointment.LastName, appointment.MiddleInitial);
            }
            else if (appointment is BlockAppointment)
            {
                titleOfAppointment = appointment.Type;
            }
            else if (appointment is OtherAppointment)
            {
                titleOfAppointment = appointment.Type;
            }
            string statusOfAppointment = AppCommon.GetAppointmentStatusString(appointment.Status);
            string locationOfAppointment = AppCommon.GetStatusLocationString(appointment.StatusLocation);
            if (calenderView == AppEnum.CalendarViewTypes.agendaDay.ToString())
            {
                titleOfAppointment = "<span class='header-text'>" + titleOfAppointment + "</span>" + AppCommon.SemicolonSeperator + GetProviderName(appointment, AppCommon.CommaSeperator);
                if (statusOfAppointment != String.Empty)
                {
                    titleOfAppointment = titleOfAppointment + AppCommon.SemicolonSeperator + statusOfAppointment;
                }
                if (locationOfAppointment != String.Empty)
                {
                    titleOfAppointment = titleOfAppointment + AppCommon.SemicolonSeperator + locationOfAppointment;
                }
            }
            else
            {
                titleOfAppointment = "<span class='header-text'>"
                    + AppCommon.SliceStringAfterLength(titleOfAppointment, AppCommon.EventsTitleMaxLength) + "</span>"
                    + AppCommon.NewLineSeperator
                    + AppCommon.SliceStringAfterLength(GetProviderName(appointment, AppCommon.NewLineSeperator), AppCommon.EventsTitleMaxLength)
                    + AppCommon.NewLineSeperator + AppCommon.SliceStringAfterLength(statusOfAppointment, AppCommon.EventsTitleMaxLength)
                    + AppCommon.NewLineSeperator + AppCommon.SliceStringAfterLength(locationOfAppointment, AppCommon.EventsTitleMaxLength);
            }
            return titleOfAppointment;
        }
        /// <summary>
        /// Check if the appointment is recurrence type
        /// </summary>
        /// <param name="appointment"></param>
        /// <returns></returns>
        private bool CheckIfRecurrenceExists(Appointment appointment)
        {
            if (!String.IsNullOrEmpty(appointment.RecurrenceGroup))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Get tooltip value for calendar events
        /// </summary>
        /// <param name="appointment"></param>
        /// <returns></returns>
        private string GetCalendarTooltipForAppointmentType(Appointment appointment)
        {
            string tooltip = String.Empty;
            if (appointment is PatientVisit)
            {
                tooltip = AppCommon.GetPatientName(appointment.FirstName, appointment.LastName, appointment.MiddleInitial);
            }
            else if (appointment is BlockAppointment)
            {
                tooltip = appointment.Type;
            }
            else if (appointment is OtherAppointment)
            {
                tooltip = appointment.Type;
            }
            tooltip = "<span class='header-text'>"
                        + tooltip
                        + "</span>" + AppCommon.NewLineSeperator
                        + GetProviderName(appointment, AppCommon.NewLineSeperator)
                        + AppCommon.NewLineSeperator
                        + AppCommon.GetDateInGivenFormat(AppCommon.DateHMmTt, appointment.StartDateTime)
                        + AppCommon.HyphenSeperator
                        + AppCommon.GetDateInGivenFormat(AppCommon.DateHMmTt, appointment.EndDateTime);
            return tooltip;
        }

        private string GetAppointmentType(Appointment appointment)
        {
            if (appointment is PatientVisit)
            {
                return AppEnum.AppointmentTypes.PatientVisit.ToString();
            }
            if (appointment is BlockAppointment)
            {
                return AppEnum.AppointmentTypes.Block.ToString();
            }
            return AppEnum.AppointmentTypes.Other.ToString();
        }

        /// <summary>
        /// Get exam room view values for calendar
        /// </summary>
        /// <param name="calendarFilterObject"></param>
        /// <returns></returns>
        public IList<CalendarEventProxy> GetExamRoomViewFilter(CalendarFilterProxy calendarFilterObject)
        {
            IList<CalendarEventProxy> calendarEvents = new List<CalendarEventProxy>();
            calendarFilterObject.CalendarView = AppEnum.CalendarViewTypes.agendaDay.ToString();
            IList<Appointment> appointmentsList = _appointmentDocument.GetPatientVisitAppointments(calendarFilterObject, AppEnum.CalendarFilterTypes.None);
            foreach (var item in appointmentsList)
            {
                CalendarEventProxy calenderEvent = new CalendarEventProxy
                                                       {
                                                           title = GetCalendarTitleForAppointmentType(item, AppEnum.CalendarViewTypes.resourceDay.ToString()),
                                                           tooltip = GetCalendarTooltipForAppointmentType(item),
                                                           className = GetCalendarClassForAppointmentType(item),
                                                           isRecurrence = CheckIfRecurrenceExists(item),
                                                           editable = false,
                                                           start = item.StartDateTime.ToString(),
                                                           end = item.EndDateTime.ToString(),
                                                           allDay = false,
                                                           Url = item.Url,
                                                           PatientName = AppCommon.GetPatientName(item.FirstName, item.LastName, item.MiddleInitial),
                                                           AppointmentType = GetAppointmentType(item),
                                                           resourceId = item.ExamRoomIdentifier,
                                                           IsViewMode=true
                                                       };
                calendarEvents.Add(calenderEvent);
            }
            return calendarEvents;
        }
        /// <summary>
        /// Get calendar list of event proxy  from appointments
        /// </summary>
        /// <param name="appointments"></param>
        /// <param name="calenderView"></param>
        /// <returns></returns>
        private IList<CalendarEventProxy> GetAppointmentsForCalendar(List<Appointment> appointments, string calenderView)
        {
            IList<CalendarEventProxy> calendarEvents = new List<CalendarEventProxy>();
            var appointmentsForEachDay = appointments.GroupBy(s => s.StartDateTime.Date);
            List<Appointment> appointmentsTemp = new List<Appointment>();
            if (calenderView == AppEnum.CalendarViewTypes.month.ToString())
            {
                foreach (var item in appointmentsForEachDay)
                {
                    if (item.Count() > 3)
                    {
                        appointmentsTemp.AddRange(item.OrderBy(f => f.StartDateTime).Take(3).ToList());
                        PatientVisit patientVisit = new PatientVisit { Type = AppCommon.ViewMore, StartDateTime = item.Key };
                        appointmentsTemp.Add(patientVisit);
                    }
                    else
                    {
                        appointmentsTemp.AddRange(item.ToList());
                    }
                }
            }
            else
            {
                appointmentsTemp = appointments;
            }
            foreach (var item in appointmentsTemp)
            {
                CalendarEventProxy calenderEvent = new CalendarEventProxy { allDay = false, editable = false };

                if (item.Type != AppCommon.ViewMore)
                {
                    calenderEvent.title = GetCalendarTitleForAppointmentType(item, calenderView);
                    calenderEvent.tooltip = GetCalendarTooltipForAppointmentType(item);
                    calenderEvent.AppointmentType = GetAppointmentType(item);
                    calenderEvent.className = GetCalendarClassForAppointmentType(item);
                    calenderEvent.start = item.StartDateTime.ToString();
                    calenderEvent.isRecurrence = CheckIfRecurrenceExists(item);
                    calenderEvent.end = item.EndDateTime.ToString();
                    calenderEvent.Url = item.Url;
                    calenderEvent.PatientName = AppCommon.GetPatientName(item.FirstName, item.LastName, item.MiddleInitial);
                    calenderEvent.visittype = item.Type;
                    calenderEvent.timeinterval = AppCommon.GetDateInGivenFormat(AppCommon.DateHMmTt, item.StartDateTime)
                                                             + AppCommon.HyphenSeperator
                                                             + AppCommon.GetDateInGivenFormat(AppCommon.DateHMmTt, item.EndDateTime);
                    calenderEvent.Status = item.Status;
                    calenderEvent.providername = _masterService.GetProviderName(item.ProviderId, AppCommon.NewLineSeperator);
                }
                else
                {
                    calenderEvent.title = item.Type;
                    calenderEvent.className = AppCommon.ViewMoreLink;
                    calenderEvent.start = new DateTime(item.StartDateTime.Year, item.StartDateTime.Month, item.StartDateTime.Day, 23, 59, 59).ToString();
                }
                calendarEvents.Add(calenderEvent);
            }
            return calendarEvents;
        }

        /// <summary>
        /// Get appointments for calendar filter
        /// </summary>
        /// <param name="calendarFilterObject"></param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        public IList<CalendarEventProxy> GetAppointmentsForCalendar(CalendarFilterProxy calendarFilterObject, AppEnum.CalendarFilterTypes filterType)
        {
            IList<Appointment> appointmentsList = _appointmentDocument.GetAppointments(calendarFilterObject, filterType);
            IList<CalendarEventProxy> calendarEvents = GetAppointmentsForCalendar(appointmentsList.ToList(), calendarFilterObject.CalendarView);
            return calendarEvents;
        }

        public bool IsPatientValid(DropBoxLink assignmentCredentials, Patient patientItem)
        {
            bool isPatientValid = true;
            IList<Patient> duplicatePatientList = new List<Patient>();
            IList<Patient> patientList = _patientDocument.GetAllPatientForAssignment(assignmentCredentials);
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
            return false;
        }

        public bool IsStartEndTimeSame(DateTime appointmentStartTime, DateTime appointmentEndTime)
        {
            if (appointmentStartTime.TimeOfDay.Equals(appointmentEndTime.TimeOfDay))
            {
                return true;
            }
            return false;
        }

        public bool IsAppointmentExists(DateTime appointmentStartTime, IList<int> providerId, DropBoxLink dropBoxLink)
        {
            CalendarFilterProxy calendarFilterObject = new CalendarFilterProxy
                                                           {
                                                               CalendarView =
                                                                   AppEnum.CalendarViewTypes.agendaDay.ToString(),
                                                               StartDate = appointmentStartTime,
                                                               EndDate = appointmentStartTime,
                                                               ScenarioId =
                                                                   (dropBoxLink != null) ? dropBoxLink.Sid : "SID",
                                                               CourseId =
                                                                   (dropBoxLink != null) ? dropBoxLink.Cid : "Course",
                                                               UserId = (dropBoxLink != null) ? dropBoxLink.Uid : "UID",
                                                               Role = AppEnum.ApplicationRole.Student
                                                           };
            IList<Appointment> appointmentsList = _appointmentDocument.GetAppointments(calendarFilterObject, AppEnum.CalendarFilterTypes.None);
            appointmentsList =
                (from appointment in appointmentsList
                 where appointment.StartDateTime == appointmentStartTime
                 select appointment).ToList();
            //appointmentsList =
            //    (from appointment in appointmentsList
            //     where appointment.ProviderId[0] == providerId[0]
            //     select appointment).ToList();
            for (int iCount = 0; iCount < providerId.Count(); iCount++)
            {
                IList<Appointment> tempAppointmentList =
                    (from appointment in appointmentsList
                     where appointment.ProviderId.Contains(providerId[iCount])
                     select appointment).ToList();
                if (tempAppointmentList.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public PatientVisit GetPatientVisitAppointment(string appointmentUrl, DropBoxLink dropBoxLink)
        {
            PatientVisit patientVisitEdit = _patientVisitAppointmentDocument.Get(appointmentUrl);
            if (!AppCommon.CheckIfStringIsEmptyOrNull(patientVisitEdit.RecurrenceGroup))
            {
                RecurrenceGroup patientVisitRecurrenceExist = _recurrenceDocument.Get(FormatRecurrenceGroupUrl(_recurrenceDocument.GetAssignmentUrl(dropBoxLink, DocumentPath.Module.RecurrenceGroup), patientVisitEdit.RecurrenceGroup));
                patientVisitEdit.Recurrence = patientVisitRecurrenceExist;
            }
            return patientVisitEdit;
        }

        public BlockAppointment GetBlockAppointment(string appointmentUrl, DropBoxLink dropBoxLink)
        {
            //return _blockAppointmentDocument.Get(appointmentUrl);
            BlockAppointment blockAppointmentEdit = _blockAppointmentDocument.Get(appointmentUrl);
            if (!AppCommon.CheckIfStringIsEmptyOrNull(blockAppointmentEdit.RecurrenceGroup))
            {
                RecurrenceGroup blockRecurrenceExist = _recurrenceDocument.Get(FormatRecurrenceGroupUrl(_recurrenceDocument.GetAssignmentUrl(dropBoxLink, DocumentPath.Module.RecurrenceGroup), blockAppointmentEdit.RecurrenceGroup));
                blockAppointmentEdit.Recurrence = blockRecurrenceExist;
            }
            return blockAppointmentEdit;
        }

        public OtherAppointment GetOtherAppointment(string appointmentUrl, DropBoxLink dropBoxLink)
        {
            //return _blockAppointmentDocument.Get(appointmentUrl);
            OtherAppointment otherAppointmentEdit = _otherAppointmentDocument.Get(appointmentUrl);
            if (!AppCommon.CheckIfStringIsEmptyOrNull(otherAppointmentEdit.RecurrenceGroup))
            {
                RecurrenceGroup otherRecurrenceExist = _recurrenceDocument.Get(FormatRecurrenceGroupUrl(_recurrenceDocument.GetAssignmentUrl(dropBoxLink, DocumentPath.Module.RecurrenceGroup), otherAppointmentEdit.RecurrenceGroup));
                otherAppointmentEdit.Recurrence = otherRecurrenceExist;
            }
            return otherAppointmentEdit;
        }


        /// <summary>
        /// Get all the patients in Patient Repository and given Assignment
        /// </summary>
        /// <returns></returns>
        public IList<Patient> GetAppointmentPatientList(DropBoxLink dropBoxlink)
        {
            return _patientDocument.GetAllPatientForAssignment(dropBoxlink);
        }


        /// <summary>
        /// Get appointment list for a perticular patient
        /// </summary>
        /// <param name="calendarFilter"></param>
        /// <param name="sortColumnIndex"></param>
        /// <param name="sortColumnOrder"></param>
        /// <returns></returns>
        public IList<Appointment> GetAppointmentsForPatientSearch(CalendarFilterProxy calendarFilter, int sortColumnIndex, string sortColumnOrder)
        {
            IList<Appointment> appointments = _appointmentDocument.GetAppointments(calendarFilter, AppEnum.CalendarFilterTypes.Patient);
            string sortColumnName = AppCommon.GridColumnForAppointmentPatientList[sortColumnIndex - 1];
            var sortablePatientList = appointments.AsQueryable();
            appointments = sortablePatientList.OrderBy(sortColumnName, sortColumnOrder).ToList();
            return appointments;
        }


        private bool CancelAppointment(string appointmentUrl, Appointment appointmentToCancel, DropBoxLink dropBoxLink, AppEnum.EditStatus occurrenceStatus)
        {
            switch (occurrenceStatus)
            {
                case AppEnum.EditStatus.All:
                    CancelRecurringAppointment(appointmentUrl, appointmentToCancel, dropBoxLink);
                    break;
                case AppEnum.EditStatus.None:
                case AppEnum.EditStatus.Current:
                    CancelActualAppointment(appointmentUrl, appointmentToCancel, dropBoxLink);
                    break;
            }
            return true;
        }

        private void CancelActualAppointment(string appointmentUrl, Appointment appointmentToCancel, DropBoxLink dropBoxLink)
        {
            appointmentToCancel.Status = (int)AppEnum.AppointmentStatus.Canceled;
            //SetAuditFields(appointmentToCancel, true, dropBoxLink);
            SetClientAuditFields(appointmentToCancel, true, dropBoxLink);
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
