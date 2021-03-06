﻿using System.Collections.Generic;

namespace SimChartMedicalOffice.Core.DataInterfaces
{
    public interface IMasterDocument
    {
        Dictionary<int, string> GetPatientProviderUrl();
        List<string> GetPatientInsuranceUrl();
        List<string> GetAppointmentVisitType();
        List<string> GetExamRooms();
        List<string> GetAppointmentStatus();
        List<string> GetBlockType();
        List<string> GetOtherType();
    }
}
