using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.DataInterfaces.SkillSetBuilder;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Data.Repository;
namespace SimChartMedicalOffice.Data.SkillSet
{
    public class SkillSetReposistoryDocument : KeyValueRepository<Core.SkillSetBuilder.SkillSetRepository>, ISkillSetRepositoryDocument
    {
        public override string Url
        {
            get
            {
                return "SimApp/Courses/ELSEVIER_CID/Admin/SkillSetRepository";
            }
        }
        /// <summary>
        /// To get Assignment object
        /// </summary>
        /// <returns></returns>
        public Folder GetSkillSetRepository()
        {
            string jsonString = GetJsonDocument(this.Url);
            Folder assignment = JsonSerializer.DeserializeObject<Folder>(jsonString);
            return assignment;
        }
    }
}
