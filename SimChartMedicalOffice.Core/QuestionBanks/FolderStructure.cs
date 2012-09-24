using System.Collections.Generic;

namespace SimChartMedicalOffice.Core.QuestionBanks
{
    public class FolderStructure
    {
        public string Value { get; set; }
        public Dictionary<string,FolderStructure> SubFolders { get; set; }
    }
}
