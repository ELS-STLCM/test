using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.Forms
{
    public interface IBillOfRightsDocument : IKeyValueRepository<BillOfRights>
    {
        BillOfRights GetBillOfRightsDocument(string patientGUID, string courseId, string userRole, string UID, string SID);
        string SaveBillOfRightsUrl(string patientGUID, string courseId, string userRole, string UID, string SID);
        Attachment GetBillOfRightsPdfData();
    }
}
