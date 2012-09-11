using System.Collections.Generic;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.Forms
{
    public interface IReferralFormDocument : IKeyValueRepository<ReferralForm>
    {
        IList<ReferralForm> GetAllReferralFormsForPatient(string courseId, string userRole, string UID, string SID,
                                                          string patientGuid, string formId);
        string FormAndSetUrlForStudentPatient(string courseId, string userRole, string UID, string SID,
                                              string patientGuid, string formId);
    }
}
