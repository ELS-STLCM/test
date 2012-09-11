using System.Collections.Generic;
using System;
using SimChartMedicalOffice.Common;

namespace SimChartMedicalOffice.Core.QuestionBanks
{
    public class DocumentProxy : DocumentEntity
    {
        public DocumentProxy() { }
        public string FolderName { get; set; }

        public string FolderIdentifier { get; set; }

        public string FolderUrl { get; set; }

        public string Text { get; set; } 

        public string LinkedItemReference { get; set; }

        public string TypeOfQuestion { get; set; }

        public List<string> AnswerTexts { get; set; }

        public string Rationale { get; set; }

        public bool IsQuestionFromTemplate { get; set; }
        public List<DocumentListProxy> ListOfItems { get; set; } 

        public int OrderSequenceNumber { get; set; }

        public string TemplateSequenceNumber { get; set; }
        

    }
}