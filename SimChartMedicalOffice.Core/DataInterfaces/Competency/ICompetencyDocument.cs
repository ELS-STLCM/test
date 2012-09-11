using SimChartMedicalOffice.Data.Repository;
using System.Collections.Generic;

namespace SimChartMedicalOffice.Core.DataInterfaces.Competency
{
    public interface ICompetencyDocument : IKeyValueRepository<Core.Competency.Competency>
    {
        IList<Core.Competency.Competency> GetAllCompetencies();
        IList<string> GetAllCategories();
        string FormCompetencyUrl(string competencyUrl, bool isEditMode, Core.Competency.Competency competencyUIObject);
        void LoadCompetencies();
    }
}
