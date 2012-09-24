using System;
using System.Collections.Generic;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.DropBox;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.ProxyObjects;

namespace SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.FrontOffice
{
   public interface IAppointmentService
    {
       //void SavePatientVisitAppointment(PatientVisit patientVisitAppointment, string courseId, AppEnum.ApplicationRole role, string assignmentScenarioId);
       //void SaveBlockAppointment(BlockAppointment blockAppointment, string courseId, AppEnum.ApplicationRole role, string assignmentScenarioId);
       //void SaveAppointment(CalendarEventProxy calendarEventProxy, string courseId, AppEnum.ApplicationRole role, string assignmentScenarioId);
       string SaveAppointmentType(string appointmentGuid, Appointment appointmentToSave, DropBoxLink dropBoxLink, AppEnum.EditStatus occurrenceStatus);
       //bool SavePatientVisitAppointmentNew(string appointmentDate, Appointment tosaveAppointment);
       Patient SaveNewAppointmentPatient(string assignmentUniqueIdentifier, Patient patientItem,DropBoxLink dropBox);
       IList<CalendarEventProxy> GetAppointmentsForCalendar(CalendarFilterProxy calendarFilterObject, AppEnum.CalendarFilterTypes filterType);
       bool IsPatientValid(DropBoxLink dropBox, Patient patientItem);
       PatientVisit GetPatientVisitAppointment(string patientVisitAppointmentUrl, DropBoxLink dropBoxLink);
       BlockAppointment GetBlockAppointment(string appointmentUrl, DropBoxLink dropBoxLink);
       IList<CalendarEventProxy> GetExamRoomViewFilter(CalendarFilterProxy calendarFilterObject);
       bool IsFifteenMinutesAppointment(DateTime appointmentStartTime, DateTime appointmentEndTime);
       bool IsStartEndTimeSame(DateTime appointmentStartTime, DateTime appointmentEndTime);
       IList<Patient> GetAppointmentPatientList(DropBoxLink dropBoxlink);
       IList<Appointment> GetAppointmentsForPatientSearch(CalendarFilterProxy calendarFilter,int sortColumnIndex,string sortColumnOrder);
       bool IsAppointmentExists(DateTime appointmentStartTime, IList<int> lstProviderId, DropBoxLink dropBoxLink);
       Appointment DeleteAppointmentType(Appointment appointmentToSave, DropBoxLink dropBoxLink, AppEnum.EditStatus recurrenceEditStatus,
                                         Appointment appointmentAlreadyExist);
       Appointment GetAppointment(string appointmentUrl, Appointment appointmentToGet, DropBoxLink dropBoxLink);
       OtherAppointment GetOtherAppointment(string appointmentUrl, DropBoxLink dropBoxLink);
    }
}
