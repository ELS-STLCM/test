using SimChartMedicalOffice.Data.Repository;
using System.Collections.Generic;
using SimChartMedicalOffice.Core.QuestionBanks;

namespace SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks
{
    public interface IQuestionBankDocument : IKeyValueRepository<Core.QuestionBanks.QuestionBankFolder>
    {
          Folder GetQuestionBank();
          List<DocumentProxy> GetAllQuestionsInAQuestionBank();
          Dictionary<string, int> GetQuestionType();
    }
}
