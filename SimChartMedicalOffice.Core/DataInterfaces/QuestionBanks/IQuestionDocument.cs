using System.Collections.Generic;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks
{
    public interface IQuestionDocument : IKeyValueRepository<Question>
    {
        IList<Question> GetQuestionItems(string parentFolderIdentifier, int folderType, DropBoxLink dropBox);
    }
}
