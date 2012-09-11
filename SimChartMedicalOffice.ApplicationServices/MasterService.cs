using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface;
using SimChartMedicalOffice.Data;
using SimChartMedicalOffice.Core.DataInterfaces;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Common;

namespace SimChartMedicalOffice.ApplicationServices
{
    public class MasterService : IMasterService
    {
        private readonly IMasterDocument _masterDocument;

        public MasterService(IMasterDocument masterDocumentInstance)
        {
            this._masterDocument = masterDocumentInstance;
        }

        public Dictionary<int, string> GetPatientProviderValues()
        {
             Dictionary<int, string> patientProviderValues = _masterDocument.GetPatientProviderUrl();
             List<KeyValuePair<int, string>> providerSortedList = patientProviderValues.ToList();
             providerSortedList.Sort(
                 delegate(KeyValuePair<int, string> firstPair,
                 KeyValuePair<int, string> nextPair)
                 {
                     if (!nextPair.Value.Equals(AppConstants.Select_DropDown))
                     {
                         return firstPair.Value.CompareTo(nextPair.Value);
                     }
                     else
                     {
                         return 0;
                     }
                 }

                 );             
             patientProviderValues = providerSortedList.Where(x => !x.Value.Equals("All Staff")).ToDictionary(x => x.Key, x => x.Value);
             return patientProviderValues;
        }

        public List<string> GetPatientInsuranceValues()
        {
            return _masterDocument.GetPatientInsuranceUrl();
        }

        public List<string> GetAppointmentVisitType()
        {
            return _masterDocument.GetAppointmentVisitType();
        }

        public List<string> GetExamRooms()
        {
            return _masterDocument.GetExamRooms();
        }
        public List<string> GetOtherType()
        {
            return _masterDocument.GetOtherType();
        }
        public List<AutoCompleteProxy> GetExamRoomViewResourceList()
        {
            List<string> examRoomList=GetExamRooms();
            List<AutoCompleteProxy> examRoomViewResourceList = new List<AutoCompleteProxy>();
            foreach (var item in examRoomList)
            {
                if (item != AppCommon.DROPDOWN_SELECT)
                {
                    AutoCompleteProxy autoCompleteProxy = new AutoCompleteProxy();
                    autoCompleteProxy.id = item;
                    autoCompleteProxy.name = item;
                    autoCompleteProxy.Sources = null;
                    examRoomViewResourceList.Add(autoCompleteProxy);
                }
            }
            return examRoomViewResourceList;
        }

        public List<string> GetAppointmentStatus()
        {
            return _masterDocument.GetAppointmentStatus();
        }

        public List<string> GetBlockType()
        {
            return _masterDocument.GetBlockType();
        }
        public string GetProviderName(int providerId)
        {
            return GetPatientProviderValues().Where(s => s.Key.Equals(providerId)).Select(s => s.Value.ToString()).SingleOrDefault();
        }
        public Dictionary<int, string> GetPatientProviderValuesBlock()
        {
            Dictionary<int, string> patientProviderValues = _masterDocument.GetPatientProviderUrl();
            List<KeyValuePair<int, string>> providerSortedList = patientProviderValues.ToList();
            providerSortedList.Sort(
                delegate(KeyValuePair<int, string> firstPair,
                KeyValuePair<int, string> nextPair)
                {
                    if (!nextPair.Value.Equals(AppConstants.Select_DropDown))
                    {
                        return firstPair.Value.CompareTo(nextPair.Value);
                    }
                    else
                    {
                        return 0;
                    }
                }

                );
            patientProviderValues = providerSortedList.ToDictionary(x => x.Key, x => x.Value);
            return patientProviderValues;
        }
    }
}
