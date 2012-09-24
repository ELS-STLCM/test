
namespace SimChartMedicalOffice.Core.FrontOfficeCalendar
{
    public class Appointment : DocumentEntity
    {
        /// <summary>
        /// This property holds the type of the appointment-- enum value
        /// </summary>
        public string AppointmentType { get; set; }

        /// <summary>
        /// This property holds the sub type of the appointment- Visit Type, Block Type, Other Type
        /// </summary>
        public string AppointmentSubType { get; set; }

        /// <summary>
        /// This property holds the Provider data
        /// </summary>
        public string AppointmentFor { get; set; }

        /// <summary>
        /// This property holds the Provider data- enum value
        /// </summary>
        public string ExamRoom { get; set; }

        /// <summary>
        /// This property holds patient reference- Always one to one
        /// </summary>
        public string PatientReferenceGuid { get; set; }

        /// <summary>
        /// This property holds flag for information verification
        /// </summary>
        public string IsVerified { get; set; }

        /// <summary>
        /// This property holds the start date time of an appointment
        /// </summary>
        public string StartDateTime { get; set; }

        /// <summary>
        /// This property holds the end date time of an appointment
        /// </summary>
        public string EndDateTime { get; set; }

        /// <summary>
        /// This property holds flag for Recurrence
        /// </summary>
        public string IsRecurrence { get; set; }

        /// <summary>
        /// This property holds the Recurrence pattern
        /// </summary>
        public string RecurrencePattern { get; set; }

        /// <summary>
        /// This property holds the number of Occurences for the recurring appointment
        /// </summary>
        public string RecurrenceTotalOccurences { get; set; }

        /// <summary>
        /// This property holds the Recurrence pattern
        /// </summary>
        public string RecurrenceEndDate { get; set; }

        /// <summary>
        /// This property holds the description of an appointment
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// This property holds the status of an appointment
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// This property holds the status of an appointment
        /// </summary>
        public string CancellationReason { get; set; }
        
        /// <summary>
        /// This property holds the Created Time of the Appointment
        /// </summary>
        public string CreatedOn { get; set; }
        
    }
}