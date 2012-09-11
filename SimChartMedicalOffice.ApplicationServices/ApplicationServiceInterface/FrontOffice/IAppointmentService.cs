using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.FrontOffice
{
   public interface IAppointmentService
    {
       //void SavePatientVisitAppointment(PatientVisit patientVisitAppointment, string courseId, AppEnum.ApplicationRole role, string assignmentScenarioId);
       //void SaveBlockAppointment(BlockAppointment blockAppointment, string courseId, AppEnum.ApplicationRole role, string assignmentScenarioId);
       //void SaveAppointment(CalendarEventProxy calendarEventProxy, string courseId, AppEnum.ApplicationRole role, string assignmentScenarioId);
       void SaveAppointmentType(string appointmentGuid, Appointment appointmentToSave, DropBoxLink dropBoxLink, AppEnum.EditStatus occurrenceStatus);
       //bool SavePatientVisitAppointmentNew(string appointmentDate, Appointment tosaveAppointment);
       Patient SaveNewAppointmentPatient(string assignmentUniqueIdentifier, Patient patientItem);
       IList<CalendarEventProxy> GetAppointmentsForCalendar(CalendarFilterProxy CalendarFilterObject, AppEnum.CalendarFilterTypes filterType);
       bool IsPatientValid(string assignmentUniqueIdentifier, Patient patientItem);
       PatientVisit GetPatientVisitAppointment(string patientVisitAppointmentUrl, DropBoxLink dropBoxLink);
       BlockAppointment GetBlockAppointment(string appointmentUrl, DropBoxLink dropBoxLink);
       IList<CalendarEventProxy> GetExamRoomViewFilter(CalendarFilterProxy CalendarFilterObject);
       bool IsFifteenMinutesAppointment(DateTime appointmentStartTime, DateTime appointmentEndTime);
       IList<Patient> GetAppointmentPatientList(string assignmentUniqueIdentifier);
       IList<Appointment> GetAppointmentsForPatientSearch(CalendarFilterProxy calendarFilter,string patientUniqueIdentifier,int sortColumnIndex,string sortColumnOrder);
       bool IsAppointmentExists(DateTime appointmentStartTime, int lstProviderId, DropBoxLink dropBoxLink);       
    }
}
