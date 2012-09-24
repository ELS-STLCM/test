using System.Collections.Generic;
using SimChartMedicalOffice.Core.QuestionBanks;

namespace SimChartMedicalOffice.Core.AssignmentBuilder
{
    public class AssignmentRepository : DocumentEntity
    {
        /// <summary>
        /// This property holds the list of assignments
        /// string will be the scenarioId from dropbox 
        /// </summary>
        public Dictionary<string, Assignment> Assignments { get; set; }

        /// <summary>
        /// This property holds the list of subFolders
        /// </summary>
        public Dictionary<string, Folder> SubFolders { get; set; }
    }
}