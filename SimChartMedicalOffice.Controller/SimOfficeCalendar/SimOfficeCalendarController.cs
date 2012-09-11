using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Forms;
using SimChartMedicalOffice.Core.Json;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.FrontOffice;

namespace SimChartMedicalOffice.Web.Controllers
{
    public class SimOfficeCalendarController : BaseController
    {

        private readonly IMasterService _masterService;
        private readonly IAppointmentService _appointmentService;
        public SimOfficeCalendarController(IMasterService masterService, IAppointmentService appointmentService)
        {
            this._masterService = masterService;
            this._appointmentService = appointmentService;
        }

       

        public ActionResult LoadNewPatient()
        {
            Dictionary<int, string> patientProviderValues = _masterService.GetPatientProviderValues();
            var patientProviderList = (from item in patientProviderValues select new { Id = item.Key, Name = item.Value }).ToList();
            ViewData["patientAppointmentProvider"] = new SelectList(patientProviderList, "Id", "Name", 2);
            //ViewData["patientAppointmentProvider"] = new SelectList(_masterService.GetPatientProviderValues(), "patientAppointmentProvider");
            ViewData["insurance"] = new SelectList(_masterService.GetPatientInsuranceValues(), "insurance");
            return View("../../Views/SimOfficeCalendar/_NewPatientForAppointment");
        }
 
        public ActionResult LoadNewAppointment()
        {
            ViewData["VisitType"] = new SelectList(_masterService.GetAppointmentVisitType(), "VisitType");
            ViewData["ProviderList"] = new SelectList(_masterService.GetPatientProviderValues(), "ProviderList");
            ViewData["ExamRoom"] = new SelectList(_masterService.GetExamRooms(), "ExamRoom");
            ViewData["BlockType"] = new SelectList(_masterService.GetBlockType(), "BlockType");
            ViewData["BlockFor"] = new SelectList(_masterService.GetPatientProviderValues(), "BlockFor");
            ViewData["StartTime"] = new SelectList(AppCommon.TimeList, "StartTime");
            ViewData["EndTime"] = new SelectList(AppCommon.TimeList, "EndTime");
            return View("../../Views/SimOfficeCalendar/_LoadAppointment");
        }
    }
}
