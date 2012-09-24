using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.DataInterfaces.Competency;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Data.Competency
{
    public class ApplicationModuleDocument : KeyValueRepository<Core.Competency.ApplicationModules>, IApplicationModuleDocument
    {
        private static IList<Core.Competency.ApplicationModules> _mApplicationModuleList;

        


        /// <summary>
        /// To cache the list of Application Modules in sataic list 
        /// </summary>
        public ApplicationModuleDocument()
        {
            LoadApplicationModule();
        }

        /// <summary>
        /// To clear the list of Competecny Source List
        /// </summary>
        private static void ClearApplicationModule()
        {
            if (_mApplicationModuleList != null && _mApplicationModuleList.Count > 0)
            {
                _mApplicationModuleList.Clear();
            }
        }

        /// <summary>
        /// To load the list of Competecny Source in sataic list
        /// </summary>
        private void LoadApplicationModule()
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(string.Format(GetAssignmentUrl(Core.DocumentPath.Module.Masters,AppConstants.ApplicationModule), ""))));
            if (jsonString.ToString() != "null")
            {
                ClearApplicationModule();
                var applicationModuleList = JsonSerializer.DeserializeObject<Dictionary<string, Core.Competency.ApplicationModules>>(jsonString.ToString());
                _mApplicationModuleList = new List<Core.Competency.ApplicationModules>();
                if (applicationModuleList != null)
                {
                    foreach (var appMod in applicationModuleList)
                    {
                        Core.Competency.ApplicationModules applicationModules = appMod.Value;
                        applicationModules.UniqueIdentifier = appMod.Key;
                        applicationModules.Url = FormApplicationModulesUrl(GetAssignmentUrl(Core.DocumentPath.Module.Masters, AppConstants.ApplicationModule), false, applicationModules);
                        _mApplicationModuleList.Add(appMod.Value);
                    }
                }
            }
        }

        public IList<Core.Competency.ApplicationModules> GetAllApplicationModules()
        {
            _mApplicationModuleList = _mApplicationModuleList.OrderBy(appMod => appMod.Name).ToList();
            return _mApplicationModuleList;
        }

        /// <summary>
        /// To form(Insert) or set(update) the URL.
        /// </summary>
        /// <param ></param>
        /// <param name="applicationModulesUrl"> </param>
        /// <param name="isEditMode"></param>
        /// <param name="applicationModulesUiObject"> </param>
        /// <returns>string</returns>
        public string FormApplicationModulesUrl(string applicationModulesUrl, bool isEditMode, Core.Competency.ApplicationModules applicationModulesUiObject)
        {
            if (isEditMode)
            {
                return applicationModulesUrl;
            }
            return string.Format(GetAssignmentUrl(Core.DocumentPath.Module.Masters,AppConstants.ApplicationModule), applicationModulesUiObject.GetNewGuidValue());
        }
    }
}
