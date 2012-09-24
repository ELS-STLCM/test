using System;
using System.Collections.Generic;
using System.Linq;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using SimChartMedicalOffice.Core.DropBox;
using SimChartMedicalOffice.Core.Patient;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Data.Forms
{
    public class PatientDocument : KeyValueRepository<Patient>, IPatientDocument
    {
        

        public PatientDocument()
        {
            LoadAllPatients();
        }

        public static IList<Patient> AllPatients = new List<Patient>();

        public IList<Patient>
            GetPatientItems(string parentFolderIdentifier, int folderType, DropBoxLink dropBox)
        {
            if (dropBox == null)
            {
                dropBox = GetAdminDropBox();
            }
            string jsonString = GetJsonDocument(((parentFolderIdentifier == "") ? string.Format(GetAssignmentUrl(dropBox, DocumentPath.Module.Patients, AppConstants.Create), "") : (parentFolderIdentifier + "/" + Respository.PatientsNode)));
            Dictionary<string, Patient> patientsList = (jsonString != "null") ? JsonSerializer.DeserializeObject<Dictionary<string, Patient>>(jsonString) : new Dictionary<string, Patient>();
            foreach (var folderItem in patientsList)
            {
                folderItem.Value.UniqueIdentifier = folderItem.Key;
                folderItem.Value.Url = string.Concat(string.Format(GetAssignmentUrl(dropBox, DocumentPath.Module.Patients, AppConstants.Create), ""), folderItem.Value.UniqueIdentifier);
            }
            return ConvertDictionarytoObject(patientsList);
        }
        //private static IList<Patient> totalPatientList = new List<Patient>();

        //private static IList<Patient> patientRepositoryList = new List<Patient>();

        public IList<Patient> GetPatientRepositoryList()
        {
            return AllPatients;
        }

        public IList<Patient> GetAllPatientForAssignment(DropBoxLink assignmentCredentials)
        {
            IList<Patient> patientListFromAssignmentRepo = GetAssignmentRepositoryPatients(assignmentCredentials);
            IList<Patient> patientListFromPatientRepo = GetPatientRepositoryList();

            IList<Patient> totalPatientList;

            if (patientListFromAssignmentRepo != null && patientListFromAssignmentRepo.Count > 0)
            {
                foreach (var patient in patientListFromAssignmentRepo)
                {
                    if (!string.IsNullOrEmpty(patient.ParentReferenceGuid))
                    {
                        Patient patient1 = patient;
                        patientListFromPatientRepo =
                            patientListFromPatientRepo.Where(x => (x.UniqueIdentifier != patient1.ParentReferenceGuid.Split('/').Last())).ToList();
                    }
                }
                totalPatientList = patientListFromPatientRepo.Union(patientListFromAssignmentRepo).ToList();
            }
            else
            {
                totalPatientList = patientListFromPatientRepo;
            }
            return totalPatientList;
        }

        public Patient GetPatientFromPatientRepository(string patientGuid)
        {
            Patient patientFromRepo = null;
            IList<Patient> patientRepoList = GetPatientRepositoryList();
            foreach (var patient in patientRepoList)
            {
                if (patient.UniqueIdentifier == patientGuid)
                {
                    patientFromRepo = patient;
                    break;
                }
            }
            return patientFromRepo;

        }

        public Patient GetPatientFromAssignmentRepository(string patientGuid, DropBoxLink assignmentCredentials)
        {
            Patient patientFromRepo = null;
            IList<Patient> patientRepoList = GetAssignmentRepositoryPatients(assignmentCredentials);
            foreach (var patient in patientRepoList)
            {
                if (patient.UniqueIdentifier == patientGuid)
                {
                    patientFromRepo = patient;
                    break;
                }
            }
            return patientFromRepo;

        }

        public string FormAndSetUrlForStudentPatient(string courseId, string userRole, string uid, string sid, string patientGuid)
        {
            if (userRole.Equals(AppConstants.StudentRole))
            {
                DropBoxLink dropBox = new DropBoxLink {Cid = courseId, UserRole = userRole, Uid = uid, Sid = sid};
                //return string.Format(Url, courseId + "/" + userRole + "/" + UID + "/Assignments/" + SID, patientGuid);
                string url = GetAssignmentUrl(dropBox, DocumentPath.Module.Patients, AppConstants.Create);
                return string.Format(url, patientGuid);
            }
            // if Admin or Instructor
            return "";
        }

        private IList<Patient> ConvertDictionarytoObject(Dictionary<string, Patient> patientsListItems)
        {
            return ((patientsListItems != null) ? (patientsListItems.Select(patientsListItem => patientsListItem.Value).ToList()) : new List<Patient>());
        }

        public string FormAndSetUrlForPatient(string patientGuid, DropBoxLink dropBox, string patientUrl, string folderIdentifier, bool isEditMode)
        {
            if (dropBox == null)
            {
                dropBox = GetAdminDropBox();
            }
            if (isEditMode)
            {
                if (String.IsNullOrEmpty(patientUrl))
                    return string.Format(GetAssignmentUrl(dropBox, DocumentPath.Module.Patients, AppConstants.Create), patientGuid);
                return patientUrl;
            }
            if (String.IsNullOrEmpty(patientUrl))
            {
                return string.Format(GetAssignmentUrl(dropBox, DocumentPath.Module.Patients, AppConstants.Create), patientGuid);
            }
            return string.Concat(patientUrl, '/', folderIdentifier, "/" + Respository.PatientsNode + "/", patientGuid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Patient> GetAllPatients(string course, string userRole)
        {
            return AllPatients.ToList();
        }



        public void LoadAllPatients()
        {
            ClearAllPatients();
            Folder patientRepository = GetPatientRepository(AppConstants.AdminCourseId, AppConstants.AdminRole);
            AllPatients = new List<Patient>();
            if (patientRepository != null)
            {
                GetTotalPatientList(patientRepository);
            }
        }

        /// <summary>
        /// To clear the list of competencies 
        /// </summary>
        private static void ClearAllPatients()
        {
            if (AllPatients != null && AllPatients.Count > 0)
            {
                AllPatients.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentFolder"></param>
        private void GetTotalPatientList(Folder parentFolder)
        {
            TraverseEachFolderForPatients(parentFolder.SubFolders);
            CollectPatientsFromParentPatient(parentFolder.Patients);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderContent"></param>
        private void TraverseEachFolderForPatients(Dictionary<string, Folder> folderContent)
        {
            if (folderContent != null && folderContent.Count > 0)
            {
                IList<Folder> folders = folderContent.Select(folder => folder.Value).ToList();
                folders.ToList().ForEach(f => GetTotalPatientList(f));
            }
        }

        /// <summary>
        /// Get Patients
        /// </summary>
        /// <param name="patients"></param>
        private void CollectPatientsFromParentPatient(Dictionary<string, Patient> patients)
        {
            if (patients != null && patients.Count > 0)
            {
                //var patientList = patients.Select(question => question.Value).ToList();
                IList<Patient> patientList = new List<Patient>();
                foreach (var patient in patients)
                {
                    patient.Value.UniqueIdentifier = patient.Key;
                    patientList.Add(patient.Value);
                }
                AllPatients = AllPatients.Concat(patientList).ToList();
            }
        }
        /// <summary>
        /// To get all patient object based on role & course
        /// </summary>
        /// <returns></returns>
        public Folder GetPatientRepository(string course, string userRole)
        {
            // strUrlToGet = "SimApp/Courses/ELSEVIER_CID/Admin/PatientRepository"; // for Iteration 1 it will be hard coded.
            DropBoxLink dropBoxObject = new DropBoxLink {Cid = course, UserRole = userRole};
            string strUrlToGet = GetAssignmentUrl(dropBoxObject, DocumentPath.Module.Patients, AppConstants.Read);
            string jsonString = GetJsonDocument(strUrlToGet);
            Folder patientRepository = JsonSerializer.DeserializeObject<Folder>(jsonString);
            return patientRepository;
        }


        /// <summary>
        /// To get all patient object from Assignment Repository
        /// to be used until Assignment Copy implemented
        /// </summary>
        /// <returns></returns>
        public IList<Patient> GetAssignmentRepositoryPatients(DropBoxLink dropBox)
        {
            IList<Patient> assigmentRepositoryPatientList = new List<Patient>();
            DropBoxLink dropBoxObject = dropBox;
            string strUrlToGet = GetAssignmentUrl(dropBoxObject, DocumentPath.Module.Patients, AppConstants.Read);
            string jsonString = GetJsonDocument(strUrlToGet);
            Dictionary<string, Patient> assignmentRepoPatientDict =
                JsonSerializer.DeserializeObject<Dictionary<string, Patient>>(jsonString);
            if (assignmentRepoPatientDict != null && assignmentRepoPatientDict.Count > 0)
            {
                foreach (var patient in assignmentRepoPatientDict)
                {
                    patient.Value.UniqueIdentifier = patient.Key;
                    assigmentRepositoryPatientList.Add(patient.Value);
                }
            }
            return assigmentRepositoryPatientList;
        }
    }
}
