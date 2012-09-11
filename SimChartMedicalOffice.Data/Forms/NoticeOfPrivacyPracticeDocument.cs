using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.Forms;
using SimChartMedicalOffice.Core.Forms;
using System.Collections.Generic;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core;
namespace SimChartMedicalOffice.Data.Forms
{
    public class NoticeOfPrivacyPracticeDocument : KeyValueRepository<NoticeOfPrivacyPractice>, INoticeOfPrivacyPracticeDocument
    {
        public override string Url
        {
            get
            {
                return "SimApp/Courses/{0}/Patients/{1}";
            }
        }

        public NoticeOfPrivacyPractice GetNoticeOfPrivacyPracticeDocument(string patientGUID, string courseId, string userRole, string UID, string SID)
        {
            string noticeOfPrivacyPracticeUrl = string.Format(Url, courseId + "/" + userRole + "/" + UID + "/Assignments/" + SID, patientGUID);
            noticeOfPrivacyPracticeUrl = noticeOfPrivacyPracticeUrl + "/NoticeOfPrivacyPractice";
            string noticeOfPrivacyJSON = GetJsonDocument(noticeOfPrivacyPracticeUrl);
            NoticeOfPrivacyPractice noticeOfPrivacyPracticeForm =
                JsonSerializer.DeserializeObject<NoticeOfPrivacyPractice>(noticeOfPrivacyJSON);
            return noticeOfPrivacyPracticeForm;
        }

        public string SaveNoticeFormUrl(string patientGUID, string courseId, string userRole, string UID, string SID)
        {
            string noticeOfPrivacyPracticeUrl = string.Format(Url, courseId + "/" + userRole + "/" + UID + "/Assignments/" + SID, patientGUID);
            noticeOfPrivacyPracticeUrl = noticeOfPrivacyPracticeUrl + "/NoticeOfPrivacyPractice";
            return noticeOfPrivacyPracticeUrl;
        }

        public Attachment GetNoticeOfPrivacyPracticePdfData()
        {
            Dictionary<string, Attachment> noticeOfPrivacyPracticePdf = new Dictionary<string, Attachment>();
            Attachment billOfRight = new Attachment();
            string pdfUrl = "SimApp/Master/Forms/NoticeOfPrivacyPractice";
            string noticeOfPrivacyPracticePdfJSON = GetJsonDocument(pdfUrl);
            noticeOfPrivacyPracticePdf = JsonSerializer.DeserializeObject<Dictionary<string, Attachment>>(noticeOfPrivacyPracticePdfJSON);
            foreach (var item in noticeOfPrivacyPracticePdf)
            {
                billOfRight = (Attachment)item.Value;
            }
            return billOfRight;
        }
    }
}