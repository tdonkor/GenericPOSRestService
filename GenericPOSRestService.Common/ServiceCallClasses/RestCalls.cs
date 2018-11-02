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

        #region HTTPClient

        /// <summary>
        /// A GET request call
        /// </summary>
        /// <param name="fileName"></param>
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
                //TODO Chnge to Log
                Console.WriteLine($"Error: {ex}");
                Console.WriteLine($"Error: {ex.InnerException}");
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
                //TODO Chnge to Log
                Console.WriteLine($"Error: {ex}");
                Console.WriteLine($"Error: {ex.InnerException}");
            }

            return contents;
        }
        #endregion HTTPClient

        #region Rest sharp methods

        /// <summary>
        /// RestSharp POST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string RESTSharpPOSTRequest(string url, string data)
        {
            string strResponse = string.Empty;

            try
            {
                RestClient restClient = new RestClient("http://localhost:9721");
                RestRequest restRequest = new RestRequest(url, Method.POST);
                restRequest.AddParameter("application/json", data, ParameterType.RequestBody);
                restRequest.RequestFormat = DataFormat.Json;

                //Execute the request
                IRestResponse response = restClient.Execute(restRequest);
                Log.Info(response.Content);
                strResponse = response.Content;
              
               
            }
            catch (Exception ex)
            {
                //TODO Chnge to Log
                Console.WriteLine($"Error: {ex}");
                Console.WriteLine($"Error: {ex.InnerException}");
            }

            return strResponse;

        }

        /// <summary>
        /// RestSharp GET
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string RESTSharpGETRequest(string url)
        {
            string rawResponse = String.Empty;

            //RestClient restClient = new RestClient("http://localhost:9721");
            //RestRequest restRequest = new RestRequest(url, Method.GET);
            //restClient.ClearHandlers();
            //JsonDeserializer jsonDeserializer = new JsonDeserializer();
            //restClient.AddHandler("application/json", jsonDeserializer);
            ////restRequest.RequestFormat = DataFormat.Json;
            ////restRequest.AddHeader("Accept", "application/json");

            ////Execute the request
            //IRestResponse response = restClient.Execute(restRequest);

            //Log.Info(response.Content);

            //return response.Content;

            var client = new RestClient();
            var request = new RestRequest(url);
            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                // Two ways to get the result:
                 rawResponse = response.Content;

            }
            return rawResponse;

        }
        #endregion Rest sharp methods
    }
}
