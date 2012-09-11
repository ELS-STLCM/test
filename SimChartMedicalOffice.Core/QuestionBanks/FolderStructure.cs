using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimChartMedicalOffice.Core.QuestionBanks
{
    public class FolderStructure
    {
        public string Value { get; set; }
        public Dictionary<string,FolderStructure> SubFolders { get; set; }
    }
}
