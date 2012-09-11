using SimChartMedicalOffice.Data.Repository;
using System.Collections.Generic;

namespace SimChartMedicalOffice.Core.DataInterfaces.Competency
{
    public interface ICompetencySourceDocument : IKeyValueRepository<Core.Competency.CompetencySources>
    {
        IList<Core.Competency.CompetencySources> GetAllCompetecnySources();
        string FormCompetencySourceUrl(string competencySourceUrl, bool isEditMode, Core.Competency.CompetencySources competencySourceUIObject);
        void LoadCompetecnySource();
    }
}
