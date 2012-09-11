﻿using System.Collections.Generic;
using System;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;

namespace SimChartMedicalOffice.Core.ProxyObjects
{
    public class CalendarEventProxy : DocumentEntity
    {
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string className { get; set; }
        public bool editable { get; set; }
        public bool allDay { get; set; }
        public string url { get; set; }  
        public string AppointmentType { get; set; }
        public string PatientName { get; set; }
        public string resourceId { get; set; } 
        public string tooltip { get; set; }
        public int Status { get; set; }
        public bool isRecurrence { get; set; }
   
    }
}