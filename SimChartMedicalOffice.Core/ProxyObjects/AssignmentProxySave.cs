using System.Collections.Generic;
using System;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.AssignmentBuilder;

namespace SimChartMedicalOffice.Core.ProxyObjects
{
    public class AssignmentProxySave : DocumentEntity
    {
        public AssignmentProxySave() { }
        public string FolderName { get; set; }

        public string FolderIdentifier { get; set; }

        public string FolderUrl { get; set; }

        public Patient.Patient Patient { get; set; }

        public IList<AssignmentSkillSetProxy> SkillSets { get; set; }

        public string PatientReference { get; set; }

        public string Title { get; set; }

        public IList<string> Module { get; set; }

        public string LinkedCompetencies { get; set; }

        public string Duration { get; set; }

        public string Status { get; set; }

        public string Keywords { get; set; }

        public IList<Resource> Resources { get; set; }
    }
}