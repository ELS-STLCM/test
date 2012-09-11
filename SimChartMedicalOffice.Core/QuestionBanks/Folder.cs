﻿using System.Collections.Generic;
using SimChartMedicalOffice.Core.Patient;
using System;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.SkillSetBuilder;
using SimChartMedicalOffice.Core.AssignmentBuilder;

namespace SimChartMedicalOffice.Core.QuestionBanks
{
    public class Folder : DocumentEntity
    {
        public Folder() { }

        /// <summary>
        /// This property holds the Name of the Folder.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// This holds list of questions associated with a folder
        /// </summary>
        public virtual Dictionary<string, Question> QuestionItems { get; set; }

        /// <summary>
        /// Folder List inside Folder
        /// </summary>
        public virtual Dictionary<string, Folder> SubFolders { get; set; }

        /// <summary>
        /// This holds list of Patients associated with a folder
        /// </summary>        
        public virtual Dictionary<string, Core.Patient.Patient> Patients { get; set; }

        /// <summary>
        /// This holds list of SkillSets associated with a folder
        /// </summary>        
        public virtual Dictionary<string, SkillSet> SkillSets { get; set; }

        /// <summary>
        /// This holds list of Assignment associated with a folder
        /// </summary>        
        public virtual Dictionary<string, Assignment> Assignments { get; set; }

        /// <summary>
        /// This holds Sequence Number of the folder in its parent folder, if any
        /// </summary>        
        public virtual string SequenceNumber { get; set; }
    }
}