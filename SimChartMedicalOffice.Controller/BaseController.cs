using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.DropBox;
using System.Web.Script.Serialization;
using System.Web;
using System.Collections;
using SimChartMedicalOffice.Core.ProxyObjects;
using System.IO;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>s
        /// To Serialize the AjaxResult object
        /// </summary>
        /// <param name="AjaxResult"></param>        
        protected string AjaxCallResult(AjaxResult applicationException)
        {
            return JsonSerializer.SerializeObject(applicationException);
        }
        /// <summary>
        /// To set audit fields for document
        /// </summary>
        /// <param name="documentEntity"></param>
        /// <param name="isEditMode"></param>
        public void SetAuditFields(DocumentEntity documentEntity,bool isEditMode)
        {
            if (!documentEntity.IsActive)
            {
                documentEntity.DeletedBy = GetLoginUserId();
                documentEntity.DeletedTimeStamp = DateTime.Now;
            }
            else
            {
                if (isEditMode)
                {
                    documentEntity.ModifiedBy = GetLoginUserId();
                    documentEntity.ModifiedTimeStamp = DateTime.Now;
                }
                else
                {
                    documentEntity.CreatedBy = GetLoginUserId();
                    documentEntity.CreatedTimeStamp = DateTime.Now;
                }
            }

        }
        public string Serialize<T>() where T : DocumentEntity
        {
            return "";
        }
        /// <summary>
        /// To get login course id from cookie
        /// </summary>
        /// <returns></returns>
        protected string GetLoginUserCourse()
        {
            DropBoxLink drpLinkFromCookie = GetDropBoxFromCookie();
            return (drpLinkFromCookie != null ? drpLinkFromCookie.CID : String.Empty);
        }
        /// <summary>
        /// To get login role from cookie
        /// </summary>
        /// <returns></returns>
        protected string GetLoginUserRole()
        {
            DropBoxLink drpLinkFromCookie = GetDropBoxFromCookie();
            return (drpLinkFromCookie != null ? drpLinkFromCookie.UserRole : String.Empty);
        }
        /// <summary>
        /// To get the login Course and Role from cookie
        /// </summary>
        /// <returns></returns>
        protected string GetLoginCourseAndRole()
        {
            return GetLoginUserCourse() + "/" + GetLoginUserRole();
        }
        /// <summary>
        /// To deserialize the Json string to List of Objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectString"></param>
        /// <returns></returns>
        public IList<T> DeSerializeToList<T>()
        {
            string objectString = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
            return JsonSerializer.DeserializeObject<IList<T>>(objectString);

        }
        /// <summary>
        /// To deserialize the Json string to an Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectString"></param>
        /// <returns></returns>
        public T DeSerialize<T>()
        {
            string objectString = HttpUtility.UrlDecode(new StreamReader(Request.InputStream).ReadToEnd());
            return JsonSerializer.DeserializeObject<T>(objectString);

        }

        /// <summary>
        /// To get user id from cookie
        /// </summary>
        /// <returns>UID</returns>
        protected string GetLoginUserId()
        {
            DropBoxLink drpLinkFromCookie = GetDropBoxFromCookie();
            return (drpLinkFromCookie != null ? "LN" + drpLinkFromCookie.UID + ", FN" + drpLinkFromCookie.UID : String.Empty);
        }

        /// <summary>
        /// To get login role from cookie
        /// </summary>
        /// <returns></returns>
        protected string GetLoginScenarioId()
        {
            DropBoxLink drpLinkFromCookie = GetDropBoxFromCookie();
            return (drpLinkFromCookie != null ? drpLinkFromCookie.SID : String.Empty);
        }

        /// <summary>
        /// To get dropbox from cookie
        /// </summary>
        /// <returns></returns>
        protected DropBoxLink GetDropBoxFromCookie()
        {
            string dropboxLinkJson;
            DropBoxLink dropBoxLinkObj = new DropBoxLink();
            dropboxLinkJson = (Request.Cookies["DROPBOXLINK"]!=null)?HttpUtility.UrlDecode(Request.Cookies["DROPBOXLINK"].Value):string.Empty;
            if (!String.IsNullOrEmpty(dropboxLinkJson))
            {
                Dictionary<string, DropBoxLink> dropBoxDictionary = JsonSerializer.DeserializeObject<Dictionary<string, DropBoxLink>>(dropboxLinkJson);
                foreach (var item in dropBoxDictionary)
                {
                    dropBoxLinkObj = (DropBoxLink)item.Value;
                }
            }
            return dropBoxLinkObj;
        }

        public IList<AutoCompleteProxy> GetQuestionTypeFlexBoxList()
        {
            Dictionary<int, string> questionTypeList = AppCommon.QuestionTypeOptionsForLanding;
            List<AutoCompleteProxy> filterByTypeList = new List<AutoCompleteProxy>();
            foreach (var questionType in questionTypeList)
            {
                AutoCompleteProxy filterType = new AutoCompleteProxy();
                filterType.id = questionType.Key.ToString();
                filterType.name = questionType.Value;
                filterByTypeList.Add(filterType);
            }
            filterByTypeList = filterByTypeList.OrderBy(f => f.name).ToList();
            return filterByTypeList;
        }
        
        public IList<AutoCompleteProxy> GetQuestionTypeFlexBoxList(Dictionary<string, int> questionTypeList)
        {
            List<AutoCompleteProxy> filterByTypeList = new List<AutoCompleteProxy>();
            if (questionTypeList != null)
            {
                foreach (var questionType in questionTypeList)
                {
                    AutoCompleteProxy filterType = new AutoCompleteProxy();
                    filterType.id = questionType.Value.ToString();
                    filterType.name = questionType.Key.ToString();
                    filterByTypeList.Add(filterType);
                }
            }
            filterByTypeList = filterByTypeList.OrderBy(f => f.name).ToList();
            return filterByTypeList;
        }

        
        
        
    }
}
