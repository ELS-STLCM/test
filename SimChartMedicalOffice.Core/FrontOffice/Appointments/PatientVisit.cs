using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimChartMedicalOffice.Core.FrontOffice.Appointments
{
    public class PatientVisit:Appointment
    {
        //Property stores Patient Guid, Patient can reside either PatientRepository (or) within Assigment
        //public class Patient
        //{
        //    public string PatientIdentifier { get; set; }
        //    public string FirstName { get; set; }
        //    public string LastName { get; set; }
        //    public string MiddleInitial { get; set; }
        //    public string Name { get { return string.Format("{1},{0} {2}", FirstName, LastName, MiddleInitial); } }
        //}
        public bool IsInformationVerified { get; set; }
        //public string VisitType { get; set; }
        //public override IList<Appointment> GetAppointments()
        //{
        //    IList<PatientVisit> patientVisitAppointment = new List<PatientVisit>();
        //    if (IsRecurrence == true)
        //    {
                
        //    }
        //    return null;
        //}
        public override Appointment Clone()
        {
            Type type = this.GetType();
            PatientVisit deepCopyData = new PatientVisit();
            foreach (System.Reflection.PropertyInfo objProp in type.GetProperties())
            {
                if (objProp.CanWrite)
                {
                    objProp.SetValue(deepCopyData, type.GetProperty(objProp.Name).GetValue(this, null), null);
                }
            }
            return (Appointment)deepCopyData;
        }
    }
}
