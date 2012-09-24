using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.DataInterfaces.Competency;
using SimChartMedicalOffice.Data.Repository;


namespace SimChartMedicalOffice.Data.Competency
{
   public class CompetencySourceDocument : KeyValueRepository<Core.Competency.CompetencySources>, ICompetencySourceDocument
    {
       private static IList<Core.Competency.CompetencySources> _mCompetecnySourceList;
       

       /// <summary>
       /// To cache the list of Competecny Source in sataic list 
       /// </summary>
       public CompetencySourceDocument()
        {
            LoadCompetecnySource();
        }

       /// <summary>
       /// To clear the list of Competecny Source List 
       /// </summary>
       private static void ClearCompetecnySource()
       {
           if (_mCompetecnySourceList != null && _mCompetecnySourceList.Count > 0)
           {
               _mCompetecnySourceList.Clear();
           }
       }

       /// <summary>
       /// To load the list of Competecny Source in sataic list 
       /// </summary>
       public void LoadCompetecnySource()
       {
           StringBuilder jsonString = new StringBuilder();
           jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(string.Format(GetAssignmentUrl(Core.DocumentPath.Module.Masters,AppConstants.CompetencySource), ""))));
           if (jsonString.ToString() != "null")
           {
               ClearCompetecnySource();

               var competecnySourceList = JsonSerializer.DeserializeObject<Dictionary<string, Core.Competency.CompetencySources>>(jsonString.ToString());
               _mCompetecnySourceList = new List<Core.Competency.CompetencySources>();
               if (competecnySourceList != null)
               {
                   foreach (var comSource in competecnySourceList)
                   {
                       Core.Competency.CompetencySources competencySources = comSource.Value;
                       competencySources.UniqueIdentifier = comSource.Key;
                       competencySources.Url = FormCompetencySourceUrl(GetAssignmentUrl(Core.DocumentPath.Module.Masters, AppConstants.CompetencySource), false, competencySources);
                       _mCompetecnySourceList.Add(comSource.Value);
                   }
               }
           }
       }

       /// <summary>
       /// To get the list of Competecny Source from sataic list 
       /// </summary>
       /// <returns></returns>
       public IList<Core.Competency.CompetencySources> GetAllCompetecnySources()
       {
           _mCompetecnySourceList = _mCompetecnySourceList.OrderBy(comSource => comSource.Name).ToList();
           return _mCompetecnySourceList;
       }

       /// <summary>
       /// To form(Insert) or set(update) the URL.
       /// </summary>
       /// <param ></param>
       /// <param name="competencySourceUrl"></param>
       /// <param name="isEditMode"></param>
       /// <param name="competencySourceUiObject"></param>
       /// <returns>string</returns>
       public string FormCompetencySourceUrl(string competencySourceUrl, bool isEditMode, Core.Competency.CompetencySources competencySourceUiObject)
       {
           if (isEditMode)
           {
               return competencySourceUrl;
           }
           return string.Format(GetAssignmentUrl(Core.DocumentPath.Module.Masters, AppConstants.CompetencySource), competencySourceUiObject.GetNewGuidValue());
       }
    }
}
