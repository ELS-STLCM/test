using System.Collections.Generic;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Forms;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DataInterfaces.Forms;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using SimChartMedicalOffice.Core.DropBox;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Core.Patient;



namespace SimChartMedicalOffice.ApplicationServices.Forms
{
    public class FormsService : IFormsService
    {
        private readonly IPriorAuthorizationRequestFormDocument _priorAuthorizationRequestFormDocument;
        private readonly IPatientDocument _patientDocument;
        private readonly IPatientRecordsAccessFormDocument _patientRecordsAccessForm;
        private readonly IReferralFormDocument _referralForm;
        private readonly INoticeOfPrivacyPracticeDocument _noticeOfPrivacyPracticeDocument;
       // private readonly IAttachmentDocument _attachmentDocument;
        private readonly IBillOfRightsDocument _billOfRightsDocument;
        private readonly IMedicalRecordsRelease _medicalRecordsRelease;
        

        public FormsService(IPriorAuthorizationRequestFormDocument priorAuthorizationRequestFormDocumentInstance
                                , IPatientDocument patientDocumentInstance, IPatientRecordsAccessFormDocument patientRecordsAccessForm,
            IReferralFormDocument referralFormDocumentInstance, INoticeOfPrivacyPracticeDocument noticeOfPrivacyPracticeDocument,
            IBillOfRightsDocument billOfRightsDocument, IMedicalRecordsRelease medicalRecordsRelease)
        {
            _priorAuthorizationRequestFormDocument = priorAuthorizationRequestFormDocumentInstance;
            _patientDocument = patientDocumentInstance;
            _patientRecordsAccessForm = patientRecordsAccessForm;
            _referralForm = referralFormDocumentInstance;
            _noticeOfPrivacyPracticeDocument = noticeOfPrivacyPracticeDocument;
          //  _attachmentDocument = attachmentDocument;
            _billOfRightsDocument = billOfRightsDocument;
            _medicalRecordsRelease = medicalRecordsRelease;
        }

        public bool SavePriorAuthorizationRequestForm(PriorAuthorizationRequestForm priorAuthorizationRequestFormObject, DropBoxLink dropBox)
        {
            _priorAuthorizationRequestFormDocument.SaveOrUpdate(_priorAuthorizationRequestFormDocument.FormAndSetUrlForStudentPatient(dropBox, priorAuthorizationRequestFormObject.PatientReferenceId, priorAuthorizationRequestFormObject.UniqueIdentifier), priorAuthorizationRequestFormObject);
            return true;
        }

        public bool SaveMedicalRecordsReleaseForm(MedicalRecordsRelease medicalRecordsReleaseFormObject, DropBoxLink dropBox)
        {
            _medicalRecordsRelease.SaveOrUpdate(_medicalRecordsRelease.FormAndSetUrlForStudentPatient(dropBox, medicalRecordsReleaseFormObject.PatientReferenceId, medicalRecordsReleaseFormObject.UniqueIdentifier), medicalRecordsReleaseFormObject);
            return true;
        }
        

        public bool SaveReferralForm(ReferralForm referralFormObject, DropBoxLink dropBox)
        {
            _referralForm.SaveOrUpdate(_referralForm.FormAndSetUrlForStudentPatient(dropBox, referralFormObject.PatientReferenceId, referralFormObject.UniqueIdentifier), referralFormObject);
            return true;
        }

        //public bool SavePatientRegistration(Patient patientObject)
        //{
        //    _patientDocument.SaveOrUpdate(_patientDocument.GetAssignmentUrl(), patientObject);
        //    return true;
        //}

        public IList<Patient> GetAllPatient(DropBoxLink assignmentCredentials)
        {
            //return _patientDocument.GetAll(_patientDocument.FormAndSetUrlForStudentPatient(courseId,userRole,UID,SID,""));
            return _patientDocument.GetAllPatientForAssignment(assignmentCredentials);
        }

        //public Patient GetPatientRegistration(string patientGuid)
        //{
        //    return _patientDocument.Get(_patientDocument.GetAssignmentUrl(), patientGuid);
        //}

        //public IList<PriorAuthorizationRequestForm> GetAllPriorAuthorizationRequest()
        //{
        //    return
        //        _priorAuthorizationRequestFormDocument.GetAll(string.Format(_priorAuthorizationRequestFormDocument.GetAssignmentUrl(),
        //                                                                    ""));
        //}

        //public PriorAuthorizationRequestForm GetPatientPriorAuthorizationRequest(string patientGuid)
        //{
        //    return _priorAuthorizationRequestFormDocument.Get(_priorAuthorizationRequestFormDocument.GetAssignmentUrl(), patientGuid);
        //}

        public Patient GetPatient (string courseId,string userRole, string uid,string sid, string patientGuid)
        {
            return _patientDocument.Get(_patientDocument.FormAndSetUrlForStudentPatient(courseId,userRole,uid,sid,patientGuid));
        }

        public Patient GetPatientFromPatientRepository (string patientGuid)
        {
            return _patientDocument.GetPatientFromPatientRepository(patientGuid);
        }

        public Patient GetPatientFromAssignmentRepository(string patientGuid, DropBoxLink dropDownCredentials)
        {
            return _patientDocument.GetPatientFromAssignmentRepository(patientGuid, dropDownCredentials);
        }

        /// <summary>
        /// To save Notice of privacy form
        /// </summary>
        /// <param name="noticeOfPrivacyPractice"></param>
        /// <param name="patientGuid"></param>
        /// <param name="dropBox"> </param>
        /// <returns></returns>
        public bool SaveNoticeOfPrivacyPractice(NoticeOfPrivacyPractice noticeOfPrivacyPractice, string patientGuid, DropBoxLink dropBox)
        {
            string noticeDocumentUrl = _noticeOfPrivacyPracticeDocument.SaveNoticeFormUrl(patientGuid, dropBox);
            _noticeOfPrivacyPracticeDocument.SaveOrUpdate(noticeDocumentUrl, noticeOfPrivacyPractice);
            return true;
        }

        /// <summary>
        /// To save Bill of rights form
        /// </summary>
        /// <param name="patientRecordsAccessForm"></param>
        /// <param name="dropBox"> </param>
        /// <returns></returns>
        public bool SavePatientRecordsAccessForm(PatientRecordsAccessForm patientRecordsAccessForm, DropBoxLink dropBox)
        {
            _patientRecordsAccessForm.SaveOrUpdate(_patientRecordsAccessForm.FormAndSetUrlForStudentPatient(dropBox, patientRecordsAccessForm.PatientReferenceId, patientRecordsAccessForm.UniqueIdentifier), patientRecordsAccessForm);
            return true;
        }
        //public bool UpdatePatientRecordsAccessForm(string patientGuid, PatientRecordsAccessForm patientRecordsAccessForm)
        //{
        //    string patientRecordsAccessFormUrl = string.Format(_patientRecordsAccessForm.GetAssignmentUrl(), patientGuid);
        //    _patientRecordsAccessForm.SaveOrUpdate(patientRecordsAccessFormUrl,patientRecordsAccessForm);
        //    return true;
        //}

        //public bool DeletePatientRecordsAccessForm(string patientGuid)
        //{
        //    string result;
        //    string patientRecordsAccessFormUrl = string.Format(_patientRecordsAccessForm.GetAssignmentUrl(),patientGuid);
        //    _patientRecordsAccessForm.Delete(patientRecordsAccessFormUrl, out result);
        //    return true;
        //}

        //public IList<PatientRecordsAccessForm> GetAllPatientRecordsAccessForm()
        //{
        //    return _patientRecordsAccessForm.GetAll(string.Format(_patientRecordsAccessForm.GetAssignmentUrl(),""));
        //}

        public IList<PatientRecordsAccessForm> GetAllPatientRecordsAccessFormsForPatient(DropBoxLink dropBox, string patientGuid, string formId)
        {
            return _patientRecordsAccessForm.GetAllPatientRecordsAccessFormsForPatient(dropBox, patientGuid, formId);
        }

        public IList<PriorAuthorizationRequestForm> GetAllPriorAuthorizationRequestFormsForPatient(DropBoxLink dropBox,  string patientGuid, string formId)
        {
            return _priorAuthorizationRequestFormDocument.GetAllPriorAuthorizationRequestFormsForPatient(dropBox, patientGuid, formId);
        }
        public IList<MedicalRecordsRelease> GetAllMedicalRecordsReleaseFormsForPatient(DropBoxLink dropBox, string patientGuid, string formId)
        {
            return _medicalRecordsRelease.GetAllMedicalRecordsReleaseFormsForPatient(dropBox, patientGuid, formId);
        }

        public IList<ReferralForm> GetAllReferralFormsForPatient(DropBoxLink dropBox,string patientGuid, string formId)
        {
            return _referralForm.GetAllReferralFormsForPatient(dropBox, patientGuid, formId);
        }

        /// <summary>
        /// To get the Notice of privacy form for perticular patient
        /// </summary>
        /// <param name="patientGuid"></param>
        /// <param name="dropBox"> </param>
        /// <returns></returns>
        public NoticeOfPrivacyPractice GetNoticeOfPrivacyPracticeDocument(string patientGuid, DropBoxLink dropBox)
        {
            return _noticeOfPrivacyPracticeDocument.GetNoticeOfPrivacyPracticeDocument(patientGuid, dropBox);
        }

        /// <summary>
        /// To get the bill of rights form for perticular patient
        /// </summary>
        /// <param name="patientGuid"></param>
        /// <param name="dropBox"> </param>
        /// <returns></returns>
        public BillOfRights GetBillOfRightsDocument(string patientGuid, DropBoxLink dropBox)
        {
            return _billOfRightsDocument.GetBillOfRightsDocument(patientGuid, dropBox);
        }
        public PatientRecordsAccessForm GetPatientRecordsAccessForm(DropBoxLink dropBox, string patientGuid, string formId)
        {
            return _patientRecordsAccessForm.Get(_patientRecordsAccessForm.FormAndSetUrlForStudentPatient(dropBox,patientGuid,formId));
        }

        public PriorAuthorizationRequestForm GetPriorAuthorizationRequestForm(DropBoxLink dropBox, string patientGuid, string formId)
        {
            return _priorAuthorizationRequestFormDocument.Get(_priorAuthorizationRequestFormDocument.FormAndSetUrlForStudentPatient(dropBox,patientGuid,formId));
        }
         
              public MedicalRecordsRelease GetMedicalRecordsReleaseForm(DropBoxLink dropBox, string patientGuid, string formId)
        {
            return _medicalRecordsRelease.Get(_medicalRecordsRelease.FormAndSetUrlForStudentPatient(dropBox, patientGuid, formId));
        }

        public ReferralForm GetReferralForm(DropBoxLink dropBox, string patientGuid, string formId)
        {
           return _referralForm.Get(_referralForm.FormAndSetUrlForStudentPatient(dropBox,patientGuid,formId));
        }

        //public IList<ReferralForm> GetAllReferralFormItems()
        //{
        //    return _referralForm.GetAll(string.Format(_referralForm.GetAssignmentUrl(),""));
        //}

        //public ReferralForm GetReferralFormItem(string patientGuid)
        //{
        //    string referralFormUrl = string.Format(_referralForm.GetAssignmentUrl(), patientGuid);
        //    return _referralForm.Get(referralFormUrl);
        //}

        //public bool DeleteReferralForm(string patientGuid)
        //{
        //    string referralFormUrlList;
        //    string result;
        //    referralFormUrlList = string.Format(_referralForm.GetAssignmentUrl(),patientGuid);
        //    _patientRecordsAccessForm.Delete(referralFormUrlList, out result);
        //    return true;
        //}

        //public bool UpdateReferralForm(string patientGuid, ReferralForm referralForm)
        //{
        //    string referralFormUrl = string.Format(_referralForm.GetAssignmentUrl(),patientGuid);
        //    _referralForm.SaveOrUpdate(referralFormUrl, referralForm);
        //    return true;
        //}

       

        //public bool DeletePriorAuthorizationRequestForm(string patientGuid)
        //{
        //    string result;
        //    string priorAuthorizationRequestFormUrl = string.Format(_priorAuthorizationRequestFormDocument.GetAssignmentUrl(),patientGuid);
        //    _priorAuthorizationRequestFormDocument.Delete(priorAuthorizationRequestFormUrl, out result);
        //    return true;
        //}

        //public Attachment GetPdfAttachment(string attachmentGuid)
        //{
        //    string attachmentDocumentUrl = string.Format(_attachmentDocument.GetAssignmentUrl(), attachmentGuid);
        //    return _attachmentDocument.Get(attachmentDocumentUrl);
        //}

        public Attachment GetBillOfRightsPdfData()
        {
            return _billOfRightsDocument.GetBillOfRightsPdfData();

        }

        public Attachment GetNoticeOfPrivacyPracticePdfData()
        {
            return _noticeOfPrivacyPracticeDocument.GetNoticeOfPrivacyPracticePdfData();

        }
        public bool SavePatientBillOfRights(BillOfRights patientBillOfRights, string patientGuid, DropBoxLink dropBox)
        {
            string billOfRightsUrl = _billOfRightsDocument.SaveBillOfRightsUrl(patientGuid, dropBox);
            _billOfRightsDocument.SaveOrUpdate(billOfRightsUrl, patientBillOfRights);
            return true;
        }
    }
}
