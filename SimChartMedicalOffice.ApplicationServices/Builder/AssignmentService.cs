using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.AssignmentBuilder;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder;
using SimChartMedicalOffice.Core.DataInterfaces.AssignmentBuilder;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.SkillSetBuilder;
using System.Linq.Expressions;
using SimChartMedicalOffice.Common.Extensions;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.SkillSetBuilder;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DataInterfaces;
using System.Collections;


namespace SimChartMedicalOffice.ApplicationServices.Builder
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IFolderDocument _folderDocument;
        private readonly IAssignmentDocument _assignmentDocument;
        private readonly IAssignmentRepositoryDocument _assignmentRepositoryDocument;
        private readonly IPatientDocument _patientDocument;
        private readonly ICompetencyService _competencyService;
        private readonly IAttachmentDocument _attachmentDocument;
        private readonly ISkillSetService _skillSetService;

        private IList<AssignmentProxy> allAssignments = new List<AssignmentProxy>();

        public AssignmentService(IAssignmentDocument assignmentDocumentInstance, IAssignmentRepositoryDocument assignmentRepositoryDocument, ICompetencyService competencyService, IPatientDocument patientDocument, IAttachmentDocument attachmentDocument, IFolderDocument folderDocument, ISkillSetService skillSetService)
        {
            this._assignmentDocument = assignmentDocumentInstance;
            this._assignmentRepositoryDocument = assignmentRepositoryDocument;
            this._patientDocument = patientDocument;
            this._competencyService = competencyService;
            this._attachmentDocument = attachmentDocument;
            this._folderDocument = folderDocument;
            this._skillSetService = skillSetService;
        }

        /// <summary>
        /// Save the Assignment
        /// </summary>
        /// <param name="assignment"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public bool SaveAssignment(Assignment assignment, string courseId, bool isEditMode, string assignmentUrl)
        {
            if (!isEditMode)
            {
                assignmentUrl = _assignmentDocument.FormAssignmentUrl(courseId, assignment.GetNewGuidValue());
            }

            if (assignmentUrl != null && assignmentUrl != "")
            {
                SavePersistentImage(false, assignment, assignmentUrl);
                _assignmentDocument.SaveOrUpdate(assignmentUrl, assignment);
            }
            return true;
        }
        /// <summary>
        /// To save Persistent image under SimApp/Attachment/Persistent
        /// </summary>
        /// <param name="isEditMode"></param>
        /// <param name="questionEntity"></param>
        /// <param name="questionUrlReference"></param>
        private void SavePersistentImage(bool isEditMode, Assignment assignmentObject, string assignmentUrl)
        {
            string transientImage = String.Empty;
            string persistentImage = String.Empty;
            string imgUrl;
            if (!isEditMode)
            {
                if (assignmentObject.PatientImageReferance != null)
                {
                    for (int item = 0; item < assignmentObject.PatientImageReferance.Count(); item++)
                    {
                        string strPersistentImage = string.Empty;
                        MoveTransientToPersistentAttachment(assignmentObject.PatientImageReferance[item], out strPersistentImage);
                        assignmentObject.PatientImageReferance[item] = strPersistentImage;
                    }
                }
            }
            else
            {
                Assignment assignmentFromDb = GetAssignment(assignmentUrl);
                string imgReference = String.Empty;
                if (assignmentObject.PatientImageReferance.Count() > 0 && assignmentObject.PatientImageReferance.Count() > 0)
                {
                    for (int item = 0; item < assignmentObject.PatientImageReferance.Count(); item++)
                    {
                        CheckAndMoveTransientImages(AppEnum.AttachmentActions.CreatePersistent, string.Empty, assignmentObject.PatientImageReferance[item], out imgReference);
                        assignmentObject.PatientImageReferance[item] = imgReference;
                    }
                }
                if (assignmentFromDb.PatientImageReferance.Count() > 0 && assignmentFromDb.PatientImageReferance.Count() > 0)
                {
                    for (int item = 0; item < assignmentFromDb.PatientImageReferance.Count(); item++)
                    {
                        CheckAndMoveTransientImages(AppEnum.AttachmentActions.RemovePersistent, assignmentObject.PatientImageReferance[item], string.Empty, out imgReference);

                    }
                }
            }
        }

        /// <summary>
        /// To move transient images from Attachment/Transient to Attachment/Persistent
        /// </summary>
        /// <param name="attachmentActions"></param>
        /// <param name="persistentImage"></param>
        /// <param name="transientImage"></param>
        /// <param name="imgReference"></param>
        private void CheckAndMoveTransientImages(AppEnum.AttachmentActions attachmentActions, string persistentImage, string transientImage, out string imgReference)
        {
            string persistentImageTemp = String.Empty;
            switch (attachmentActions)
            {
                case AppEnum.AttachmentActions.RemovePersistent:
                    RemoveAttachment(persistentImage);
                    persistentImageTemp = String.Empty;
                    break;
                case AppEnum.AttachmentActions.RemoveTransientPersistentAndCreatePersistent:
                    {
                        RemoveAttachment(persistentImage);
                        MoveTransientToPersistentAttachment(transientImage, out persistentImageTemp);
                    }
                    break;
                case AppEnum.AttachmentActions.CreatePersistent:
                    {
                        MoveTransientToPersistentAttachment(transientImage, out persistentImageTemp);
                    }
                    break;
                case AppEnum.AttachmentActions.None:
                    imgReference = persistentImage;
                    return;
                default:
                    break;
            }
            imgReference = persistentImageTemp;
        }

        /// <summary>
        /// To remove the Transient attachment and move to persistent.
        /// </summary>
        /// <param name="transientImage"></param>
        /// <param name="persistentImageTemp"></param>
        public void MoveTransientToPersistentAttachment(string transientImage, out string persistentImageTemp)
        {
            Attachment transientAttachment = GetAttachment(transientImage);
            SaveAttachment(String.Empty, transientAttachment, false, out persistentImageTemp);
            RemoveAttachment(transientImage);
        }

        /// <summary>
        /// to get the saved image 
        /// </summary>
        /// <param name="attachmentGuid"></param>
        /// <returns></returns>
        public Attachment GetAttachment(string attachmentGuid)
        {
            return _attachmentDocument.Get(attachmentGuid);
        }

        /// <summary>
        /// to save the image 
        /// </summary>
        /// <param name="attachmentGuid"></param>
        /// <param name="attachmentObject"></param>
        /// <param name="attachmentUrl"></param>
        /// <returns></returns>
        public bool SaveAttachment(string attachmentGuid, Attachment attachmentObject, bool isTransient, out string attachmentUrl)
        {
            if (isTransient)
            {
                attachmentUrl = _attachmentDocument.GetAttachementTransientUrl();
            }
            else
            {
                attachmentUrl = _attachmentDocument.Url;
            }
            attachmentUrl = _attachmentDocument.SaveOrUpdate(attachmentUrl, attachmentObject);
            return true;
        }

        /// <summary>
        /// to remove the saved image 
        /// </summary>
        /// <param name="attachmentGuid"></param>
        /// <returns></returns>
        public bool RemoveAttachment(string attachmentGuid)
        {
            try
            {
                string result;
                _attachmentDocument.Delete(attachmentGuid, out result);
            }
            catch
            {

                //To-Do
            }

            return true;
        }

        public IList<Assignment> GetAssignmentItems(string parentFolderIdentifier, int folderType, string courseId, string folderUrl)
        {
            try
            {
                if (AppCommon.CheckIfStringIsEmptyOrNull(parentFolderIdentifier))
                {
                    return _assignmentDocument.GetAssignmentItems(parentFolderIdentifier, folderType, courseId);
                }
                return _folderDocument.GetAssignmentItems(parentFolderIdentifier, courseId, folderUrl);
            }
            catch
            {
                //To-Do                
            }
            return new List<Assignment>();
        }

        /// <summary>
        /// To get the perticular assignment
        /// </summary>
        /// <param name="assignmentUrl"></param>
        /// <returns></returns>
        public Assignment GetAssignment(string assignmentUrl)
        {
            Assignment assignmentObj;
            assignmentObj = _assignmentDocument.Get(assignmentUrl);
            return assignmentObj;
        }

        /// <summary>
        /// To get search results from question bank
        /// </summary>
        /// <param name="strSearchText"></param>
        /// <param name="sortColumnIndex"></param>
        /// <param name="sortColumnOrder"></param>
        /// <returns></returns>
        public List<AssignmentProxy> GetSearchResultsForAssignment(string strSearchText, int sortColumnIndex, string sortColumnOrder, string strModule)
        {
            IList<AssignmentProxy> lstAssignmentSearchResult = new List<AssignmentProxy>();
            IList<AssignmentProxy> lstAssignmentSearchResultTemp = new List<AssignmentProxy>();
            lstAssignmentSearchResult = GetAllAssignmentsInAssignmentRepository();
            if (!String.IsNullOrEmpty(strSearchText))
            {
                lstAssignmentSearchResultTemp = GetAssignmentsMatchingText(strSearchText, lstAssignmentSearchResult);
            }
            if (!String.IsNullOrEmpty(strModule))
            {
                lstAssignmentSearchResultTemp = GetAssignmentsMatchingModule(strModule, lstAssignmentSearchResultTemp);
            }
            string sortColumnName = AppCommon.gridColumnForAssignmentSearchList[sortColumnIndex];
            var sortableList = lstAssignmentSearchResultTemp.AsQueryable();
            lstAssignmentSearchResultTemp = sortableList.OrderBy<AssignmentProxy>(sortColumnName, sortColumnOrder).ToList<AssignmentProxy>();
            return lstAssignmentSearchResultTemp.ToList();
        }


        /// <summary>
        /// Method for saving the assignment Meta Data
        /// </summary>
        /// <param name="assignmentProxyObject"></param>
        /// <param name="assignmentUrl"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public string SaveAssignmentMetaData(AssignmentProxySave assignmentProxyObject, string assignmentUrl, string courseId,bool isEditMode)
        {
            Assignment assignmentToSave = TransformAssignmentFromProxy(assignmentProxyObject,isEditMode,assignmentUrl);
            assignmentToSave.Patients = new Dictionary<string, Patient>();
            string assignmentGuid = assignmentToSave.GetNewGuidValue();
            if (!isEditMode)
            {
                if (string.IsNullOrEmpty(assignmentUrl))
                {
                    assignmentUrl = _assignmentDocument.FormAssignmentUrl(courseId, assignmentGuid);
                }
                else
                {
                    assignmentUrl += "/Assignments/" + assignmentGuid;
                }
            }

            if (assignmentProxyObject.Resources != null && assignmentProxyObject.Resources.Count > 0)
            {
                assignmentToSave.Resources = new Dictionary<string, Resource>();
                foreach (Resource resource in assignmentProxyObject.Resources)
                {
                    string resourceGuid = resource.GetNewGuidValue();
                    resource.Url = assignmentUrl + "/Resources/" + resourceGuid;
                    assignmentToSave.Resources.Add(resourceGuid, resource);
                }
            }
            if (AppCommon.CheckIfStringIsEmptyOrNull(assignmentProxyObject.PatientReference))
            {
                if (assignmentProxyObject.Patient != null)
                {
                    assignmentToSave.Patients.Add(assignmentProxyObject.Patient.GetNewGuidValue(),
                                                  assignmentProxyObject.Patient);
                }
            }
            else
            {
                Patient patientFromRepository = _assignmentDocument.GetPatientFromPatientRepository(assignmentProxyObject.PatientReference);
                if (patientFromRepository != null)
                {
                    patientFromRepository.ParentReferenceGuid = patientFromRepository.Url;
                    string patientGuid = patientFromRepository.GetNewGuidValue();
                    patientFromRepository.Url = assignmentUrl + "/Patients/" + patientGuid;
                    patientFromRepository.IsAssignmentPatient = true;
                    patientFromRepository.UniqueIdentifier = "";
                    assignmentToSave.Patients = new Dictionary<string, Patient>();
                    assignmentToSave.Patients.Add(patientGuid, patientFromRepository);
                }
            }
            assignmentToSave.SkillSets = new Dictionary<string, Core.SkillSetBuilder.SkillSet>();
            if (assignmentProxyObject.SkillSets != null)
            {
                foreach (AssignmentSkillSetProxy assignmentSkillSetProxy in assignmentProxyObject.SkillSets)
                {
                    SkillSet skillSetForAssignment = _skillSetService.GetSkillSet(assignmentSkillSetProxy.SkillSetIdentifier);
                    if (skillSetForAssignment != null)
                    {
                        //if skillset's parentreferenctguid is empty, new skillset save
                        if (string.IsNullOrEmpty(skillSetForAssignment.ParentReferenceGuid))
                        {
                            string skillSetGuid = skillSetForAssignment.GetNewGuidValue();
                            skillSetForAssignment.ParentReferenceGuid = assignmentSkillSetProxy.SkillSetIdentifier;
                            skillSetForAssignment.SequenceNumber = assignmentSkillSetProxy.SequenceNumber;
                            skillSetForAssignment = UpdateSkillSetUrlForAssignment(assignmentUrl, skillSetGuid,
                                                                                   skillSetForAssignment);
                            assignmentToSave.SkillSets.Add(skillSetGuid, skillSetForAssignment);
                        }
                            //else its an existing skillset in the assignment
                        else
                        {
                            var skillSetUniqueIdenitfier = assignmentSkillSetProxy.SkillSetIdentifier.Split('/').Last();
                            skillSetForAssignment.SequenceNumber = assignmentSkillSetProxy.SequenceNumber;
                            assignmentToSave.SkillSets.Add(skillSetUniqueIdenitfier, skillSetForAssignment);
                        }
                    }
                }
            }
            SaveAssignment(assignmentUrl,assignmentToSave);
            return assignmentUrl;
        }

        private SkillSet UpdateSkillSetUrlForAssignment(string assignmentUrl, string skillSetGuid, SkillSet skillSetToUpdate)
        {
            skillSetToUpdate.Url = assignmentUrl + "/SkillSets/" + skillSetGuid;
            return UpdateQuestionUrlForAssignmentSkillSet(skillSetToUpdate);
        }

        private SkillSet UpdateQuestionUrlForAssignmentSkillSet(SkillSet skillSetToUpdate)
        {
            foreach (KeyValuePair<string, Question> question in skillSetToUpdate.Questions)
            {
                question.Value.SkillSetReferenceUrlOfQuestion = question.Value.Url;
                question.Value.Url = skillSetToUpdate.Url + "/Questions/" + question.Key;
            }
            return skillSetToUpdate;
        }

        public string SaveAssignment(string assignmentUrl, Assignment assignmentToSave)
        {
            return _assignmentDocument.SaveOrUpdate(assignmentUrl, assignmentToSave);
        }

        private Assignment TransformAssignmentFromProxy(AssignmentProxySave assignmentProxyObject, bool isEditMode, string assignmentUrl)
        {
            Assignment assignmentToSave = new Assignment();

            if(isEditMode)
            {
                assignmentToSave = _assignmentDocument.Get(assignmentUrl);
            }

            if (assignmentToSave != null)
            {
                assignmentToSave.Title = assignmentProxyObject.Title;
                assignmentToSave.Keywords = assignmentProxyObject.Keywords;
                assignmentToSave.Module = assignmentProxyObject.Module;
                assignmentToSave.Duration = assignmentProxyObject.Duration;
                assignmentToSave.Status = AppCommon.Status_InProgress;
                if (!assignmentProxyObject.IsActive)
                {
                    assignmentToSave.DeletedBy = assignmentProxyObject.DeletedBy;
                    assignmentToSave.DeletedTimeStamp = assignmentProxyObject.DeletedTimeStamp;
                }
                else
                {
                    assignmentToSave.ModifiedBy = assignmentProxyObject.ModifiedBy;
                    assignmentToSave.ModifiedTimeStamp = assignmentProxyObject.ModifiedTimeStamp;
                    assignmentToSave.CreatedBy = assignmentProxyObject.CreatedBy;
                    assignmentToSave.CreatedTimeStamp = assignmentProxyObject.CreatedTimeStamp;
                }
            }
            return assignmentToSave;
        }

        /// <summary>
        /// To get the search text matching questions , search for question text and competencies
        /// </summary>
        /// <param name="strSearchText"></param>
        /// <param name="lstQuestionSearchResult"></param>
        /// <returns></returns>
        private List<AssignmentProxy> GetAssignmentsMatchingText(string strSearchText, IList<AssignmentProxy> lstQuestionSearchResult)
        {
            var lstQuestionSearchResultTemp = (from lstSearch in lstQuestionSearchResult
                                               where (
                                                   !String.IsNullOrEmpty(lstSearch.AssignmentTitle) && lstSearch.AssignmentTitle.ToLower().Contains(strSearchText.ToLower())
                                                   || (!String.IsNullOrEmpty(lstSearch.Keywords) && lstSearch.Keywords.ToLower().Contains(strSearchText.ToLower()))
                                                   || (!String.IsNullOrEmpty(lstSearch.ModuleText) && lstSearch.ModuleText.ToLower().Contains(strSearchText.ToLower()))
                                                   || (!String.IsNullOrEmpty(lstSearch.Patients) && lstSearch.Patients.ToLower().Contains(strSearchText.ToLower()))
                                                   || (from lstSkillSet in lstSearch.SkillSets where (!String.IsNullOrEmpty(lstSkillSet.SkillSetTitle) && lstSkillSet.SkillSetTitle.ToLower().Contains(strSearchText.ToLower())) select lstSkillSet).Any())
                                               select lstSearch).ToList();
            return lstQuestionSearchResultTemp.ToList();
        }

        public List<Patient> GetAdminPatientList()
        {
            return _patientDocument.GetAllPatients("", "");
        }

        public List<Patient> GetPatientListOfAssignment(string assignmentUrl)
        {
            Assignment assignment = new Assignment();
            List<Patient> patientListInAdmin = new List<Patient>();
            List<Patient> patientListInAssignment = new List<Patient>();
            List<Patient> patientList = new List<Patient>();

            patientListInAdmin = GetAdminPatientList();
            assignment = _assignmentDocument.Get(assignmentUrl);
            patientListInAssignment = ConvertDictionarytoPatientObject(assignment.Patients, assignmentUrl);

            patientList = patientListInAdmin.Where(admin => patientListInAssignment.Any(assign => assign.UniqueIdentifier == admin.UniqueIdentifier)).ToList();

            if (patientList.Count() <= 0)
            {
                patientListInAdmin = patientListInAdmin.OrderBy(x => x.LastName).ToList();
                if (patientListInAssignment.Count() != 0)
                {
                    if (patientListInAssignment[0].LastName != "" && patientListInAssignment[0].LastName != null)
                    {
                        patientList.AddRange(patientListInAssignment);
                    }
                }
                patientList.AddRange(patientListInAdmin);
            }
            else
            {
                patientListInAdmin = patientListInAdmin.Where(admin => patientListInAssignment.Any(assign => assign.UniqueIdentifier != admin.UniqueIdentifier)).ToList();
                patientListInAdmin = patientListInAdmin.OrderBy(x => x.LastName).ToList();
                patientList.AddRange(patientListInAdmin);
            }

            return patientList;
        }

        private List<Patient> ConvertDictionarytoPatientObject(Dictionary<string, Patient> patientsListItems,string assignmentUrl)
        {
            List<Patient> patientList = new List<Patient>();
            if (patientsListItems != null && patientsListItems.Count > 0)
            {
                foreach (var patient in patientsListItems)
                {
                    if (patient.Value.ParentReferenceGuid == null)
                    {
                        patient.Value.UniqueIdentifier = patient.Key;
                        patient.Value.Url = assignmentUrl + "/Patients/" + patient.Key;
                    }
                    else
                    {
                        patient.Value.UniqueIdentifier = patient.Value.ParentReferenceGuid.Split('/').Last();
                    }
                    patientList.Add(patient.Value);

                }
            }

            return patientList;
        }

        /// <summary>
        /// To get the search text matching questions , search for question text and competencies
        /// </summary>
        /// <param name="strModule"></param>
        /// <param name="lstQuestionSearchResult"></param>
        /// <returns></returns>
        private List<AssignmentProxy> GetAssignmentsMatchingModule(string strModule, IList<AssignmentProxy> lstQuestionSearchResult)
        {
            var lstAssignmentSearchResultTemp = (from lstSearch in lstQuestionSearchResult
                                                 where (
                                                     !String.IsNullOrEmpty(lstSearch.ModuleText) && lstSearch.ModuleText.ToLower().Contains(strModule.ToLower()))
                                                 select lstSearch).ToList();
            return lstAssignmentSearchResultTemp.ToList();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentFolder"></param>
        private void GetTotalAssignmentList(Folder parentFolder)
        {
            if (parentFolder != null)
            {
                TraverseEachFolderForAssignments(parentFolder.SubFolders);
                CollectAssignmentFromAssignmentRepository(parentFolder.Assignments, parentFolder);
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderContent"></param>
        private void TraverseEachFolderForAssignments(Dictionary<string, Folder> folderContent)
        {
            if (folderContent != null && folderContent.Count > 0)
            {
                IList<Folder> folders = folderContent.Select(folder => folder.Value).ToList();
                folders.ToList().ForEach(F => GetTotalAssignmentList(F));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignments"></param>
        private void CollectAssignmentFromAssignmentRepository(Dictionary<string, Assignment> assignments, Folder parentFolder)
        {
            if (assignments != null && assignments.Count > 0)
            {
                var assignmentList = assignments.Select(assignment => assignment.Value).ToList();
                allAssignments = allAssignments.Concat(TransformAssignmentsToAssignmentProxy(assignmentList, parentFolder)).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignmentList"></param>
        /// <param name="parentFolder"></param>
        /// <returns></returns>
        private List<AssignmentProxy> TransformAssignmentsToAssignmentProxy(IList<Assignment> assignmentList, Folder parentFolder)
        {
            List<AssignmentProxy> lstOfAssignmentProxy = (from lstassignmentItem in assignmentList
                                                          select new AssignmentProxy
                                                          {
                                                              FolderName = (!String.IsNullOrEmpty(parentFolder.Name) ? parentFolder.Name : AppCommon.AssignmentLandingPageFolderName),
                                                              FolderIdentifier = (!String.IsNullOrEmpty(parentFolder.Url) ? parentFolder.Url.Split('/').Last() : String.Empty),
                                                              FolderUrl = (!String.IsNullOrEmpty(parentFolder.Url) ? parentFolder.Url.Replace((!String.IsNullOrEmpty(parentFolder.Url) ? "/" + parentFolder.Url.Split('/').Last() : String.Empty), "") : String.Empty),
                                                              Patients = ((lstassignmentItem.Patients != null) && (lstassignmentItem.Patients.Count) > 0) ? ConvertDictionarytoObject(lstassignmentItem.Patients).Aggregate((first, next) => first + ", " + next) : "",
                                                              AssignmentTitle = (!String.IsNullOrEmpty(lstassignmentItem.Title) ? lstassignmentItem.Title : string.Empty),
                                                              ModuleText = ((lstassignmentItem.Module) != null && (lstassignmentItem.Module.Count) > 0) ? lstassignmentItem.Module.Aggregate((first, next) => first + ", " + next) : "",
                                                              LinkedCompetencies = ((lstassignmentItem.SkillSets) != null && (lstassignmentItem.SkillSets.Count) > 0) ? _competencyService.GetCompetencyNameForListofCompetencies(ConvertSkillsetToCompetencies(lstassignmentItem.SkillSets)) : "",
                                                              Duration = (!String.IsNullOrEmpty(lstassignmentItem.Duration) ? lstassignmentItem.Duration : string.Empty),
                                                              CreatedOn = (!String.IsNullOrEmpty(lstassignmentItem.CreatedOn) ? lstassignmentItem.CreatedOn : string.Empty),
                                                              Status = (!String.IsNullOrEmpty(lstassignmentItem.Status) ? lstassignmentItem.Status : string.Empty),
                                                              CreatedTimeStamp = lstassignmentItem.CreatedTimeStamp,
                                                              Url = lstassignmentItem.Url,
                                                              UniqueIdentifier = (!String.IsNullOrEmpty(lstassignmentItem.Url) ? lstassignmentItem.Url.Split('/').Last() : String.Empty),
                                                              Keywords = (!String.IsNullOrEmpty(lstassignmentItem.Keywords) ? lstassignmentItem.Keywords.Split('/').Last() : String.Empty),
                                                              SkillSets = ((lstassignmentItem.SkillSets != null) ? (lstassignmentItem.SkillSets.Select(skillSetItem => skillSetItem.Value).ToList()) : new List<SkillSet>())
                                                          }).ToList();

            //lstQuestionSearchResult.ToList().ForEach(question => _competencyService.SetLinkedCompetencyTextForAQuestions(question.LinkedItemReference, question))
            return lstOfAssignmentProxy;
        }

        public IList<string> ConvertSkillsetToCompetencies(Dictionary<string, SkillSet> skillsetItems)
        {
            List<string> competencyList = new List<string>();
            if (skillsetItems != null)
            {
                foreach (var skillsetItem in skillsetItems)
                {
                    if (skillsetItem.Value.Competencies != null)
                    {
                        competencyList.AddRange(skillsetItem.Value.Competencies.ToList());
                    }
                }
            }
            return competencyList.Distinct().ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IList<String> ConvertDictionarytoObject(Dictionary<string, Patient> patientsListItems)
        {
            IList<Patient> patientList = ((patientsListItems != null) ? (patientsListItems.Select(patientsListItem => patientsListItem.Value).ToList()) : new List<Patient>());
            return patientList.Select(patient => patient.LastName + ", " + patient.FirstName + " " + patient.MiddleInitial).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<AssignmentProxy> GetAllAssignmentsInAssignmentRepository()
        {
            Folder assignmentRepository = _assignmentRepositoryDocument.GetAssignmentRepository();
            allAssignments = new List<AssignmentProxy>();
            GetTotalAssignmentList(assignmentRepository);
            return allAssignments.ToList();
        }

        public bool DeleteAssignmentQuestion(string questionUrl)
        {
            string result;
            return _assignmentDocument.Delete(questionUrl, out result);
        }
        /// <summary>
        /// To get list of skill sets for an assignment
        /// </summary>
        /// <param name="assignmentObj"></param>
        /// <returns></returns>
        public IList<SkillSet> GetSkillSetsForAnAssignment(Assignment assignmentObj)
        {
            List<SkillSet> skillSetsForAssignment = new List<SkillSet>();
            skillSetsForAssignment = assignmentObj.SkillSets.Select(s => s.Value).ToList();
            return skillSetsForAssignment;
        }
        /// <summary>
        /// To get list of questions in a skill set for an assignment 
        /// </summary>
        /// <param name="skillSetObj"></param>
        /// <returns></returns>
        public IList<Question> GetQuestionsInASkillForAnAssignment(SkillSet skillSetObj)
        {
            List<Question> QuestionskillSetsForAssignment = new List<Question>();
            QuestionskillSetsForAssignment = skillSetObj.Questions.Select(s => s.Value).ToList();
            return QuestionskillSetsForAssignment;
        }

        /// <summary>
        /// To get list of questions from a skill set grouped by competency names.
        /// </summary>
        /// <param name="skillSetItem"></param>
        /// <returns></returns>
        private List<DocumentListProxy> GetQuestionGroupedByCompetencyForASkillSet(SkillSet skillSetItem)
        {
            IList<Question> QuestionskillSetsForAssignmentTemp = GetQuestionsInASkillForAnAssignment(skillSetItem);
            List<DocumentListProxy> questionsForAssignmentList = new List<DocumentListProxy>();
            List<DocumentListProxy> questionsForAssignmentListTemp = new List<DocumentListProxy>();

            //Get List of questions with competency reference.
            foreach (var itemVal in QuestionskillSetsForAssignmentTemp)
            {
                DocumentListProxy DocumentProxyItem = new DocumentListProxy();
                string linkedComp = _competencyService.GetLinkedCompetencyNameForAGuid(itemVal.CompetencyReferenceGUID);
                DocumentProxyItem.ItemReference = linkedComp;
                DocumentProxyItem.Name = itemVal.QuestionText;
                questionsForAssignmentList.Add(DocumentProxyItem);
            }
            var questionsGroupedByCompetency = questionsForAssignmentList.GroupBy(question => question.ItemReference);
            //Grouping questions by competency
            foreach (var group in questionsGroupedByCompetency)
            {
                List<string> qnTextList = new List<string>();
                string competencyVal = String.Empty;
                DocumentListProxy DocumentProxyItem = new DocumentListProxy();
                foreach (var groupItem in group)
                {
                    qnTextList.Add(groupItem.Name);
                    competencyVal = groupItem.ItemReference;
                }
                DocumentProxyItem.Name = competencyVal;
                DocumentProxyItem.Items = qnTextList;
                questionsForAssignmentListTemp.Add(DocumentProxyItem);
            }
            return questionsForAssignmentListTemp;
        }

        /// <summary>
        /// To get list of skill set information with the questions grouped by competencies.
        /// </summary>
        /// <param name="assignmentIdentifier"></param>
        /// <returns></returns>
        public IList<DocumentProxy> GetAssignmentInfos(string assignmentIdentifier)
        {
            List<DocumentProxy> skillSetsForAssignment = new List<DocumentProxy>();
            Assignment assignmentObj = GetAssignment(assignmentIdentifier);
            IList<SkillSet> skillSetsForAssignmentTemp = GetSkillSetsForAnAssignment(assignmentObj);
            foreach (SkillSet item in skillSetsForAssignmentTemp)
            {
                DocumentProxy docObj = new DocumentProxy();
                docObj.Text = item.SkillSetTitle;
                docObj.ListOfItems = GetQuestionGroupedByCompetencyForASkillSet(item);
                skillSetsForAssignment.Add(docObj);
            }

            return skillSetsForAssignment;
        }

        /// <summary>
        /// To get list of resources for an assignment
        /// </summary>
        /// <param name="assignmentUrl"></param>
        /// <returns></returns>
        public IList<Resource> GetResourcesForAnAssignment(string assignmentUrl)
        {
            Assignment assignmentObj = _assignmentDocument.Get(assignmentUrl);
            if (assignmentObj != null)
            {
                if (assignmentObj.Resources != null && assignmentObj.Resources.Count > 0)
                {
                    return assignmentObj.Resources.Select(r => r.Value).ToList();
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignmentUrl"></param>
        /// <returns></returns>
        public bool PublishAnAssignment(string assignmentUrl)
        {
            Assignment assignmentObj = _assignmentDocument.Get(assignmentUrl);
            if (assignmentObj != null)
            {
                assignmentObj.Status = AppCommon.Status_Published;
                _assignmentDocument.SaveOrUpdate(assignmentUrl, assignmentObj);
                return true;
            }
            return false;
        }

        public List<Patient> GetAllPatientName()
        {
            return _patientDocument.GetAllPatients("", "");
        }
    }
}
