using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.Forms
{
    public interface INoticeOfPrivacyPracticeDocument : IKeyValueRepository<NoticeOfPrivacyPractice>
    {
        NoticeOfPrivacyPractice GetNoticeOfPrivacyPracticeDocument(string patientGUID, string courseId, string userRole, string UID, string SID);
        string SaveNoticeFormUrl(string patientGUID, string courseId, string userRole, string UID, string SID);
        Attachment GetNoticeOfPrivacyPracticePdfData();
    }
}
