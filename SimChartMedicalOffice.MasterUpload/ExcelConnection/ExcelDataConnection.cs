using System.Data;
using System.Data.OleDb;
using SimChartMedicalOffice.MasterUpload.Utilities;

namespace SimChartMedicalOffice.MasterUpload.ExcelConnection
{
    public static class ExcelDataConnection
    {
        public static DataSet GetExcelData(string fileName, string sheetName)
        {
            OleDbConnection connectionObject = new OleDbConnection
                                                   {
                                                       ConnectionString =
                                                           "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                                           ApplicationUtility.GetApplicationPath() + "\\" + fileName +
                                                           ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1\'"
                                                   };
            //connectionObject.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ApplicationUtility.GetApplicationPath() + "\\" + fileName + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
            OleDbDataAdapter excelAdapter = new OleDbDataAdapter("Select * from [" + sheetName + "$]", connectionObject);
            DataSet excelDataset = new DataSet();
            excelAdapter.Fill(excelDataset, "Competency");
            return excelDataset;
        }
    }
}
