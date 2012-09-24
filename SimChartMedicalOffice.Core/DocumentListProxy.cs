using System.Collections.Generic;

namespace SimChartMedicalOffice.Core.QuestionBanks
{
    public class DocumentListProxy : DocumentEntity
    {
        public string Name { get; set; }

        public string ItemReference { get; set; }

        public List<string> Items { get; set; }
    }
}