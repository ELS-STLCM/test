using System.Collections.Generic;
using SimChartMedicalOffice.Core.QuestionBanks;

namespace SimChartMedicalOffice.Core.SkillSetBuilder
{
    public class SkillSetProxy : DocumentEntity
    {
        public string SkillSetTitle { get; set; }

        public  IList<string> LinkedCompetencies { get; set; }

        public string LinkedCompetenciesText { get; set; }

        public IList<string> Source { get; set; }

        public string SourceNameText { get; set; }

        public string CreatedOn { get; set; }

        public string Status { get; set; }

        public IList<string> Focus { get; set; }

        public IList<Question> Questions { get; set; }

        //SkillSet Competency Filter attirbutes
        public IList<string> FilterSourceList { get; set; }
        public IList<string> SelectedCompetencyList { get; set; }
        public string CompetencyText { get; set; }
        public string FilterQuestionType { get; set; }
        public string SearchText { get; set; }

        public string Guid { get; set; }

        public new string Url { get; set; }
        public new string ParentReferenceGuid { get; set; }
        public string QuestionCount { get; set; }
        public int SequenceNumber { get; set; }
    }
}
