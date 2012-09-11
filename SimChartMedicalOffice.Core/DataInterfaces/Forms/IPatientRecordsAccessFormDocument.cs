using System.Collections.Generic;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.Forms
{
    public interface IPatientRecordsAccessFormDocument : IKeyValueRepository<PatientRecordsAccessForm>
    {
        IList<PatientRecordsAccessForm> GetAllPatientRecordsAccessFormsForPatient(string courseId, string userRole,
                                                                                  string UID, string SID,
                                                                                  string patientGuid, string formId);
        string FormAndSetUrlForStudentPatient(string courseId, string userRole, string UID, string SID,
                                              string patientGuid, string formId);
    }
}
