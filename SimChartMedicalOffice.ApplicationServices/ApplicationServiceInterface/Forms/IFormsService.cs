using SimChartMedicalOffice.Core;
using System.Collections.Generic;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.Forms;

namespace SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Forms
{
    public interface IFormsService
    {
        bool SavePriorAuthorizationRequestForm(PriorAuthorizationRequestForm priorAuthorizationRequestFormObject, string courseId,
                                          string userRole,
                                          string UID, string SID);
        bool SavePatientRegistration(Patient patientObject);
        IList<Patient> GetAllPatient(string courseId, string userRole, string UID, string SID);
        Patient GetPatientRegistration(string patientGuid);
        IList<PriorAuthorizationRequestForm> GetAllPriorAuthorizationRequest();
        PriorAuthorizationRequestForm GetPatientPriorAuthorizationRequest(string patientGuid);

        bool SavePatientRecordsAccessForm(PatientRecordsAccessForm patientRecordsAccessForm, string courseId,
                                          string userRole,
                                          string UID, string SID);
        bool UpdatePatientRecordsAccessForm(string patientGuid, PatientRecordsAccessForm patientRecordsAccessForm);
        bool DeletePatientRecordsAccessForm(string patientGuid);
        IList<PatientRecordsAccessForm> GetAllPatientRecordsAccessForm();

        PatientRecordsAccessForm GetPatientRecordsAccessForm(string courseId, string userRole, string UID, string SID,
                                                             string patientGuid, string formId);
        bool SaveReferralForm(ReferralForm referralFormObject, string courseId,
                                          string userRole,
                                          string UID, string SID);
        IList<ReferralForm> GetAllReferralFormItems();
        ReferralForm GetReferralFormItem(string patientGuid);
        bool DeleteReferralForm(string patientGuid);
        bool UpdateReferralForm(string patientGuid, ReferralForm referralForm);
        bool DeletePriorAuthorizationRequestForm(string patientGuid);
        Patient GetPatient(string courseId, string userRole, string UID, string SID, string patientGuid);
        bool SaveNoticeOfPrivacyPractice(NoticeOfPrivacyPractice noticeOfPrivacyPractice, string patientGuid, string courseId, string userRole, string UID, string SID);
        Attachment GetPdfAttachment(string attachmentGuid);
        bool SavePatientBillOfRights(BillOfRights patientBillOfRights, string patientGuid, string courseId, string userRole, string UID, string SID);
        IList<PatientRecordsAccessForm> GetAllPatientRecordsAccessFormsForPatient(string courseId,
                                                                                         string userRole, string UID,
                                                                                         string SID, string patientGuid, string formId);
        IList<PriorAuthorizationRequestForm> GetAllPriorAuthorizationRequestFormsForPatient(string courseId,
                                                                                         string userRole, string UID,
                                                                                         string SID, string patientGuid, string formId);

        PriorAuthorizationRequestForm GetPriorAuthorizationRequestForm(string courseId, string userRole, string UID,
                                                                       string SID, string patientGuid, string formId);
        IList<ReferralForm> GetAllReferralFormsForPatient(string courseId, string userRole, string UID,
                                                                                         string SID, string patientGuid, string formId);

        ReferralForm GetReferralForm(string courseId, string userRole, string UID, string SID, string patientGuid,
                                     string formId);
        NoticeOfPrivacyPractice GetNoticeOfPrivacyPracticeDocument(string patientGUID, string courseId, string userRole, string UID, string SID);
        BillOfRights GetBillOfRightsDocument(string patientGUID, string courseId, string userRole, string UID, string SID);
        Attachment GetBillOfRightsPdfData();
        Attachment GetNoticeOfPrivacyPracticePdfData();
        Patient GetPatientFromPatientRepository(string patientGUID);
        Patient GetPatientFromAssignmentRepository(string patientGUID);
    }
}
