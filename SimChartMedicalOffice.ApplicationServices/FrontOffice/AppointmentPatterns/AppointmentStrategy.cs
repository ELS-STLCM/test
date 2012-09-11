using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Common.Utility;

namespace SimChartMedicalOffice.ApplicationServices.FrontOffice.AppointmentPatterns
{
    public class AppointmentStrategy<T> :IAppointmentTypeStrategy where T:Appointment
    {
        T _appointmentDocument;
        IList<T> _appoinmentDocumentList;
        AppEnum.AppointmentTypes _appointmentType;
        //,AppEnum.AppointmentTypes appointmentType;
        public AppointmentStrategy(T appointmentDocument)
        {
            _appointmentDocument = appointmentDocument;
            //_appointmentType = appointmentType;
        }
        public string GenerateAppointments()
        {

            DateTime endByDate;
            _appoinmentDocumentList = new List<T>();
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
            return JsonSerializer.SerializeObject(_appoinmentDocumentList);
        }
        private void DailyAppointmentsByOccurence()
        {
            int numberOfOccurenceCreated = 0;
            int numberOfOccurenceToCreate;
            numberOfOccurenceToCreate = _appointmentDocument.Recurrence.NumberOfOccurences;
            while (numberOfOccurenceCreated < numberOfOccurenceToCreate)
            {
                T newAppointment;
                newAppointment = (T)_appointmentDocument.Clone();
                newAppointment.StartDateTime = newAppointment.StartDateTime.AddDays(numberOfOccurenceCreated);
                newAppointment.EndDateTime = newAppointment.EndDateTime.AddDays(numberOfOccurenceCreated);
                _appoinmentDocumentList.Add(newAppointment);
                numberOfOccurenceCreated++;
            }
        }
        private void WeeklyAppointmentsByOccurence()
        {
            int numberOfOccurenceCreated = 0;
            int numberOfOccurenceToCreate;
            numberOfOccurenceToCreate = _appointmentDocument.Recurrence.NumberOfOccurences;
            while (numberOfOccurenceCreated < numberOfOccurenceToCreate)
            {
                T newAppointment;
                newAppointment = (T)_appointmentDocument.Clone();
                newAppointment.StartDateTime = newAppointment.StartDateTime.AddDays(7 * numberOfOccurenceCreated);
                newAppointment.EndDateTime = newAppointment.EndDateTime.AddDays(7 * numberOfOccurenceCreated);
                _appoinmentDocumentList.Add(newAppointment);
                numberOfOccurenceCreated++;
            }
        }
        private void WeeklyAppointmentByEndDate(DateTime endBy)
        {
            DateTime startDateTime;
            startDateTime = _appointmentDocument.StartDateTime;
            int numberOfOccurenceCreated = 0;
            while (startDateTime <= endBy)
            {
                T newAppointment;
                newAppointment =(T)_appointmentDocument.Clone();
                newAppointment.StartDateTime = newAppointment.StartDateTime.AddDays(7 * numberOfOccurenceCreated);
                newAppointment.EndDateTime = newAppointment.EndDateTime.AddDays(7 * numberOfOccurenceCreated);
                startDateTime = startDateTime.AddDays(1);
                _appoinmentDocumentList.Add(newAppointment);
                numberOfOccurenceCreated++;
            }
        }
        private void DailyAppointmentByEndDate(DateTime endBy)
        {
            DateTime startDateTime;
            startDateTime = _appointmentDocument.StartDateTime;
            int numberOfOccurenceCreated = 0;
            while (startDateTime <= endBy)
            {
                T newAppointment;
                newAppointment = (T)_appointmentDocument.Clone();
                newAppointment.StartDateTime = newAppointment.StartDateTime.AddDays(numberOfOccurenceCreated);
                newAppointment.EndDateTime = newAppointment.EndDateTime.AddDays(numberOfOccurenceCreated);
                startDateTime = startDateTime.AddDays(1);
                _appoinmentDocumentList.Add(newAppointment);
                numberOfOccurenceCreated++;
            }
        }
        private void MonthlyAppointmentByOccurence()
        {
            int numberOfOccurenceCreated = 0;
            int numberOfOccurenceToCreate;
            numberOfOccurenceToCreate = _appointmentDocument.Recurrence.NumberOfOccurences;
            while (numberOfOccurenceCreated < numberOfOccurenceToCreate)
            {
                T newAppointment;
                newAppointment = (T)_appointmentDocument.Clone();
                newAppointment.StartDateTime = newAppointment.StartDateTime.AddMonths(numberOfOccurenceCreated);
                newAppointment.EndDateTime = newAppointment.EndDateTime.AddMonths(numberOfOccurenceCreated);
                _appoinmentDocumentList.Add(newAppointment);
                numberOfOccurenceCreated++;
            }
        }
        private void MonthlyAppointmentByEndDate(DateTime endBy)
        {
            DateTime startDateTime;
            startDateTime = _appointmentDocument.StartDateTime;
            int numberOfOccurenceCreated = 0;
            while (startDateTime <= endBy)
            {
                T newAppointment;
                newAppointment = (T)_appointmentDocument.Clone();
                newAppointment.StartDateTime = newAppointment.StartDateTime.AddMonths(numberOfOccurenceCreated);
                newAppointment.EndDateTime = newAppointment.EndDateTime.AddMonths(numberOfOccurenceCreated);
                startDateTime = startDateTime.AddMonths(1);
                _appoinmentDocumentList.Add(newAppointment);
                numberOfOccurenceCreated++;
            }
        }
    }
}
