using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Services.Services
{
    public class WebAPIRest
    {
        public async Task<T> RequestGet<T>(string UrlAPI, Dictionary<string, string> headers = null)
        {
            using (var httpClient = new HttpClient())
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                using (var response = await httpClient.GetAsync(UrlAPI))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<T>(apiResponse);
                    return result;
                }
            }
        }
        public async Task<T> RequestPost<T>(string UrlAPI, object jsonData, Dictionary<string, string> headers = null)
        {

            using (var httpClient = new HttpClient())
            {

                var json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
                WriteLog(UrlAPI);
                WriteLog(json);
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                using (var response = await httpClient.PostAsync(UrlAPI, new StringContent(json, Encoding.UTF8, "application/json")))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    WriteLog(apiResponse);
                    var result = JsonConvert.DeserializeObject<T>(apiResponse);
                    return result;
                }
            }
        }
        //public async Task<T> RequestGet<T>(string urlAPI, Dictionary<string, string> headers = null)
        //{
        //    var response = await HttpClient.GetWithHeadersAsync(urlAPI, headers);

        //    response.EnsureSuccessStatusCode();

        //    return await response.Content.ReadAsStringAsync();
        //}

        private void WriteLog(string str)
        {
            str += "\n";
            string strPath = AppDomain.CurrentDomain.BaseDirectory + $"/LogFile/LogFile_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile");
            }
            System.IO.File.AppendAllLines(strPath, new[] { str });
        }
    }
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> GetWithHeadersAsync(this HttpClient httpClient, string requestUri, Dictionary<string, string> headers)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

                return await httpClient.SendAsync(request);
            }
        }
    }
}
