using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.Forms;
using SimChartMedicalOffice.Core.Forms;
using System.Collections.Generic;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core;

namespace SimChartMedicalOffice.Data.Forms
{
    public class BillOfRightsDocument : KeyValueRepository<SimChartMedicalOffice.Core.Forms.BillOfRights>, IBillOfRightsDocument
    {
        public override string Url
        {
            get
            {
                return "SimApp/Courses/{0}/Patients/{1}";
            }
        }
        public BillOfRights GetBillOfRightsDocument(string patientGUID, string courseId, string userRole, string UID, string SID)
        {
            string billOfRightsUrl = string.Format(Url, courseId + "/" + userRole + "/" + UID + "/Assignments/" + SID, patientGUID);
            billOfRightsUrl = billOfRightsUrl + "/BillOfRights";
            string billOfRightsJSON = GetJsonDocument(billOfRightsUrl);
            BillOfRights billOfRightsJSONForm =
                JsonSerializer.DeserializeObject<BillOfRights>(billOfRightsJSON);
            return billOfRightsJSONForm;
        }
        public string SaveBillOfRightsUrl(string patientGUID, string courseId, string userRole, string UID, string SID)
        {
            string billOfRightsUrl = string.Format(Url, courseId + "/" + userRole + "/" + UID + "/Assignments/" + SID, patientGUID);
            billOfRightsUrl = billOfRightsUrl + "/BillOfRights";
            return billOfRightsUrl;
        }
        public Attachment GetBillOfRightsPdfData()
        {
            Dictionary<string, Attachment> billOfRightPdf = new Dictionary<string, Attachment>();
            Attachment billOfRight = new Attachment();
            string pdfUrl = "SimApp/Master/Forms/BillofRights";
            string billOfRightPdfJSON = GetJsonDocument(pdfUrl);
            billOfRightPdf = JsonSerializer.DeserializeObject<Dictionary<string, Attachment>>(billOfRightPdfJSON);
            foreach (var item in billOfRightPdf)
            {
                billOfRight = ( Attachment )item.Value;
            }
            return billOfRight;
        }
    }
}