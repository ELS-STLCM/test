using SimChartMedicalOffice.Core.AssignmentBuilder;
using SimChartMedicalOffice.Core.SkillSetBuilder;
using SimChartMedicalOffice.Data.Repository;
using System.Collections.Generic;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.ProxyObjects;

namespace SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks
{
    public interface IFolderDocument : IKeyValueRepository<Folder>
    {
        //Dictionary<string, FolderStructure> GetQuestionBankFolders(string url);
        IList<Folder> GetSubfolders(string parentFolderIdentifier, string courseId, int folderType, string folderUrl, out IList<BreadCrumbProxy> breadCrumbFolders, bool breadCrumbNeeded);
        string GetCorrectFolderUrl(string courseId, int folderType, string folderUrl, string parentFolderIdentifier);
        IList<Question> ConvertDictionarytoObject(Dictionary<string, Question> questionItems);
        IList<Question> GetQuestionItems(string parentFolderIdentifier, string courseId, string folderUrl);
        IList<Core.Patient.Patient> GetPatientItems(string parentFolderIdentifier, string courseId, string folderUrl);
        IList<Assignment> GetAssignmentItems(string parentFolderIdentifier, string courseId, string folderUrl);
        IList<SkillSet> GetSkillSetItems(string parentFolderIdentifier, string courseId, string folderUrl);
      
    }
}
