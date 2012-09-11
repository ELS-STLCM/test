using SimChartMedicalOffice.Core;
using System.Collections.Generic;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Common;

namespace SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder
{
    public interface IQuestionBankService
    {
        bool SaveQuestion(Question questionObject, string courseId, string questionUrl, string folderIdentifier, bool isEditMode);
        List<DocumentProxy> GetAllQuestions();
        Question GetQuestion(string strGuidOfQuestion);
        bool SaveAttachment(string attachmentGuid, Attachment attachmentObject, bool isTransient, out string attachmentUrl);
        Attachment GetAttachment(string attachmentGuid);
        bool RemoveAttachment(string attachmentGuid);
        //IList<FolderProxy> GetSubfolders(string parentFolderIdentifier);
        bool DeleteQuestion(string questionGuid);
        string SaveFolder(Folder folderObject, int folderType, string courseId, string folderUrl, string folderGuid);
        Folder GetFolder(string folderGuid);
        bool DeleteFolder(string folderGuid);
        bool UpdateFolder(int folderType, string folderUrl, string folderGuid, string courseId, Folder folderFromForm);
        bool UpdateSubFoldersForFolder(int folderType, string folderUrl, string folderGuid, string courseId,
                                       Dictionary<string, Folder> subFolderDictToSave);
        IList<Folder> GetSubfolders(string parentFolderIdentifier, string courseId, int folderType, string folderUrl, out IList<BreadCrumbProxy> breadCrumbFolders, bool breadCrumbNeeded);
        IList<Question> GetQuestionItems(string parentFolderIdentifier, int folderType, string courseId, string folderUrl);
        List<DocumentProxy> GetSearchResultsForQuestionBank(string strSearchText, int sortColumnIndex, string sortColumnOrder, string strQuestionType);
        Dictionary<string, int> GetQuestionType();
        
        string GetQuestionUrlToUpdate(string strQuestionGuid);
        string FormUrlForSkillSetQuestionToQuestionBank(Question questionItemToEdit, string courseId);
        bool IsQuestionNameExists(string strQuestionText);
    }
}
