using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using SimChartMedicalOffice.MasterUpload.Utilities;

namespace SimChartMedicalOffice.MasterUpload.ExcelConnection
{
    public static class ExcelDataConnection
    {
        public static DataSet GetExcelData(string fileName, string sheetName)
        {

            OleDbConnection connectionObject;
            OleDbDataAdapter excelAdapter;
            DataSet excelDataset;
            connectionObject = new OleDbConnection();
            connectionObject.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ApplicationUtility.GetApplicationPath() + "\\" + fileName + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1\'";
            //connectionObject.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ApplicationUtility.GetApplicationPath() + "\\" + fileName + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
            excelAdapter = new OleDbDataAdapter("Select * from [" + sheetName + "$]", connectionObject);
            excelDataset = new DataSet();
            excelAdapter.Fill(excelDataset, "Competency");
            return excelDataset;
        }
    }
}
