using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net;
using RestSharp.Deserializers;
using System.Net.Http.Headers;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class RestCalls
    {
        /// <summary>
        ///  A GET method 
        /// </summary>
        /// <param name="urlString"></param>
        /// <returns></returns>
        public string GetAsyncRequest(string urlString)
        {
            string resultStr = string.Empty;
            HttpResponseMessage response;

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(urlString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = client.GetAsync(urlString).Result;
                resultStr = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Log.Error($"Error: {ex}");
                Log.Error($"Error: {ex.InnerException}");
            }

            return resultStr;
        }

        /// <summary>
        /// REST POST Json method
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="url"></param>
        public string PostAsyncRequest(string url, string data)
        {
            HttpResponseMessage response = null;
            StringContent content = null;    
            string contents = string.Empty;
          
            try
            {
               HttpClient httpClient = new HttpClient();
                 
               content = new StringContent(data, Encoding.UTF8, "application/json");
               response = httpClient.PostAsync(url, content).Result;// storing the results of posts in response
               contents = response.Content.ReadAsStringAsync().Result;
               Log.Info(contents);
               
            }
            catch (Exception ex)
            {
                Log.Error($"Error: {ex}");
                Log.Error($"Error: {ex.InnerException}");
            }

            return contents;
        }
       
        
    }
}
