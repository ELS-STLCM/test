using System.Collections.Generic;
using System;
using SimChartMedicalOffice.Common;

namespace SimChartMedicalOffice.Core.QuestionBanks
{
    public class DocumentListProxy : DocumentEntity
    {
        public DocumentListProxy() { }

        public string Name { get; set; }

        public string ItemReference { get; set; }

        public List<string> Items { get; set; }
    }
}