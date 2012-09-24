using System.Collections.Generic;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks
{
    public interface IQuestionBankDocument : IKeyValueRepository<QuestionBankFolder>
    {
          Folder GetQuestionBank();
          List<DocumentProxy> GetAllQuestionsInAQuestionBank();
          Dictionary<string, int> GetQuestionType();
    }
}
