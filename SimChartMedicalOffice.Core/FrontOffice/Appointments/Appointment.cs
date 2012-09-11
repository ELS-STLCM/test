using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Common;
namespace SimChartMedicalOffice.Core.FrontOffice.Appointments
{
    public abstract class Appointment : DocumentEntity
    {
        //private Appointment()
        //{
        //}
        //public Appointment(string AppointmentTitle, string startDate, string startTime,string endTime,int numberOfOccurences)
        //{

        //}
        //public Appointment(string AppointmentTitle, string startDate, string startTime,string endTime, string recurrenceEndDate)
        //{

        //}

        //private DateTime _startDateTime;
        //private DateTime _endDateTime;
        private bool _isRecurrence = false;
        /// <summary>
        /// For Patient Visit - this property maps to Visit Type
        /// For Block - this property maps to Block Type
        /// For Other - this property maps to Other Type
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Property applicable only for Block and Other type Appointment
        /// </summary>
        public string OtherText { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int Duration { get { return (StartDateTime - EndDateTime).Minutes; } }
        public bool IsRecurrence { get { if (this.Recurrence != null) { return true; } else { return false; } } }
        //For other Appointment we have more than one Attendee
        //For Patient Visit Appointment we have only one Provider
        //For Block Appointment, if "All Staff" was selected then we need to add all the Provider Id to the List
        public int ProviderId { get; set; }
        public string Description { get; set; }
        public string ExamRoomIdentifier { get; set; }
        public int Status { get; set; }
        public int StatusLocation { get; set; }
        public string ReasonForCancellation { get; set; }
        public string RecurrenceGroup { get; set; }
        public RecurrenceGroup Recurrence { get; set; }
        public string PatientIdentifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string Name { get { return string.Format("{1},{0} {2}", FirstName, LastName, MiddleInitial); } }
        //public abstract IList<Appointment> GetAppointments();
        public void ClearRecurrenceGroup()
        {
            this.Recurrence = null;
        }
        //public virtual Appointment Clone()
        //{
        //}
        public virtual Appointment Clone() { return null; }
     
    }
}
