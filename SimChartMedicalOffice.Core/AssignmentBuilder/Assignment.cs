using System.Collections.Generic;
using System;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.SkillSetBuilder;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.QuestionBanks;

namespace SimChartMedicalOffice.Core.AssignmentBuilder
{
    public class Assignment : DocumentEntity
    {
        /// <summary>
        /// Constructor of Assignment object
        /// </summary>
        public Assignment() { }

        /// <summary>
        /// This property holds the title of the Assignment
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// This property holds the list of skill sets
        /// </summary>
        public Dictionary<string, SkillSet> SkillSets { get; set; }

        /// <summary>
        /// This property holds the list of patients
        /// </summary>
        public Dictionary<string, Patient.Patient> Patients { get; set; }

        /// <summary>
        /// This property holds the list of the Questions
        /// </summary>
        public Dictionary<string, Question> Questions { get; set; }

        /// <summary>
        /// This property holds the module type of the Assignment
        /// </summary>
        public IList<string> Module { get; set; }

        /// <summary>
        /// This property holds the orientation of the Assignment
        /// </summary>
        public string Orientation { get; set; }

        /// <summary>
        /// This property holds the Duration of the Assignment
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// This property holds the Keywords of the Assignment
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// This property holds the Created Time of the Assignment
        /// </summary>
        public string CreatedOn { get; set; }

        /// <summary>
        /// This property holds the status of the Assignment
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// This property holds the PatientImageReferance in Orientation of the Assignment
        /// </summary>
        public IList<string> PatientImageReferance { get; set; }

        /// <summary>
        /// This property holds the PatientImageReferance in Orientation of the Assignment
        /// </summary>
        public IList<string> PatientImageDescription { get; set; }

        /// <summary>
        /// This property holds the videoReferance id in Orientation of the Assignment
        /// </summary>
        public IList<string> VideoReferance { get; set; }

        /// <summary>
        /// This property holds the list of the Resources
        /// </summary>
        public Dictionary<string, Resource> Resources { get; set; }

        public string NoOfAttemptsAllowed { get; set; }

        public string AssignmentPassRate { get; set; }
    }
}