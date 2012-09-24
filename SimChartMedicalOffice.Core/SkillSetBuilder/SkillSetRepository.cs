using System.Collections.Generic;
using SimChartMedicalOffice.Core.QuestionBanks;

namespace SimChartMedicalOffice.Core.SkillSetBuilder
{
    public class SkillSetRepository : DocumentEntity
    {
        /// <summary>
        /// This property holds the dictionary of skillset with new GUID 
        /// Relationship is one to many .
        /// String value cantains new GUID value 
        /// </summary>
        public Dictionary<string, SkillSet> SkillSets { get; set; }

        /// <summary>
        /// This property holds the user created Folders and their children and grand children.
        /// </summary>
        public Dictionary<string, Folder> SubFolders { get; set; }
    }
}
