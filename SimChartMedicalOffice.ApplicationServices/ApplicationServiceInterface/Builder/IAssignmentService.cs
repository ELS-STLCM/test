using System.Collections.Generic;
using SimChartMedicalOffice.Core.AssignmentBuilder;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.SkillSetBuilder;

namespace SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder
{
    public interface IAssignmentService
    {
        bool SaveAssignment(Assignment assignment, string courseId, bool isEditMode, string assignmentUrl);
        IList<Assignment> GetAssignmentItems(string parentFolderIdentifier, int folderType, string courseId, string folderUrl);
        Assignment GetAssignment(string courseId);
        List<AssignmentProxy> GetSearchResultsForAssignment(string strSearchText, int sortColumnIndex,
                                                            string sortColumnOrder, string strQuestionType);
        List<Patient> GetPatientListOfAssignment(string assignmentUrl);
        IList<string> ConvertSkillsetToCompetencies(Dictionary<string, SkillSet> skillsetItems);        
        bool DeleteAssignmentQuestion(string questionUrl);
        IList<DocumentProxy> GetAssignmentInfos(string assignmentIdentifier);
        IList<Resource> GetResourcesForAnAssignment(string assignmentUrl);
        bool PublishAnAssignment(string assignmentUrl);
        List<Patient> GetAllPatientName();
        string SaveAssignment(string assignmentUrl, Assignment assignmentToSave);
        string SaveAssignmentMetaData(AssignmentProxySave assignmentProxyObject, string assignmentUrl, string courseId, bool isEditMode);
    }
}
