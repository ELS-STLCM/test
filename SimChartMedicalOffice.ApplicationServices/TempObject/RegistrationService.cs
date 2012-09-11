using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.TempObject;
using SimChartMedicalOffice.Core.DataInterfaces.TempObject;
using SimChartMedicalOffice.Core.TempObject;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;

namespace SimChartMedicalOffice.ApplicationServices.TempObject
{
    public class RegistrationService : IRegistrationService
    {
        private IRegistrationDocument _registrationDocument;
        private IAppointmentDocument _appointmentDocument;

        private const string APPOINTMENT_BASE_URL = "SimApp/Appointments";

        public RegistrationService(IRegistrationDocument registrationDocumentInstance,
                                    IAppointmentDocument appointmentDocument)
        {
            this._registrationDocument = registrationDocumentInstance;
            this._appointmentDocument = appointmentDocument;
        }
        public bool SavePatientRegistration(Registration registrationObject)
        {
            _registrationDocument.SaveOrUpdate(_registrationDocument.Url,registrationObject);
            return true;
        }
        public bool SavePatientAppointment(string patientGuid, Appointment appointmentData)
        {
            Registration patientRegistration = GetPatientRegistration(patientGuid);
            string appointmentUrlList;
            appointmentUrlList = string.Format(_appointmentDocument.Url, (string.Format("{0:MM-yyyy/dd}", appointmentData.ScheduledTimeStamp)));

            appointmentUrlList = _appointmentDocument.SaveOrUpdate(appointmentUrlList, appointmentData);
            if (patientRegistration.AppointmentReference == null)
            {
                patientRegistration.AppointmentReference = new List<string>();
            }
            patientRegistration.AppointmentReference.Add(appointmentUrlList);
            appointmentData.PatientReference = _registrationDocument.SaveOrUpdate(_registrationDocument.Url, patientRegistration);
            _appointmentDocument.SaveOrUpdate(string.Format(_appointmentDocument.Url, (string.Format("{0:MM-yyyy/dd}", appointmentData.ScheduledTimeStamp))), appointmentData);

            return true;
        }
        public string GetAppointmentsJson()
        {
            return _appointmentDocument.GetJsonDocument(APPOINTMENT_BASE_URL);
        }
        public IList<Appointment> GetAllAppointments()
        {
            return _appointmentDocument.GetAll(string.Format(_appointmentDocument.Url,""));
        }
        public IList<Appointment> GetPatientAppointments(string patientGuid)
        {
            return _appointmentDocument.GetAll(string.Format(_appointmentDocument.Url, patientGuid));
        }
        public bool SavePatientPhone(PhoneNumber phoneData) { return true; }
        public bool SavePatientAddress(Address addressData)
        {

            return true;
        }
        public Registration GetPatientRegistration(string patientGuid)
        {
            return _registrationDocument.Get(_registrationDocument.Url,patientGuid);
        }
        public IList<Registration> GetAllPatientRegistration()
        {
            return _registrationDocument.GetAll(_registrationDocument.Url);
        }
    }
}
