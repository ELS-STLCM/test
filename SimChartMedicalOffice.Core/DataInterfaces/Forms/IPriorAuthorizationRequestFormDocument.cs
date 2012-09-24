using System.Collections.Generic;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Core.DataInterfaces.Forms
{
    public interface IPriorAuthorizationRequestFormDocument : IKeyValueRepository<PriorAuthorizationRequestForm>
    {
        IList<PriorAuthorizationRequestForm> GetAllPriorAuthorizationRequestFormsForPatient(DropBoxLink dropBox,
                                                                                            string patientGuid,
                                                                                            string formId);
        string FormAndSetUrlForStudentPatient(DropBoxLink dropBox,
                                              string patientGuid, string formId);
    }
}
