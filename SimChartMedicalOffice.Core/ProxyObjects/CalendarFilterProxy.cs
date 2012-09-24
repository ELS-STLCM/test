using System;
using SimChartMedicalOffice.Common;

namespace SimChartMedicalOffice.Core.ProxyObjects
{
    public class CalendarFilterProxy
    {
        public string CalendarView { get; set; }
        public string ProviderId { get; set; }
        public string AppointmentType { get; set; }
        public string Patient { get; set; }
        public string ExamRoom { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CurrentDate { get; set; }
        public string CourseId { get; set; }
        public string ScenarioId { get; set; }
        public string UserId { get; set; }
        public string PatientGuid { get; set; }
        public AppEnum.ApplicationRole Role { get; set; }
    }
}