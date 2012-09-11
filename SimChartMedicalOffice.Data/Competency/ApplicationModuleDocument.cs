using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.Competency;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Common;

namespace SimChartMedicalOffice.Data.Competency
{
    public class ApplicationModuleDocument : KeyValueRepository<Core.Competency.ApplicationModules>, IApplicationModuleDocument
    {
        private static IList<Core.Competency.ApplicationModules> m_ApplicationModuleList;

        public override string Url
        {
            get
            {
                return "SimApp/Master/ApplicationModule/{0}";
            }
        }


        /// <summary>
        /// To cache the list of Application Modules in sataic list 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ApplicationModuleDocument()
        {
            this.LoadApplicationModule();
        }

        /// <summary>
        /// To clear the list of Competecny Source List 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private static void ClearApplicationModule()
        {
            if (m_ApplicationModuleList != null && m_ApplicationModuleList.Count > 0)
            {
                m_ApplicationModuleList.Clear();
            }
        }

        /// <summary>
        /// To load the list of Competecny Source in sataic list 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private void LoadApplicationModule()
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(string.Format(this.Url, ""))));
            if (jsonString.ToString() != "null")
            {
                ClearApplicationModule();
                var applicationModuleList = JsonSerializer.DeserializeObject<Dictionary<string, Core.Competency.ApplicationModules>>(jsonString.ToString());
                m_ApplicationModuleList = new List<Core.Competency.ApplicationModules>();
                if (applicationModuleList != null)
                {
                    foreach (var appMod in applicationModuleList)
                    {
                        Core.Competency.ApplicationModules applicationModules = (Core.Competency.ApplicationModules)appMod.Value;
                        applicationModules.UniqueIdentifier = appMod.Key;
                        applicationModules.Url = FormApplicationModulesUrl(this.Url, false, applicationModules);
                        m_ApplicationModuleList.Add(appMod.Value);
                    }
                }
            }
        }

        public IList<Core.Competency.ApplicationModules> GetAllApplicationModules()
        {
            m_ApplicationModuleList = m_ApplicationModuleList.OrderBy(appMod => appMod.Name).ToList();
            return m_ApplicationModuleList;
        }

        /// <summary>
        /// To form(Insert) or set(update) the URL.
        /// </summary>
        /// <param ></param>
        /// <param name="competencyUrl"></param>
        /// <param name="isEditMode"></param>
        /// <param name="competencyUIObject"></param>
        /// <returns>string</returns>
        public string FormApplicationModulesUrl(string applicationModulesUrl, bool isEditMode, Core.Competency.ApplicationModules applicationModulesUIObject)
        {
            if (isEditMode)
            {
                return applicationModulesUrl;
            }
            else
            {
                return string.Format(this.Url, applicationModulesUIObject.GetNewGuidValue());
            }
        }
    }
}
