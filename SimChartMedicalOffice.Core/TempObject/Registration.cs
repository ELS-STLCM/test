using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimChartMedicalOffice.Core.TempObject
{
    public class Registration:DocumentEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public Address Address { get; set; }
        public string Gender { get; set; }
        public byte[] Avatar { get; set; }
        //public AptoList<AppointmentApto> AptoAppointment { get; set; }
        public List<string> AppointmentReference { get; set; }

    }

    //public class AppointmentApto:DocumentEntity
    //{
    //    public DateTime BookTimeStamp { get; set; }
    //    public int ReminderTime { get; set; }
    //    public AptoList<AppointmentDetails> Details { get; set; }
    //    public int Status { get; set; }
    //    public bool IsRecurrence { get; set; }
    //}
    public class AppointmentEntry
    {
        public Dictionary<DateTime, Appointment> appointmentEntry { get; set; }
    }
    public class Appointment:DocumentEntity
    {
        public DateTime ScheduledTimeStamp { get; set; }        
        public List<AppointmentDetails> Details { get; set; }        
        public bool IsRecurrence { get; set; }
        public int ReminderTime { get; set; }
        public string PatientReference { get; set; }
    }
    public class AppointmentDetails:DocumentEntity
    {
        public string Description { get; set; }
        public DateTime BookedTimeStamp { get; set; }
        public int Duration { get; set; }
        public int Status { get; set; }        
    }
    public class PhoneNumber:DocumentEntity
    {
        public string Home { get; set; }
        public string Mobile { get; set; }
        public string WorkPlace { get; set; }
        public string EmergencyContact { get; set; }
    }
    public class Address:DocumentEntity
    {
        public string StreetLine1 { get; set; }
        public string StreetLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}
