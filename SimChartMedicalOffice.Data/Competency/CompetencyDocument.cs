using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.Competency;
using System.Collections.Generic;
using System.Text;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Common;
using System.Linq;
using System.Collections;
using System;

namespace SimChartMedicalOffice.Data
{
    public class CompetencyDocument : KeyValueRepository<Core.Competency.Competency>, ICompetencyDocument
    {
        private static IList<string> m_categoryMasterList;
        private static IList<Core.Competency.Competency> m_competencyMasterList;
        public override string Url
        {
            get
            {
                return "SimApp/Master/Competencies{0}{1}";
            }
        }

        /// <summary>
        /// To cache the list of competencies and list of category in sataic list 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public CompetencyDocument()
        {
            this.LoadCompetencies();
        }

        /// <summary>
        /// To load the list of competencies and list of category in sataic list 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public void LoadCompetencies()
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(string.Format(this.Url,"",""))));
            if (jsonString.ToString() != "null")
            {
                ClearCompetencyMasters();
                ClearCategoryMasters();
                m_categoryMasterList = JsonSerializer.GetAllKeysFromJson(jsonString.ToString());
                var competencyDictionary = JsonSerializer.DeserializeObject<Dictionary<string, Dictionary<string, Core.Competency.Competency>>>(jsonString.ToString());
                m_competencyMasterList = new List<Core.Competency.Competency>();
                if (competencyDictionary != null)
                {
                    foreach (var categoryKey in competencyDictionary)
                    {
                        foreach (var competencyValue in categoryKey.Value)
                        {
                            Core.Competency.Competency competency = (Core.Competency.Competency)competencyValue.Value;
                            competency.UniqueIdentifier = competencyValue.Key.ToString();
                            competency.Category = categoryKey.Key.ToString();
                            competency.Url = FormCompetencyUrl(this.Url, true, competency);
                            m_competencyMasterList.Add(competency);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// To clear the list of competencies 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private static void ClearCompetencyMasters()
        {
            if (m_competencyMasterList != null && m_competencyMasterList.Count > 0)
            {
                m_competencyMasterList.Clear();
            }
        }

        /// <summary>
        /// To clear the list of category 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private static void ClearCategoryMasters()
        {
            if (m_categoryMasterList != null && m_categoryMasterList.Count > 0)
            {
                m_categoryMasterList.Clear();
            }
        }

        /// <summary>
        /// To get list of Competency with category value as property.
        /// </summary>
        /// <param name=""></param>
        /// <returns>IList<Competency></returns>
        public IList<Core.Competency.Competency> GetAllCompetencies()
        {
            return m_competencyMasterList;
        }

        /// <summary>
        /// To get list of dropdown values for Categories Dropdown.
        /// </summary>
        /// <param ></param>
        /// <returns>IList<string></returns>
        public IList<string> GetAllCategories()
        {
            return m_categoryMasterList;
        }

        /// <summary>
        /// To form(Insert) or set(update) the URL.
        /// </summary>
        /// <param ></param>
        /// <param name="competencyUrl"></param>
        /// <param name="isEditMode"></param>
        /// <param name="competencyUIObject"></param>
        /// <returns>string</returns>
        public string FormCompetencyUrl(string competencyUrl, bool isEditMode, Core.Competency.Competency competencyUIObject)
        {
            if (isEditMode)
            {
                if (AppCommon.CheckIfStringIsEmptyOrNull(competencyUIObject.UniqueIdentifier))
                {
                    return competencyUrl;
                }
                else
                {
                    return string.Format(this.Url, string.Concat("/", competencyUIObject.Category), string.Concat("/", competencyUIObject.UniqueIdentifier));                    
                }                
            }
            else
            {
                return string.Format(this.Url, string.Concat("/",competencyUIObject.Category),string.Concat("/",competencyUIObject.GetNewGuidValue()));
            }
        }


    }
}
