using SimChartMedicalOffice.Core.DataInterfaces.TempObject;

namespace SimChartMedicalOffice.ApplicationServices.TempObject
{
    public class RegistrationService 
    {
       // private IRegistrationDocument _registrationDocument;
       // private IAppointmentDocument _appointmentDocument;

        private const string AppointmentBaseUrl = "SimApp/Appointments";

        public RegistrationService(IRegistrationDocument registrationDocumentInstance,
                                    IAppointmentDocument appointmentDocument)
        {
          //  _registrationDocument = registrationDocumentInstance;
      //      _appointmentDocument = appointmentDocument;
        }
        //public bool SavePatientRegistration(Registration registrationObject)
        //{
        //    _registrationDocument.SaveOrUpdate(_registrationDocument.GetAssignmentUrl(), registrationObject);
        //    return true;
        //}
        //public bool SavePatientAppointment(string patientGuid, Appointment appointmentData)
        //{
        //    Registration patientRegistration = GetPatientRegistration(patientGuid);
        //    string appointmentUrlList;
        //    appointmentUrlList = string.Format(_appointmentDocument.GetAssignmentUrl(), (string.Format("{0:MM-yyyy/dd}", appointmentData.ScheduledTimeStamp)));

        //    appointmentUrlList = _appointmentDocument.SaveOrUpdate(appointmentUrlList, appointmentData);
        //    if (patientRegistration.AppointmentReference == null)
        //    {
        //        patientRegistration.AppointmentReference = new List<string>();
        //    }
        //    patientRegistration.AppointmentReference.Add(appointmentUrlList);
        //    appointmentData.PatientReference = _registrationDocument.SaveOrUpdate(_registrationDocument.GetAssignmentUrl(), patientRegistration);
        //    _appointmentDocument.SaveOrUpdate(string.Format(_appointmentDocument.GetAssignmentUrl(), (string.Format("{0:MM-yyyy/dd}", appointmentData.ScheduledTimeStamp))), appointmentData);

        //    return true;
        //}
        //public string GetAppointmentsJson()
        //{
        //    return _appointmentDocument.GetJsonDocument(APPOINTMENT_BASE_URL);
        //}
        //public IList<Appointment> GetAllAppointments()
        //{
        //    return _appointmentDocument.GetAll(string.Format(_appointmentDocument.GetAssignmentUrl(),""));
        //}
        //public IList<Appointment> GetPatientAppointments(string patientGuid)
        //{
        //    return _appointmentDocument.GetAll(string.Format(_appointmentDocument.GetAssignmentUrl(), patientGuid));
        //}
        //public bool SavePatientPhone(PhoneNumber phoneData) { return true; }
        //public bool SavePatientAddress(Address addressData)
        //{

        //    return true;
        //}
        //public Registration GetPatientRegistration(string patientGuid)
        //{
        //    return _registrationDocument.Get(_registrationDocument.GetAssignmentUrl(),patientGuid);
        //}
        //public IList<Registration> GetAllPatientRegistration()
        //{
        //    return _registrationDocument.GetAll(_registrationDocument.GetAssignmentUrl());
        //}
    }
}
