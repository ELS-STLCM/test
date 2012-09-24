using System.Collections.Generic;
using SimChartMedicalOffice.Core.QuestionBanks;

namespace SimChartMedicalOffice.Core.SkillSetBuilder
{
    public class SkillSet : DocumentEntity
    {
        /// <summary>
        /// This property holds the skillset title
        /// </summary>
        public string SkillSetTitle { get; set; }

        /// <summary>
        /// This property holds the pusblished status of skill set 
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// This property holds the focus applicable to this skillset
        /// </summary>
        public IList<string> Focus { get; set; }

        /// <summary>
        /// String value cantains the reference to Object Competency GUID 
        /// </summary>
        public IList<string> Competencies { get; set; }

        /// <summary>
        /// This property holds the skillset item of Question 
        /// Relationship is one to many .
        /// String value cantains the reference to Object Question GUID 
        /// </summary>
        public Dictionary<string, Question> Questions { get; set; }

        /// <summary>
        /// This property holds the Status of the skillset
        /// </summary>
        public string Status { get; set; }

        public int SequenceNumber { get; set; }
    }
}
