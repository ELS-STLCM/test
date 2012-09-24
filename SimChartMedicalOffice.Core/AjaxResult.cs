
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
        public AjaxResult(Common.AppEnum.ResultType code, string desc, string addDes)
        {
            Code = code.ToString();
            Description = desc;
            AdditionalDescription = addDes;
        }
        public string Code { get; set; }
        public string Description { get; set; }
        public string AdditionalDescription { get; set; }
    }
}