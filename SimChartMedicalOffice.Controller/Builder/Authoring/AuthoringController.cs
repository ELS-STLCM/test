using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder;
using SimChartMedicalOffice.Core;
using System.Web.Script.Serialization;
using System.Configuration;
using SimChartMedicalOffice.Common;
using System.IO;
using SimChartMedicalOffice.Core.Competency;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency;
using System.Globalization;
using SimChartMedicalOffice.Core.Competency;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class AuthoringController : BaseController
    {
        private readonly ICompetencyService _competencyService;

        public AuthoringController(ICompetencyService competencyService)
        {
            this._competencyService = competencyService;
        }

        public ActionResult Practice()
        {
            return View("../Builder/Authoring/Practice");
        }
        public ActionResult PatientBuilder()
        {
            List<string> filterAge = new List<string>();
            filterAge.Add("Filter by Age");
            for (int ageCount = 1; ageCount <= 100; ageCount++)
            {
                filterAge.Add(ageCount.ToString());
            }
            ViewData["filterByAge"] = new SelectList(filterAge.ToList(), "filterByAge");
            return View("../Builder/Patient/PatientBuilder");
        }
        public ActionResult QuestionBankLanding()
        {
            List<string> filterByType = new List<string>(); 
            ViewBag.FilterByQuestionType = GetQuestionTypeFlexBoxList();
            return View("../Builder/Authoring/QuestionBankLanding");
        }
        
        //public ActionResult AssignmentBuilder()
        //{
        //    ViewData["NoOfAttempts"] = new SelectList(AppCommon.NoOfAttempts.ToList(), "NoOfAttempts");
        //    double noOfQuestions = 3;
        //    string defaultValue = "";
        //    List<string> percentages = AppCommon.CalculatePassRate(noOfQuestions);
        //    ViewData["PassRate"] = new SelectList(percentages, "PassRate");
        //    return View("../Builder/Assignment/AssignmentBuilder");

        //}
        public ActionResult AssignmentBuilderLanding()
        {
           //Method for getting the list of modules has to be written
            IList<ApplicationModules> competencyModuleList = _competencyService.GetAllApplicationModules();
            int index = 0;
            ViewBag.ModuleList = competencyModuleList.Select(cat => new { id = index++, name = cat.Name.ToString(CultureInfo.InvariantCulture) }).ToList();
           return View("../Builder/Authoring/AssignmentBuilderLanding");
        }
        public ActionResult SkillSetLanding()
        {
           return RedirectToAction("SkillSetBuilderLanding", "SkillSet");
            
        }
        //public ActionResult SkillSetBuilderLanding()
        //{
        //    //Method for getting the list of modules has to be written
        //    IList<string> competencyMainFocus = new List<string>();
        //    //competencyMainFocus.Add("ABHES");
        //    //competencyMainFocus.Add("CAAHEP");
        //    //competencyMainFocus.Add("MAERB");
        //    int index = 0;
        //    if (ViewBag != null)
        //        ViewBag.filterByTypeList = competencyMainFocus.Select(cat => new { id = index++, name = cat.ToString(CultureInfo.InvariantCulture) }).ToList();
        //    return View("../Builder/SkillSet/SkillSetBuilderLanding");
        //}
    }
}
