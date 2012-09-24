using SimChartMedicalOffice.Core.DataInterfaces.SkillSetBuilder;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Data.SkillSet
{
    public class SkillSetReposistoryDocument : KeyValueRepository<Core.SkillSetBuilder.SkillSetRepository>, ISkillSetRepositoryDocument
    {
        //protected override string NewUrl
        //{
        //    get
        //    {
        //        return "SimApp/Courses/ELSEVIER_CID/Admin/SkillSetRepository";
        //    }
        //}
        
    }
}
