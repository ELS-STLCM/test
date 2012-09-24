

namespace SimChartMedicalOffice.Core.AssignmentBuilder
{
    public class Resource : DocumentEntity
    {
        /// <summary>
        /// This property holds Author name
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// This property holds Title name
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// This property holds Edition name
        /// </summary>
        public string Edition { get; set; }

        /// <summary>
        /// This property holds chapter number
        /// </summary>
        public string Chapter { get; set; }
 
        /// <summary>
        /// This property holds page range
        /// </summary>
        public string PageRange { get; set; }

        /// <summary>
        /// This property holds sequence number of the resource
        /// </summary>
        public string SequenceNumber { get; set; }

    }
}
