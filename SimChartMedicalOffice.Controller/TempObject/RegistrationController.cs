using System;
using System.Collections.Generic;
using SimChartMedicalOffice.Core.TempObject;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.TempObject;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class RegistrationController:BaseController
    {
        private readonly IRegistrationService _registrationService;
        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }
        public void NewPatient()
        {
            Registration registrationObject = new Registration
                                                  {
                                                      FirstName = "FirstName" + DateTime.Now.ToString(),
                                                      LastName = "LastName" + DateTime.Now.ToString(),
                                                      Gender = "Male",
                                                      CreatedTimeStamp = DateTime.Now
                                                  };
            //registrationObject.Address = new Address();
            //registrationObject.Address.City = "STL";
            //registrationObject.Address.State = "MO";
            //registrationObject.Address.StreetLine1 = "StreetLine1" + DateTime.Now.ToString();
            //registrationObject.Address.StreetLine2 = "StreetLine1" + DateTime.Now.ToString();
            //registrationObject.Address.ZipCode = "63043";
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
            _registrationService.GetPatientRegistration(patientGuid);
            Address addressObject = new Address
                                        {
                                            City = "STL",
                                            State = "MO",
                                            StreetLine1 = "StreetLine1" + DateTime.Now.ToString(),
                                            StreetLine2 = "StreetLine1" + DateTime.Now.ToString(),
                                            ZipCode = "63043"
                                        };
            _registrationService.SavePatientAddress(addressObject);
        }
        public void AddPatientAppointment(string patientGuid)
        {
            Appointment patientAppointment = new Appointment
                                                 {ScheduledTimeStamp = DateTime.Now.AddDays(3), IsRecurrence = false};


            patientAppointment.IsRecurrence = true;
            _registrationService.SavePatientAppointment(patientGuid,patientAppointment);
        }
        public string GetAllAppointments()
        {
            return JsonSerializer.SerializeObject(_registrationService.GetAllAppointments());
        }
        public string GetAppointmentsJson()
        {
            string jsonString = _registrationService.GetAppointmentsJson();
            JsonSerializer.Jobject(jsonString);
            return jsonString;
        }
        public string GetPatientAppointments(string patientGuid)
        {
            return JsonSerializer.SerializeObject(_registrationService.GetPatientAppointments(patientGuid));
        }
        private bool GetAppointmentList(string jsonString,ref List<Appointment> appointmentList)
        {
            Dictionary<string, object> data = JsonSerializer.DeserializeObject<Dictionary<string, object>>(jsonString);
            foreach (KeyValuePair<string, object> kp in data)
            {
                if (IsDeserializable<Appointment>(kp.Value.ToString()))
                {
                    appointmentList.Add(JsonSerializer.DeserializeObject<Appointment>(kp.Value.ToString()));
                    return true;
                }
                return GetAppointmentList(kp.Value.ToString(),ref appointmentList);
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
