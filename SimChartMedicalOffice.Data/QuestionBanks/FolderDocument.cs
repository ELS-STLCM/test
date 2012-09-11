using SimChartMedicalOffice.Core.AssignmentBuilder;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using SimChartMedicalOffice.Core.DataInterfaces.SkillSetBuilder;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.SkillSetBuilder;
using System.Collections.Generic;
using SimChartMedicalOffice.Common.Utility;
using System.Linq;
using System;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.DataInterfaces.AssignmentBuilder;

namespace SimChartMedicalOffice.Data
{
    public class FolderDocument : KeyValueRepository<Folder>, IFolderDocument
    {
        public override string Url
        {
            get
            {
                //return "SimApp/Courses/{3}/{0}/{1}/{2}";
                return "SimApp/Courses/{0}/{1}/SubFolders{2}";
            }
        }

        private readonly IQuestionBankDocument _questionBankDocument;
        private readonly IPatientDocument _patientDocument;
        private readonly IAssignmentRepositoryDocument _assignmentDocument;
        private readonly ISkillSetRepositoryDocument _skillSetDocument;
        
        
        /// <summary>
        /// initalising the questionbank and patient document
        /// </summary>
        /// <param name="questionBankDocument"></param>
        /// <param name="patientDocument"></param>
        /// /// <param name="assignmentDocument"></param>
        public FolderDocument(IQuestionBankDocument questionBankDocument, IPatientDocument patientDocument, IAssignmentRepositoryDocument assignmentDocument, ISkillSetRepositoryDocument skillSetDocument)
        {
            this._questionBankDocument = questionBankDocument;
            this._patientDocument = patientDocument;
            this._assignmentDocument = assignmentDocument;
            this._skillSetDocument = skillSetDocument;
        }

        /// <summary>
        /// To fetch sub folders of an expanded folder.
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="courseId"></param>
        /// <param name="folderType"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public IList<Folder> GetSubfolders(string parentFolderIdentifier, string courseId, int folderType, string folderUrl, out IList<BreadCrumbProxy> breadCrumbFolders, bool breadCrumbNeeded)
        {
            breadCrumbFolders = null;
            // if breadCrumb needed and not parentTab folders like QuestionBank
            if (breadCrumbNeeded && parentFolderIdentifier!="")
            {
                breadCrumbFolders = GetBreadCrumbList(folderUrl, parentFolderIdentifier, folderType);
            }
            string jsonString = GetJsonDocument(GetCorrectFolderUrl(courseId, folderType, folderUrl,parentFolderIdentifier));
            Dictionary<string, Folder> subFolderList;
            subFolderList = (jsonString=="null")?new Dictionary<string, Folder>():JsonSerializer.DeserializeObject<Dictionary<string, Folder>>(jsonString);
            if (subFolderList != null)
            {
                foreach (var folderItem in subFolderList)
                {
                    folderItem.Value.UniqueIdentifier = folderItem.Key.ToString();
                    folderItem.Value.Url = GetCorrectFolderUrl(courseId, folderType, folderUrl, parentFolderIdentifier);
                }
            }
            return ConvertDictionarytoObject(subFolderList);
        }

        private IList<BreadCrumbProxy> GetBreadCrumbList(string currentFoderUrl, string currentFolderIdentifier, int folderType)
        {
            IList<BreadCrumbProxy> lstFolder = new List<BreadCrumbProxy>();
            Folder MasterFolder = null;
            string sourceUrl = "";
            string[] stringSeparators = new string[] { "SubFolders" };
            string[] array = currentFoderUrl.Split(stringSeparators, StringSplitOptions.None);
            if (array.Length > 0)
            {
                sourceUrl = array[0];
            }
            //initiate breadcrumb implementation only if a subfolder
            if (array.Length > 1)
            {
                // get root folder object
                switch ((AppCommon.FolderType) folderType)
                {
                    case AppCommon.FolderType.QuestionBank:
                        MasterFolder = _questionBankDocument.GetQuestionBank();

                        break;
                    case AppCommon.FolderType.PatientRepository:
                        MasterFolder = _patientDocument.GetPatientRepository("", "");
                        break;
                    case AppCommon.FolderType.AssignmentRepository:
                        MasterFolder = _assignmentDocument.GetAssignmentRepository();
                        break;
                    case AppCommon.FolderType.SkillSetRepository:
                        MasterFolder = _skillSetDocument.GetSkillSetRepository();
                        break;
                }

                // to give rootFolder name and send other properties as ""
                targetFolder=new BreadCrumbProxy();
                targetFolder.Name = AppCommon.GetFolderTypeName(folderType);
                targetFolder.Url = "";
                targetFolder.UniqueIdentifier = "";
                lstFolder.Add(targetFolder);
                targetFolder = null;
                // to get parents and grand parents
                foreach (string item in array.Skip(1))
                {
                    if (item != "")
                    {
                        var folderId = item.Split('/')[1];
                        var folderUrl = sourceUrl + "SubFolders";

                        sourceUrl = sourceUrl + "SubFolders" + item;

                        if (MasterFolder != null)
                        {
                            TraverseEachFolderForQuestions(MasterFolder.SubFolders, folderId);
                        }
                        if (targetFolder != null)
                        {
                            // assigning the correct URL for this folder
                            targetFolder.Url = folderUrl;
                            lstFolder.Add(targetFolder);
                            targetFolder = null;
                        }
                    }
                }

                // to get current Folder
                if (MasterFolder != null)
                {
                    TraverseEachFolderForQuestions(MasterFolder.SubFolders, currentFolderIdentifier);
                }
                if (targetFolder != null)
                {
                    lstFolder.Add(targetFolder);
                    targetFolder = null;
                }

            }
            return lstFolder;

        }

        //do not delete - this proxy folder is used for each BreadCrumb link creation
        public BreadCrumbProxy targetFolder = null;

        
        /// <summary>
        /// to recursive find a folder inside the Master Folder object
        /// </summary>
        /// <param name="folderContent"></param>
        private void TraverseEachFolderForQuestions(Dictionary<string, Folder> folderContent, string folderId)
        {
            if (folderContent != null && folderContent.Count > 0)
            {
                IList<Folder> folders = folderContent.Select(folder => folder.Value).ToList();
                //folders.ToList().ForEach(F => GetTotalFolderList(F));
                foreach (var folderDict in folderContent)
                {
                    if (folderDict.Key == folderId)
                    {
                        targetFolder=new BreadCrumbProxy();
                        targetFolder.Name = folderDict.Value.Name;
                        targetFolder.UniqueIdentifier = folderId;
                        break;
                    }
                    TraverseEachFolderForQuestions(folderDict.Value.SubFolders, folderId);
                }
            }
        }

        /// <summary>
        /// Method to get the question items
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="courseId"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public IList<Question> GetQuestionItems(string parentFolderIdentifier, string courseId, string folderUrl)
        {
            string jsonString = GetJsonDocument(string.Concat(folderUrl, "/", parentFolderIdentifier, "/QuestionItems"));
            Dictionary<string, Question> questionList;
            questionList = (jsonString=="null")?new Dictionary<string, Question>():JsonSerializer.DeserializeObject<Dictionary<string, Question>>(jsonString);
            foreach (var folderItem in questionList)
            {
                folderItem.Value.UniqueIdentifier = folderItem.Key.ToString();
                folderItem.Value.Url = string.Concat(folderUrl, "/", parentFolderIdentifier, "/QuestionItems/", folderItem.Value.UniqueIdentifier);
            }
            return ConvertDictionarytoObject(questionList);
        }

        public IList<Patient> GetPatientItems(string parentFolderIdentifier, string courseId, string folderUrl)
        {
            string jsonString = GetJsonDocument(string.Concat(folderUrl, "/", parentFolderIdentifier, "/Patients"));
            Dictionary<string, Patient> patientList;
            patientList = (jsonString == "null") ? new Dictionary<string, Patient>() : JsonSerializer.DeserializeObject<Dictionary<string, Patient>>(jsonString);
            foreach (var folderItem in patientList)
            {
                folderItem.Value.UniqueIdentifier = folderItem.Key.ToString();
                folderItem.Value.Url = string.Concat(folderUrl, "/", parentFolderIdentifier, "/Patients/", folderItem.Value.UniqueIdentifier);
            }
            return ConvertDictionarytoObject(patientList);
        }

        public IList<Core.SkillSetBuilder.SkillSet> GetSkillSetItems(string parentFolderIdentifier, string courseId, string folderUrl)
        {
            string jsonString = GetJsonDocument(string.Concat(folderUrl, "/", parentFolderIdentifier, "/SkillSets"));
            Dictionary<string, Core.SkillSetBuilder.SkillSet> skillSetList;
            skillSetList = (jsonString == "null") ? new Dictionary<string, Core.SkillSetBuilder.SkillSet>() : JsonSerializer.DeserializeObject<Dictionary<string, Core.SkillSetBuilder.SkillSet>>(jsonString);
            foreach (var folderItem in skillSetList)
            {
                folderItem.Value.UniqueIdentifier = folderItem.Key.ToString();
            }
            return ConvertDictionarytoObject(skillSetList);
        }

        /// <summary>
        /// Method to get the correct folder url
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="folderType"></param>
        /// <param name="folderUrl"></param>
        /// <param name="parentFolderIdentifier"></param>
        /// <returns></returns>
        public string GetCorrectFolderUrl(string courseId, int folderType, string folderUrl, string parentFolderIdentifier)
        {
            if (String.IsNullOrEmpty(folderUrl))
                return string.Format(Url, courseId, AppCommon.GetFolderType(folderType), folderUrl);
            else
            {
                return  string.Concat(folderUrl, "/", parentFolderIdentifier, "/SubFolders");
            }
        }
        /// <summary>
        /// To convert Dictionary of Folder objects to List of Folder Objects
        /// </summary>
        /// <param name="subFolderItems"></param>
        /// <returns></returns>
        public IList<Folder> ConvertDictionarytoObject(Dictionary<string, Folder> subFolderItems)
        {

            return ((subFolderItems != null) ? (subFolderItems.Select(subFolder => subFolder.Value).ToList()) : new List<Folder>());
        }

        public IList<Question> ConvertDictionarytoObject(Dictionary<string, Question> questionItems)
        {
            return ((questionItems != null) ? (questionItems.Select(questionItem => questionItem.Value).ToList()) : new List<Question>());
        }

        public IList<Core.SkillSetBuilder.SkillSet> ConvertDictionarytoObject(Dictionary<string, Core.SkillSetBuilder.SkillSet> skillSetItems)
        {
            return ((skillSetItems != null) ? (skillSetItems.Select(skillSetItem => skillSetItem.Value).ToList()) : new List<Core.SkillSetBuilder.SkillSet>());
        }
        /// <summary>
        /// method to convert dictionary to object
        /// </summary>
        /// <param name="patients"></param>
        /// <returns></returns>
        public IList<Patient> ConvertDictionarytoObject(Dictionary<string, Patient> patients)
        {
            return ((patients != null) ? (patients.Select(patient => patient.Value).ToList()) : new List<Patient>());
        }

        #region Folder to Folder Proxy Transformation - Do not delete it
        /*
        /// <summary>
        /// To fetch sub folders of an expanded folder as list of Folder Proxy objects.
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <returns></returns>
        public IList<FolderProxy> GetSubfolders(string parentFolderIdentifier)
        {
            string jsonString = GetJsonDocument((parentFolderIdentifier == "") ? (string.Format(Url, parentFolderIdentifier)) : parentFolderIdentifier + "/SubFolders");
            Dictionary<string, Folder> subFolderList;
            subFolderList = JsonSerializer.DeserializeObject<Dictionary<string, Folder>>(jsonString);
            return ConvertDictionarytoObject(subFolderList);
        }         
        /// <summary>
        /// To convert Dictionary of Folder objects to List of Folder Proxy Objects
        /// </summary>
        /// <param name="subFolderItems"></param>
        /// <returns></returns>
        private IList<FolderProxy> ConvertDictionarytoObject(Dictionary<string, Folder> subFolderItems)
        {
            IList<FolderProxy> subFolderList = new List<FolderProxy>();
            if (subFolderItems != null)
            {
                foreach (KeyValuePair<string, Folder> subFolder in subFolderItems)
                {
                    ((Folder)subFolder.Value).Name = subFolder.Key;
                    subFolderList.Add(ConvertFoldertoProxy(subFolder.Value));
                }
            }
            return subFolderList;
        }

        /// <summary>
        /// To convert a Folder object to Folder Proxy Object
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private FolderProxy ConvertFoldertoProxy(Folder folder)
        {
            FolderProxy folderProxy = new FolderProxy();
            if (folder != null)
            {
                Type type = folder.GetType();
                //foreach (System.Reflection.PropertyInfo objProp in type.GetProperties())
                //{
                //    if (objProp.CanWrite && objProp.Name.ToUpper() != "QUESTIONITEMS" && objProp.Name.ToUpper() != "SUBFOLDERS")
                //    {
                //        objProp.SetValue(folderProxy, type.GetProperty(objProp.Name).GetValue(this, null), null);
                //    }
                //}
                folderProxy = ObjectTransfer<Folder, FolderProxy>(folder);
                folderProxy.QuestionItems = (folder.QuestionItems!=null)?(folder.QuestionItems.Select(question => question.Value).ToList()):new List<Question>();
                folderProxy.SubFolders = (folder.SubFolders!=null)?(folder.SubFolders.Select(subFolder => subFolder.Value).ToList()): new List<Folder>();
            }
            return folderProxy;
        }*/
        #endregion Folder Proxy object usage End



        /// <summary>
        /// Method to get the Assignment items
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="courseId"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public IList<Assignment> GetAssignmentItems(string parentFolderIdentifier, string courseId, string folderUrl)
        {
            string jsonString = GetJsonDocument(string.Concat(folderUrl, "/", parentFolderIdentifier, "/Assignments"));
            Dictionary<string, Assignment> assignmentList;
            assignmentList = (jsonString == "null") ? new Dictionary<string, Assignment>() : JsonSerializer.DeserializeObject<Dictionary<string, Assignment>>(jsonString);
            foreach (var folderItem in assignmentList)
            {
                folderItem.Value.UniqueIdentifier = folderItem.Key.ToString();
                folderItem.Value.Url = string.Concat(folderUrl, "/", parentFolderIdentifier, "/Assignments/", folderItem.Value.UniqueIdentifier);
            }
            return ConvertDictionarytoObject(assignmentList);
        }

        public IList<Assignment> ConvertDictionarytoObject(Dictionary<string, Assignment> questionItems)
        {
            return ((questionItems != null) ? (questionItems.Select(questionItem => questionItem.Value).ToList()) : new List<Assignment>());
        }


    }
}
