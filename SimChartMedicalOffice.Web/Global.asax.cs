using System.Web.Mvc;
using System.Web.Routing;
using SimChartMedicalOffice.Infrastructure;
using SimChartMedicalOffice.Web.Controllers;

namespace SimChartMedicalOffice.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
        

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RouteRegistration.RegisterRoutes(RouteTable.Routes);
            RegisterCastle();
        }
        private void RegisterCastle()
        {
            BootStrapContainer.Install();
        }
    }
}