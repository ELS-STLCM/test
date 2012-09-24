using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Core.DataInterfaces.Forms
{
    public interface INoticeOfPrivacyPracticeDocument : IKeyValueRepository<NoticeOfPrivacyPractice>
    {
        NoticeOfPrivacyPractice GetNoticeOfPrivacyPracticeDocument(string patientGuid, DropBoxLink dropBox);
        string SaveNoticeFormUrl(string patientGuid, DropBoxLink dropBox);
        Attachment GetNoticeOfPrivacyPracticePdfData();
    }
}
