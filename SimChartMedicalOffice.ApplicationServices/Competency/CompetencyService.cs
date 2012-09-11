using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency;
using SimChartMedicalOffice.Core.DataInterfaces.Competency;
using SimChartMedicalOffice.Core.Competency;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.SkillSetBuilder;


namespace SimChartMedicalOffice.ApplicationServices.Competency
{
    public class CompetencyService : ICompetencyService
    {
        private readonly ICompetencyDocument _competencyDocument;

        private readonly ICompetencySourceDocument _competecnySourceDocument;
        private readonly IApplicationModuleDocument _applicationModuleDocument;
        public static Dictionary<string, string> competencyStringList { get; set; }

        public CompetencyService(ICompetencyDocument competencyDocumentInstance, ICompetencySourceDocument competecnySourceDocument, IApplicationModuleDocument applicationModuleDocument)
        {
            this._competencyDocument = competencyDocumentInstance;
            this._applicationModuleDocument = applicationModuleDocument;
            this._competecnySourceDocument = competecnySourceDocument;
        }

        /// <summary>
        /// To get list of dropdown values for Categories Dropdown.
        /// </summary>
        /// <param ></param>
        /// <returns>IList<string></returns>
        public IList<string> GetAllCategories()
        {
            return _competencyDocument.GetAllCategories();
        }

        /// <summary>
        /// To save dictionary of dictionary competency .
        /// </summary>
        /// <param name="competencyDictionar">to save the competecny</param>
        /// <returns></returns>
        public void SaveCompetencyList(Dictionary<string, Dictionary<string, SimChartMedicalOffice.Core.Competency.Competency>> competencyDictionar)
        {
            _competencyDocument.SaveOrUpdate(string.Format(_competencyDocument.Url, "",""), competencyDictionar);
        }

        /// <summary>
        /// To get list of Competency with category value as property.
        /// </summary>
        /// <param name=""></param>
        /// <returns>IList<Competency></returns>
        public IList<Core.Competency.Competency> GetAllCompetencies()
        {
            IList<Core.Competency.Competency> competencyList = _competencyDocument.GetAllCompetencies();
            if (competencyList != null && competencyList.Count > 0)
            {
                return competencyList;
            }
            else
            {
                return competencyList;
                return competencyList.ToList();
            }
        }

        /// <summary>
        /// To get list of dropdown values for Competeny Dropdown.
        /// </summary>
        /// <param name="competencyList"></param>
        /// <returns></returns>
        public List<AutoCompleteProxy> GetAllCompetencyListForDropDown()
        {
            List<AutoCompleteProxy> competencyStringList = new List<AutoCompleteProxy>();
            IList<Core.Competency.Competency> competencyList = GetAllCompetencies();
            if (competencyList != null)
            {
                competencyList = (from competencyListTemp in competencyList where competencyListTemp.IsActive == true select competencyListTemp).ToList();
            }
            competencyStringList = GetCompetenciesStringListInFormat(competencyList);
            return competencyStringList;
        }

        /// <summary>
        /// To get list of competencies in format as per requirement
        /// </summary>
        /// <param name="competencyList"></param>
        /// <returns></returns>
        public List<AutoCompleteProxy> GetCompetenciesStringListInFormat(IList<Core.Competency.Competency> competencyList)
        {
            List<AutoCompleteProxy> competencyStringListTemp = new List<AutoCompleteProxy>();
            string sourceListString = string.Empty;
            string competencyListString = string.Empty;
            //Order by source Name in alphabetical order
            if (competencyList != null)
            {
                foreach (Core.Competency.Competency competencyItem in competencyList)
                {
                    AutoCompleteProxy autoComplete = new AutoCompleteProxy();
                    if (competencyItem.Sources != null && competencyItem.Sources.Count > 0)
                    {
                        competencyItem.Sources = (from lstSource in competencyItem.Sources orderby lstSource.Name select lstSource).ToList();
                        sourceListString = String.Join(", ", competencyItem.Sources.Select(s => s.Name + " " + s.Number));
                        competencyListString = competencyItem.Name + ", " + sourceListString;
                    autoComplete.Sources = competencyItem.Sources.Select(s => s.Name).ToList();
                    }
                    else
                    {
                        competencyListString = competencyItem.Name;
                    }
                    autoComplete.id = competencyItem.UniqueIdentifier;
                    autoComplete.name = competencyListString;
                    competencyStringListTemp.Add(autoComplete);
                }
            }
            competencyStringListTemp = competencyStringListTemp.OrderBy(F => F.name).ToList();
            return competencyStringListTemp;
        }

        /// <summary>
        /// To get list of competencies based on filter text
        /// </summary>
        /// <param name="strFilterText">filter text for the dropdown</param>
        /// <returns></returns>
        public List<AutoCompleteProxy> GetFilteredCompetenciesBasedOnString(string strFilterText)
        {
            List<Core.Competency.Competency> comList = new List<Core.Competency.Competency>();//assign the static list of competency
            List<AutoCompleteProxy> competencyStringListTemp = new List<AutoCompleteProxy>();
            List<Core.Competency.Competency> lstTempCom = (from lst in comList
                                                           where lst.Name.Contains(strFilterText)
                                                           && (lst.Sources == null || (lst.Sources != null && (lst.Sources.Exists(s => s.Name.Contains(strFilterText))
                                                           || lst.Sources.Exists(s => s.Number.Contains(strFilterText)))))
                                                           select lst).ToList();
            competencyStringListTemp = GetCompetenciesStringListInFormat(lstTempCom);
            return competencyStringListTemp;
        }

        /// <summary>
        /// To get list of All Competecny Sources
        /// </summary>
        /// <param name=></param>
        /// <returns>IList<CompetencySources></returns>
        public IList<CompetencySources> GetAllCompetecnySources()
        {
            return _competecnySourceDocument.GetAllCompetecnySources();
        }

        /// <summary>
        /// To get list of Application Modules
        /// </summary>
        /// <param name=></param>
        /// <returns>IList<ApplicationModules></returns>
        public IList<ApplicationModules> GetAllApplicationModules()
        {
            return _applicationModuleDocument.GetAllApplicationModules();
        } 
        /// <summary>
        /// To save Competency.
        /// </summary>
        /// <param name="">to save the competecny</param>
        /// <param name="competencyUIObject"></param>
        /// <param name="competencyUrl"></param>
        /// <param name="isEditMode"></param>
        /// <returns>bool</returns>
        public bool SaveCompetency(Core.Competency.Competency competencyUIObject, string competencyUrl, bool isEditMode)
        {
            _competencyDocument.SaveOrUpdate(_competencyDocument.FormCompetencyUrl(competencyUrl, isEditMode, competencyUIObject), competencyUIObject);
            _competencyDocument.LoadCompetencies();
            return true;
        }

        /// <summary>
        /// To save Competency Source.
        /// </summary>
        /// <param name="">to save the competency Sources</param>
        /// <param name="competencySourcesUIObject"></param>
        /// <param name="competencySourcesUrl"></param>
        /// <param name="isEditMode"></param>
        /// <returns>bool</returns>
        public bool SaveCompetencySource(CompetencySources competencySourcesUIObject, string competencySourcesUrl, bool isEditMode)
        {
            _competecnySourceDocument.SaveOrUpdate(_competecnySourceDocument.FormCompetencySourceUrl(competencySourcesUrl, isEditMode, competencySourcesUIObject), competencySourcesUIObject);
            _competecnySourceDocument.LoadCompetecnySource();            
            return true;
        }

        /// <summary>
        /// To save ApplicationModules.
        /// </summary>
        /// <param name="">to save the ApplicationModules</param>
        /// <param name="applicationModuleUIObject"></param>
        /// <param name="applicationModuleUrl"></param>
        /// <param name="isEditMode"></param>
        /// <returns>bool</returns>
        public bool SaveApplicationModule(ApplicationModules applicationModuleUIObject, string applicationModuleUrl, bool isEditMode)
        {
            _applicationModuleDocument.SaveOrUpdate(_applicationModuleDocument.FormApplicationModulesUrl(applicationModuleUrl, isEditMode, applicationModuleUIObject), applicationModuleUIObject);
            return true;
        }

        /// <summary>
        /// To delete competency.
        /// </summary>
        /// <param name="">To delete competency</param>
        /// <param name="Competency"></param>
        /// <returns>bool</returns>
        public bool DeleteCompetency(Core.Competency.Competency deleteCompetency)
        {
            string competencyUrl = (deleteCompetency.Url != "") ? deleteCompetency.Url : "";
            _competencyDocument.SaveOrUpdate(_competencyDocument.FormCompetencyUrl(competencyUrl, true, deleteCompetency), deleteCompetency);
            return true;
        }
        /// <summary>
        /// Get LinkedCompetency Text for a competency guid
        /// </summary>
        /// <param name="guidOfLinkedCompetency"></param>
        /// <returns></returns>
        public void SetLinkedCompetencyTextForAQuestions(string guidOfLinkedCompetency,DocumentProxy documentProxy)
        {
            List<AutoCompleteProxy> lstOfAutoComplete = GetAllCompetencyListForDropDown();
            documentProxy.LinkedItemReference=(from lstCompetency in lstOfAutoComplete where lstCompetency.id == guidOfLinkedCompetency select lstCompetency.name).SingleOrDefault();
        }

        private string GetLinkedCompetencyForAGuid(string guidOfLinkedCompetency)
        {
            List<AutoCompleteProxy> lstOfAutoComplete = GetAllCompetencyListForDropDown();
            return (from lstCompetency in lstOfAutoComplete where lstCompetency.id == guidOfLinkedCompetency select lstCompetency.name).SingleOrDefault();
        }
        public void SetLinkedCompetencyForAGuid(string linkedCompetencyRef)
        {
            linkedCompetencyRef = GetLinkedCompetencyForAGuid(linkedCompetencyRef);
        }

        public string GetLinkedCompetencyNameForAGuid(string linkedCompetencyRef)
        {
           return GetLinkedCompetencyForAGuid(linkedCompetencyRef);
        }
        /// <summary>
        /// Get LinkedCompetency Text for a competency guid
        /// </summary>
        /// <param name="guidOfLinkedCompetency"></param>
        /// <returns></returns>
        public void SetLinkedCompetencyTextForAQuestions(IList<string> guidOfLinkedCompetency, SkillSetProxy documentProxy)
        {
            List<AutoCompleteProxy> lstOfAutoComplete = GetAllCompetencyListForDropDown();
            documentProxy.LinkedCompetencies.ToList().ForEach(c => SetLinkedCompetencyForAGuid(c));
        }

 
        /// <summary>
        /// Get Competency Name for a list of competencies
        /// </summary>
        /// <param name="guidOfLinkedCompetencies"></param>
        /// <returns></returns>
        public string GetCompetencyNameForListofCompetencies(IList<string> guidOfLinkedCompetencies)
        {
            List<AutoCompleteProxy> lstOfAutoComplete = GetAllCompetencyListForDropDown();
            var linkedCompetencies = (from lstCompetency in lstOfAutoComplete join guidOfCompetency in guidOfLinkedCompetencies on lstCompetency.id equals guidOfCompetency select lstCompetency.name);
            string listOfCompetencies = "<ul class = align-competency>";
            foreach (var linkedCompetency in linkedCompetencies)
            {
                listOfCompetencies += "<li>" + linkedCompetency + "</li>";
            }
            listOfCompetencies += "</ul>";
            return listOfCompetencies;
        }

        /// <summary>
        /// Get Competency Name in numbers for a list of competencies
        /// </summary>
        /// <param name="guidOfLinkedCompetencies"></param>
        /// <returns></returns>
        public string GetCompetencyNameListInNumbers(IList<string> guidOfLinkedCompetencies)
        {
            List<AutoCompleteProxy> lstOfAutoComplete = GetAllCompetencyListForDropDown();
            var linkedCompetencies = (from lstCompetency in lstOfAutoComplete join guidOfCompetency in guidOfLinkedCompetencies on lstCompetency.id equals guidOfCompetency select lstCompetency.name);
            string listOfCompetencies = "<ol class='Linked-competencies-view'>";
            foreach (var linkedCompetency in linkedCompetencies)
            {
                listOfCompetencies += "<li>" + linkedCompetency + "</li>";
                listOfCompetencies += "<div class='clear-height-spacing clear'></div>";                
            }
            listOfCompetencies += "</ol>";
            return listOfCompetencies;
        }

        /// <summary>
        /// To Get Comptency from comptency List .
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <returns>Core.Competency.Competency</returns>
        public Core.Competency.Competency GetCompetency(string competencyGUID)
        {
            Core.Competency.Competency competency = new Core.Competency.Competency();
            IList<Core.Competency.Competency> competencyList  = _competencyDocument.GetAllCompetencies();
            if (competencyGUID != null)
            {
                 competency = (from com in competencyList where com.UniqueIdentifier.Equals(competencyGUID.ToString()) select com).SingleOrDefault();
            }
            return competency;
        }
    }
}
