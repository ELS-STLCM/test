using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces
{
    public interface IAttachmentDocument : IKeyValueRepository<Attachment>
    {
       string GetAttachementTransientUrl();
    }
}
