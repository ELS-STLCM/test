using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SimChartMedicalOffice.ApplicationServices.Competency;
using SimChartMedicalOffice.Core.Competency;
using SimChartMedicalOffice.Data;
using SimChartMedicalOffice.Data.Competency;
using SimChartMedicalOffice.MasterUpload.Utilities;

namespace SimChartMedicalOffice.MasterUpload.Forms
{
    public partial class CompetencyForm : SimOfficeForm
    {

        DataSet _excelDataCompetency;
        readonly CompetencyDocument _competencyDocumentInstance = new CompetencyDocument();
        readonly List<CompetencySources> _competencySourcesList = new List<CompetencySources>();
        readonly List<ApplicationModules> _applicationModulesList = new List<ApplicationModules>();
        readonly Dictionary<string, Dictionary<string, Competency>> _competencyCategotyDic = new Dictionary<string, Dictionary<string, Competency>>();

        private readonly CompetencyService _competencyService;
        readonly ApplicationModuleDocument _applicationModuleDocument = new ApplicationModuleDocument();
        readonly CompetencySourceDocument _competecnySourceDocument = new CompetencySourceDocument();
        public CompetencyForm()
        {
            InitializeComponent();
            _competencyService = new CompetencyService(_competencyDocumentInstance, _competecnySourceDocument, _applicationModuleDocument);
        }


        private void CompetencyLoad(object sender, EventArgs e)
        {
            try
            {
                ConfigurationObject configObject = new ConfigurationObject
                                                       {
                                                           DataSheetName = "Competency",
                                                           DataFileName = "Competency.xls",
                                                           FormObject = this
                                                       };

                _excelDataCompetency = ExcelConnection.ExcelDataConnection.GetExcelData(configObject.DataFileName, configObject.DataSheetName);
                //DataTable dtCompetency = excelDataCompetency.Tables["Competency"].Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field as string).Trim(), string.Empty) == 0)).CopyToDataTable();
                DataTable dtCompetency = ApplicationUtility.RemoveNullValueRow(_excelDataCompetency, configObject.DataSheetName);
                dataGridView1.DataSource = dtCompetency;





                #region Option5


                List<string> catXlList = dtCompetency.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("Category")).Distinct().ToList();




                foreach (string catXl in catXlList)
                {

                    DataRow[] groupCatXl = dtCompetency.Select("Category like '%" + catXl + "%'");

                    Dictionary<string, Competency> competencyDic = new Dictionary<string, Competency>();

                    foreach (DataRow drCom in groupCatXl)
                    {
                        try
                        {
                            Competency com = new Competency
                                                 {
                                                     Focus = drCom.Field<string>("Focus"),
                                                     Name = drCom.Field<string>("Competency Name")
                                                 };
                            //com.SetGuidValue();
                            if (com.Name != null)
                            {
                                List<Source> lstSource = new List<Source>();
                                if (drCom.Field<string>("CAAHEP") != null)
                                {
                                    Source source = new Source
                                                        {
                                                            Name = "CAAHEP",
                                                            Number = drCom.Field<string>("CAAHEP"),
                                                            IsActive = true
                                                        };
                                    lstSource.Add(source);
                                    AddCompetencySources(source.Name);
                                }
                                if (drCom.Field<string>("ABHES") != null)
                                {
                                    Source source = new Source
                                                        {
                                                            Name = "ABHES",
                                                            Number = drCom.Field<string>("ABHES"),
                                                            IsActive = true
                                                        };
                                    lstSource.Add(source);
                                    AddCompetencySources(source.Name);
                                }
                                if (drCom.Field<string>("Source") == "MAERB")
                                {
                                    Source source = new Source {Name = "MAERB", Number = "", IsActive = true};
                                    lstSource.Add(source);
                                    AddCompetencySources(source.Name);
                                }
                                com.Sources = lstSource;
                                com.IsActive = true;


                                //bool exist = applicationModulesList.Select(foc => foc.Name.Equals(com.Focus)).SingleOrDefault();
                                int intdd = _applicationModulesList.FindIndex(foc => foc.Name.Equals(com.Focus));
                                if (intdd < 0)
                                {
                                    if (com.Focus != null)
                                    {
                                        ApplicationModules appModule = new ApplicationModules {Name = com.Focus};
                                        _applicationModulesList.Add(appModule);
                                    }
                                }
                                com.Category = catXl;
                                competencyDic.Add(com.GetNewGuidValue(), com);
                            }


                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.ToString());
                        }

                    }

                    if (competencyDic.Count > 0)
                    {
                        _competencyCategotyDic.Add(catXl, competencyDic);
                    }
                }

                #endregion Option5
                #region Option4
                //List<string> catList = dtCompetency.Select(cat => cat.Field<string>("Category")).ToList<string>();
                //List<string> catXLList = dtCompetency.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("Category")).Distinct().ToList<string>();



                //foreach (string catXL in catXLList)
                //{

                //    DataRow[] groupCatXL = dtCompetency.Select("Category like '%" + catXL.ToString() + "%'");


                //    Category cat = new Category();
                //    cat.Name = catXL.ToString();


                //    Dictionary<string, Competency> competencyDic = new Dictionary<string, Competency>();

                //    foreach (DataRow drCom in groupCatXL)
                //    {
                //        Competency com = new Competency();
                //        com.SetGuidValue();
                //        com.Focus = drCom.Field<string>("Focus");
                //        com.Name = drCom.Field<string>("Competency Name");
                //        List<Source> lstSource = new List<Source>();
                //        if (drCom.Field<string>("CAAHEP") != null)
                //        {
                //            Source source = new Source();
                //            source.Name = "CAAHEP";
                //            source.Number = drCom.Field<string>("CAAHEP");
                //            lstSource.Add(source);
                //        }
                //        if (drCom.Field<string>("ABHES") != null)
                //        {
                //            Source source = new Source();
                //            source.Name = "ABHES";
                //            source.Number = drCom.Field<string>("ABHES");
                //            lstSource.Add(source);
                //        }
                //        if (drCom.Field<string>("Source") == "MAERB")
                //        {
                //            Source source = new Source();
                //            source.Name = "MAERB";
                //            source.Number = "";
                //            lstSource.Add(source);
                //        }
                //        com.SourceList = lstSource;
                //        competencyDic.Add(com.UniqueIdentifier, com);
                //    }

                //    cat.Competencies = competencyDic;
                //    categoryDic.Add(cat.Name, cat);
                //}
                //#endregion Option4




                //foreach (DataRow drCom in dtCompetency.Rows)
                //{
                //    Competency com = new Competency();

                //    Category newCategory = new Category();



                //    newCategory.Name = drCom.Field<string>("Category").ToString();
                //    newCategory.SetGuidValue();
                //    categoryListToSave.Add(newCategory);


                //    //drSatellite["MedNameSpace"] = drCom.Field<string>("").ToString();
                //    //drSatellite["value"] = drCom.Field<string>("").ToString();
                //    //drSatellite["Score"] = drCom.Field<string>("").ToString();
                //    //drSatellite["DOINumber"] = drCom.Field<string>("").ToString();
                //    //drSatellite["ConceptID"] = drCom.Field<string>("").ToString();

                //}






                //List<Category> fireBaseCategoryList = (List<Category>)_competencyService.GetAllCategories();
                //List<Competency> fireBaseCompetencyList = (List<Competency>)_competencyService.GetAllCompetencies();

                //#region Option1
                //string strJson = ApplicationUtility.GetJson(dtCompetency);
                //List<Competency> xlCompetencyList = JsonSerializer.DeserializeObject<List<Competency>>(strJson);
                //List<string> xlCategoryList = (from com in xlCompetencyList select com.CompetencyCategoryReference.ToString()).Distinct().ToList<string>();
                //// for list object with xl data 
                //foreach (string xlCategory in xlCategoryList)
                //{
                //    Category fireBaseCategory = fireBaseCategoryList.FirstOrDefault(x => x.Name == xlCategory.ToString());
                //    bool isCategoryExistFB = (fireBaseCategory != null) ? true : false;
                //    if (!isCategoryExistFB)
                //    {
                //        Category newCategory = new Category();
                //        newCategory.Name = xlCategory.ToString();
                //        newCategory.SetGuidValue();
                //        categoryListToSave.Add(newCategory);
                //        // get xl Category Wise Competency List
                //        List<Competency> xlCategoryWiseCompetencyList = (from xlcat in xlCompetencyList where xlcat.CompetencyCategoryReference.Equals(xlCategory.ToString()) select xlcat).ToList<Competency>();
                //        xlCategoryWiseCompetencyList.ForEach(xlcat => xlcat.CompetencyCategoryReference = newCategory.UniqueIdentifier);
                //        xlCategoryWiseCompetencyList.ForEach(xlcat => xlcat.SetGuidValue());
                //        competencyListToSave.AddRange(xlCategoryWiseCompetencyList);
                //    }
                //    else
                //    {
                //        categoryListToSave.Add(fireBaseCategory);
                //        // get fire base Category Wise Competency List
                //        List<Competency> fbCategoryWiseCompentencyList = (from com in fireBaseCompetencyList where com.CompetencyCategoryReference.Equals(fireBaseCategory.UniqueIdentifier.ToString()) select com).ToList<Competency>();
                //        competencyListToSave.AddRange(fbCategoryWiseCompentencyList);
                //        // get xl Category Wise Competency List
                //        List<Competency> xlCategoryWiseList = (from xlcat in xlCompetencyList where xlcat.CompetencyCategoryReference.Equals(xlCategory.ToString()) select xlcat).ToList<Competency>();
                //        xlCategoryWiseList.ForEach(xlcat => xlcat.CompetencyCategoryReference = fireBaseCategory.UniqueIdentifier);
                //        xlCategoryWiseList.ForEach(xlcat => xlcat.SetGuidValue());
                //        competencyListToSave.AddRange(xlCategoryWiseList);
                //    }
                //}
                //// form list object with firebase existing data
                //List<Category> fbCategortListNotInXL = fireBaseCategoryList.Where(cat => !xlCategoryList.Contains(cat.Name)).ToList<Category>();
                //foreach (Category catFB in fbCategortListNotInXL)
                //{
                //    categoryListToSave.Add(catFB);
                //    List<Competency> fbCategoryWiseCompentencyList = (from com in fireBaseCompetencyList where com.CompetencyCategoryReference.Equals(catFB.UniqueIdentifier.ToString()) select com).ToList<Competency>();
                //    competencyListToSave.AddRange(fbCategoryWiseCompentencyList);
                //}
                //dataGridView2.DataSource = categoryListToSave;
                //dataGridView3.DataSource = competencyListToSave;
                #endregion

            }
            catch (Exception ex)
            {
                string exstr = ex.ToString();
                MessageBox.Show(exstr);
                // to do               
            }


        }

        private void AddCompetencySources(string sourceName)
        {
            int existIndex = _competencySourcesList.FindIndex(foc => foc.Name.Equals(sourceName));
            if (existIndex < 0)
            {
                CompetencySources comSource = new CompetencySources {Name = sourceName};
                _competencySourcesList.Add(comSource);
            }
        }

        private void BtnFireBaseClick(object sender, EventArgs e)
        {

            const string savedMsg = "Saved !";
            //_competencyService.SaveCompetencyList(competencyCategotyDic);
            foreach (var item in _competencyCategotyDic)
            {
                foreach (var itemComp in item.Value)
                {
                    Competency comp = itemComp.Value;
                    comp.CreatedTimeStamp = DateTime.Now;
                    _competencyService.SaveCompetency(comp, "", false);

                }
            }


            foreach (ApplicationModules appModule in _applicationModulesList)
            {
                _competencyService.SaveApplicationModule(appModule, "", false);

            }
            foreach (CompetencySources comSource in _competencySourcesList)
            {
                _competencyService.SaveCompetencySource(comSource, "", false);

            }
            MessageBox.Show(savedMsg);
        }

        private void Button1Click(object sender, EventArgs e)
        {
            CompetencySources comSource = new CompetencySources();
            _competencyService.SaveCompetencySource(comSource, "", false);
        }

    }
}
