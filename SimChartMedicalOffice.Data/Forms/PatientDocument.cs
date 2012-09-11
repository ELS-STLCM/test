using System.Collections.Generic;
using System.Linq;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using SimChartMedicalOffice.Core.Patient;
using System.Text;
using System;
using SimChartMedicalOffice.Core.QuestionBanks;

namespace SimChartMedicalOffice.Data.Forms
{
    public class PatientDocument : KeyValueRepository<Patient>, IPatientDocument
    {
        public override string Url
        {
            get
            {
                return "SimApp/Courses/{0}/Patients/{1}";
            }
        }

        public PatientDocument()
        {
            this.LoadAllPatients();
        }

        public static IList<Patient> allPatients = new List<Patient>();

        public IList<Patient> 
            GetPatientItems(string parentFolderIdentifier, int folderType, string courseId)
        {
            string jsonString = GetJsonDocument(((parentFolderIdentifier == "") ? string.Format(Url, courseId + "/PatientRepository", "", "") : (parentFolderIdentifier + "/Patients")));
            Dictionary<string, Patient> patientsList;
            patientsList = JsonSerializer.DeserializeObject<Dictionary<string, Patient>>(jsonString);
            foreach (var folderItem in patientsList)
            {
                folderItem.Value.UniqueIdentifier = folderItem.Key.ToString();
                folderItem.Value.Url = string.Concat(string.Format(Url, courseId + "/PatientRepository", ""), folderItem.Value.UniqueIdentifier);
            }
            return ConvertDictionarytoObject(patientsList);
        }
        //private static IList<Patient> totalPatientList = new List<Patient>();

        //private static IList<Patient> patientRepositoryList = new List<Patient>();

        public IList<Patient> GetPatientRepositoryList ()
        {
            return allPatients;
        }
 
        public IList<Patient> GetAllPatientForAssignment(string assignmentUniqueIdentifier)
        {
            IList<Patient> patientListFromAssignmentRepo =GetAssignmentRepositoryPatients();
            IList<Patient> patientListFromPatientRepo = GetPatientRepositoryList();

            IList<Patient> totalPatientList = new List<Patient>();

            if (patientListFromAssignmentRepo != null && patientListFromAssignmentRepo.Count>0)
            {
                foreach (var patient in patientListFromAssignmentRepo)
                {
                    if (patient.ParentReferenceGuid != null && patient.ParentReferenceGuid != "")
                    {
                        patientListFromPatientRepo =
                            patientListFromPatientRepo.Where(x => (x.UniqueIdentifier != patient.ParentReferenceGuid.Split('/').Last())).ToList();
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

        public Patient GetPatientFromPatientRepository (string patientGUID)
        {
            Patient patientFromRepo = null;
            IList<Patient> patientRepoList = GetPatientRepositoryList();
            foreach (var patient in patientRepoList)
            {
                if (patient.UniqueIdentifier == patientGUID)
                {
                    patientFromRepo = patient;
                    break;
                }
            }
            return patientFromRepo;

        }

        public Patient GetPatientFromAssignmentRepository(string patientGUID)
        {
            Patient patientFromRepo = null;
            IList<Patient> patientRepoList = GetAssignmentRepositoryPatients();
            foreach (var patient in patientRepoList)
            {
                if (patient.UniqueIdentifier==patientGUID)
                {
                    patientFromRepo = patient;
                    break;
                }
            }
            return patientFromRepo;

        }

        public string FormAndSetUrlForStudentPatient (string courseId, string userRole, string UID,string SID, string patientGuid)
        {
            if (userRole=="Student")
            {
                return string.Format(Url, courseId + "/" + userRole + "/" + UID + "/Assignments/" + SID, patientGuid);
            }
            // if Admin or Instructor
            return "";
        }

        private IList<Patient> ConvertDictionarytoObject(Dictionary<string, Patient> patientsListItems)
        {
            return ((patientsListItems != null) ? (patientsListItems.Select(patientsListItem => patientsListItem.Value).ToList()) : new List<Patient>());
        }

        public string FormAndSetUrlForPatient(string patientGuid, string courseId, string patientUrl, string folderIdentifier, bool isEditMode)
        {
            try
            {
                if (isEditMode)
                {
                    if (String.IsNullOrEmpty(patientUrl))
                        return string.Format(Url, courseId + "/PatientRepository", patientGuid);
                    else
                    {
                        return patientUrl;
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(patientUrl))
                        return string.Format(Url, courseId + "/PatientRepository", patientGuid);
                    else
                    {
                        return string.Concat(patientUrl, '/', folderIdentifier, "/Patients/", patientGuid);
                    }
                }

            }
            catch
            {
                //To-Do
            }
            return String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Patient> GetAllPatients(string course, string userRole)
        {            
            return allPatients.ToList();
        }



        public void LoadAllPatients()
        {
            ClearAllPatients();
            Folder patientRepository = GetPatientRepository("", "");
            allPatients = new List<Patient>();
            if (patientRepository != null)
            {
                GetTotalPatientList(patientRepository);
            }
        }

        /// <summary>
        /// To clear the list of competencies 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private static void ClearAllPatients()
        {
            if (allPatients != null && allPatients.Count > 0)
            {
                allPatients.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentFolder"></param>
        private void GetTotalPatientList(Folder parentFolder)
        {
            TraverseEachFolderForPatients(parentFolder.SubFolders);
            CollectPatientsFromParentPatient(parentFolder.Patients, parentFolder);
            return;
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
                folders.ToList().ForEach(F => GetTotalPatientList(F));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="questions"></param>
        private void CollectPatientsFromParentPatient(Dictionary<string, Patient> patients, Folder parentFolder)
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
                allPatients = allPatients.Concat(patientList).ToList();
            }
        }
        /// <summary>
        /// To get all patient object based on role & course
        /// </summary>
        /// <returns></returns>
        public Folder GetPatientRepository(string course, string userRole)
        {
            string strUrlToGet = FormAndSetUrlForPatient("" , course +"/" + userRole,"","",false);
            strUrlToGet = "SimApp/Courses/ELSEVIER_CID/Admin/PatientRepository"; // for Iteration 1 it will be hard coded.
            string jsonString = GetJsonDocument(strUrlToGet);
            Folder patientRepository = JsonSerializer.DeserializeObject<Folder>(jsonString);
            return patientRepository;
        }
        

        /// <summary>
        /// To get all patient object from Assignment Repository
        /// to be used until Assignment Copy implemented
        /// </summary>
        /// <returns></returns>
        public IList<Patient> GetAssignmentRepositoryPatients()
        {
            IList<Patient> assigmentRepositoryPatientList = new List<Patient>();
            string strUrlToGet = "SimApp/Courses/ELSEVIER_CID/Admin/AssignmentRepository/Assignments/143a6002-eef3-4ad8-adca-f5d272e164cc/Patients";// for Iteration 1 it will be hard coded.
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
