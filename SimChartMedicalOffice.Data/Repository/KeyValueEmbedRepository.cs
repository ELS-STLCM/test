using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Common;
using System.Collections;

namespace SimChartMedicalOffice.Data.Repository
{
    public abstract class KeyValueEmbedRepository<T>:IKeyValueRepository<T> where T:DocumentEntity
    {
        public abstract string URL { get; }
        public T Get(string guidValue)
        {
            string jsonString = HttpClient.Get(AppCommon.GetDocumentUrl(URL + "/" + guidValue));
            T result;
            result = JsonSerializer.DeserializeObject<T>(jsonString);
            return result;
        }
        public List<T> GetAll()
        {
            List<T> finalResultList;
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append(HttpClient.Get(AppCommon.GetDocumentUrl(URL)));
            var resultList = JsonSerializer.DeserializeObject<Hashtable>(jsonString.ToString());
            finalResultList = new List<T>();
            foreach (DictionaryEntry ucs in resultList)
            {
                finalResultList.Add(JsonSerializer.DeserializeObject<T>(ucs.Value.ToString()));
            }            
            return finalResultList;
        }
        public string SaveOrUpdate(T dataToSave)
        {
            KeyValuePair<string, T> ht;
            if (dataToSave.GuidValue == null)
            {
                //dataToSave.GuidValue = Guid.NewGuid().ToString();
                dataToSave.SetGuidValue();
            }
            ht = new KeyValuePair<string, T>(dataToSave.GuidValue, dataToSave);
            HttpClient.Put(AppCommon.GetDocumentUrl(URL), JsonSerializer.SerializeObject(ht.Value));
            return AppCommon.GetDocumentUrl(URL);
        }
        public bool Delete(string guidValue)
        {
            string jsonString = HttpClient.Delete(AppCommon.GetDocumentUrl(URL + "/" + guidValue));
            return true;
        }
    }
}
