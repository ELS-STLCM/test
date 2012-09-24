using System.Collections.Generic;
using SimChartMedicalOffice.Core.ProxyObjects;

namespace SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface
{
    public interface IMasterService
    {
        Dictionary<int, string> GetPatientProviderValues();
        List<string> GetPatientInsuranceValues();
        List<string> GetAppointmentVisitType();
        List<string> GetExamRooms();
        List<string> GetAppointmentStatus();
        List<string> GetBlockType();
        string GetProviderName(int providerId);
        List<AutoCompleteProxy> GetExamRoomViewResourceList();
        List<string> GetOtherType();
        Dictionary<int, string> GetPatientProviderValuesBlock();
        string GetProviderName(IList<int> providerIds, string seperatorForNames);
    }
}
