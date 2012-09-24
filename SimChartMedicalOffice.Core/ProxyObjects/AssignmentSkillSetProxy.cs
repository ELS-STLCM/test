
namespace SimChartMedicalOffice.Core.ProxyObjects
{
    public class AssignmentSkillSetProxy : DocumentEntity
    {
        /// <summary>
        /// This property holds the uniqueidentifier of a skillset
        /// </summary>
        public string SkillSetIdentifier { get; set; }

        /// <summary>
        /// This property holds the sequencenumber of the skillset
        /// </summary>
        public int SequenceNumber { get; set; }
    }
}