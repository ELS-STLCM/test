using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;

namespace SimChartMedicalOffice.Data
{
    public class AnswerOptionDocument : KeyValueRepository<Core.QuestionBanks.AnswerOption>, IAnswerOptionDocument
    {
        public override string Url
        {
            get
            {
                return "SimApp/Authoring/QuestionBanks/Questions/AnswerOptions/{0}";
            }
        }
    }
}
