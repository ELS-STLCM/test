
namespace SimChartMedicalOffice.Core.Competency
{
  public  class ApplicationModules: DocumentEntity
    {
        public ApplicationModules()
        {

        }
        /// <summary>
        /// This property holds the Application Modules Name(Billing,coding...).
        /// </summary>
        public string Name { get; set; }
    }
}
