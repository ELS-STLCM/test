using System.Collections.Generic;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DataInterfaces.Forms;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DropBox;
using SimChartMedicalOffice.Common;
namespace SimChartMedicalOffice.Data.Forms
{
    public class NoticeOfPrivacyPracticeDocument : KeyValueRepository<NoticeOfPrivacyPractice>, INoticeOfPrivacyPracticeDocument
    {
       

        public NoticeOfPrivacyPractice GetNoticeOfPrivacyPracticeDocument(string patientGuid, DropBoxLink dropBox)
        {
            string noticeOfPrivacyPracticeUrl = string.Format(GetAssignmentUrl(dropBox, DocumentPath.Module.NoticeOfPrivacyPractice), patientGuid);
            //noticeOfPrivacyPracticeUrl = noticeOfPrivacyPracticeUrl + "/NoticeOfPrivacyPractice";
            string noticeOfPrivacyJson = GetJsonDocument(noticeOfPrivacyPracticeUrl);
            NoticeOfPrivacyPractice noticeOfPrivacyPracticeForm =
                JsonSerializer.DeserializeObject<NoticeOfPrivacyPractice>(noticeOfPrivacyJson);
            return noticeOfPrivacyPracticeForm;
        }

        public string SaveNoticeFormUrl(string patientGuid,  DropBoxLink dropBox)
        {
            string noticeOfPrivacyPracticeUrl = string.Format(GetAssignmentUrl(dropBox, DocumentPath.Module.NoticeOfPrivacyPractice), patientGuid);
            //noticeOfPrivacyPracticeUrl = noticeOfPrivacyPracticeUrl + "/NoticeOfPrivacyPractice";
            return noticeOfPrivacyPracticeUrl;
        }

        public Attachment GetNoticeOfPrivacyPracticePdfData()
        {
            Attachment billOfRight = new Attachment();
            string pdfUrl = GetAssignmentUrl(DocumentPath.Module.Masters, AppConstants.NoticePrivacyPractice);
            string noticeOfPrivacyPracticePdfJson = GetJsonDocument(pdfUrl);
            Dictionary<string, Attachment> noticeOfPrivacyPracticePdf = JsonSerializer.DeserializeObject<Dictionary<string, Attachment>>(noticeOfPrivacyPracticePdfJson);
            foreach (var item in noticeOfPrivacyPracticePdf)
            {
                billOfRight = item.Value;
            }
            return billOfRight;
        }
    }
}