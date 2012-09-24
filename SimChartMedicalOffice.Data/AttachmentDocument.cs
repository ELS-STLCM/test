using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DataInterfaces;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Data
{
    public class AttachmentDocument : KeyValueRepository<Attachment>, IAttachmentDocument
    {
        //protected override string NewUrl
        //{
        //    get
        //    {
        //        return "SimApp/Attachments/Persistent/{0}";
        //    }
        //}
        //public string GetAttachementTransientUrl()
        //{
        //        return "SimApp/Attachments/Transient/{0}";
        //}
    }
}
