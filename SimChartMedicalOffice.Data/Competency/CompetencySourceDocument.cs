using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.Competency;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;


namespace SimChartMedicalOffice.Data.Competency
{
   public class CompetencySourceDocument : KeyValueRepository<Core.Competency.CompetencySources>, ICompetencySourceDocument
    {
       private static IList<Core.Competency.CompetencySources> m_CompetecnySourceList;
       public override string Url
       {
           get
           {
               return "SimApp/Master/CompetencySource/{0}";
           }
       }

       /// <summary>
       /// To cache the list of Competecny Source in sataic list 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
       public CompetencySourceDocument()
        {
            this.LoadCompetecnySource();
        }

       /// <summary>
       /// To clear the list of Competecny Source List 
       /// </summary>
       /// <param name=""></param>
       /// <returns></returns>
       private static void ClearCompetecnySource()
       {
           if (m_CompetecnySourceList != null && m_CompetecnySourceList.Count > 0)
           {
               m_CompetecnySourceList.Clear();
           }
       }

       /// <summary>
       /// To load the list of Competecny Source in sataic list 
       /// </summary>
       /// <param name=""></param>
       /// <returns></returns>
       public void LoadCompetecnySource()
       {
           StringBuilder jsonString = new StringBuilder();
           jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(string.Format(this.Url, ""))));
           if (jsonString.ToString() != "null")
           {
               ClearCompetecnySource();

               var competecnySourceList = JsonSerializer.DeserializeObject<Dictionary<string, Core.Competency.CompetencySources>>(jsonString.ToString());
               m_CompetecnySourceList = new List<Core.Competency.CompetencySources>();
               if (competecnySourceList != null)
               {
                   foreach (var comSource in competecnySourceList)
                   {
                       Core.Competency.CompetencySources competencySources = (Core.Competency.CompetencySources)comSource.Value;
                       competencySources.UniqueIdentifier = comSource.Key;
                       competencySources.Url = FormCompetencySourceUrl(this.Url, false, competencySources);
                       m_CompetecnySourceList.Add(comSource.Value);
                   }
               }
           }
       }

       /// <summary>
       /// To get the list of Competecny Source from sataic list 
       /// </summary>
       /// <param name=""></param>
       /// <returns> IList<Core.Competency.Source></returns>
       public IList<Core.Competency.CompetencySources> GetAllCompetecnySources()
       {
           m_CompetecnySourceList = m_CompetecnySourceList.OrderBy(comSource => comSource.Name).ToList();
           return m_CompetecnySourceList;
       }

       /// <summary>
       /// To form(Insert) or set(update) the URL.
       /// </summary>
       /// <param ></param>
       /// <param name="competencySourceUrl"></param>
       /// <param name="isEditMode"></param>
       /// <param name="competencySourceUIObject"></param>
       /// <returns>string</returns>
       public string FormCompetencySourceUrl(string competencySourceUrl, bool isEditMode, Core.Competency.CompetencySources competencySourceUIObject)
       {
           if (isEditMode)
           {
               return competencySourceUrl;
           }
           else
           {
               return string.Format(this.Url, competencySourceUIObject.GetNewGuidValue());
           }
       }
    }
}
