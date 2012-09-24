using System;

namespace SimChartMedicalOffice.Core.FrontOffice.Appointments
{
    public class OtherAppointment : Appointment
    {
        //public IList<BlockAppointment> GetAppointments()
        //{
        //    return null;
        //}
        public override Appointment Clone()
        {
            Type type = GetType();
            OtherAppointment deepCopyData = new OtherAppointment();
            foreach (System.Reflection.PropertyInfo objProp in type.GetProperties())
            {
                if (objProp.CanWrite)
                {
                    objProp.SetValue(deepCopyData, type.GetProperty(objProp.Name).GetValue(this, null), null);
                }
            }
            return deepCopyData;


        }
    }
}
