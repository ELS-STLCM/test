using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.DataInterfaces;

namespace SimChartMedicalOffice.Data
{
    public class MasterDocument : IMasterDocument
    {
        public MasterDocument()
        {
            GetProviderValues();
            GetPatientInsuranceValues();
            GetAppointmentVisitTypeValues();
            GetExamRoomList();
            LoadBlockType();
            LoadAppointmentStatus();
            GetOtherTypeValues();
        }

        private static Dictionary<int, string> _providerList = new Dictionary<int, string>();
        private static List<string> _patientInsuranceList = new List<string>();
        private static List<string> _appointmentVisitTypeList = new List<string>();
        private static List<string> _examRoomList = new List<string>();
        private static List<string> _mBlockType = new List<string>();
        private static List<string> _mAppointmentStatus = new List<string>();
        private static List<string> _otherTypeList = new List<string>();
        

        /// <summary>
        /// to get static list of Other Type
        /// </summary>
        public void GetOtherTypeValues()
        {
            ClearOtherTypeList();
            _otherTypeList = new List<string>();
            StringBuilder jsonString = new StringBuilder();
            const string otherTypeUrl = "SimApp/Master/AppointmentOtherType";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(otherTypeUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                _otherTypeList = JsonSerializer.DeserializeObject<List<string>>(resultList);
                _otherTypeList = _otherTypeList.Where(x => !x.Equals(AppConstants.SelectDropDown)).OrderBy(x => x).ToList();
            }
        }

        /// <summary>
        /// Clear Static Other Type List 
        /// </summary>
        private static void ClearOtherTypeList()
        {
            if (_otherTypeList != null && _otherTypeList.Count > 0)
            {
                _otherTypeList.Clear();
            }
        }

        public List<string> GetOtherType()
        {
            List<string> otherType = new List<string> {AppConstants.SelectDropDown};
            otherType.AddRange(_otherTypeList);
            return otherType;
        }
        /// <summary>
        /// to get static list of provider values
        /// </summary>
        private void GetProviderValues()
        {
            ClearproviderList();
            _providerList = new Dictionary<int, string>();
            StringBuilder jsonString = new StringBuilder();
            const string providerUrl = "SimApp/Master/PatientProvider";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(providerUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                _providerList = JsonSerializer.DeserializeObject<Dictionary<int, string>>(resultList);
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
            return _providerList;
        }


        private static void ClearproviderList()
        {
            if (_providerList != null && _providerList.Count > 0)
            {
                _providerList.Clear();
            }
        }


        /// <summary>
        /// to get static list of Insurance values
        /// </summary>
        private void GetPatientInsuranceValues()
        {
            ClearpatientInsuranceList();
            _patientInsuranceList = new List<string>();
            StringBuilder jsonString = new StringBuilder();
            const string providerUrl = "SimApp/Master/PatientInsurance";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(providerUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                _patientInsuranceList = JsonSerializer.DeserializeObject<List<string>>(resultList);
                _patientInsuranceList = _patientInsuranceList.Where(x => !x.Equals(AppConstants.SelectDropDown)).OrderBy(x => x).ToList();
            }

        }

        /// <summary>
        /// Insurance list from satic Insurance list
        /// </summary>
        /// <returns></returns>
        public List<string> GetPatientInsuranceUrl()
        {
            List<string> insurance = new List<string> {AppConstants.SelectDropDown};
            insurance.AddRange(_patientInsuranceList);
            return insurance;
        }

        private static void ClearpatientInsuranceList()
        {
            if (_patientInsuranceList != null && _patientInsuranceList.Count > 0)
            {
                _patientInsuranceList.Clear();
            }
        }

        /// <summary>
        /// to get appointment values from static list
        /// </summary>
        /// <returns></returns>
        public List<string> GetAppointmentVisitType()
        {
            List<string> appointmentVisit = new List<string> {AppConstants.SelectDropDown};
            appointmentVisit.AddRange(_appointmentVisitTypeList);
            return appointmentVisit;
        }

        /// <summary>
        /// to get static list of Appointment Visit type
        /// </summary>
        private void GetAppointmentVisitTypeValues()
        {
            ClearappointmentVisitType();
            _appointmentVisitTypeList = new List<string>();
            StringBuilder jsonString = new StringBuilder();
            const string providerUrl = "SimApp/Master/AppointmentVisitType";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(providerUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                _appointmentVisitTypeList = JsonSerializer.DeserializeObject<List<string>>(resultList);
                _appointmentVisitTypeList = _appointmentVisitTypeList.Where(x => !x.Equals(AppConstants.SelectDropDown)).OrderBy(x => x).ToList();
            }
        }

        private static void ClearappointmentVisitType()
        {
            if (_appointmentVisitTypeList != null && _appointmentVisitTypeList.Count > 0)
            {
                _appointmentVisitTypeList.Clear();
            }
        }

        /// <summary>
        /// to get Exam room values from static list
        /// </summary>
        /// <returns></returns>
        public List<string> GetExamRooms()
        {
            List<string> examRoom = new List<string> {AppConstants.SelectDropDown};
            examRoom.AddRange(_examRoomList);
            return examRoom;
        }

        /// <summary>
        /// to get static list of exam room
        /// </summary>
        private void GetExamRoomList()
        {
            ClearExamRoomList();
            _examRoomList = new List<string>();
            StringBuilder jsonString = new StringBuilder();
            const string providerUrl = "SimApp/Master/ExamRoom";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(providerUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                _examRoomList = JsonSerializer.DeserializeObject<List<string>>(resultList);
                _examRoomList = _examRoomList.Where(x => !x.Equals(AppConstants.SelectDropDown)).ToList();
            }
        }

        private static void ClearExamRoomList()
        {
            if (_examRoomList != null && _examRoomList.Count > 0)
            {
                _examRoomList.Clear();
            }
        }

        private void LoadBlockType()
        {
            ClearLoadBlockType();
            StringBuilder jsonString = new StringBuilder();
            const string providerUrl = "SimApp/Master/BlockType";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(providerUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                _mBlockType = JsonSerializer.DeserializeObject<List<string>>(resultList);
                _mBlockType = _mBlockType.Where(x => !x.Equals(AppConstants.SelectDropDown)).OrderBy(x => x).ToList();
            }
        }

        private static void ClearLoadBlockType()
        {
            if (_mBlockType != null && _mBlockType.Count > 0)
            {
                _mBlockType.Clear();
            }
        }

        public List<string> GetBlockType()
        {
            List<string> blockType = new List<string> {AppConstants.SelectDropDown};
            blockType.AddRange(_mBlockType);
            return blockType;
        }

        private void LoadAppointmentStatus()
        {
            ClearLoadAppointmentStatus();
            List<string> appointmentStatus = new List<string>();
            StringBuilder jsonString = new StringBuilder();
            const string providerUrl = "SimApp/Master/AppointmentStatus";
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(providerUrl)));
            string resultList = jsonString.ToString();
            if (!AppCommon.CheckIfStringIsEmptyOrNull(resultList))
            {
                _mAppointmentStatus = JsonSerializer.DeserializeObject<List<string>>(resultList);
                _mAppointmentStatus = _mAppointmentStatus.Where(x => !x.Equals(AppConstants.SelectDropDown)).OrderBy(x => x).ToList();
            }
            appointmentStatus.Add(AppConstants.SelectDropDown);
            appointmentStatus.AddRange(_mAppointmentStatus);
            _mAppointmentStatus = appointmentStatus;
        }


        private static void ClearLoadAppointmentStatus()
        {
            if (_mAppointmentStatus != null && _mAppointmentStatus.Count > 0)
            {
                _mAppointmentStatus.Clear();
            }
        }

        public List<string> GetAppointmentStatus()
        {
            List<string> appoinmentstatus = new List<string> {AppConstants.SelectDropDown};
            appoinmentstatus.AddRange(_mAppointmentStatus);
            return appoinmentstatus;
        }
    }
}
