using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.TempObject;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.TempObject;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class RegistrationController:BaseController
    {
        private IRegistrationService _registrationService;
        public RegistrationController(IRegistrationService registrationService)
        {
            this._registrationService = registrationService;
        }
        public void NewPatient()
        {
            Registration registrationObject = new Registration();
            registrationObject.FirstName = "FirstName" + DateTime.Now.ToString();
            registrationObject.LastName = "LastName" + DateTime.Now.ToString();
            //registrationObject.Address = new Address();
            //registrationObject.Address.City = "STL";
            //registrationObject.Address.State = "MO";
            //registrationObject.Address.StreetLine1 = "StreetLine1" + DateTime.Now.ToString();
            //registrationObject.Address.StreetLine2 = "StreetLine1" + DateTime.Now.ToString();
            //registrationObject.Address.ZipCode = "63043";
            registrationObject.Gender = "Male";
            registrationObject.CreatedTimeStamp = DateTime.Now;
            //registrationObject.PhoneNumber = new PhoneNumber();
            //registrationObject.PhoneNumber.EmergencyContact = "123-456-7890";
            //registrationObject.PhoneNumber.Home = "123-456-HOME";
            //registrationObject.PhoneNumber.Mobile = "123-4MO-BILE";
            //registrationObject.PhoneNumber.WorkPlace = "1WO-RKP-LACE";
            //registrationObject.AptoAppointment = new AptoList<AppointmentApto>();
            //registrationObject.AptoAppointment.Collections = new Dictionary<string, AppointmentApto>();
            //AppointmentApto aptoAppt=new AppointmentApto();
            //aptoAppt.BookTimeStamp = DateTime.Now;
            //aptoAppt.Status = 1;
            //registrationObject.AptoAppointment.Collections.Add(Guid.NewGuid().ToString(),aptoAppt);
            //aptoAppt = new AppointmentApto();
            //aptoAppt.BookTimeStamp = DateTime.Now;
            //aptoAppt.Status = 1;
            //aptoAppt.IsRecurrence = true;
            //registrationObject.AptoAppointment.Collections.Add(Guid.NewGuid().ToString(), aptoAppt);

            _registrationService.SavePatientRegistration(registrationObject);

        }
        public string GetAllPatient()
        {
            return JsonSerializer.SerializeObject(_registrationService.GetAllPatientRegistration());
        }
        public void SavePatientAddress(string patientGuid)
        {
            Registration registrationObject;
            Address addressObject;
            registrationObject = _registrationService.GetPatientRegistration(patientGuid);
            addressObject = new Address();
            addressObject.City = "STL";
            addressObject.State = "MO";
            addressObject.StreetLine1 = "StreetLine1" + DateTime.Now.ToString();
            addressObject.StreetLine2 = "StreetLine1" + DateTime.Now.ToString();
            addressObject.ZipCode = "63043";
            _registrationService.SavePatientAddress(addressObject);
        }
        public void AddPatientAppointment(string patientGuid)
        {
            Appointment patientAppointment = new Appointment();           
            patientAppointment.ScheduledTimeStamp = DateTime.Now.AddDays(3);            
            patientAppointment.IsRecurrence = false;                
            patientAppointment.IsRecurrence = true;
            _registrationService.SavePatientAppointment(patientGuid,patientAppointment);
        }
        public string GetAllAppointments()
        {
            return JsonSerializer.SerializeObject(_registrationService.GetAllAppointments());
        }
        public string GetAppointmentsJson()
        {
            string jsonString;
            jsonString = _registrationService.GetAppointmentsJson();
            //Dictionary<string, string> data = JsonSerializer.DeserializeObject<Dictionary<string, string>>(jsonString);
            List<Appointment> appointmentList = new List<Appointment>();
            //GetAppointmentList(jsonString, ref appointmentList);
            JsonSerializer.Jobject(jsonString);
            return jsonString;
        }
        public string GetPatientAppointments(string patientGuid)
        {
            return JsonSerializer.SerializeObject(_registrationService.GetPatientAppointments(patientGuid));
        }
        private bool GetAppointmentList(string jsonString,ref List<Appointment> appointmentList)
        {
            Dictionary<string, object> data;
            data = JsonSerializer.DeserializeObject<Dictionary<string, object>>(jsonString);
            foreach (KeyValuePair<string, object> kp in data)
            {
                if (IsDeserializable<Appointment>(kp.Value.ToString()))
                {
                    appointmentList.Add(JsonSerializer.DeserializeObject<Appointment>(kp.Value.ToString()));
                    return true;
                }
                else
                {
                    return GetAppointmentList(kp.Value.ToString(),ref appointmentList);
                }
            }
            return true;
        }
        private bool IsDeserializable<T>(string jsonDocument) where T:DocumentEntity
        {
            try
            {
                T data=JsonSerializer.DeserializeObject<T>(jsonDocument);
                if (data.UniqueIdentifier == null)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
