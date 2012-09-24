using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using SimChartMedicalOffice.Common.Logging;

namespace SimChartMedicalOffice.Common.Utility
{
    public static class HttpClient
    {
        public enum RestfulMethods
        {
            GET = 1,
            Post = 2,
            Put = 3,
            Delete = 4
        }

        public static string Get(string url)
        {
            return MakeRestFulApiCall(url, RestfulMethods.GET, null);
        }

        public static string Post(string url, string jsonData)
        {
            return MakeRestFulApiCall(url, RestfulMethods.Post, jsonData);
        }

        public static string Put(string url, string jsonData)
        {
            return MakeRestFulApiCall(url, RestfulMethods.Put, jsonData);
        }

        public static string Delete(string url)
        {
            return MakeRestFulApiCall(url, RestfulMethods.Delete, null);
        }

        private static string MakeRestFulApiCall(string url, RestfulMethods requestMethod, string postdata)
        {
            StringBuilder restURL = new StringBuilder();
            try
            {
                restURL.AppendFormat(url);
                HttpWebRequest restRequest = (HttpWebRequest) WebRequest.Create(restURL.ToString());
                restRequest.Method = GetRequestMethod(requestMethod);
                restRequest.Timeout = System.Threading.Timeout.Infinite;
                restRequest.ContentType = "application/json";

                #region For Offshore Connectivity
                string webProxy = AppCommon.GetAppSettingValue("WebProxyUrl");
                if (webProxy != null && !webProxy.Equals(""))
                {
                    WebProxy proxy = new WebProxy {UseDefaultCredentials = true};
                    Uri proxyurl = new Uri(webProxy);
                    proxy.Address = proxyurl;
                    restRequest.Proxy = proxy;
                    restRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                }
                #endregion

                if (postdata != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(postdata);
                    restRequest.ContentLength = bytes.Length;
                    using (Stream ps = restRequest.GetRequestStream())
                    {
                        ps.Write(bytes, 0, bytes.Length);
                    }
                }
                // restRequest.Credentials = new NetworkCredential("admin", "Frinov25");
                string aptoLogin = AppCommon.GetAppSettingValue("AptoUserId");
                string aptoPassword = AppCommon.GetAppSettingValue("AptoPassword");
                restRequest.Credentials = new NetworkCredential(aptoLogin, aptoPassword);
                
                HttpWebResponse restResponse = (HttpWebResponse) restRequest.GetResponse();
                ExceptionManager.Info(RestApiRequestString(url,GetRequestMethod(requestMethod),postdata));
                string result;
                using (StreamReader reader = new StreamReader(restResponse.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
                return result;
            }
            catch (WebException webEx)
            {
                StringBuilder errorString = new StringBuilder();
                if (webEx.Status == WebExceptionStatus.ProtocolError)
                {
                    errorString.AppendFormat("Status Code : {0}", ((HttpWebResponse) webEx.Response).StatusCode);
                    errorString.AppendFormat("Status Description : {0}",
                                             ((HttpWebResponse) webEx.Response).StatusDescription);
                }
                ExceptionManager.Error(RestApiRequestString(url,GetRequestMethod(requestMethod),postdata),webEx);
            }
            return "";
        }
        private static string RestApiRequestString(string url,string action,string postData)
        {
            return "Url=" + url + ";Action=" + action + ";" + ";Data =" + postData + ";User Info=" + AppCommon.GetCookieValue("DROPBOXLINK");
        }
        private static string GetRequestMethod(RestfulMethods methodType)
        {
            string method;
            switch (methodType)
            {
                case RestfulMethods.Delete:
                    method = "DELETE";
                    break;
                case RestfulMethods.Post:
                    method = "POST";
                    break;
                case RestfulMethods.Put:
                    method = "PUT";
                    break;
                default:
                    method = "GET";
                    break;
            }
            return method;
        }
    }
}
