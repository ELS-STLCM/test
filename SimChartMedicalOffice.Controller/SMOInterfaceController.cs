using System.Web.Mvc;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class SMOInterfaceController : BaseController
    {
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
        }
        private DropBoxLink getDropBoxLinkObj(string target, string UID, string CID, string SID, string UserRole)
        {
            DropBoxLink dropBoxLink = new DropBoxLink {Cid = CID, Uid = UID, Sid = SID, UserRole = UserRole};
            //dropBoxLink.Target = target;

            return dropBoxLink;
        }
       
    }
}
