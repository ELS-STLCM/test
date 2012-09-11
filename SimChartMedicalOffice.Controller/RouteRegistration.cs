using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SimChartMedicalOffice.ApplicationServices;
using SimChartMedicalOffice.Common;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class RouteRegistration
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Account", action = "Logon", id = UrlParameter.Optional } // Parameter defaults
            );

            AppCommon.RegisterLog4Net();
        }
        
    }
}
