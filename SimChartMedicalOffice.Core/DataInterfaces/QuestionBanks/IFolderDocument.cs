using System.Collections.Generic;
using SimChartMedicalOffice.Core.AssignmentBuilder;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.SkillSetBuilder;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks
{
    public interface IFolderDocument : IKeyValueRepository<Folder>
    {
        //Dictionary<string, FolderStructure> GetQuestionBankFolders(string url);
        IList<Folder> GetSubfolders(string parentFolderIdentifier, string courseId, int folderType, string folderUrl, out IList<BreadCrumbProxy> breadCrumbFolders, bool breadCrumbNeeded);
        string GetCorrectFolderUrl(DropBoxLink dropBox, int folderType, string folderUrl, string parentFolderIdentifier);
        IList<Question> ConvertDictionarytoObject(Dictionary<string, Question> questionItems);
        IList<Question> GetQuestionItems(string parentFolderIdentifier, DropBoxLink dropBox, string folderUrl);
        IList<Core.Patient.Patient> GetPatientItems(string parentFolderIdentifier, string courseId, string folderUrl);
        IList<Assignment> GetAssignmentItems(string parentFolderIdentifier, string courseId, string folderUrl);
        IList<SkillSet> GetSkillSetItems(string parentFolderIdentifier, string courseId, string folderUrl);
      
    }
}
