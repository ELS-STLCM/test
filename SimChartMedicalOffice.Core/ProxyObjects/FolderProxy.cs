using System.Collections.Generic;
using SimChartMedicalOffice.Core.QuestionBanks;

namespace SimChartMedicalOffice.Core.ProxyObjects
{
    public class FolderProxy:DocumentEntity
    {
        public string Name { get; set; }
        public List<Folder> SubFolders { get; set; }
        public List<Question> QuestionItems { get; set; }
    }
}
