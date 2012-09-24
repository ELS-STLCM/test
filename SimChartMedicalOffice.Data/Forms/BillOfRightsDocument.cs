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
    public class BillOfRightsDocument : KeyValueRepository<BillOfRights>, IBillOfRightsDocument
    {
       
        public BillOfRights GetBillOfRightsDocument(string patientGuid, DropBoxLink dropBox)
        {
            string billOfRightsUrl = string.Format(GetAssignmentUrl(dropBox, DocumentPath.Module.BillOfRights), patientGuid);
            //billOfRightsUrl = billOfRightsUrl + "/BillOfRights";
            string billOfRightsJSON = GetJsonDocument(billOfRightsUrl);
            BillOfRights billOfRightsJsonForm =
                JsonSerializer.DeserializeObject<BillOfRights>(billOfRightsJSON);
            return billOfRightsJsonForm;
        }
        public string SaveBillOfRightsUrl(string patientGuid, DropBoxLink dropBox)
        {
            string billOfRightsUrl = string.Format(GetAssignmentUrl(dropBox, DocumentPath.Module.BillOfRights), patientGuid);
            //billOfRightsUrl = billOfRightsUrl + "/BillOfRights";
            return billOfRightsUrl;
        }
        public Attachment GetBillOfRightsPdfData()
        {
            Attachment billOfRight = new Attachment();
            string pdfUrl = GetAssignmentUrl(DocumentPath.Module.Masters,AppConstants.BillofRights);
            string billOfRightPdfJson = GetJsonDocument(pdfUrl);
            Dictionary<string, Attachment> billOfRightPdf = JsonSerializer.DeserializeObject<Dictionary<string, Attachment>>(billOfRightPdfJson);
            foreach (var item in billOfRightPdf)
            {
                billOfRight = item.Value;
            }
            return billOfRight;
        }
    }
}