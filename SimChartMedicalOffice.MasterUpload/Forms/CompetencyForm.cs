using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimChartMedicalOffice.MasterUpload;
using SimChartMedicalOffice.Core.Competency;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency;
using SimChartMedicalOffice.MasterUpload.Utilities;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.ApplicationServices.Competency;
using SimChartMedicalOffice.Data;
using System.Xml.Linq;
using SimChartMedicalOffice.Data.Competency;

namespace SimChartMedicalOffice.MasterUpload.Forms
{
    public partial class CompetencyForm : SimChartMedicalOffice.MasterUpload.Forms.SimOfficeForm
    {

        DataSet excelDataCompetency;
        CompetencyDocument competencyDocumentInstance = new CompetencyDocument();
        List<Competency> competencyListToSave = new List<Competency>();
        List<Core.Competency.CompetencySources> competencySourcesList = new List<CompetencySources>();
        List<Core.Competency.ApplicationModules> applicationModulesList = new List<ApplicationModules>();
        Dictionary<string, Dictionary<string, Competency>> competencyCategotyDic = new Dictionary<string, Dictionary<string, Competency>>();

        private CompetencyService _competencyService;
        ApplicationModuleDocument applicationModuleDocument = new ApplicationModuleDocument();
        CompetencySourceDocument competecnySourceDocument = new CompetencySourceDocument();
        public CompetencyForm()
        {
            InitializeComponent();
            _competencyService = new CompetencyService(competencyDocumentInstance, competecnySourceDocument, applicationModuleDocument);
        }


        private void Competency_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurationObject configObject = new ConfigurationObject();
                configObject.DataSheetName = "Competency";
                configObject.DataFileName = "Competency.xls";
                configObject.FormObject = this;

                excelDataCompetency = SimChartMedicalOffice.MasterUpload.ExcelConnection.ExcelDataConnection.GetExcelData(configObject.DataFileName, configObject.DataSheetName);
                //DataTable dtCompetency = excelDataCompetency.Tables["Competency"].Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field as string).Trim(), string.Empty) == 0)).CopyToDataTable();
                DataTable dtCompetency = ApplicationUtility.RemoveNullValueRow(excelDataCompetency, configObject.DataSheetName);
                dataGridView1.DataSource = dtCompetency;





                #region Option5


                List<string> catXLList = dtCompetency.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("Category")).Distinct().ToList<string>();




                foreach (string catXL in catXLList)
                 {

                    DataRow[] groupCatXL = dtCompetency.Select("Category like '%" + catXL.ToString() + "%'");

                    Dictionary<string, Competency> competencyDic = new Dictionary<string, Competency>();

                    foreach (DataRow drCom in groupCatXL)
                    {
                        try
                        {
                            Competency com = new Competency();
                            //com.SetGuidValue();
                            com.Focus = drCom.Field<string>("Focus");
                            com.Name = drCom.Field<string>("Competency Name");
                            if (com.Name != null)
                            {
                                List<Source> lstSource = new List<Source>();
                                if (drCom.Field<string>("CAAHEP") != null)
                                {
                                    Source source = new Source();
                                    source.Name = "CAAHEP";
                                    source.Number = drCom.Field<string>("CAAHEP");
                                    source.IsActive = true;
                                    lstSource.Add(source);
                                    AddCompetencySources(source.Name);
                                }
                                if (drCom.Field<string>("ABHES") != null)
                                {
                                    Source source = new Source();
                                    source.Name = "ABHES";
                                    source.Number = drCom.Field<string>("ABHES");
                                    source.IsActive = true;
                                    lstSource.Add(source);
                                    AddCompetencySources(source.Name);
                                }
                                if (drCom.Field<string>("Source") == "MAERB")
                                {
                                    Source source = new Source();
                                    source.Name = "MAERB";
                                    source.Number = "";
                                    source.IsActive = true;
                                    lstSource.Add(source);
                                    AddCompetencySources(source.Name);
                                }
                                com.Sources = lstSource;
                                com.IsActive = true;
                                

                                //bool exist = applicationModulesList.Select(foc => foc.Name.Equals(com.Focus)).SingleOrDefault();
                                int intdd = applicationModulesList.FindIndex(foc => foc.Name.Equals(com.Focus));
                                if (intdd < 0)
                                {
                                    if (com.Focus != null)
                                    {
                                        ApplicationModules appModule = new ApplicationModules();
                                        appModule.Name = com.Focus;
                                        applicationModulesList.Add(appModule);
                                    }
                                }
                                com.Category = catXL;
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
                        competencyCategotyDic.Add(catXL, competencyDic);
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
            int existIndex = competencySourcesList.FindIndex(foc => foc.Name.Equals(sourceName));
            if (existIndex < 0)
            {
                CompetencySources comSource = new CompetencySources();
                comSource.Name = sourceName;
                competencySourcesList.Add(comSource);
            }
        }

        private void btnFireBase_Click(object sender, EventArgs e)
        {

            try
            {
                //_competencyService.SaveCompetencyList(competencyCategotyDic);
                foreach (var item in competencyCategotyDic)
                {
                    foreach(var itemComp in item.Value)
                    {
                        Competency comp = (Competency)itemComp.Value;
                        comp.CreatedTimeStamp = DateTime.Now;
                        _competencyService.SaveCompetency(comp, "", false);    
                    
                    }                    
                }               


                foreach (ApplicationModules appModule in applicationModulesList)
                {
                    _competencyService.SaveApplicationModule(appModule, "", false);

                }
                foreach (CompetencySources comSource in competencySourcesList)
                {
                    _competencyService.SaveCompetencySource(comSource, "", false);

                }
                MessageBox.Show("Saved !");
            }
            catch (Exception ex)
            {
                string exst = ex.ToString();
                //to  do throw;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CompetencySources comSource = new CompetencySources();
            _competencyService.SaveCompetencySource(comSource, "", false);
        }

    }
}
