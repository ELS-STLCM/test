namespace SimChartMedicalOffice.Core
{
    public abstract class AbstractChartData : DocumentEntity
    {
        public string ChartTimeStamp { get; set; }
        public string Signature { get; set; }
        public string InactivatedBy { get; set; }
        public string InactiveTimeStamp { get; set; }
        public string ChartingRole { get; set; }
        public string ChartModifiedTimeStamp { get; set; }
        public string ChartModifiedBy { get; set; }
    }
}
