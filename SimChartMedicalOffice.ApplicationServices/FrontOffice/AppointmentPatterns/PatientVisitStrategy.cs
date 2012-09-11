using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Common.Utility;

namespace SimChartMedicalOffice.ApplicationServices.FrontOffice.AppointmentPatterns
{
    public class PatientVisitStrategy : IAppointmentTypeStrategy
    {
        private PatientVisitStrategy()
        {
        }
        private PatientVisit _appointmentDocument;
        IList<PatientVisit> _patientVisitAppointments;
        public PatientVisitStrategy(PatientVisit appointmentDocument)
        {
            _appointmentDocument = appointmentDocument;
        }
        public string GenerateAppointments()
        {
            DateTime endByDate;
            _patientVisitAppointments = new List<PatientVisit>();
            if (_appointmentDocument.IsRecurrence == true)
            {
                if (_appointmentDocument.Recurrence.Pattern == Common.AppEnum.RecurrencePattern.Daily)
                {
                    if (_appointmentDocument.Recurrence.NumberOfOccurences > 0)
                    {
                        DailyAppointmentsByOccurence();
                    }
                    else if (_appointmentDocument.Recurrence.EndBy != null && DateTime.TryParse(_appointmentDocument.Recurrence.EndBy, out endByDate))
                    {
                        DailyAppointmentByEndDate(endByDate);
                    }
                }
                else if (_appointmentDocument.Recurrence.Pattern == Common.AppEnum.RecurrencePattern.Weekly)
                {
                    if (_appointmentDocument.Recurrence.NumberOfOccurences > 0)
                    {
                        WeeklyAppointmentsByOccurence();
                    }
                    else if (_appointmentDocument.Recurrence.EndBy != null && DateTime.TryParse(_appointmentDocument.Recurrence.EndBy, out endByDate))
                    {
                        WeeklyAppointmentByEndDate(endByDate);
                    }
                }
                else if (_appointmentDocument.Recurrence.Pattern == Common.AppEnum.RecurrencePattern.Monthly)
                {
                    if (_appointmentDocument.Recurrence.NumberOfOccurences > 0)
                    {
                        MonthlyAppointmentByOccurence();
                    }
                    else if (_appointmentDocument.Recurrence.EndBy != null && DateTime.TryParse(_appointmentDocument.Recurrence.EndBy, out endByDate))
                    {
                        MonthlyAppointmentByEndDate(endByDate);
                    }
                }
            }
            return JsonSerializer.SerializeObject(_patientVisitAppointments);
        }
        private void DailyAppointmentsByOccurence()
        {
            //int numberOfOccurenceCreated = 0;
            //int numberOfOccurenceToCreate;
            //numberOfOccurenceToCreate = _appointmentDocument.Recurrence.NumberOfOccurences;
            //while (numberOfOccurenceCreated <= numberOfOccurenceToCreate)
            //{
            //    PatientVisit newAppointment;
            //    newAppointment = _appointmentDocument.Clone();
            //    newAppointment.StartDateTime = newAppointment.StartDateTime.AddDays(numberOfOccurenceCreated);
            //    newAppointment.EndDateTime = newAppointment.EndDateTime.AddDays(numberOfOccurenceCreated);
            //    _patientVisitAppointments.Add(newAppointment);
            //    numberOfOccurenceCreated++;
            //}
        }
        private void WeeklyAppointmentsByOccurence()
        {
            //int numberOfOccurenceCreated = 0;
            //int numberOfOccurenceToCreate;
            //numberOfOccurenceToCreate = _appointmentDocument.Recurrence.NumberOfOccurences;
            //while (numberOfOccurenceCreated <= numberOfOccurenceToCreate)
            //{
            //    PatientVisit newAppointment;
            //    newAppointment = _appointmentDocument.Clone();
            //    newAppointment.StartDateTime = newAppointment.StartDateTime.AddDays(7 * numberOfOccurenceCreated);
            //    newAppointment.EndDateTime = newAppointment.EndDateTime.AddDays(7 * numberOfOccurenceCreated);
            //    _patientVisitAppointments.Add(newAppointment);
            //    numberOfOccurenceCreated++;
            //}
        }
        private void WeeklyAppointmentByEndDate(DateTime endBy)
        {
            //DateTime startDateTime;
            //startDateTime = _appointmentDocument.StartDateTime;
            //int numberOfOccurenceCreated = 0;
            //while (startDateTime <= endBy)
            //{
            //    PatientVisit newAppointment;
            //    newAppointment = _appointmentDocument.Clone();
            //    newAppointment.StartDateTime = newAppointment.StartDateTime.AddDays(7 * numberOfOccurenceCreated);
            //    newAppointment.EndDateTime = newAppointment.EndDateTime.AddDays(7 * numberOfOccurenceCreated);
            //    startDateTime = startDateTime.AddDays(1);
            //    _patientVisitAppointments.Add(newAppointment);
            //    numberOfOccurenceCreated++;
            //}
        }
        private void DailyAppointmentByEndDate(DateTime endBy)
        {
            //DateTime startDateTime;
            //startDateTime = _appointmentDocument.StartDateTime;
            //int numberOfOccurenceCreated = 0;
            //while (startDateTime <= endBy)
            //{
            //    PatientVisit newAppointment;
            //    newAppointment = _appointmentDocument.Clone();
            //    newAppointment.StartDateTime = newAppointment.StartDateTime.AddDays(numberOfOccurenceCreated);
            //    newAppointment.EndDateTime = newAppointment.EndDateTime.AddDays(numberOfOccurenceCreated);
            //    startDateTime = startDateTime.AddDays(1);
            //    _patientVisitAppointments.Add(newAppointment);
            //    numberOfOccurenceCreated++;
            //}
        }
        private void MonthlyAppointmentByOccurence()
        {
            //int numberOfOccurenceCreated = 0;
            //int numberOfOccurenceToCreate;
            //numberOfOccurenceToCreate = _appointmentDocument.Recurrence.NumberOfOccurences;
            //while (numberOfOccurenceCreated <= numberOfOccurenceToCreate)
            //{
            //    PatientVisit newAppointment;
            //    newAppointment = _appointmentDocument.Clone();
            //    newAppointment.StartDateTime = newAppointment.StartDateTime.AddMonths(numberOfOccurenceCreated);
            //    newAppointment.EndDateTime = newAppointment.EndDateTime.AddMonths(numberOfOccurenceCreated);
            //    _patientVisitAppointments.Add(newAppointment);
            //    numberOfOccurenceCreated++;
            //}
        }
        private void MonthlyAppointmentByEndDate(DateTime endBy)
        {
            //DateTime startDateTime;
            //startDateTime = _appointmentDocument.StartDateTime;
            //int numberOfOccurenceCreated = 0;
            //while (startDateTime <= endBy)
            //{
            //    PatientVisit newAppointment;
            //    newAppointment = _appointmentDocument.Clone();
            //    newAppointment.StartDateTime = newAppointment.StartDateTime.AddMonths(numberOfOccurenceCreated);
            //    newAppointment.EndDateTime = newAppointment.EndDateTime.AddMonths(numberOfOccurenceCreated);
            //    startDateTime = startDateTime.AddMonths(1);
            //    _patientVisitAppointments.Add(newAppointment);
            //    numberOfOccurenceCreated++;
            //}
        }
    }
}
