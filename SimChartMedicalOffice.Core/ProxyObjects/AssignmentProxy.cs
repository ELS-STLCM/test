using System.Collections.Generic;
using System;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.SkillSetBuilder;

namespace SimChartMedicalOffice.Core.ProxyObjects
{
    public class AssignmentProxy : DocumentEntity
    {
        public AssignmentProxy() { }
        public string FolderName { get; set; }

        public string FolderIdentifier { get; set; }

        public string FolderUrl { get; set; }

        public string Patients { get; set; }

        public string AssignmentTitle { get; set; }

        public string ModuleText { get; set; }

        public string LinkedCompetencies { get; set; }

        public string Duration { get; set; }

        public string CreatedOn { get; set; }

        public string Status { get; set; }

        public string Keywords { get; set; }

        public IList<SkillSet> SkillSets { get; set; }
    }
}