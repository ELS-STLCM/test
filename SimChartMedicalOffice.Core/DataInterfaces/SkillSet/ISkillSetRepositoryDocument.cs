using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.SkillSetBuilder
{
    public interface ISkillSetRepositoryDocument : IKeyValueRepository<Core.SkillSetBuilder.SkillSetRepository>
    {
        Folder GetSkillSetRepository();
    }
}
