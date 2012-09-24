using System;
using System.Collections.Generic;
using System.Linq;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DataInterfaces.FrontOffice;
using SimChartMedicalOffice.Core.DropBox;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Data.FrontOffice
{
    public class OtherAppointmentDocument : KeyValueRepository<OtherAppointment>, IOtherAppointmentDocument
    {
        #region Appointments- Deserialiazation based on View Types of calendar and Providers

        /// <summary>
        ///  Get all appointments under block type node
        /// </summary>
        /// <param name="providerType"></param>
        /// <param name="urlForAppointments"> </param>
        /// <returns></returns>
        private List<Appointment> GetAllAppointmentsInOther(AppEnum.ProviderType providerType, string urlForAppointments)
        {
            List<Appointment> appointmentsInOtherAppointment = new List<Appointment>();
            switch (providerType)
            {
                case AppEnum.ProviderType.SingleProvider:
                    Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, OtherAppointment>>>>> otherAppointmentAppointments = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, OtherAppointment>>>>>();
                    foreach (var months in otherAppointmentAppointments)
                    {
                        foreach (var weeks in months.Value)
                        {
                            foreach (var days in weeks.Value)
                            {
                                foreach (var providers in days.Value)
                                {
                                    foreach (var appointments in providers.Value)
                                    {
                                        appointmentsInOtherAppointment.Add(appointments.Value);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case AppEnum.ProviderType.MultiProvider:
                    Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, OtherAppointment>>>> multiProviderOtherAppointment = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, OtherAppointment>>>>();
                    foreach (var months in multiProviderOtherAppointment)
                    {
                        foreach (var weeks in months.Value)
                        {
                            foreach (var days in weeks.Value)
                            {
                                foreach (var appointments in days.Value)
                                {
                                    appointmentsInOtherAppointment.Add(appointments.Value);
                                }
                            }
                        }
                    }
                    break;
                case AppEnum.ProviderType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("providerType");
            }

            return appointmentsInOtherAppointment;
        }

        /// <summary>
        /// Get all appointments from Day node
        /// </summary>
        /// <param name="urlToGetAppointments"></param>
        /// <param name="providerType"></param>
        /// <returns></returns>
        private List<Appointment> GetAllAppointmentsInDay(string urlToGetAppointments, AppEnum.ProviderType providerType)
        {
            List<Appointment> appointments = new List<Appointment>();
            string jsonString;
            switch (providerType)
            {
                case AppEnum.ProviderType.SingleProvider:
                    jsonString = GetJsonDocument(urlToGetAppointments);
                    Dictionary<string, Dictionary<string, OtherAppointment>> appoinmentsForADaySingleProvider = AppCommon.IsValidJsonString(jsonString) ? JsonSerializer.DeserializeObject<Dictionary<string, Dictionary<string, OtherAppointment>>>(jsonString) : new Dictionary<string, Dictionary<string, OtherAppointment>>();
                    foreach (var appointmentsDay in appoinmentsForADaySingleProvider)
                    {//Provider Node
                        foreach (var appointment in appointmentsDay.Value) //ListoAppintmentsf 
                        {
                            appointments.Add(appointment.Value);
                        }
                    }
                    break;
                case AppEnum.ProviderType.MultiProvider:
                    jsonString = GetJsonDocument(urlToGetAppointments);
                    Dictionary<string, OtherAppointment> appoinmentsForADay = AppCommon.IsValidJsonString(jsonString) ? JsonSerializer.DeserializeObject<Dictionary<string, OtherAppointment>>(jsonString) : new Dictionary<string, OtherAppointment>();
                    foreach (var appointmentDay in appoinmentsForADay)
                    {
                        appointments.Add(appointmentDay.Value);
                    }
                    break;
                case AppEnum.ProviderType.None:
                    break;
            }

            return appointments;
        }

        /// <summary>
        /// Get all appointments from Week node
        /// </summary>
        /// <param name="urlToGetAppointments"></param>
        /// <param name="providerType"></param>
        /// <returns></returns>
        private List<Appointment> GetAllAppointmentsInWeek(string urlToGetAppointments, AppEnum.ProviderType providerType)
        {
            List<Appointment> appointments = new List<Appointment>();
            string jsonString;
            switch (providerType)
            {
                case AppEnum.ProviderType.SingleProvider:
                    jsonString = GetJsonDocument(urlToGetAppointments);
                    Dictionary<string, Dictionary<string, Dictionary<string, OtherAppointment>>> appoinmentsForAWeekSingleProvider = AppCommon.IsValidJsonString(jsonString) ? JsonSerializer.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, OtherAppointment>>>>(jsonString) : new Dictionary<string, Dictionary<string, Dictionary<string, OtherAppointment>>>();
                    //days
                    foreach (var appointmentWeek in appoinmentsForAWeekSingleProvider)
                    {//providers
                        foreach (var providers in appointmentWeek.Value)
                        {//appointment
                            foreach (var appointment in providers.Value)
                            {
                                appointments.Add(appointment.Value);
                            }
                        }
                    }
                    break;
                case AppEnum.ProviderType.MultiProvider:
                    jsonString = GetJsonDocument(urlToGetAppointments);
                    Dictionary<string, Dictionary<string, OtherAppointment>> appoinmentsForAWeek = AppCommon.IsValidJsonString(jsonString) ? JsonSerializer.DeserializeObject<Dictionary<string, Dictionary<string, OtherAppointment>>>(jsonString) : new Dictionary<string, Dictionary<string, OtherAppointment>>();
                    //days
                    foreach (var appointmentWeek in appoinmentsForAWeek)
                    {//appointments
                        foreach (var appointment in appointmentWeek.Value)
                        {
                            appointments.Add(appointment.Value);
                        }
                    }
                    break;
                case AppEnum.ProviderType.None:
                    break;
            }

            return appointments;
        }

        /// <summary>
        /// Get all appointments from a month node
        /// </summary>
        /// <param name="urlTogetAppointments"></param>
        /// <param name="providerType"></param>
        /// <returns></returns>
        private List<Appointment> GetAllAppointmentsInAMonth(string urlTogetAppointments, AppEnum.ProviderType providerType)
        {
            List<Appointment> appointments = new List<Appointment>();
            string jsonString;

            switch (providerType)
            {
                case AppEnum.ProviderType.SingleProvider:
                    jsonString = GetJsonDocument(urlTogetAppointments);
                    Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, OtherAppointment>>>> appointmentsSingleProvider = AppCommon.IsValidJsonString(jsonString) ? JsonSerializer.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, OtherAppointment>>>>>(jsonString) : new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, OtherAppointment>>>>();
                    //Weeks
                    foreach (var weeks in appointmentsSingleProvider)
                    { //Week
                        foreach (var week in weeks.Value)
                        {    //Day
                            foreach (var days in week.Value)
                            { //Appointments for provider
                                foreach (var appointmentsInADay in days.Value)
                                {
                                    appointments.Add(appointmentsInADay.Value);
                                }
                            }
                        }
                    }
                    break;
                case AppEnum.ProviderType.MultiProvider:
                    jsonString = GetJsonDocument(urlTogetAppointments);
                    Dictionary<string, Dictionary<string, Dictionary<string, OtherAppointment>>> appointmentsMultiProvider = AppCommon.IsValidJsonString(jsonString) ? JsonSerializer.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, OtherAppointment>>>>(jsonString) : new Dictionary<string, Dictionary<string, Dictionary<string, OtherAppointment>>>();
                    //weeks
                    foreach (var weeks in appointmentsMultiProvider)
                    {  //Days
                        foreach (var days in weeks.Value)
                        { //Appointments
                            foreach (var dayAppointment in days.Value)
                            {
                                appointments.Add(dayAppointment.Value);
                            }
                        }
                    }
                    break;
                case AppEnum.ProviderType.None:
                    break;
            }
            return appointments;
        }
        #endregion
        #region Calendar Filters
        /// <summary>
        /// Get List of appointments for the filters in calendar.
        /// </summary>
        /// <param name="calendarFilter"></param>
        /// <param name="calendarFilterType"></param>
        /// <returns></returns>
        public List<Appointment> GetAppointmentsForOtherType(CalendarFilterProxy calendarFilter, AppEnum.CalendarFilterTypes calendarFilterType)
        {
            AppEnum.CalendarViewTypes calendarViewTypes = (AppEnum.CalendarViewTypes)Enum.Parse(typeof(AppEnum.CalendarViewTypes), calendarFilter.CalendarView, true);
            //List<string> urlsForAppointments = new List<string>();
            string urlToGetAppointments;
            List<Appointment> appointments = new List<Appointment>();
            DropBoxLink dropBox = new DropBoxLink
            {
                Cid = calendarFilter.CourseId,
                Sid = calendarFilter.ScenarioId,
                Uid = calendarFilter.UserId,
                UserRole = AppCommon.GetRoleDescription(calendarFilter.Role)
            };
            if (calendarFilterType == AppEnum.CalendarFilterTypes.Patient)
            {
                urlToGetAppointments = DbCommon.GetUrlBasedNodeTypeForFilter(
                                                    GetAssignmentUrl(dropBox, DocumentPath.Module.OtherAppointment,
                                                                               AppEnum.ProviderType.SingleProvider.ToString()), calendarFilter,
                                                                                calendarFilterType, AppEnum.ProviderType.SingleProvider);

                appointments.AddRange(GetAllAppointmentsInOther(AppEnum.ProviderType.SingleProvider, urlToGetAppointments));
                urlToGetAppointments = DbCommon.GetUrlBasedNodeTypeForFilter(
                                                    GetAssignmentUrl(dropBox, DocumentPath.Module.OtherAppointment,
                                                                               AppEnum.ProviderType.MultiProvider.ToString()), calendarFilter,
                                                                                calendarFilterType, AppEnum.ProviderType.MultiProvider);
                appointments.AddRange(GetAllAppointmentsInOther(AppEnum.ProviderType.MultiProvider, urlToGetAppointments));
            }
            else
            {
                switch (calendarViewTypes)
                {
                    case AppEnum.CalendarViewTypes.month:
                        urlToGetAppointments =
                            DbCommon.GetUrlBasedNodeTypeForFilter(
                                GetAssignmentUrl(dropBox, DocumentPath.Module.OtherAppointment,
                                                 AppEnum.ProviderType.SingleProvider.ToString()), calendarFilter,
                                calendarFilterType, AppEnum.ProviderType.SingleProvider);
                        appointments.AddRange(GetAllAppointmentsInAMonth(urlToGetAppointments,
                                                                         AppEnum.ProviderType.SingleProvider));
                        urlToGetAppointments =
                            DbCommon.GetUrlBasedNodeTypeForFilter(
                                GetAssignmentUrl(dropBox, DocumentPath.Module.OtherAppointment,
                                                 AppEnum.ProviderType.MultiProvider.ToString()), calendarFilter,
                                calendarFilterType, AppEnum.ProviderType.MultiProvider);
                        appointments.AddRange(GetAllAppointmentsInAMonth(urlToGetAppointments,
                                                                         AppEnum.ProviderType.MultiProvider));
                        break;
                    case AppEnum.CalendarViewTypes.agendaDay:
                        urlToGetAppointments =
                            DbCommon.GetUrlBasedNodeTypeForFilter(
                                GetAssignmentUrl(dropBox, DocumentPath.Module.OtherAppointment,
                                                 AppEnum.ProviderType.SingleProvider.ToString()), calendarFilter,
                                calendarFilterType, AppEnum.ProviderType.SingleProvider);
                        appointments.AddRange(GetAllAppointmentsInDay(urlToGetAppointments,
                                                                      AppEnum.ProviderType.SingleProvider));
                        urlToGetAppointments =
                            DbCommon.GetUrlBasedNodeTypeForFilter(
                                GetAssignmentUrl(dropBox, DocumentPath.Module.OtherAppointment,
                                                 AppEnum.ProviderType.MultiProvider.ToString()), calendarFilter,
                                calendarFilterType, AppEnum.ProviderType.MultiProvider);
                        appointments.AddRange(GetAllAppointmentsInDay(urlToGetAppointments,
                                                                      AppEnum.ProviderType.MultiProvider));
                        break;
                    case AppEnum.CalendarViewTypes.agendaWeek:
                        urlToGetAppointments =
                            DbCommon.GetUrlBasedNodeTypeForFilter(
                                GetAssignmentUrl(dropBox, DocumentPath.Module.OtherAppointment,
                                                 AppEnum.ProviderType.SingleProvider.ToString()), calendarFilter,
                                calendarFilterType, AppEnum.ProviderType.SingleProvider);
                        var appointmentUrlSingle = AppCommon.GetValuesFromDelimitedString(urlToGetAppointments,
                                                                                          AppCommon.DataDelimiter);
                        foreach (var appointmentSingle in appointmentUrlSingle)
                        {
                            appointments.AddRange(GetAllAppointmentsInWeek(appointmentSingle,
                                                                           AppEnum.ProviderType.SingleProvider));
                        }
                        urlToGetAppointments =
                            DbCommon.GetUrlBasedNodeTypeForFilter(
                                GetAssignmentUrl(dropBox, DocumentPath.Module.OtherAppointment,
                                                 AppEnum.ProviderType.MultiProvider.ToString()), calendarFilter,
                                calendarFilterType, AppEnum.ProviderType.MultiProvider);
                        var appointmentUrlMultiple = AppCommon.GetValuesFromDelimitedString(urlToGetAppointments,
                                                                                            AppCommon.DataDelimiter);
                        foreach (var appointmentMulti in appointmentUrlMultiple)
                        {
                            appointments.AddRange(GetAllAppointmentsInWeek(appointmentMulti,
                                                                           AppEnum.ProviderType.MultiProvider));
                        }
                        break;
                }

            }
            switch (calendarFilterType)
                {
                    case AppEnum.CalendarFilterTypes.ExamRoom:
                        appointments = (from lstAppointment in appointments
                                        where lstAppointment.ExamRoomIdentifier.Equals(calendarFilter.ExamRoom)
                                        select lstAppointment).ToList();
                        break;
                    case AppEnum.CalendarFilterTypes.Provider:
                        appointments = (from lstAppointment in appointments
                                        where
                                            lstAppointment.ProviderId.Contains(Convert.ToInt32(calendarFilter.ProviderId))
                                        select lstAppointment).ToList();
                        break;
                    case AppEnum.CalendarFilterTypes.Patient:
                        appointments = (from lstAppointment in appointments
                                        where
                                            lstAppointment.PatientIdentifier.Equals(calendarFilter.PatientGuid)
                                        select lstAppointment).ToList();
                        break;
                }
            
            appointments = (from lstappointments in appointments where lstappointments.Status != 4 select lstappointments).ToList();
            return appointments;
        }
        #endregion 
    }
}
