using System.Collections.Generic;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Core.DataInterfaces.Forms
{
    public interface IPatientRecordsAccessFormDocument : IKeyValueRepository<PatientRecordsAccessForm>
    {
        IList<PatientRecordsAccessForm> GetAllPatientRecordsAccessFormsForPatient(DropBoxLink dropBox,
                                                                                  string patientGuid, string formId);
        string FormAndSetUrlForStudentPatient(DropBoxLink dropBox,
                                              string patientGuid, string formId);
    }
}
