using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DropBox;
using SimChartMedicalOffice.Core.ProxyObjects;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// To Serialize the AjaxResult object
        /// </summary>
        /// <param name="applicationException"></param>
        /// <returns></returns>
        protected string AjaxCallResult(AjaxResult applicationException)
        {
            return JsonSerializer.SerializeObject(applicationException);
        }
        /// <summary>
        /// To set audit fields for document
        /// </summary>
        /// <param name="documentEntity"></param>
        /// <param name="isEditMode"></param>
        public void SetAuditFields(DocumentEntity documentEntity, bool isEditMode)
        {
            string loginUserId = GetLoginUserId();
            DateTime currentServerTime = DateTime.Now;
            if (!documentEntity.IsActive)
            {
                documentEntity.DeletedBy = loginUserId;
                documentEntity.DeletedTimeStamp = currentServerTime;
            }
            else
            {
                if (isEditMode)
                {
                    documentEntity.ModifiedBy = loginUserId;
                    documentEntity.ModifiedTimeStamp = currentServerTime;
                }
                else
                {
                    documentEntity.CreatedBy = loginUserId;
                    documentEntity.CreatedTimeStamp = currentServerTime;
                }
            }

        }
        public void SetClientAuditFields(AbstractChartData abstractChartData, bool isEditMode)
        {
            string loginUserId = GetLoginUserId();
            SetAuditFields(abstractChartData, isEditMode);
            if (!abstractChartData.IsActive)
            {
                abstractChartData.InactivatedBy = loginUserId;
                abstractChartData.InactiveTimeStamp = DateTime.Now.ToString("");
            }
            else
            {
                if (isEditMode)
                {
                    abstractChartData.ChartModifiedBy = loginUserId;
                    abstractChartData.ChartModifiedTimeStamp= DateTime.Now.ToString("");
                }
                else
                {
                    abstractChartData.Signature = loginUserId;
                    abstractChartData.ChartTimeStamp = DateTime.Now.ToString("");
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
            return (drpLinkFromCookie != null ? drpLinkFromCookie.Cid : String.Empty);
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
            return (drpLinkFromCookie != null ? "LN" + drpLinkFromCookie.Uid + ", FN" + drpLinkFromCookie.Uid : String.Empty);
        }

        /// <summary>
        /// To get login role from cookie
        /// </summary>
        /// <returns></returns>
        protected string GetLoginScenarioId()
        {
            DropBoxLink drpLinkFromCookie = GetDropBoxFromCookie();
            return (drpLinkFromCookie != null ? drpLinkFromCookie.Sid : String.Empty);
        }

        /// <summary>
        /// To get dropbox from cookie
        /// </summary>
        /// <returns></returns>
        protected DropBoxLink GetDropBoxFromCookie()
        {
            DropBoxLink dropBoxLinkObj = new DropBoxLink();
            string dropboxLinkJson = (Request.Cookies["DROPBOXLINK"] != null) ? HttpUtility.UrlDecode(Request.Cookies["DROPBOXLINK"].Value) : string.Empty;
            if (!String.IsNullOrEmpty(dropboxLinkJson))
            {
                Dictionary<string, DropBoxLink> dropBoxDictionary = JsonSerializer.DeserializeObject<Dictionary<string, DropBoxLink>>(dropboxLinkJson);
                foreach (var item in dropBoxDictionary)
                {
                    dropBoxLinkObj = item.Value;
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
                AutoCompleteProxy filterType = new AutoCompleteProxy
                                                   {id = questionType.Key.ToString(), name = questionType.Value};
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
                    AutoCompleteProxy filterType = new AutoCompleteProxy
                                                       {
                                                           id = questionType.Value.ToString(),
                                                           name = questionType.Key
                                                       };
                    filterByTypeList.Add(filterType);
                }
            }
            filterByTypeList = filterByTypeList.OrderBy(f => f.name).ToList();
            return filterByTypeList;
        }




    }
}
