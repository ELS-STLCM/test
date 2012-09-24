using System.Collections.Generic;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.Competency;
using SimChartMedicalOffice.Core.QuestionBanks;

namespace SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency
{
   public interface ICompetencyService
    {
       void SaveCompetencyList(Dictionary<string, Dictionary<string, Core.Competency.Competency>> compList);
       bool SaveCompetency(Core.Competency.Competency competencyUiObject, string competencyUrl, bool isEditMode);
       bool DeleteCompetency(Core.Competency.Competency deleteCompetency);
       bool SaveCompetencySource(CompetencySources competencySourcesUiObject, string competencySourcesUrl, bool isEditMode);
       bool SaveApplicationModule(ApplicationModules applicationModuleUiObject, string applicationModuleUrl, bool isEditMode);
       IList<Core.Competency.Competency> GetAllCompetencies();
       IList<string> GetAllCategories();
       List<AutoCompleteProxy> GetAllCompetencyListForDropDown();
       List<AutoCompleteProxy> GetCompetenciesStringListInFormat(IList<Core.Competency.Competency> competencyList);
       IList<CompetencySources> GetAllCompetecnySources();
       IList<ApplicationModules> GetAllApplicationModules();
       List<AutoCompleteProxy> GetFilteredCompetenciesBasedOnString(string strFilterText);
       Core.Competency.Competency GetCompetency(string competencyGuid);
       void SetLinkedCompetencyTextForAQuestions(string guidOfLinkedCompetency, DocumentProxy documentProxy);
       string GetCompetencyNameForListofCompetencies(IList<string> guidOfLinkedCompetencies);
       void SetLinkedCompetencyForAGuid(string linkedCompetencyRef);
       string GetLinkedCompetencyNameForAGuid(string linkedCompetencyRef);
       string GetCompetencyNameListInNumbers(IList<string> guidOfLinkedCompetencies);
    }

}
