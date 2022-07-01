using ServerAgent.Web.Controller;
using System.Net;
using System.Reflection;

namespace ServerAgent.Web
{
    public interface IRoute
    {
        bool IsMatch(string httpMethod, string url);
        HttpListenerResponse Invoke(HttpListenerRequest request, HttpListenerResponse response);
    }

    public class Route : IRoute
    {
        private ControllerBase Controller { get; set; }
        private MethodInfo MethodInfo { get; set; }
        private RouteAttribute Attribute { get; set; }

        public Route(ControllerBase controller, MethodInfo methodInfo)
        {
            Controller = controller;
            MethodInfo = methodInfo;

            Attribute = methodInfo.GetCustomAttribute<RouteAttribute>(true);
        }

        public bool IsMatch(string httpMethod, string url) => Attribute.IsMatch(httpMethod, url);

        public HttpListenerResponse Invoke(HttpListenerRequest request, HttpListenerResponse response)
        {
            return MethodInfo.Invoke(Controller, new object[] { request, response }) as HttpListenerResponse;
        }
    }
}
