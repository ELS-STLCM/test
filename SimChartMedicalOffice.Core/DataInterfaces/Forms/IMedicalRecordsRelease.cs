using System.Collections.Generic;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Core.DataInterfaces.Forms
{
    public interface IMedicalRecordsRelease : IKeyValueRepository<MedicalRecordsRelease>
    {
        IList<MedicalRecordsRelease> GetAllMedicalRecordsReleaseFormsForPatient(DropBoxLink dropBox,
                                                                                            string patientGuid,
                                                                                            string formId);
        string FormAndSetUrlForStudentPatient(DropBoxLink dropBox,
                                              string patientGuid, string formId);

        //System.Collections.Generic.IList<MedicalRecordsRelease> GetAllMedicalRecordsReleaseFormsForPatient(DropBoxLink dropBox, string patientGuid, string formId);
    }
}
