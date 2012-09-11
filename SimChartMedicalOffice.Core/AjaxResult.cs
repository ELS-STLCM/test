using System;

namespace SimChartMedicalOffice.Core
{
    public class AjaxResult
    {
        /// <summary>
        /// To Set Ajax call result attributes  
        /// </summary>
        public AjaxResult()
        {

        }
        public AjaxResult(SimChartMedicalOffice.Common.AppEnum.ResultType code, string desc, string addDes)
        {
            Code = code;
            Description = desc;
            AdditionalDescription = addDes;
        }
        public SimChartMedicalOffice.Common.AppEnum.ResultType Code { get; set; }
        public string Description { get; set; }
        public string AdditionalDescription { get; set; }
    }
}