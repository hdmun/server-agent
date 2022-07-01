using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace ServerAgent.Web.Controller
{
    public abstract class ControllerBase
    {
        protected readonly ILog _logger;

        public ControllerBase(Type type)
        {
            _logger = LogManager.GetLogger(type);
        }

        protected string GetBody(HttpListenerRequest request)
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                return reader.ReadToEnd();
            }
        }

        protected T GetBody<T>(HttpListenerRequest request)
        {
            var body = GetBody(request);
            return JsonConvert.DeserializeObject<T>(body);
        }
    }
}
