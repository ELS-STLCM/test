using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core.DataInterfaces;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.QuestionBanks;

namespace SimChartMedicalOffice.Data
{
    public class MasterDocument : IMasterDocument
    {
        public MasterDocument()
        {
            this.GetProviderValues();
            this.GetPatientInsuranceValues();
            this.GetAppointmentVisitTypeValues();
            this.GetExamRoomList();
            this.LoadBlockType();
            this.LoadAppointmentStatus();
            this.GetOtherTypeValues();
        }

        private static Dictionary<int, string> providerList = new Dictionary<int, string>();
        private static List<string> patientInsuranceList = new List<string>();
        private static List<string> appointmentVisitTypeList = new List<string>();
        private static List<string> examRoomList = new List<string>();
        private static List<string> m_BlockType = new List<string>();
        private static List<string> m_appointmentStatus = new List<string>();
        private static List<string> otherTypeList = new List<string>();
        

        /// <summary>
        /// to get static list of Other Type
        /// </summary>
        public void GetOtherTypeValues()
        {
            ClearOtherTypeList();
            otherTypeList = new List<string>();
            StringBuilder jsonString = new StringBuilder();
            string otherTypeUrl = "SimApp/Master/AppointmentOtherType";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(otherTypeUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                otherTypeList = JsonSerializer.DeserializeObject<List<string>>(resultList);
                otherTypeList = otherTypeList.Where(x => !x.Equals(AppConstants.Select_DropDown)).OrderBy(x => x).ToList();
            }
        }

        /// <summary>
        /// Clear Static Other Type List 
        /// </summary>
        private static void ClearOtherTypeList()
        {
            if (otherTypeList != null && otherTypeList.Count > 0)
            {
                otherTypeList.Clear();
            }
        }

        public List<string> GetOtherType()
        {
            List<string> otherType = new List<string>();
            otherType.Add(AppConstants.Select_DropDown);
            otherType.AddRange(otherTypeList);
            return otherType;
        }
        /// <summary>
        /// to get static list of provider values
        /// </summary>
        private void GetProviderValues()
        {
            ClearproviderList();
            providerList = new Dictionary<int, string>();
            StringBuilder jsonString = new StringBuilder();
            string providerUrl = "SimApp/Master/PatientProvider";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(providerUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                providerList = JsonSerializer.DeserializeObject<Dictionary<int, string>>(resultList);
                //providerList = providerList.Where(x => !x.Equals(AppConstants.Select_DropDown)).OrderBy(x => x).ToList();
            }

        }


        /// <summary>
        /// Provider list from static object
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetPatientProviderUrl()
        {
            //List<string> provider = new List<string>();
            //provider.Add(AppConstants.Select_DropDown);
            //provider.AddRange(providerList);
            return providerList;
        }


        private static void ClearproviderList()
        {
            if (providerList != null && providerList.Count > 0)
            {
                providerList.Clear();
            }
        }


        /// <summary>
        /// to get static list of Insurance values
        /// </summary>
        private void GetPatientInsuranceValues()
        {
            ClearpatientInsuranceList();
            patientInsuranceList = new List<string>();
            StringBuilder jsonString = new StringBuilder();
            string providerUrl = "SimApp/Master/PatientInsurance";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(providerUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                patientInsuranceList = JsonSerializer.DeserializeObject<List<string>>(resultList);
                patientInsuranceList = patientInsuranceList.Where(x => !x.Equals(AppConstants.Select_DropDown)).OrderBy(x => x).ToList();
            }

        }

        /// <summary>
        /// Insurance list from satic Insurance list
        /// </summary>
        /// <returns></returns>
        public List<string> GetPatientInsuranceUrl()
        {
            List<string> insurance = new List<string>();
            insurance.Add(AppConstants.Select_DropDown);
            insurance.AddRange(patientInsuranceList);
            return insurance;
        }

        private static void ClearpatientInsuranceList()
        {
            if (patientInsuranceList != null && patientInsuranceList.Count > 0)
            {
                patientInsuranceList.Clear();
            }
        }

        /// <summary>
        /// to get appointment values from static list
        /// </summary>
        /// <returns></returns>
        public List<string> GetAppointmentVisitType()
        {
            List<string> appointmentVisit = new List<string>();
            appointmentVisit.Add(AppConstants.Select_DropDown);
            appointmentVisit.AddRange(appointmentVisitTypeList);
            return appointmentVisit;
        }

        /// <summary>
        /// to get static list of Appointment Visit type
        /// </summary>
        private void GetAppointmentVisitTypeValues()
        {
            ClearappointmentVisitType();
            appointmentVisitTypeList = new List<string>();
            StringBuilder jsonString = new StringBuilder();
            string providerUrl = "SimApp/Master/AppointmentVisitType";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(providerUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                appointmentVisitTypeList = JsonSerializer.DeserializeObject<List<string>>(resultList);
                appointmentVisitTypeList = appointmentVisitTypeList.Where(x => !x.Equals(AppConstants.Select_DropDown)).OrderBy(x => x).ToList();
            }
        }

        private static void ClearappointmentVisitType()
        {
            if (appointmentVisitTypeList != null && appointmentVisitTypeList.Count > 0)
            {
                appointmentVisitTypeList.Clear();
            }
        }

        /// <summary>
        /// to get Exam room values from static list
        /// </summary>
        /// <returns></returns>
        public List<string> GetExamRooms()
        {
            List<string> examRoom = new List<string>();
            examRoom.Add(AppConstants.Select_DropDown);
            examRoom.AddRange(examRoomList);
            return examRoom;
        }

        /// <summary>
        /// to get static list of exam room
        /// </summary>
        private void GetExamRoomList()
        {
            ClearExamRoomList();
            examRoomList = new List<string>();
            StringBuilder jsonString = new StringBuilder();
            string providerUrl = "SimApp/Master/ExamRoom";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(providerUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                examRoomList = JsonSerializer.DeserializeObject<List<string>>(resultList);
                examRoomList = examRoomList.Where(x => !x.Equals(AppConstants.Select_DropDown)).ToList();
            }
        }

        private static void ClearExamRoomList()
        {
            if (examRoomList != null && examRoomList.Count > 0)
            {
                examRoomList.Clear();
            }
        }

        private void LoadBlockType()
        {
            ClearLoadBlockType();
            List<string> blocktype = new List<string>();
            StringBuilder jsonString = new StringBuilder();
            string providerUrl = "SimApp/Master/BlockType";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(providerUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                m_BlockType = JsonSerializer.DeserializeObject<List<string>>(resultList);
                m_BlockType = m_BlockType.Where(x => !x.Equals(AppConstants.Select_DropDown)).OrderBy(x => x).ToList();
            }
        }

        private static void ClearLoadBlockType()
        {
            if (m_BlockType != null && m_BlockType.Count > 0)
            {
                m_BlockType.Clear();
            }
        }

        public List<string> GetBlockType()
        {
            List<string> blockType = new List<string>();
            blockType.Add(AppConstants.Select_DropDown);
            blockType.AddRange(m_BlockType);
            return blockType;
        }

        private void LoadAppointmentStatus()
        {
            ClearLoadAppointmentStatus();
            StringBuilder jsonString = new StringBuilder();
            string providerUrl = "SimApp/Master/AppointmentStatus";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(providerUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                m_appointmentStatus = JsonSerializer.DeserializeObject<List<string>>(resultList);
                m_appointmentStatus = m_appointmentStatus.Where(x => !x.Equals(AppConstants.Select_DropDown)).OrderBy(x => x).ToList();
            }
        }


        private static void ClearLoadAppointmentStatus()
        {
            if (m_appointmentStatus != null && m_appointmentStatus.Count > 0)
            {
                m_appointmentStatus.Clear();
            }
        }

        public List<string> GetAppointmentStatus()
        {
            List<string> appoinmentstatus = new List<string>();
            appoinmentstatus.Add(AppConstants.Select_DropDown);
            appoinmentstatus.AddRange(m_appointmentStatus);
            return appoinmentstatus;
        }
    }
}
