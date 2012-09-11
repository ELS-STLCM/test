using System.Collections.Generic;
using System;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.SkillSetBuilder;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.QuestionBanks;

namespace SimChartMedicalOffice.Core.AssignmentBuilder
{
    public class AssignmentRepository : DocumentEntity
    {
        /// <summary>
        /// Constructor of AssignmentRepository object
        /// </summary>
        public AssignmentRepository() { }

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