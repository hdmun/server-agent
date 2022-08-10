using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ServerAgent.Actor.Message
{
    public class HttpContextMessage
    {
        public HttpListenerRequest Request { get; set; }
        public HttpListenerResponse Response { get; set; }

        public string HttpMethod { get => Request.HttpMethod; }
        public string RawUrl { get => Request.RawUrl; }
        public string Url { get => Request.Url.ToString(); }

        public string[] UrlPath { get => RawUrl.Split('/').Skip(1).ToArray(); }

        public void SendStatus(HttpStatusCode statusCode)
        {
            Response.StatusCode = (int)statusCode;
            Response.Close();
        }

        public void SendJson(HttpStatusCode statusCode, object obj)
        {
            Response.StatusCode = (int)statusCode;
            Response.ContentType = "application/json";
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
            Response.OutputStream.Write(buffer, 0, buffer.Length);
            Response.OutputStream.Close();
            Response.Close();
        }

        public string GetRequestBody()
        {
            using (var reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
            {
                return reader.ReadToEnd();
            }
        }

        public T GetRequestBody<T>()
        {
            try
            {
                var body = GetRequestBody();
                return JsonConvert.DeserializeObject<T>(body);
            }
            catch (Exception)
            {
                return default(T);  // is null
            }
        }
    }
}
