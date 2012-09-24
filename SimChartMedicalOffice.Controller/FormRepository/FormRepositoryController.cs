using System.Web.Mvc;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Forms;


namespace SimChartMedicalOffice.Web.Controllers
{
    public class FormRepositoryController : BaseController
    {
        //private readonly IFormsService _formsService;

        public FormRepositoryController(IFormsService formsService)
        {
          //  this._formsService = formsService;
        }

        public ActionResult Index()
        {
            ////string result = JsonSerializer.SerializeObject(_simOfficeService.GetAllPriorAuthorisationRequest());
            //List<PriorAuthorizationRequestForm> priorAuthorizationRequestForm = new List<PriorAuthorizationRequestForm>();
            //priorAuthorizationRequestForm = _simOfficeService.GetAllPriorAuthorizationRequest().ToList();
            //return View("Views/FormRepository/Index", priorAuthorizationRequestForm);
            ////return Json(new { Result = JsonSerializer.SerializeObject(_simOfficeService.GetAllPriorAuthorisationRequest()) }); 
            return View();
        }

    } 
}
