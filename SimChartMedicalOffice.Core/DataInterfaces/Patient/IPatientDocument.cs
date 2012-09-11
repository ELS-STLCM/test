using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Data.Repository;
using System.Collections.Generic;


namespace SimChartMedicalOffice.Core.DataInterfaces.Patient
{
    public interface IPatientDocument : IKeyValueRepository<Core.Patient.Patient>
    {
        IList<Core.Patient.Patient> GetPatientItems(string parentFolderIdentifier, int folderType, string courseId);
        string FormAndSetUrlForPatient(string patientObject, string courseId, string patientUrl, string folderIdentifier, bool isEditMode);
        string FormAndSetUrlForStudentPatient(string courseId, string userRole, string UID, string SID, string patientGuid);
        List<Core.Patient.Patient> GetAllPatients(string course, string userRole);
        IList<Core.Patient.Patient> GetAllPatientForAssignment(string assignmentUniqueIdentifier);
        IList<Core.Patient.Patient> GetPatientRepositoryList();
        Core.Patient.Patient GetPatientFromPatientRepository(string patientGUID);
        Core.Patient.Patient GetPatientFromAssignmentRepository(string patientGUID);
        Folder GetPatientRepository(string course, string userRole);
        void LoadAllPatients();
    }
}
