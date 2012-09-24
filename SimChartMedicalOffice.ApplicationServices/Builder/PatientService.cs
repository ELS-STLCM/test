using System;
using System.Collections.Generic;
using System.Linq;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Extensions;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DataInterfaces;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;
using SimChartMedicalOffice.Core.Patient;

namespace SimChartMedicalOffice.ApplicationServices.Builder
{
    public class PatientService : IPatientService
    {
        private readonly IPatientDocument _patientDocument;
        private readonly IFolderDocument _folderDocument;
        private readonly IAttachmentDocument _attachmentDocument;
        public PatientService(IPatientDocument patientDocumentInstance, IFolderDocument folderDocumentInstance, IAttachmentDocument attachmentDocument)
        {
            _patientDocument = patientDocumentInstance;
            _folderDocument = folderDocumentInstance;
            _attachmentDocument = attachmentDocument;
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
            string strUrlToSave = _patientDocument.FormAndSetUrlForPatient(isEditMode ? patientObject.UniqueIdentifier : patientObject.GetNewGuidValue(), null, patientUrl, folderIdentifier, isEditMode);
            SavePersistentImage(isEditMode, patientObject, strUrlToSave);
            _patientDocument.SaveOrUpdate(strUrlToSave, patientObject);
            _patientDocument.LoadAllPatients();
            return true;
        }

        /// <summary>
        /// To save Persistent image under SimApp/Attachment/Persistent
        /// </summary>
        /// <param name="isEditMode"></param>
        /// <param name="patientObject"> </param>
        /// <param name="patientUrl"> </param>
        private void SavePersistentImage(bool isEditMode, Patient patientObject, string patientUrl)
        {
            if (!isEditMode)
            {
                string transientImage = patientObject.UploadImage;
                if (!String.IsNullOrEmpty(transientImage))
                {
                    string persistentImage;
                    MoveTransientToPersistentAttachment(transientImage, out persistentImage);
                    patientObject.UploadImage = persistentImage;
                }
            }
            else
            {
                Patient patientFromDb = GetPatientForGuid(patientUrl);
                string imgUrl = CheckIfTransientImageExistsAndCreatePersistent(patientFromDb, patientObject);
                patientObject.UploadImage = imgUrl;
            }
        }

        /// <summary>
        /// Attachment objects handler
        /// </summary>
        /// <param name="patientFromDb"></param>
        /// <param name="patientObject"></param>
        /// <returns></returns>
        private string CheckIfTransientImageExistsAndCreatePersistent(Patient patientFromDb, Patient patientObject)
        {
            string persistentImage = patientFromDb.UploadImage;
            string transientImage = patientObject.UploadImage;
            string imgUrl;
            bool isImageExistsInDb = !String.IsNullOrEmpty(patientFromDb.UploadImage);
            bool isImageExistsInUi = !String.IsNullOrEmpty(patientObject.UploadImage);
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
                case AppEnum.AttachmentFlagsForStatus.ExistsInDbNotInUi:
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
                        return AppEnum.AttachmentActions.RemoveTransientPersistentAndCreatePersistent;
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
            if (isImageExistsInUi && isImageExistsInDb)
            {
                return AppEnum.AttachmentFlagsForStatus.ExistsInUiAndDb;
            }
            if (isImageExistsInDb & !isImageExistsInUi)
            {
                return AppEnum.AttachmentFlagsForStatus.ExistsInDbNotInUi;
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
        /// <param name="isTransient"> </param>
        /// <param name="attachmentUrl"></param>
        /// <returns></returns>
        public bool SaveAttachment(string attachmentGuid, Attachment attachmentObject, bool isTransient, out string attachmentUrl)
        {
            attachmentUrl = isTransient ? _attachmentDocument.GetAssignmentUrl(DocumentPath.Module.Attachments, AppConstants.TransientAttachment) : _attachmentDocument.GetAssignmentUrl(DocumentPath.Module.Attachments);
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
            string result;
            _attachmentDocument.Delete(attachmentGuid, out result);
            return true;
        }

        //public bool DeletePatient(string patientGuid)
        //{
        //    string result;
        //    _patientDocument.Delete(string.Format(_patientDocument.GetAssignmentUrl(), patientGuid), out result);
        //    return true;
        //}
        /// <summary>
        /// to delete the perticular patient
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// To get all patients
        /// </summary>
        /// <returns></returns>
        public List<Patient> GetAllPatients()
        {
            return _patientDocument.GetAll(string.Format(_patientDocument.GetAssignmentUrl(DocumentPath.Module.Patients,AppConstants.Create), ""));
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
            IList<Patient> patientList = AppCommon.CheckIfStringIsEmptyOrNull(parentFolderIdentifier) ? _patientDocument.GetPatientItems(parentFolderIdentifier, folderType, null) : _folderDocument.GetPatientItems(parentFolderIdentifier, courseId, folderUrl);
            string sortColumnName = AppCommon.GridColumnForPatientList[sortColumnIndex - 1];
            var sortableList = patientList.AsQueryable();
            patientList = sortableList.OrderBy(sortColumnName, sortColumnOrder).ToList();
            return patientList;
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
            IList<Patient> lstPatientSearchResult = _patientDocument.GetAllPatients(course, userRole);
            IList<PatientProxy> lstPatientProxySearchResult = TransformPatientToPatientProxy(lstPatientSearchResult);
            if (!String.IsNullOrEmpty(strSearchText))
            {
                lstPatientProxySearchResult = GetPatientMatchingText(strSearchText, lstPatientProxySearchResult);
            }
            string sortColumnName = AppCommon.GridColumnForPatientSearchList[sortColumnIndex];
            var sortableList = lstPatientProxySearchResult.AsQueryable();
            lstPatientProxySearchResult = sortableList.OrderBy(sortColumnName, sortColumnOrder).ToList();
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
                        (lstSearch.CreatedTimeStamp.Date.ToString().Contains(filterBySearch)) ||
                        (lstSearch.Status != null && lstSearch.Status.ToLower().Contains(filterBySearch))
                    select lstSearch).ToList();
        }
    }
}
