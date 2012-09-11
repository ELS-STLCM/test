using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class SMOInterfaceController : BaseController
    {
        public SMOInterfaceController()
        { }
        public ActionResult FormsRepository(string target, string UID, string CID, string SID, string UserRole)
        {
            DropBoxLink dropBoxLinkObj = getDropBoxLinkObj(target, UID, CID, SID, UserRole);
            PushDropBoxLinkToCookie(dropBoxLinkObj);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult QuestionBankBuilder(string target, string UID, string CID, string SID, string UserRole)
        {
            DropBoxLink dropBoxLinkObj = getDropBoxLinkObj(target, UID, CID, SID, UserRole);
            PushDropBoxLinkToCookie(dropBoxLinkObj);
            return RedirectToAction("AdminLandingPage", "Home");
        }
        private void PushDropBoxLinkToCookie(DropBoxLink dropBoxLinkObj)
        {
            var linkObj = new { DROPBOXLINK = dropBoxLinkObj };
            if (Request.Cookies["DROPBOXLINK"] != null)
            {
                Request.Cookies.Remove("DROPBOXLINK");
            }
            Response.Cookies["DROPBOXLINK"].Value = JsonSerializer.SerializeObject(linkObj);

            return;
        }
        private DropBoxLink getDropBoxLinkObj(string target, string UID, string CID, string SID, string UserRole)
        {
            DropBoxLink dropBoxLink = new DropBoxLink();
            //dropBoxLink.Target = target;
            dropBoxLink.CID = CID;
            dropBoxLink.UID = UID;
            dropBoxLink.SID = SID;
            dropBoxLink.UserRole = UserRole;

            return dropBoxLink;
        }
       
    }
}
