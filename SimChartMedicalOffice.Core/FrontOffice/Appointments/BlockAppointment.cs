﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SimChartMedicalOffice.Core.FrontOffice.Appointments
{
    public class BlockAppointment : Appointment
    {
        public override Appointment Clone()
        {
            Type type = this.GetType();
            BlockAppointment deepCopyData = new BlockAppointment();
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

