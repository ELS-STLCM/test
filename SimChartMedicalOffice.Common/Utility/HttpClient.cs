using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Configuration;
using SimChartMedicalOffice.Common.Logging;
using System.Web;
using System.Net.Cache;

namespace SimChartMedicalOffice.Common.Utility
{
    public static class HttpClient
    {
        public enum RestfulMethods
        {
            GET = 1,
            POST = 2,
            PUT = 3,
            DELETE = 4
        }

        public static string Get(string url)
        {
            return MakeRestFulApiCall(url, RestfulMethods.GET, null);
        }

        public static string Post(string url, string jsonData)
        {
            return MakeRestFulApiCall(url, RestfulMethods.POST, jsonData);
        }

        public static string Put(string url, string jsonData)
        {
            return MakeRestFulApiCall(url, RestfulMethods.PUT, jsonData);
        }

        public static string Delete(string url)
        {
            return MakeRestFulApiCall(url, RestfulMethods.DELETE, null);
        }

        private static string MakeRestFulApiCall(string url, RestfulMethods requestMethod, string postdata)
        {
            StringBuilder restURL = new StringBuilder();
            HttpWebRequest restRequest;
            HttpWebResponse restResponse;
            string result = string.Empty;
            try
            {
                restURL.AppendFormat(url);
                restRequest = (HttpWebRequest) WebRequest.Create(restURL.ToString());
                restRequest.Method = GetRequestMethod(requestMethod);
                restRequest.Timeout = System.Threading.Timeout.Infinite;
                restRequest.ContentType = "application/json";

                #region For Offshore Connectivity
                string webProxy = AppCommon.GetAppSettingValue("WebProxyUrl");
                if (webProxy != null && !webProxy.ToString().Equals(""))
                {
                    WebProxy proxy = new WebProxy();
                    proxy.UseDefaultCredentials = true;
                    Uri proxyurl = new Uri(webProxy);
                    proxy.Address = proxyurl;
                    restRequest.Proxy = proxy;
                    restRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                }
                #endregion

                if (postdata != null)
                {
                    byte[] bytes = UTF8Encoding.UTF8.GetBytes(postdata.ToString());
                    restRequest.ContentLength = bytes.Length;
                    using (Stream ps = restRequest.GetRequestStream())
                    {
                        ps.Write(bytes, 0, bytes.Length);
                    }
                }
                string aptoLogin;
                string aptoPassword;
               // restRequest.Credentials = new NetworkCredential("admin", "Frinov25");
                aptoLogin = AppCommon.GetAppSettingValue("AptoUserId");
                aptoPassword = AppCommon.GetAppSettingValue("AptoPassword");
                restRequest.Credentials = new NetworkCredential(aptoLogin, aptoPassword);
                
                restResponse = (HttpWebResponse) restRequest.GetResponse();
                ExceptionManager.Info(RestApiRequestString(url,GetRequestMethod(requestMethod),postdata));
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
            return "Url=" + url + ";Action=" + action + ";" + ";Data =" + postData + ";User Info=" + AppCommon.GetCookieValue("DROPBOXLINK").ToString();
        }
        private static string GetRequestMethod(RestfulMethods methodType)
        {
            string method = "";
            switch (methodType)
            {
                case RestfulMethods.DELETE:
                    method = "DELETE";
                    break;
                case RestfulMethods.POST:
                    method = "POST";
                    break;
                case RestfulMethods.PUT:
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
