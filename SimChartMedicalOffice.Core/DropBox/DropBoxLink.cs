using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimChartMedicalOffice.Core.Account;
using SimChartMedicalOffice.Common;
using System.Configuration;
namespace SimChartMedicalOffice.Core.DropBox
{
    public class DropBoxLink
    {
        public DropBoxLink() {}
       
        

        /// <summary>
        /// This property holds the UID value for dropbox
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// This property holds the CID value for dropbox
        /// </summary>
        public string CID { get; set; }
        
        /// <summary>
        /// This property holds the ScenarioId value for dropbox
        /// </summary>
        public string SID { get; set; }

        /// <summary>
        /// This property holds the user role for dropbox (Admin/Instructor/Student
        /// </summary>
        public string UserRole { get; set; }
    }
}
