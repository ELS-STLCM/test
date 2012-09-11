using System.Collections.Generic;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks
{
    public interface IQuestionDocument : IKeyValueRepository<Core.QuestionBanks.Question>
    {
        IList<Question> GetQuestionItems(string parentFolderIdentifier, int folderType, string courseId);
    }
}
