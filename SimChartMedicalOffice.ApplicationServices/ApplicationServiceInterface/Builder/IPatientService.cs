using System.Collections.Generic;
using SimChartMedicalOffice.Core.Patient;

namespace SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder
{
    public interface IPatientService
    {
        bool SavePatient(Patient patientObject, string courseId, string patientUrl, string folderIdentifier, bool isEditMode);
        //bool DeletePatient(string patientGuid);
        List<Patient> GetAllPatients();
        Patient GetPatientForGuid(string patientGuid);
        IList<Patient> GetPatientItems(string parentFolderIdentifier, int folderType, int sortColumnIndex, string sortColumnOrder, string courseId, string folderUrl);
        List<PatientProxy> GetSearchResultsForPatient(string strSearchText, int sortColumnIndex, string sortColumnOrder, string course, string userRole);
    }
}
