using System;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Forms;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DataInterfaces;
using SimChartMedicalOffice.Core.DataInterfaces.Forms;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using System.Collections.Generic;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Core.Patient;



namespace SimChartMedicalOffice.ApplicationServices.Forms
{
    public class FormsService : IFormsService
    {
        private readonly ISimAppDocument _simOfficeDocument;
        private readonly IPriorAuthorizationRequestFormDocument _priorAuthorizationRequestFormDocument;
        private readonly IPatientDocument _patientDocument;
        private readonly IPatientRecordsAccessFormDocument _patientRecordsAccessForm;
        private readonly IReferralFormDocument _referralForm;
        private readonly INoticeOfPrivacyPracticeDocument _noticeOfPrivacyPracticeDocument;
        private readonly IAttachmentDocument _attachmentDocument;
        private readonly IBillOfRightsDocument _billOfRightsDocument;
        

        public FormsService(ISimAppDocument simOfficeDocumentInstance,
                                IPriorAuthorizationRequestFormDocument priorAuthorizationRequestFormDocumentInstance
                                , IPatientDocument patientDocumentInstance, IPatientRecordsAccessFormDocument patientRecordsAccessForm,
            IReferralFormDocument referralFormDocumentInstance, INoticeOfPrivacyPracticeDocument noticeOfPrivacyPracticeDocument, 
            IAttachmentDocument attachmentDocument, IBillOfRightsDocument billOfRightsDocument)
        {
            this._simOfficeDocument = simOfficeDocumentInstance;
            this._priorAuthorizationRequestFormDocument = priorAuthorizationRequestFormDocumentInstance;
            this._patientDocument = patientDocumentInstance;
            this._patientRecordsAccessForm = patientRecordsAccessForm;
            this._referralForm = referralFormDocumentInstance;
            this._noticeOfPrivacyPracticeDocument = noticeOfPrivacyPracticeDocument;
            this._attachmentDocument = attachmentDocument;
            this._billOfRightsDocument = billOfRightsDocument;
        }

        public bool SavePriorAuthorizationRequestForm(PriorAuthorizationRequestForm priorAuthorizationRequestFormObject, string courseId, string userRole,
                                                                                  string UID, string SID)
        {
            _priorAuthorizationRequestFormDocument.SaveOrUpdate(_priorAuthorizationRequestFormDocument.FormAndSetUrlForStudentPatient(courseId,userRole,UID,SID,priorAuthorizationRequestFormObject.PatientReferenceId,priorAuthorizationRequestFormObject.UniqueIdentifier), priorAuthorizationRequestFormObject);
            return true;
        }
        

        public bool SaveReferralForm(ReferralForm referralFormObject, string courseId, string userRole,
                                                                                  string UID, string SID)
        {
            _referralForm.SaveOrUpdate(_referralForm.FormAndSetUrlForStudentPatient(courseId,userRole,UID,SID,referralFormObject.PatientReferenceId,referralFormObject.UniqueIdentifier), referralFormObject);
            return true;
        }

        public bool SavePatientRegistration(Patient patientObject)
        {
            _patientDocument.SaveOrUpdate(_patientDocument.Url, patientObject);
            return true;
        }

        public IList<Patient> GetAllPatient(string courseId, string userRole, string UID,string SID)
        {
            //return _patientDocument.GetAll(_patientDocument.FormAndSetUrlForStudentPatient(courseId,userRole,UID,SID,""));
            return _patientDocument.GetAllPatientForAssignment(courseId);
        }

        public Patient GetPatientRegistration(string patientGuid)
        {
            return _patientDocument.Get(_patientDocument.Url, patientGuid);
        }

        public IList<PriorAuthorizationRequestForm> GetAllPriorAuthorizationRequest()
        {
            return
                _priorAuthorizationRequestFormDocument.GetAll(string.Format(_priorAuthorizationRequestFormDocument.Url,
                                                                            ""));
        }

        public PriorAuthorizationRequestForm GetPatientPriorAuthorizationRequest(string patientGuid)
        {
            return _priorAuthorizationRequestFormDocument.Get(_priorAuthorizationRequestFormDocument.Url, patientGuid);
        }

        public Patient GetPatient (string courseId,string userRole, string UID,string SID, string patientGuid)
        {
            return _patientDocument.Get(_patientDocument.FormAndSetUrlForStudentPatient(courseId,userRole,UID,SID,patientGuid));
        }

        public Patient GetPatientFromPatientRepository (string patientGUID)
        {
            return _patientDocument.GetPatientFromPatientRepository(patientGUID);
        }

        public Patient GetPatientFromAssignmentRepository(string patientGUID)
        {
            return _patientDocument.GetPatientFromAssignmentRepository(patientGUID);
        }
        /// <summary>
        /// To save Notice of privacy form
        /// </summary>
        /// <param name="noticeOfPrivacyPractice"></param>
        /// <param name="patientGuid"></param>
        /// <param name="courseId"></param>
        /// <param name="userRole"></param>
        /// <param name="UID"></param>
        /// <param name="SID"></param>
        /// <returns></returns>
        public bool SaveNoticeOfPrivacyPractice(NoticeOfPrivacyPractice noticeOfPrivacyPractice, string patientGuid, string courseId, string userRole, string UID, string SID)
        {
            string _noticeDocumentUrl = "";
            _noticeDocumentUrl = _noticeOfPrivacyPracticeDocument.SaveNoticeFormUrl(patientGuid, courseId, userRole, UID, SID);
            _noticeOfPrivacyPracticeDocument.SaveOrUpdate(_noticeDocumentUrl, noticeOfPrivacyPractice);
            return true;
        }
        /// <summary>
        /// To save Bill of rights form
        /// </summary>
        /// <param name="patientRecordsAccessForm"></param>
        /// <param name="courseId"></param>
        /// <param name="userRole"></param>
        /// <param name="UID"></param>
        /// <param name="SID"></param>
        /// <returns></returns>
        public bool SavePatientRecordsAccessForm(PatientRecordsAccessForm patientRecordsAccessForm, string courseId, string userRole,
                                                                                  string UID, string SID)
        {
            _patientRecordsAccessForm.SaveOrUpdate(_patientRecordsAccessForm.FormAndSetUrlForStudentPatient(courseId,userRole,UID,SID,patientRecordsAccessForm.PatientReferenceId,patientRecordsAccessForm.UniqueIdentifier), patientRecordsAccessForm);
            return true;
        }
        public bool UpdatePatientRecordsAccessForm(string patientGuid, PatientRecordsAccessForm patientRecordsAccessForm)
        {
            string patientRecordsAccessFormUrl = string.Format(_patientRecordsAccessForm.Url, patientGuid);
            _patientRecordsAccessForm.SaveOrUpdate(patientRecordsAccessFormUrl,patientRecordsAccessForm);
            return true;
        }

        public bool DeletePatientRecordsAccessForm(string patientGuid)
        {
            string result;
            string patientRecordsAccessFormUrl = string.Format(_patientRecordsAccessForm.Url,patientGuid);
            _patientRecordsAccessForm.Delete(patientRecordsAccessFormUrl, out result);
            return true;
        }

        public IList<PatientRecordsAccessForm> GetAllPatientRecordsAccessForm()
        {
            return _patientRecordsAccessForm.GetAll(string.Format(_patientRecordsAccessForm.Url,""));
        }

        public IList<PatientRecordsAccessForm> GetAllPatientRecordsAccessFormsForPatient(string courseId, string userRole, string UID, string SID, string patientGuid, string formId)
        {
            return _patientRecordsAccessForm.GetAllPatientRecordsAccessFormsForPatient(courseId,userRole,UID,SID,patientGuid, formId);
        }

        public IList<PriorAuthorizationRequestForm> GetAllPriorAuthorizationRequestFormsForPatient(string courseId, string userRole, string UID, string SID, string patientGuid, string formId)
        {
            return _priorAuthorizationRequestFormDocument.GetAllPriorAuthorizationRequestFormsForPatient(courseId, userRole, UID, SID, patientGuid, formId);
        }

        public IList<ReferralForm> GetAllReferralFormsForPatient(string courseId, string userRole, string UID, string SID, string patientGuid, string formId)
        {
            return _referralForm.GetAllReferralFormsForPatient(courseId, userRole, UID, SID, patientGuid, formId);
        }
        /// <summary>
        /// To get the Notice of privacy form for perticular patient
        /// </summary>
        /// <param name="patientGUID"></param>
        /// <param name="courseId"></param>
        /// <param name="userRole"></param>
        /// <param name="UID"></param>
        /// <param name="SID"></param>
        /// <returns></returns>
        public NoticeOfPrivacyPractice GetNoticeOfPrivacyPracticeDocument(string patientGUID, string courseId, string userRole, string UID, string SID)
        {
            return _noticeOfPrivacyPracticeDocument.GetNoticeOfPrivacyPracticeDocument(patientGUID, courseId, userRole, UID, SID);
        }
        /// <summary>
        /// To get the bill of rights form for perticular patient
        /// </summary>
        /// <param name="patientGUID"></param>
        /// <param name="courseId"></param>
        /// <param name="userRole"></param>
        /// <param name="UID"></param>
        /// <param name="SID"></param>
        /// <returns></returns>
        public BillOfRights GetBillOfRightsDocument(string patientGUID, string courseId, string userRole, string UID, string SID)
        {
            return _billOfRightsDocument.GetBillOfRightsDocument(patientGUID, courseId, userRole, UID, SID);
        }
        public PatientRecordsAccessForm GetPatientRecordsAccessForm(string courseId, string userRole, string UID, string SID, string patientGuid, string formId)
        {
            return _patientRecordsAccessForm.Get(_patientRecordsAccessForm.FormAndSetUrlForStudentPatient(courseId,userRole,UID,SID,patientGuid,formId));
        }

        public PriorAuthorizationRequestForm GetPriorAuthorizationRequestForm(string courseId, string userRole, string UID, string SID, string patientGuid, string formId)
        {
            return _priorAuthorizationRequestFormDocument.Get(_priorAuthorizationRequestFormDocument.FormAndSetUrlForStudentPatient(courseId,userRole,UID,SID,patientGuid,formId));
        }

        public ReferralForm GetReferralForm(string courseId, string userRole, string UID, string SID, string patientGuid, string formId)
        {
           return _referralForm.Get(_referralForm.FormAndSetUrlForStudentPatient(courseId,userRole,UID,SID,patientGuid,formId));
        }

        public IList<ReferralForm> GetAllReferralFormItems()
        {
            return _referralForm.GetAll(string.Format(_referralForm.Url,""));
        }

        public ReferralForm GetReferralFormItem(string patientGuid)
        {
            string referralFormUrl = string.Format(_referralForm.Url, patientGuid);
            return _referralForm.Get(referralFormUrl);
        }

        public bool DeleteReferralForm(string patientGuid)
        {
            string referralFormUrlList;
            string result;
            referralFormUrlList = string.Format(_referralForm.Url,patientGuid);
            _patientRecordsAccessForm.Delete(referralFormUrlList, out result);
            return true;
        }

        public bool UpdateReferralForm(string patientGuid, ReferralForm referralForm)
        {
            string referralFormUrl = string.Format(_referralForm.Url,patientGuid);
            _referralForm.SaveOrUpdate(referralFormUrl, referralForm);
            return true;
        }

       

        public bool DeletePriorAuthorizationRequestForm(string patientGuid)
        {
            string result;
            string priorAuthorizationRequestFormUrl = string.Format(_priorAuthorizationRequestFormDocument.Url,patientGuid);
            _priorAuthorizationRequestFormDocument.Delete(priorAuthorizationRequestFormUrl, out result);
            return true;
        }

        public Attachment GetPdfAttachment(string attachmentGuid)
        {
            string attachmentDocumentUrl = string.Format(_attachmentDocument.Url, attachmentGuid);
            return _attachmentDocument.Get(attachmentDocumentUrl);
        }

        public Attachment GetBillOfRightsPdfData()
        {
            return _billOfRightsDocument.GetBillOfRightsPdfData();

        }

        public Attachment GetNoticeOfPrivacyPracticePdfData()
        {
            return _noticeOfPrivacyPracticeDocument.GetNoticeOfPrivacyPracticePdfData();

        }
        public bool SavePatientBillOfRights(BillOfRights patientBillOfRights, string patientGuid, string courseId, string userRole, string UID, string SID)
        {
            string billOfRightsUrl = "";
            billOfRightsUrl = _billOfRightsDocument.SaveBillOfRightsUrl(patientGuid, courseId, userRole, UID, SID);
            _billOfRightsDocument.SaveOrUpdate(billOfRightsUrl, patientBillOfRights);
            return true;
        }
    }
}
