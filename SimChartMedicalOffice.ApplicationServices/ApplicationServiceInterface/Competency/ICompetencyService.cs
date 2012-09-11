using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.Competency;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.QuestionBanks;

namespace SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency
{
   public interface ICompetencyService
    {
       void SaveCompetencyList(Dictionary<string, Dictionary<string, SimChartMedicalOffice.Core.Competency.Competency>> compList);
       bool SaveCompetency(Core.Competency.Competency competencyUIObject, string competencyUrl, bool isEditMode);
       bool DeleteCompetency(Core.Competency.Competency deleteCompetency);
       bool SaveCompetencySource(Core.Competency.CompetencySources competencySourcesUIObject, string competencySourcesUrl, bool isEditMode);
       bool SaveApplicationModule(Core.Competency.ApplicationModules applicationModuleUIObject, string applicationModuleUrl, bool isEditMode);
       IList<Core.Competency.Competency> GetAllCompetencies();
       IList<string> GetAllCategories();
       List<AutoCompleteProxy> GetAllCompetencyListForDropDown();
       List<AutoCompleteProxy> GetCompetenciesStringListInFormat(IList<Core.Competency.Competency> competencyList);
       IList<Core.Competency.CompetencySources> GetAllCompetecnySources();
       IList<Core.Competency.ApplicationModules> GetAllApplicationModules();
       List<AutoCompleteProxy> GetFilteredCompetenciesBasedOnString(string strFilterText);
       Core.Competency.Competency GetCompetency(string competencyGUID);
       void SetLinkedCompetencyTextForAQuestions(string guidOfLinkedCompetency, DocumentProxy documentProxy);
       string GetCompetencyNameForListofCompetencies(IList<string> guidOfLinkedCompetencies);
       void SetLinkedCompetencyForAGuid(string linkedCompetencyRef);
       string GetLinkedCompetencyNameForAGuid(string linkedCompetencyRef);
       string GetCompetencyNameListInNumbers(IList<string> guidOfLinkedCompetencies);
    }

}
