using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.FrontOffice;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Common.Utility;

namespace SimChartMedicalOffice.Data.FrontOffice
{
    public class BlockAppointmentDocument : KeyValueRepository<BlockAppointment>, IBlockAppointmentDocument
    {
        public override string Url
        {
            get
            {
                //"SimApp/Courses/{CourseId}/{Role}/Assignments/{SID}/Appointments/Type/Block/{YYYYMM}/{dd}/{ProvideInteger}/{DictionaryofAppointments}
                return _blockAppointmentIdentifier + "/{5}/{6}/{7}";
            }
        }



        //To Form the Url Dynamically to fetch the data for an assignment type;
        private string _blockAppointmentIdentifier = "SimApp/Courses/{0}/{1}/{2}/Assignments/{3}/Appointments/Type/Block/{4}";

        //Format the Url till month node
        private string FormatUrl(string courseId, AppEnum.ApplicationRole role, string assignmentScenarioId, DateTime appointmentDate)
        {
            return string.Format(Url, courseId, AppCommon.GetRoleDescription(role), assignmentScenarioId, string.Format("{0:YYYYmmDD}", appointmentDate));
        }
        /// <summary>
        /// Get all appointments from Day node
        /// </summary>
        /// <param name="appoinmentsForDays"></param>
        /// <returns></returns>
        private List<Appointment> GetAllAppointmentsInADay(Dictionary<string, Dictionary<string, BlockAppointment>> appoinmentsForDays)
        {
            List<Appointment> appointments = new List<Appointment>();
            foreach (var providerItems in appoinmentsForDays.Values)
            {
                foreach (var appoinmentForProvider in providerItems.Values)
                {
                    appointments.Add(appoinmentForProvider);
                }
            }
            return appointments;
        }

        /// <summary>
        /// Get all appointments from a month node
        /// </summary>
        /// <param name="appointmentsInAMonthList"></param>
        /// <param name="calendarFilterType"></param>
        /// <param name="calendarFilter"></param>
        /// <returns></returns>
        private List<Appointment> GetAllAppointmentsInAMonth(Dictionary<string, Dictionary<string, Dictionary<string, BlockAppointment>>> appointmentsInAMonthList, AppEnum.CalendarFilterTypes calendarFilterType, CalendarFilterProxy calendarFilter)
        {
            List<Appointment> appointments = new List<Appointment>();
            //Month Node
            foreach (var itemDays in appointmentsInAMonthList)
            {  //Day Node
                foreach (var itemDay in itemDays.Value)
                //Provider Node
                {  //1 .Check Provider Key value for Provider filter
                    if (calendarFilterType == AppEnum.CalendarFilterTypes.Provider)
                    {
                        //Appointment Node
                        //Check for provider id with key to filter the providers
                        if (itemDay.Key == calendarFilter.ProviderId)
                        {
                            foreach (var itemProvider in itemDay.Value)
                            {
                                appointments.Add(itemProvider.Value);
                            }
                        }
                    }
                    else //2. Else Add all the appointments
                    {
                        foreach (var itemProvider in itemDay.Value)
                        {
                            appointments.Add(itemProvider.Value);
                        }
                    }
                }
            }
            return appointments;
        }

        /// <summary>
        /// Get List of appointments for the filters in calendar.
        /// </summary>
        /// <param name="calendarFilter"></param>
        /// <param name="calendarFilterType"></param>
        /// <returns></returns>
        public List<Appointment> GetAppointmentsForBlockType(CalendarFilterProxy calendarFilter, AppEnum.CalendarFilterTypes calendarFilterType)
        {
            AppEnum.CalendarViewTypes calendarViewTypes = (AppEnum.CalendarViewTypes)Enum.Parse(typeof(AppEnum.CalendarViewTypes), calendarFilter.CalendarView, true);
            List<string> urlsForAppointments = new List<string>();
            List<Appointment> appointments = new List<Appointment>();
            switch (calendarViewTypes)
            {
                case AppEnum.CalendarViewTypes.month:
                    urlsForAppointments.Add(AppCommon.GetUrlForMonthFilter(_blockAppointmentIdentifier, calendarFilter.CourseId, calendarFilter.Role, calendarFilter.ScenarioId, calendarFilter.StartDate,calendarFilter.UserId));
                    foreach (var urlItem in urlsForAppointments)
                    {
                        string jsonString = GetJsonDocument(urlItem);
                        //Month/day/provider/appointments
                        Dictionary<string, Dictionary<string, Dictionary<string, BlockAppointment>>> appointmentsInAMonthList = AppCommon.isValidJsonString(jsonString) ? JsonSerializer.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, BlockAppointment>>>>(jsonString) : new Dictionary<string, Dictionary<string, Dictionary<string, BlockAppointment>>>();
                        appointments = GetAllAppointmentsInAMonth(appointmentsInAMonthList, calendarFilterType, calendarFilter);
                    }
                    break;
                case AppEnum.CalendarViewTypes.agendaDay:
                case AppEnum.CalendarViewTypes.agendaWeek:
                    urlsForAppointments = new List<string>();
                    urlsForAppointments.Add(AppCommon.GetUrlForMonthFilter(_blockAppointmentIdentifier, calendarFilter.CourseId, calendarFilter.Role, calendarFilter.ScenarioId, calendarFilter.StartDate,calendarFilter.UserId));
                    List<string> calendarEventsIdentifiersTemp = new List<string>();
                    DateTime startDateTemp = calendarFilter.StartDate;
                    while (startDateTemp <= calendarFilter.EndDate)
                    {
                        foreach (var item in urlsForAppointments)
                        {
                            string appointmentForADay = AppCommon.GetUrlForDateFilter(item, startDateTemp);
                            calendarEventsIdentifiersTemp.Add(appointmentForADay);
                            startDateTemp = startDateTemp.AddDays(1);
                        }
                    }
                    urlsForAppointments = calendarEventsIdentifiersTemp;
                    foreach (var item in urlsForAppointments)
                    {
                        string jsonString;
                        string itemTemp = item;
                        if (calendarFilterType == AppEnum.CalendarFilterTypes.Provider)
                        {
                            itemTemp = item + "/" + calendarFilter.ProviderId;
                            jsonString = GetJsonDocument(itemTemp);
                            //provider/appointments
                            Dictionary<string, BlockAppointment> appointmentForDays = AppCommon.isValidJsonString(jsonString) ? JsonSerializer.DeserializeObject<Dictionary<string, BlockAppointment>>(jsonString) : new Dictionary<string, BlockAppointment>();
                            appointments.AddRange(appointmentForDays.Select(a => a.Value).ToList());
                        }
                        else
                        {
                            jsonString = GetJsonDocument(itemTemp);
                            //day/provider/appointments
                            Dictionary<string, Dictionary<string, BlockAppointment>> appoinmentsForDays = AppCommon.isValidJsonString(jsonString) ? JsonSerializer.DeserializeObject<Dictionary<string, Dictionary<string, BlockAppointment>>>(jsonString) : new Dictionary<string, Dictionary<string, BlockAppointment>>();
                            appointments.AddRange(GetAllAppointmentsInADay(appoinmentsForDays));
                        }
                    }
                    break;
                default:
                    break;
            }


            switch (calendarFilterType)
            {
                case AppEnum.CalendarFilterTypes.ExamRoom:
                    appointments = (from lstAppointment in appointments where lstAppointment.ExamRoomIdentifier.Equals(calendarFilter.ExamRoom) select lstAppointment).ToList();
                    break;
                default:
                    break;
            }
            appointments = (from lstappointments in appointments where lstappointments.Status != 4 select lstappointments).ToList();
            return appointments;
        } 
    }
}
