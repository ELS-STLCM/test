using System.Web.Mvc;
using SimChartMedicalOffice.ApplicationServices;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface;


namespace SimChartMedicalOffice.Web.Controllers
{
    public class HomeController : Controller
    {
        //private ITestService testServiceInstance;
        //private ITestService testService2Instance;
        public HomeController(TestService testServiceObject,ITestService testService2Object)
        {
          //  this.testServiceInstance = testServiceObject;
            //this.testService2Instance=testService2Object;
        }
        public ActionResult Welcome()
        {
            //ViewBag.Message = testServiceInstance.HelloWorld();
            
            return View();
        }

        public ActionResult Index()
        {
            ViewBag.Message = "This is a sample layout";
            return View();
        }

        public ActionResult AdminLandingPage()
        {
            ViewBag.Message = "This is a sample layout";
            return View();
        } 
        public ViewResult Welcome2()
        {
            //ViewBag.Message = testService2Instance.HelloWorld();

            return View("Welcome");
        }
    }
}
