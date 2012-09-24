using System.Collections.Generic;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.SkillSetBuilder;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder
{
    public interface ISkillSetService
    {
        IList<Question> GetQuestionsForSkillSet(string skillSeTUrl);
        IList<SkillSet> GetSkillSetItems(string parentFolderIdentifier, int folderType, string courseId, string folderUrl);
        IList<DocumentProxy> GetQuestionsForPreview(string skillSeTUrl);
        string GetFocusForSkillSet(string skillSeTUrl);
        string GetSourcesForSkillSet(string skillSeTUrl);
        SkillSet GetSkillSet(string skillSetUrl);
        List<SkillSetProxy> GetSearchResultsForSkillSet(string strSearchText, int sortColumnIndex, string sortColumnOrder, string strSource);
        bool PublishSkillSet(string skillSetUrl);

        IList<DocumentProxy> GetCompetencyQuestionListInSkillSet(string skillSetUniqueIdentifier, string strSearchText,
                                                                 string strQuestionType);
        bool SaveSkillSet(SkillSet skillSetObjectFromUi, DropBoxLink dropBox, string skillSetUrl, string folderIdentifier,
                          bool isEditMode,out string skillSetIdentifier);
        IList<Core.Competency.Competency> GetAllCompetenciesForSkillSets(IList<SkillSet> skillSets);
        IList<string> GetLinkedCompetencySources(IList<string> competencyGuids,
                                                 IList<Core.Competency.Competency> competencyList);
        void SetLinkedCompetencyTextForAQuestions(string guidOfLinkedCompetency, SkillSetProxy skillSetProxy);
        IList<AutoCompleteProxy> GetFormattedCompetenciesForSkillSet(SkillSet skillSetObj);
        List<AutoCompleteProxy> GetCompetencyGuidListInSkillSetForFlexBox(string skillSetUniqueIdentifier);
        bool SaveSkillStructure(string uniqueIdentifier, List<DocumentProxy> documentProxyQuestionOrderList);
        List<SkillSetProxy> GetAllSkillSetsInSkillSetRepository();
        List<SkillSetProxy> GetFilteredSkillSetsBasedOnCompetency(string competencyGuid);
        List<SkillSetProxy> TransformSkillSetsToSkillSetProxy(IList<SkillSet> skillSetList,
                                                              Folder parentFolder);
        IList<Core.Competency.Competency> GetAllCompetenciesForASkillSet(SkillSet skillSet);
        void SwapSkillSetSave(SkillSet skillSetObjToSave, string skillSetUrl);
    }
}
