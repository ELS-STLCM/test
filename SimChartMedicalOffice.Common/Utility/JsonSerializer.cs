using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimChartMedicalOffice.Common.Utility
{
    public static class JsonSerializer
    {
        public static T DeserializeObject<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        public static XmlDocument DeserializeXmlObject(string jsonString)
        {
            return JsonConvert.DeserializeXmlNode("{\"Row\":" + jsonString + "}", "root");
        }

        public static string SerializeObject(object value)
        {   
            return JsonConvert.SerializeObject(value);
        }
        public static List<string> GetAllKeysFromJson(string jsonString)
        {
            List<string> keyList = new List<string>();
            if (!string.IsNullOrEmpty(jsonString))
            {
                JObject searchCriteria = JObject.Parse(jsonString);
                keyList.AddRange(from keyValue in (IEnumerable<JToken>) searchCriteria select ((JProperty) keyValue).Name);
            }
            return keyList;
        }

        public static List<string> GetAllValuesFromJson(string jsonString)
        {
            JObject searchCriteria = JObject.Parse(jsonString);
            List<string> keyList = new List<string>();
            foreach (JToken keyValue in (IEnumerable<JToken>)searchCriteria)
            {
                keyList.Add(((JProperty)keyValue).Value.ToString());
            }
            return keyList;
        }

        public static List<string> GetAllValuesForAPropertyFromJson(string key, string jsonString)
        {
            JObject searchCriteria = JObject.Parse(jsonString);
            List<string> keyList = new List<string>();
            //StringBuilder jsonValueString = new StringBuilder();
            foreach (JToken keyValue in (IEnumerable<JToken>)searchCriteria)
            {
                if (((JProperty)keyValue).Name == key)
                {
                    keyList.Add(((JProperty)keyValue).Value.ToString());
                }
            }
            return keyList;
        }

        public static string GetAllValuesForAPropertyAsStringFromJson(string key, string jsonString)
        {
            JObject searchCriteria = JObject.Parse(jsonString);
            StringBuilder jsonValueString = new StringBuilder();
            foreach (JToken keyValue in (IEnumerable<JToken>)searchCriteria)
            {
                if (((JProperty)keyValue).Name == key)
                {
                    jsonValueString.Append(((JProperty)keyValue).Value.ToString());
                }
            }
            return jsonValueString.ToString();
        }
       
        public static string Jobject(string jsonString)
        {
            List<string> arrayString = new List<string>();
            JObject root = JObject.Parse(jsonString);
            JToken jtMonth = root.First.First;
            while (jtMonth != null)
            {
                JToken jtDay = jtMonth.First;
                while (jtDay!=null)
                {
                    JToken jt = jtDay.First.First;
                    while (jt != null)
                    {
                        arrayString.Add(jt.ToString());
                        jt = jt.Next;
                        
                    }
                    jtDay = jtDay.Next;
                }
                jtMonth = jtMonth.Next;
            }

            
            return "";
        }
        //public void SomeMethod(JObject parent)
        //{
        //    foreach (JObject child in parent.Children())
        //    {
        //        if (child.HasValues)
        //        {
        //            //
        //            // Code to get the keys here
        //            //
        //            SomeMethod(child);
        //        }
        //    }
        //}
    }
}
