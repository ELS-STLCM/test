using System.Collections.Generic;
using SimChartMedicalOffice.Core.Competency;
using SimChartMedicalOffice.Core.QuestionBanks;

namespace SimChartMedicalOffice.Core.Authoring
{
    public class Authoring : DocumentEntity
    {
        public Authoring()
        {

        }

        /// <summary>
        /// This property holds all the competencies applicable to Authoring module.
        /// </summary>
        public IList<Competency.Competency> Competencies { get; set; }

        /// <summary>
        /// This property holds all the Questions applicable to Authoring module.
        /// </summary>
        public Dictionary<string,QuestionBankFolder> QuestionBanks { get; set; }
    }
}