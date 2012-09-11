using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;
using SimChartMedicalOffice.Common.Extensions;
using SimChartMedicalOffice.Core.DataInterfaces;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.ApplicationServices.Builder
{
    public class PatientService : IPatientService
    {
        private readonly IPatientDocument _patientDocument;
        private readonly IFolderDocument _folderDocument;
        private readonly IAttachmentDocument _attachmentDocument;
        public PatientService(IPatientDocument patientDocumentInstance, IFolderDocument folderDocumentInstance, IAttachmentDocument attachmentDocument)
        {
            this._patientDocument = patientDocumentInstance;
            this._folderDocument = folderDocumentInstance;
            this._attachmentDocument = attachmentDocument;
        }

        /// <summary>
        /// To save the patient object by getting url from document
        /// </summary>
        /// <param name="patientObject"></param>
        /// <param name="courseId"></param>
        /// <param name="patientUrl"></param>
        /// <param name="folderIdentifier"></param>
        /// <param name="isEditMode"></param>
        /// <returns></returns>
        public bool SavePatient(Patient patientObject, string courseId, string patientUrl, string folderIdentifier, bool isEditMode)
        {
            try
            {
                string strUrlToSave = "";
                if (isEditMode)
                {
                    strUrlToSave = _patientDocument.FormAndSetUrlForPatient(patientObject.UniqueIdentifier, courseId, patientUrl, folderIdentifier, isEditMode);
                }
                else {
                    strUrlToSave = _patientDocument.FormAndSetUrlForPatient(patientObject.GetNewGuidValue(), courseId, patientUrl, folderIdentifier, isEditMode);
                }
                SavePersistentImage(isEditMode, patientObject, strUrlToSave);
                _patientDocument.SaveOrUpdate(strUrlToSave, patientObject);
                _patientDocument.LoadAllPatients();
            }
            catch
            {
                //To-Do
            }
            return true;
        }
        /// <summary>
        /// To save Persistent image under SimApp/Attachment/Persistent
        /// </summary>
        /// <param name="isEditMode"></param>
        /// <param name="questionEntity"></param>
        /// <param name="questionUrlReference"></param>
        private void SavePersistentImage(bool isEditMode, Patient patientObject, string patientUrl)
        {
            string transientImage = String.Empty;
            string persistentImage = String.Empty;
            string imgUrl;
            if (!isEditMode)
            {
                transientImage = patientObject.UploadImage;
                if (!String.IsNullOrEmpty(transientImage))
                {
                    MoveTransientToPersistentAttachment(transientImage, out persistentImage);
                    patientObject.UploadImage = persistentImage;
                }                
            }
            else
            {
                Patient patientFromDb = GetPatientForGuid(patientUrl);
                imgUrl = checkIfTransientImageExistsAndCreatePersistent(patientFromDb, patientObject);
                patientObject.UploadImage = imgUrl;
            }
        }
        /// <summary>
        /// Attachment objects handler
        /// </summary>
        /// <param name="questionFromDb"></param>
        /// <param name="questionFromUi"></param>
        /// <returns></returns>
        private string checkIfTransientImageExistsAndCreatePersistent(Patient patientFromDb, Patient patientObject)
        {
            string persistentImage = patientFromDb.UploadImage;
            string transientImage = patientObject.UploadImage;
            string imgUrl = string.Empty;
            bool isImageExistsInDb = String.IsNullOrEmpty(patientFromDb.UploadImage) ? false : true;
            bool isImageExistsInUi = String.IsNullOrEmpty(patientObject.UploadImage) ? false : true;
            AppEnum.AttachmentFlagsForStatus statusOfAttachment = GetAttachmentStatus(isImageExistsInDb, isImageExistsInUi);
            AppEnum.AttachmentActions attachmentActions = GetActionToPerformForAttachment(statusOfAttachment, persistentImage, transientImage);
            CheckAndMoveTransientImages(attachmentActions, persistentImage, transientImage, out imgUrl);
            return imgUrl;
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
        /// To get the action to perform from the flags
        /// </summary>
        /// <param name="statusOfAttachment"></param>
        /// <param name="persistentImage"></param>
        /// <param name="transientImage"></param>
        /// <returns></returns>
        private AppEnum.AttachmentActions GetActionToPerformForAttachment(AppEnum.AttachmentFlagsForStatus statusOfAttachment, string persistentImage, string transientImage)
        {
            switch (statusOfAttachment)
            {
                case AppEnum.AttachmentFlagsForStatus.ExistsInDbNotInUI:
                    return AppEnum.AttachmentActions.RemovePersistent;
                case AppEnum.AttachmentFlagsForStatus.ExistsInUiNotInDb:
                    return AppEnum.AttachmentActions.CreatePersistent;
                case AppEnum.AttachmentFlagsForStatus.NotExistsInUiAndDb:
                    return AppEnum.AttachmentActions.None;
                case AppEnum.AttachmentFlagsForStatus.ExistsInUiAndDb:
                    {
                        if (persistentImage.Equals(transientImage))
                        {
                            return AppEnum.AttachmentActions.None;
                        }
                        else
                        {
                            return AppEnum.AttachmentActions.RemoveTransientPersistentAndCreatePersistent;
                        }
                    }
            }
            return AppEnum.AttachmentActions.None;
        }
        /// <summary>
        /// To check image status from Db and UI
        /// </summary>
        /// <param name="isImageExistsInDb"></param>
        /// <param name="isImageExistsInUi"></param>
        /// <returns></returns>
        private AppEnum.AttachmentFlagsForStatus GetAttachmentStatus(bool isImageExistsInDb, bool isImageExistsInUi)
        {
            if (!isImageExistsInUi && !isImageExistsInDb)
            {
                return AppEnum.AttachmentFlagsForStatus.NotExistsInUiAndDb;
            }
            else if (isImageExistsInUi && isImageExistsInDb)
            {
                return AppEnum.AttachmentFlagsForStatus.ExistsInUiAndDb;
            }
            else
            {
                if (isImageExistsInDb & !isImageExistsInUi)
                {
                    return AppEnum.AttachmentFlagsForStatus.ExistsInDbNotInUI;
                }
            }
            return AppEnum.AttachmentFlagsForStatus.ExistsInUiNotInDb;
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
        /// <summary>
        /// to delete the perticular patient
        /// </summary>
        /// <param name="patientGuid"></param>
        /// <returns></returns>
        public bool DeletePatient(string patientGuid)
        {
            string result;
            _patientDocument.Delete(string.Format(_patientDocument.Url, patientGuid), out result);
            return true;
        }
        /// <summary>
        /// To get all patients
        /// </summary>
        /// <returns></returns>
        public List<Patient> GetAllPatients()
        {
            return _patientDocument.GetAll(string.Format(_patientDocument.Url, ""));
        }
        /// <summary>
        /// to get perticular patient by passing url
        /// </summary>
        /// <param name="patientGuid"></param>
        /// <returns></returns>
        public Patient GetPatientForGuid(string patientGuid)
        {
            return _patientDocument.Get(patientGuid);
        }
        /// <summary>
        /// to get patient list for populating grid
        /// </summary>
        /// <param name="parentFolderIdentifier"></param>
        /// <param name="folderType"></param>
        /// <param name="sortColumnIndex"></param>
        /// <param name="sortColumnOrder"></param>
        /// <param name="courseId"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public IList<Patient> GetPatientItems(string parentFolderIdentifier, int folderType, int sortColumnIndex, string sortColumnOrder, string courseId, string folderUrl)
        {
            try
            {
                IList<Patient> patientList = new List<Patient>(); 
                if (AppCommon.CheckIfStringIsEmptyOrNull(parentFolderIdentifier))
                {
                    patientList = _patientDocument.GetPatientItems(parentFolderIdentifier, folderType, courseId);
                    
                }
                else
                {
                    patientList = _folderDocument.GetPatientItems(parentFolderIdentifier, courseId, folderUrl);
                }
                string sortColumnName = AppCommon.gridColumnForPatientList[sortColumnIndex - 1];
                var sortableList = patientList.AsQueryable();
                patientList = sortableList.OrderBy<Patient>(sortColumnName, sortColumnOrder).ToList<Patient>();
                return patientList;
            }
            catch
            {
                //To-Do                
            }
            return new List<Patient>();
        }

        /// <summary>
        /// To get patient list for search result
        /// </summary>
        /// <param name="strSearchText"></param>
        /// <param name="sortColumnIndex"></param>
        /// <param name="sortColumnOrder"></param>
        /// <param name="course"></param>
        /// <param name="userRole"></param>
        /// <returns></returns>
        public List<PatientProxy> GetSearchResultsForPatient(string strSearchText, int sortColumnIndex, string sortColumnOrder, string course, string userRole)
        {
            IList<Patient> lstPatientSearchResult = new List<Patient>();
            IList<PatientProxy> lstPatientProxySearchResult = new List<PatientProxy>();
            lstPatientSearchResult = _patientDocument.GetAllPatients(course, userRole);
            lstPatientProxySearchResult = TransformPatientToPatientProxy(lstPatientSearchResult);
            if (!String.IsNullOrEmpty(strSearchText))
            {
                lstPatientProxySearchResult = GetPatientMatchingText(strSearchText, lstPatientProxySearchResult);
            }
            string sortColumnName = AppCommon.gridColumnForPatientSearchList[sortColumnIndex];
            var sortableList = lstPatientProxySearchResult.AsQueryable();
            lstPatientProxySearchResult = sortableList.OrderBy<PatientProxy>(sortColumnName, sortColumnOrder).ToList<PatientProxy>();
            return lstPatientProxySearchResult.ToList();
        }

        /// <summary>
        /// Transforming the patient object to patient proxy object
        /// </summary>
        /// <param name="patientList"></param>
        /// <returns></returns>
        private List<PatientProxy> TransformPatientToPatientProxy(IList<Patient> patientList)
        {
            List<PatientProxy> lstOfQuestionProxy = (from patient in patientList
                                                     select new PatientProxy
                                                     {
                                                         FirstName = patient.FirstName,
                                                         LastName = patient.LastName,
                                                         Sex = patient.Sex,
                                                         Age = patient.FormAgeText(),
                                                         DateOfBirth = patient.DateOfBirth,
                                                         Status = patient.Status,
                                                         AgeInDays = patient.AgeInDays,
                                                         AgeInMonths = patient.AgeInMonths,
                                                         AgeInYears = patient.AgeInYears,
                                                         Url = patient.Url,
                                                         CreatedTimeStamp = patient.CreatedTimeStamp
                                                     }).ToList();
            return lstOfQuestionProxy;
        }
        /// <summary>
        /// To filter patient proxy list for search text
        /// </summary>
        /// <param name="filterBySearch"></param>
        /// <param name="lstQuestionSearchResult"></param>
        /// <returns></returns>
        private List<PatientProxy> GetPatientMatchingText(string filterBySearch, IList<PatientProxy> lstQuestionSearchResult)
        {
            return (from lstSearch in lstQuestionSearchResult
                    where
                        (lstSearch.FirstName != null && lstSearch.FirstName.ToLower().Contains(filterBySearch)) ||
                        (lstSearch.LastName != null && lstSearch.LastName.ToLower().Contains(filterBySearch)) ||
                        (lstSearch.Sex != null && lstSearch.Sex.ToLower().Contains(filterBySearch)) ||
                        (lstSearch.DateOfBirth != null && lstSearch.DateOfBirth.Contains(filterBySearch)) ||
                        (lstSearch.AgeInYears != 0 && lstSearch.AgeInYears.ToString().Contains(filterBySearch)) ||
                        (lstSearch.AgeInMonths != 0 && lstSearch.AgeInMonths.ToString().Contains(filterBySearch)) ||
                        (lstSearch.AgeInDays != 0 && lstSearch.AgeInDays.ToString().Contains(filterBySearch)) ||
                        (lstSearch.CreatedTimeStamp != null && lstSearch.CreatedTimeStamp.Date.ToString().Contains(filterBySearch)) ||
                        (lstSearch.Status != null && lstSearch.Status.ToLower().Contains(filterBySearch))
                    select lstSearch).ToList();
        }
    }
}
