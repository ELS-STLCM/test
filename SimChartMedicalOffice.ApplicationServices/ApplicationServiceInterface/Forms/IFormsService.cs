using SimChartMedicalOffice.Core;
using System.Collections.Generic;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.Forms;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Forms
{
    public interface IFormsService
    {
        bool SavePriorAuthorizationRequestForm(PriorAuthorizationRequestForm priorAuthorizationRequestFormObject, DropBoxLink dropBox);
        //bool SavePatientRegistration(Patient patientObject);
        IList<Patient> GetAllPatient(DropBoxLink dropBox);
        //Patient GetPatientRegistration(string patientGuid);
        //IList<PriorAuthorizationRequestForm> GetAllPriorAuthorizationRequest();
        //PriorAuthorizationRequestForm GetPatientPriorAuthorizationRequest(string patientGuid);

        bool SavePatientRecordsAccessForm(PatientRecordsAccessForm patientRecordsAccessForm, DropBoxLink dropBox);
        //bool UpdatePatientRecordsAccessForm(string patientGuid, PatientRecordsAccessForm patientRecordsAccessForm);
        //bool DeletePatientRecordsAccessForm(string patientGuid);
        //IList<PatientRecordsAccessForm> GetAllPatientRecordsAccessForm();

        PatientRecordsAccessForm GetPatientRecordsAccessForm(DropBoxLink dropBox,
                                                             string patientGuid, string formId);
        bool SaveMedicalRecordsReleaseForm(MedicalRecordsRelease medicalRecordsReleaseFormObject, DropBoxLink dropBox);
        //bool UpdateMedicalRecordsReleaseForm(string patientGuid, MedicalRecordsRelease medicalRecordsReleaseForm);
        //bool DeleteMedicalRecordsReleaseForm(string patientGuid);
        //IList<MedicalRecordsRelease> GetAllMedicalRecordsReleaseForm();

        //MedicalRecordsRelease GetMedicalRecordsReleaseForm(string courseId, string userRole, string UID, string SID,
        //                                                     string patientGuid, string formId);
        bool SaveReferralForm(ReferralForm referralFormObject,DropBoxLink dropBox);
        //IList<ReferralForm> GetAllReferralFormItems();
        //ReferralForm GetReferralFormItem(string patientGuid);
        //bool DeleteReferralForm(string patientGuid);
        //bool UpdateReferralForm(string patientGuid, ReferralForm referralForm);
        //bool DeletePriorAuthorizationRequestForm(string patientGuid);
        Patient GetPatient(string courseId, string userRole, string uid, string sid, string patientGuid);
        bool SaveNoticeOfPrivacyPractice(NoticeOfPrivacyPractice noticeOfPrivacyPractice, string patientGuid, DropBoxLink dropBox);
        //Attachment GetPdfAttachment(string attachmentGuid);
        bool SavePatientBillOfRights(BillOfRights patientBillOfRights, string patientGuid, DropBoxLink dropBox);
        IList<PatientRecordsAccessForm> GetAllPatientRecordsAccessFormsForPatient(DropBoxLink dropBox, string patientGuid, string formId);
        IList<PriorAuthorizationRequestForm> GetAllPriorAuthorizationRequestFormsForPatient(DropBoxLink dropBox, string patientGuid, string formId);
        IList<MedicalRecordsRelease> GetAllMedicalRecordsReleaseFormsForPatient(DropBoxLink dropBox, string patientGuid, string formId);
        PriorAuthorizationRequestForm GetPriorAuthorizationRequestForm(DropBoxLink dropBox,
                                                                        string patientGuid, string formId);
        MedicalRecordsRelease GetMedicalRecordsReleaseForm(DropBoxLink dropBox,
                                                                        string patientGuid, string formId);
        IList<ReferralForm> GetAllReferralFormsForPatient(DropBoxLink dropBox, string patientGuid, string formId);

        ReferralForm GetReferralForm(DropBoxLink dropBox, string patientGuid,
                                     string formId);
        NoticeOfPrivacyPractice GetNoticeOfPrivacyPracticeDocument(string patientGuid, DropBoxLink dropBox);
        BillOfRights GetBillOfRightsDocument(string patientGuid, DropBoxLink dropBox);
        Attachment GetBillOfRightsPdfData();
        Attachment GetNoticeOfPrivacyPracticePdfData();
        Patient GetPatientFromPatientRepository(string patientGuid);
        Patient GetPatientFromAssignmentRepository(string patientGuid, DropBoxLink dropDownCredentials);
    }
}
