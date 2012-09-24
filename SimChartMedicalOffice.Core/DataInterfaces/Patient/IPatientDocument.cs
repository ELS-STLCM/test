using System.Collections.Generic;
using SimChartMedicalOffice.Core.DropBox;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.Patient
{
    public interface IPatientDocument : IKeyValueRepository<Core.Patient.Patient>
    {
        IList<Core.Patient.Patient> GetPatientItems(string parentFolderIdentifier, int folderType, DropBoxLink dropBox);
        string FormAndSetUrlForPatient(string patientObject, DropBoxLink dropBox, string patientUrl, string folderIdentifier, bool isEditMode);
        string FormAndSetUrlForStudentPatient(string courseId, string userRole, string uid, string sid, string patientGuid);
        List<Core.Patient.Patient> GetAllPatients(string course, string userRole);
        IList<Core.Patient.Patient> GetAllPatientForAssignment(DropBoxLink assignmentCredentials);
        IList<Core.Patient.Patient> GetPatientRepositoryList();
        Core.Patient.Patient GetPatientFromPatientRepository(string patientGuid);
        Core.Patient.Patient GetPatientFromAssignmentRepository(string patientGuid, DropBoxLink assignmentCredentials);
        Folder GetPatientRepository(string course, string userRole);
        void LoadAllPatients();
    }
}
