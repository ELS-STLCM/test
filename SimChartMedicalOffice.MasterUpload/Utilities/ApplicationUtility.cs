﻿using System;
using System.Data;
using System.Linq;
using System.Text;

namespace SimChartMedicalOffice.MasterUpload.Utilities
{
    public static class ApplicationUtility
    {
        public static string GetApplicationPath()
        {
            string path = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            return @"D:\Elsevier\SimOffice_Source\SimChartMedicalOffice.MasterUpload\MasterData";
        }

        public static string ReplaceSingleQuote(string stringWithSingleQuote)
        {
            stringWithSingleQuote = stringWithSingleQuote.Replace("'", "''");
            return stringWithSingleQuote;
        }
        public static string ReplaceSingleQuoteForDatabase(string stringWithSingleQuote)
        {
            stringWithSingleQuote = stringWithSingleQuote.Replace("'", "''");
            return stringWithSingleQuote;
        }

        public static DataTable RemoveNullValueRow(DataSet dsData, string tableName)
        {
            DataTable dtData = dsData.Tables[tableName].Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field =>
                                                                                                                 {
                                                                                                                     var s = field as string;
                                                                                                                     return s != null && (String.CompareOrdinal(s.Trim(), string.Empty) == 0);
                                                                                                                 })).CopyToDataTable();
            return dtData;
        }

        public static string GetJson(DataRow r)
        {
            int index = 0;
            StringBuilder json = new StringBuilder();
            foreach (DataColumn item in r.Table.Columns)
            {
                json.Append(String.Format("\"{0}\" : \"{1}\"", item.ColumnName, r[item.ColumnName].ToString().Replace("\"", "\\\"")));
                if (index < r.Table.Columns.Count - 1)
                {
                    json.Append(", ");
                }
                index++;
            }
            return "{" + json + "}";
        }

        public static string GetJson(DataTable t)
        {
            int indexTable = 0;
            StringBuilder json = new StringBuilder();
            foreach (DataRow currRow in t.Rows)
            {
                json.Append(GetJson(currRow));
                if (indexTable < t.Rows.Count - 1)
                {
                    json.Append(", ");
                }
                indexTable++;
            }
            return "[" + json + "]";
        }
    }
}
