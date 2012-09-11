using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.FrontOffice.Appointments;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DropBox;


namespace SimChartMedicalOffice.ApplicationServices
{
    public class BaseService
    {
        /// <summary>
        /// To set audit fields for document
        /// </summary>
        /// <param name="documentEntity"></param>
        /// <param name="isEditMode"></param>
        public void SetAuditFields(DocumentEntity documentEntity, bool isEditMode,DropBoxLink drpLinkFromCookie)
        {
            if (!documentEntity.IsActive)
            {
                documentEntity.DeletedBy = GetLoginUserId(drpLinkFromCookie);
                documentEntity.DeletedTimeStamp = DateTime.Now;
            }
            else
            {
                if (isEditMode)
                {
                    documentEntity.ModifiedBy = GetLoginUserId(drpLinkFromCookie);
                    documentEntity.ModifiedTimeStamp = DateTime.Now;
                }
                else
                {
                    documentEntity.CreatedBy = GetLoginUserId(drpLinkFromCookie);
                    documentEntity.CreatedTimeStamp = DateTime.Now;
                }
            }

        }
        /// <summary>
        /// To get user id from cookie
        /// </summary>
        /// <returns>UID</returns>
        protected string GetLoginUserId(DropBoxLink drpLinkFromCookie)
        {
            return (drpLinkFromCookie != null ? "LN" + drpLinkFromCookie.UID + ", FN" + drpLinkFromCookie.UID : String.Empty);
        }
        
    }
}
