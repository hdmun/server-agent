using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Tests
{
    public static class HttpRequestMock
    {
        public static readonly string BindUrl = "http://localhost:8080/";

        private static readonly CredentialCache CredentialsCache = new CredentialCache
            {
                {
                    new Uri(BindUrl),
                    "NTLM",
                    new NetworkCredential(
                        "authtest",
                        "9C9258ACE0D418FEB30F77BD369A87F64995994C054AD654EB0C5E28582A62C8",
                        Dns.GetHostName()
                        )
                }
            };

        public static T RequestConent<T>(string method, string requestUrl, string json, HttpStatusCode statusCode)
        {
            var handler = new HttpClientHandler()
            {
                Credentials = CredentialsCache
            };
            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(BindUrl);
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));  // ACCEPT 헤더

                var requestMessage = new HttpRequestMessage(new HttpMethod(method), requestUrl)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                var response = client.SendAsync(requestMessage).Result;
                Assert.AreEqual(response.StatusCode, statusCode);

                var responseData = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(responseData);
            }
        }

        public static T RequestNoContent<T>(string method, string requestUrl, HttpStatusCode statusCode)
        {
            var handler = new HttpClientHandler()
            {
                Credentials = CredentialsCache
            };
            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(BindUrl);

                var requestMessage = new HttpRequestMessage(new HttpMethod(method), requestUrl);
                var response = client.SendAsync(requestMessage).Result;
                Assert.AreEqual(response.StatusCode, statusCode);

                var responseData = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(responseData);
            }
        }
    }
}
