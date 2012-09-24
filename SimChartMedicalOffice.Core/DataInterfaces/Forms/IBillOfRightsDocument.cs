using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Core.DataInterfaces.Forms
{
    public interface IBillOfRightsDocument : IKeyValueRepository<BillOfRights>
    {
        BillOfRights GetBillOfRightsDocument(string patientGuid, DropBoxLink dropBox);
        string SaveBillOfRightsUrl(string patientGuid, DropBoxLink dropBox);
        Attachment GetBillOfRightsPdfData();
    }
}
