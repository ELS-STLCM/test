using System.Collections.Generic;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder
{
    public interface IQuestionBankService
    {
        bool SaveQuestion(Question questionObject, DropBoxLink dropBox, string questionUrl, string folderIdentifier, bool isEditMode,bool isImageMovedToPersistent);
        List<DocumentProxy> GetAllQuestions();
        Question GetQuestion(string strGuidOfQuestion);
        bool SaveAttachment(string attachmentGuid, Attachment attachmentObject, bool isTransient, out string attachmentUrl);
        Attachment GetAttachment(string attachmentGuid);
        bool RemoveAttachment(string attachmentGuid);
        //IList<FolderProxy> GetSubfolders(string parentFolderIdentifier);
        //bool DeleteQuestion(string questionGuid);
        string SaveFolder(Folder folderObject, int folderType, string courseId, string folderUrl, string folderGuid);
        Folder GetFolder(string folderGuid);
        bool DeleteFolder(string folderGuid);
        bool UpdateFolder(int folderType, string folderUrl, string folderGuid, string courseId, Folder folderFromForm);
        bool UpdateSubFoldersForFolder(int folderType, string folderUrl, string folderGuid, string courseId,
                                       Dictionary<string, Folder> subFolderDictToSave);
        IList<Folder> GetSubfolders(string parentFolderIdentifier, string courseId, int folderType, string folderUrl, out IList<BreadCrumbProxy> breadCrumbFolders, bool breadCrumbNeeded);
        IList<Question> GetQuestionItems(string parentFolderIdentifier, int folderType, DropBoxLink dropBox, string folderUrl);
        List<DocumentProxy> GetSearchResultsForQuestionBank(string strSearchText, int sortColumnIndex, string sortColumnOrder, string strQuestionType);
        Dictionary<string, int> GetQuestionType();
        
        string GetQuestionUrlToUpdate(string strQuestionGuid);
        string FormUrlForSkillSetQuestionToQuestionBank(Question questionItemToEdit, DropBoxLink dropBox);
        bool IsQuestionNameExists(string strQuestionText);
        string CloneAttachmentToPersistent(string attachmentGuid);
        void CloneImagesForQuestion(Question questionObj);
        void DeleteImagesForQuestion(Question questionObj);
    }
}
