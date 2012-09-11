using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core;

namespace SimChartMedicalOffice.Data.Repository
{
    public interface IKeyValueRepository<T> where T : DocumentEntity
    {
        string Url { get; }
        T Get(string path, string guidValue);
        T Get(string path);
        List<T> GetAll(string path);
        string SaveOrUpdate(string path, T dataToSave);
        string SaveOrUpdate(string path, string dataToSave);
        string SaveOrUpdate(string path, Dictionary<string, T> dataToSave);
        string SaveOrUpdate(string path, Dictionary<string, object> dataToSave);
        string SaveOrUpdate(string path, Dictionary<string, Dictionary<string, SimChartMedicalOffice.Core.Competency.Competency>> dataToSave);
        bool Delete(string path, string guidValue, out string deleteResult);
        bool Delete(string path, string guidValue, string keyToDelete, out string deleteResult);
        bool Delete(string path, out string deleteResult);
        string GetJsonDocument(string path);
        string Move(string sourcePath, string destPath);
    }
}
