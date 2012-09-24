using System.Collections.Generic;
using SimChartMedicalOffice.Core.TempObject;

namespace SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.TempObject
{
    public interface IRegistrationService
    {
        bool SavePatientRegistration(Registration registrationObject);
        bool SavePatientAppointment(string patientGuid,Appointment appointmentData);
        bool SavePatientPhone(PhoneNumber phoneData);
        bool SavePatientAddress(Address addressData);

        Registration GetPatientRegistration(string patientGuid);
        IList<Registration> GetAllPatientRegistration();
        IList<Appointment> GetAllAppointments();
        IList<Appointment> GetPatientAppointments(string patientGuid);
        string GetAppointmentsJson();
    }
}
