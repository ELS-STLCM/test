using System;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.ProxyObjects;

namespace SimChartMedicalOffice.Data
{
    public static class DbCommon
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="appointmentDate"></param>
        /// <returns></returns>
        public static string GetUrlForMonthFilter(string Url, DateTime appointmentDate)
        {
            //Url = Url.Replace("/{1}/{2}/{3}", "");
            return string.Format(Url, string.Format("{0:yyyyMM}", appointmentDate));
        }

        public static string GetUrlBasedNodeTypeForFilter(string url,CalendarFilterProxy calendarFilter,AppEnum.CalendarFilterTypes calendarFilterType,AppEnum.ProviderType provider)
        {
            //string urlFilter = String.Empty;
            AppEnum.CalendarViewTypes calendarViewTypes = (AppEnum.CalendarViewTypes)Enum.Parse(typeof(AppEnum.CalendarViewTypes), calendarFilter.CalendarView, true);
            switch (calendarViewTypes)
            {
                //For Single Providers   --> "SimApp/Courses/CID/ROLE/UID/Assignments/SID/Appointments/Type/PatientVisit/SingleProvider/{0}/{1}/{2}/{3}/{4}"
                // Example                  --> "SimApp/Courses/CID/ROLE/UID/Assignments/SID/Appointments/Type/PatientVisit/SingleProvider/{YYYYMM}/{WeekOfYear}/{Day}/{ProviderId}/{DictionaryOfAppointments}"

                //For Multiple Providers --> "SimApp/Courses/CID/ROLE/UID/Assignments/SID/Appointments/Type/PatientVisit/SingleProvider/{0}/{1}/{2}/{3}"
                // Example                   --> "SimApp/Courses/CID/ROLE/UID/Assignments/SID/Appointments/Type/PatientVisit/MultipleProviders/{YYYYMM}/{WeekOfYear}/{Day}/{DictionaryOfAppointments}"
                case AppEnum.CalendarViewTypes.month:
                    url= url.Remove(url.IndexOf("/{1}"));
                    return url= string.Format(url, AppCommon.GetMonthNode(calendarFilter.StartDate));
                case AppEnum.CalendarViewTypes.resourceDay:
                case AppEnum.CalendarViewTypes.agendaDay:
                    if (calendarFilterType.Equals(AppEnum.CalendarFilterTypes.Provider) &&
                        provider == AppEnum.ProviderType.SingleProvider)
                    {
                        url = url.Remove(url.IndexOf("/{4}"));
                        url = string.Format(url, AppCommon.GetMonthNode(calendarFilter.StartDate), AppCommon.GetWeekNode(calendarFilter.StartDate),AppCommon.GetDayNode(calendarFilter.StartDate),calendarFilter.ProviderId);
                    }else{
                        url = url.Remove(url.IndexOf("/{3}"));
                         url = string.Format(url, AppCommon.GetMonthNode(calendarFilter.StartDate), AppCommon.GetWeekNode(calendarFilter.StartDate), AppCommon.GetDayNode(calendarFilter.StartDate));
                    }
                    break;
                case AppEnum.CalendarViewTypes.agendaWeek:
                    url = url.Remove(url.IndexOf("/{2}"));
                    if(calendarFilter.StartDate.Month!= calendarFilter.EndDate.Month)
                    {
                        url = string.Format(url, AppCommon.GetMonthNode(calendarFilter.StartDate), AppCommon.GetWeekNode(calendarFilter.StartDate))
                                + AppCommon.DataDelimiter + string.Format(url, AppCommon.GetMonthNode(calendarFilter.EndDate), AppCommon.GetWeekNode(calendarFilter.StartDate));
                    } else
                    {
                        url = string.Format(url, AppCommon.GetMonthNode(calendarFilter.StartDate),
                                      AppCommon.GetWeekNode(calendarFilter.StartDate));
                    }
                    break;
                case AppEnum.CalendarViewTypes.None:
                    url = url.Remove(url.IndexOf("/{0}")); 
                    break;
            }
            return url;
        }
    }
}
