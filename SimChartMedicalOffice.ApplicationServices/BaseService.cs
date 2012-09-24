using System;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DropBox;


namespace SimChartMedicalOffice.ApplicationServices
{
    public class BaseService
    {
        ///// <summary>
        ///// To set audit fields for document
        ///// </summary>
        ///// <param name="documentEntity"></param>
        ///// <param name="isEditMode"></param>
        ///// <param name="drpLinkFromCookie"> </param>
        //public void SetAuditFields(DocumentEntity documentEntity, bool isEditMode,DropBoxLink drpLinkFromCookie)
        //{
        //    if (!documentEntity.IsActive)
        //    {
        //        documentEntity.DeletedBy = GetLoginUserId(drpLinkFromCookie);
        //        documentEntity.DeletedTimeStamp = DateTime.Now;
        //    }
        //    else
        //    {
        //        if (isEditMode)
        //        {
        //            documentEntity.ModifiedBy = GetLoginUserId(drpLinkFromCookie);
        //            documentEntity.ModifiedTimeStamp = DateTime.Now;
        //        }
        //        else
        //        {
        //            documentEntity.CreatedBy = GetLoginUserId(drpLinkFromCookie);
        //            documentEntity.CreatedTimeStamp = DateTime.Now;
        //        }
        //    }

        //}


        /// <summary>
        /// To set audit fields for document
        /// </summary>
        /// <param name="documentEntity"></param>
        /// <param name="isEditMode"></param>
        /// <param name="drpLinkFromCookie"></param>
        public void SetAuditFields(DocumentEntity documentEntity, bool isEditMode, DropBoxLink drpLinkFromCookie)
        {
            string loginUserId = GetLoginUserId(drpLinkFromCookie);
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
        /// <summary>
        /// To set client audit fields for document
        /// </summary>
        /// <param name="abstractChartData"></param>
        /// <param name="isEditMode"></param>
        /// <param name="drpLinkFromCookie"></param>
        public void SetClientAuditFields(AbstractChartData abstractChartData, bool isEditMode, DropBoxLink drpLinkFromCookie)
        {
            string loginUserId = GetLoginUserId(drpLinkFromCookie);
            SetAuditFields(abstractChartData, isEditMode,drpLinkFromCookie);
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
                    abstractChartData.ChartModifiedTimeStamp = DateTime.Now.ToString("");
                }
                else
                {
                    abstractChartData.Signature = loginUserId;
                    abstractChartData.ChartTimeStamp = DateTime.Now.ToString("");
                }
            }
        }


        /// <summary>
        /// To get user id from cookie
        /// </summary>
        /// <returns>UID</returns>
        protected string GetLoginUserId(DropBoxLink drpLinkFromCookie)
        {
            return (drpLinkFromCookie != null ? "LN" + drpLinkFromCookie.Uid + ", FN" + drpLinkFromCookie.Uid : String.Empty);
        }
        
    }
}
