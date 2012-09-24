
namespace SimChartMedicalOffice.Core
{
    public class Attachment : DocumentEntity
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] FileContent { get; set; }
    }
}