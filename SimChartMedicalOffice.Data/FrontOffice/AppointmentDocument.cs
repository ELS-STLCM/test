using System;
using System.Collections.Generic;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.DataInterfaces.FrontOffice;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Data.FrontOffice
{
    public class AppointmentDocument : KeyValueRepository<Appointment>,IAppointmentDocument
    {
        private readonly IBlockAppointmentDocument _blockAppointmentDocument;
        private readonly IOtherAppointmentDocument _otherAppointmentDocument;
        private readonly IPatientVisitAppointmentDocument _patientVisitAppointmentDocument;
       

        public AppointmentDocument(IPatientVisitAppointmentDocument patientVisitAppointmentDocument,
                                    IBlockAppointmentDocument blockAppointmentDocument,
                                    IOtherAppointmentDocument otherAppointmentDocument 
                                  )
        {
            _blockAppointmentDocument = blockAppointmentDocument;
            _otherAppointmentDocument = otherAppointmentDocument;
            _patientVisitAppointmentDocument = patientVisitAppointmentDocument; 
        }
        public IList<Appointment> GetAppointments(CalendarFilterProxy calendarFilter, AppEnum.CalendarFilterTypes filterType)
        {
            List<Appointment> appointments = new List<Appointment>();
            //1) Check if filtering is based on appointment type. 
            //2) Get only the appointmets for particular type.
              switch (filterType)
                {
                    case AppEnum.CalendarFilterTypes.AppointmentType:
                        {
                            AppEnum.AppointmentTypes typeOfAppointment = (AppEnum.AppointmentTypes)Enum.Parse(typeof(AppEnum.AppointmentTypes), calendarFilter.AppointmentType, true);
                            switch (typeOfAppointment)
                            {
                                case AppEnum.AppointmentTypes.PatientVisit:
                                    {
                                        appointments.AddRange(GetPatientVisitAppointments(calendarFilter, filterType));
                                        break;
                                    }
                                case AppEnum.AppointmentTypes.Block:
                                    {
                                        appointments.AddRange(GetBlockedTypeAppointments(calendarFilter, filterType));
                                        break;
                                    }
                                case AppEnum.AppointmentTypes.Other:
                                    {
                                        appointments.AddRange(GetAppointmentsForOtherType(calendarFilter, filterType));
                                        break;
                                    }
                            } 
                            break;
                        }
                    default:
                        appointments.AddRange(GetPatientVisitAppointments(calendarFilter, filterType));
                        appointments.AddRange(GetBlockedTypeAppointments(calendarFilter, filterType));
                        appointments.AddRange(GetAppointmentsForOtherType(calendarFilter, filterType));
                        break;  
            }
              return appointments;
        }
 
        public List<Appointment> GetPatientVisitAppointments(CalendarFilterProxy calendarFilter, AppEnum.CalendarFilterTypes filterType)
        {
            return _patientVisitAppointmentDocument.GetAppointmentsForPatientVisit(calendarFilter, filterType);
        }

        public List<Appointment> GetBlockedTypeAppointments(CalendarFilterProxy calendarFilter, AppEnum.CalendarFilterTypes filterType)
        {
            return _blockAppointmentDocument.GetAppointmentsForBlockType(calendarFilter, filterType);
        }

        public List<Appointment> GetAppointmentsForOtherType(CalendarFilterProxy calendarFilter, AppEnum.CalendarFilterTypes filterType)
        {
            return _otherAppointmentDocument.GetAppointmentsForOtherType(calendarFilter, filterType);
        }
        
    }
}
