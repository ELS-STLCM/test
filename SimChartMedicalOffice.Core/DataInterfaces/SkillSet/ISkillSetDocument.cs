using System.Collections.Generic;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.SkillSetBuilder;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.SkillSetBuilder
{
    public interface ISkillSetDocument : IKeyValueRepository<Core.SkillSetBuilder.SkillSet>
    {
        IList<Question> GetQuestionsForSkillSet(string skillSetUrl);
        IList<SkillSet> GetSkillSetItems(string parentFolderIdentifier, int folderType, string courseId);
        SkillSet GetSkillSet(string skillSetUrl);
        IList<string> GetCompetencyGuidListInSkillSet(string skillSetUniqueIdentifier);

    }
}
