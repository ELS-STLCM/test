using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Common.Utility;

namespace SimChartMedicalOffice.ApplicationServices.FrontOffice.AppointmentPatterns
{
    public class BlockStrategy : IAppointmentTypeStrategy
    {
        private BlockStrategy()
        {
        }
        private BlockAppointment _blockAppointmentDocument;
        IList<BlockAppointment> _blockAppointments;
        public BlockStrategy(BlockAppointment appointmentDocument)
        {
            _blockAppointmentDocument = appointmentDocument;
        }
        public string GenerateAppointments()
        {
            DateTime endByDate;
            _blockAppointments = new List<BlockAppointment>();
            if (_blockAppointmentDocument.IsRecurrence == true)
            {
                if (_blockAppointmentDocument.Recurrence.Pattern == Common.AppEnum.RecurrencePattern.Daily)
                {
                    if (_blockAppointmentDocument.Recurrence.NumberOfOccurences > 0)
                    {
                        DailyAppointmentsByOccurence();
                    }
                    else if (_blockAppointmentDocument.Recurrence.EndBy != null && DateTime.TryParse(_blockAppointmentDocument.Recurrence.EndBy, out endByDate))
                    {
                        DailyAppointmentByEndDate(endByDate);
                    }
                }
                else if (_blockAppointmentDocument.Recurrence.Pattern == Common.AppEnum.RecurrencePattern.Weekly)
                {
                    if (_blockAppointmentDocument.Recurrence.NumberOfOccurences > 0)
                    {
                        WeeklyAppointmentsByOccurence();
                    }
                    else if (_blockAppointmentDocument.Recurrence.EndBy != null && DateTime.TryParse(_blockAppointmentDocument.Recurrence.EndBy, out endByDate))
                    {
                        WeeklyAppointmentByEndDate(endByDate);
                    }
                }
            }
            return JsonSerializer.SerializeObject(_blockAppointments);
        }
        private void DailyAppointmentsByOccurence()
        {
            //int numberOfOccurenceCreated = 0;
            //int numberOfOccurenceToCreate;
            //numberOfOccurenceToCreate = _blockAppointmentDocument.Recurrence.NumberOfOccurences;
            //while (numberOfOccurenceCreated <= numberOfOccurenceToCreate)
            //{
            //    BlockAppointment newAppointment;
            //    newAppointment = _blockAppointmentDocument.Clone();
            //    newAppointment.StartDateTime = newAppointment.StartDateTime.AddDays(numberOfOccurenceCreated);
            //    newAppointment.EndDateTime = newAppointment.EndDateTime.AddDays(numberOfOccurenceCreated);
            //    _blockAppointments.Add(newAppointment);
            //    numberOfOccurenceCreated++;
            //}
        }
        private void WeeklyAppointmentsByOccurence()
        {
            //int numberOfOccurenceCreated = 0;
            //int numberOfOccurenceToCreate;
            //numberOfOccurenceToCreate = _blockAppointmentDocument.Recurrence.NumberOfOccurences;
            //while (numberOfOccurenceCreated <= numberOfOccurenceToCreate)
            //{
            //    BlockAppointment newAppointment;
            //    newAppointment = _blockAppointmentDocument.Clone();
            //    newAppointment.StartDateTime = newAppointment.StartDateTime.AddDays(7 * numberOfOccurenceCreated);
            //    newAppointment.EndDateTime = newAppointment.EndDateTime.AddDays(7 * numberOfOccurenceCreated);
            //    _blockAppointments.Add(newAppointment);
            //    numberOfOccurenceCreated++;
            //}
        }
        private void WeeklyAppointmentByEndDate(DateTime endBy)
        {
            //DateTime startDateTime;
            //startDateTime = _blockAppointmentDocument.StartDateTime;
            //int numberOfOccurenceCreated = 0;
            //while (startDateTime <= endBy)
            //{
            //    BlockAppointment newAppointment;
            //    newAppointment = _blockAppointmentDocument.Clone();
            //    newAppointment.StartDateTime = newAppointment.StartDateTime.AddDays(7 * numberOfOccurenceCreated);
            //    newAppointment.EndDateTime = newAppointment.EndDateTime.AddDays(7 * numberOfOccurenceCreated);
            //    startDateTime = startDateTime.AddDays(1);
            //    _blockAppointments.Add(newAppointment);
            //    numberOfOccurenceCreated++;
            //}
        }
        private void DailyAppointmentByEndDate(DateTime endBy)
        {
            //DateTime startDateTime;
            //startDateTime = _blockAppointmentDocument.StartDateTime;
            //int numberOfOccurenceCreated = 0;
            //while (startDateTime <= endBy)
            //{
            //    BlockAppointment newAppointment;
            //    newAppointment = _blockAppointmentDocument.Clone();
            //    newAppointment.StartDateTime = newAppointment.StartDateTime.AddDays(numberOfOccurenceCreated);
            //    newAppointment.EndDateTime = newAppointment.EndDateTime.AddDays(numberOfOccurenceCreated);
            //    startDateTime = startDateTime.AddDays(1);
            //    _blockAppointments.Add(newAppointment);
            //    numberOfOccurenceCreated++;
            //}
        }
        private void MonthlyAppointmentByOccurence()
        {
            //int numberOfOccurenceCreated = 0;
            //int numberOfOccurenceToCreate;
            //numberOfOccurenceToCreate = _blockAppointmentDocument.Recurrence.NumberOfOccurences;
            //while (numberOfOccurenceCreated <= numberOfOccurenceToCreate)
            //{
            //    BlockAppointment newAppointment;
            //    newAppointment = _blockAppointmentDocument.Clone();
            //    newAppointment.StartDateTime = newAppointment.StartDateTime.AddMonths(numberOfOccurenceCreated);
            //    newAppointment.EndDateTime = newAppointment.EndDateTime.AddMonths(numberOfOccurenceCreated);
            //    _blockAppointments.Add(newAppointment);
            //    numberOfOccurenceCreated++;
            //}
        }
        private void MonthlyAppointmentByEndDate(DateTime endBy)
        {
            //DateTime startDateTime;
            //startDateTime = _blockAppointmentDocument.StartDateTime;
            //int numberOfOccurenceCreated = 0;
            //while (startDateTime <= endBy)
            //{
            //    BlockAppointment newAppointment;
            //    newAppointment = _blockAppointmentDocument.Clone();
            //    newAppointment.StartDateTime = newAppointment.StartDateTime.AddMonths(numberOfOccurenceCreated);
            //    newAppointment.EndDateTime = newAppointment.EndDateTime.AddMonths(numberOfOccurenceCreated);
            //    startDateTime = startDateTime.AddMonths(1);
            //    _blockAppointments.Add(newAppointment);
            //    numberOfOccurenceCreated++;
            //}
        }
    }
}
