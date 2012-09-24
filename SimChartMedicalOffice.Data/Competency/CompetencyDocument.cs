using System.Collections.Generic;
using System.Text;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.DataInterfaces.Competency;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Data
{
    public class CompetencyDocument : KeyValueRepository<Core.Competency.Competency>, ICompetencyDocument
    {
        private static IList<string> _mCategoryMasterList;
        private static IList<Core.Competency.Competency> _mCompetencyMasterList;
       

        /// <summary>
        /// To cache the list of competencies and list of category in sataic list
        /// </summary>
        public CompetencyDocument()
        {
            LoadCompetencies();
        }

        /// <summary>
        /// To load the list of competencies and list of category in sataic list 
        /// </summary>
        public void LoadCompetencies()
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(string.Format(GetAssignmentUrl(Core.DocumentPath.Module.Masters,AppConstants.Competencies),"",""))));
            if (jsonString.ToString() != "null")
            {
                ClearCompetencyMasters();
                ClearCategoryMasters();
                _mCategoryMasterList = JsonSerializer.GetAllKeysFromJson(jsonString.ToString());
                var competencyDictionary = JsonSerializer.DeserializeObject<Dictionary<string, Dictionary<string, Core.Competency.Competency>>>(jsonString.ToString());
                _mCompetencyMasterList = new List<Core.Competency.Competency>();
                if (competencyDictionary != null)
                {
                    foreach (var categoryKey in competencyDictionary)
                    {
                        foreach (var competencyValue in categoryKey.Value)
                        {
                            Core.Competency.Competency competency = competencyValue.Value;
                            competency.UniqueIdentifier = competencyValue.Key;
                            competency.Category = categoryKey.Key;
                            competency.Url = FormCompetencyUrl(GetAssignmentUrl(Core.DocumentPath.Module.Masters, AppConstants.Competencies), true, competency);
                            _mCompetencyMasterList.Add(competency);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// To clear the list of competencies 
        /// </summary>
        private static void ClearCompetencyMasters()
        {
            if (_mCompetencyMasterList != null && _mCompetencyMasterList.Count > 0)
            {
                _mCompetencyMasterList.Clear();
            }
        }

        /// <summary>
        /// To clear the list of category 
        /// </summary>
        private static void ClearCategoryMasters()
        {
            if (_mCategoryMasterList != null && _mCategoryMasterList.Count > 0)
            {
                _mCategoryMasterList.Clear();
            }
        }

        /// <summary>
        /// To get list of Competency with category value as property.
        /// </summary>
        /// <returns></returns>
        public IList<Core.Competency.Competency> GetAllCompetencies()
        {
            return _mCompetencyMasterList;
        }

        /// <summary>
        /// To get list of dropdown values for Categories Dropdown.
        /// </summary>
        /// <returns></returns>
        public IList<string> GetAllCategories()
        {
            return _mCategoryMasterList;
        }

        /// <summary>
        /// To form(Insert) or set(update) the URL.
        /// </summary>
        /// <param ></param>
        /// <param name="competencyUrl"></param>
        /// <param name="isEditMode"></param>
        /// <param name="competencyUiObject"></param>
        /// <returns>string</returns>
        public string FormCompetencyUrl(string competencyUrl, bool isEditMode, Core.Competency.Competency competencyUiObject)
        {
            if (isEditMode)
            {
                if (AppCommon.CheckIfStringIsEmptyOrNull(competencyUiObject.UniqueIdentifier))
                {
                    return competencyUrl;
                }
                return string.Format(GetAssignmentUrl(Core.DocumentPath.Module.Masters, AppConstants.Competencies), string.Concat("/", competencyUiObject.Category), string.Concat("/", competencyUiObject.UniqueIdentifier));
            }
            return string.Format(GetAssignmentUrl(Core.DocumentPath.Module.Masters, AppConstants.Competencies), string.Concat("/", competencyUiObject.Category), string.Concat("/", competencyUiObject.GetNewGuidValue()));
        }
    }
}
