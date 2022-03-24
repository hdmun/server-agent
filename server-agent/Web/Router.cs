using ServerAgent.Web.Controller;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ServerAgent.Web
{
    public interface IRouter
    {
        void Register(IController controller);
        int Route(HttpListenerContext context);
    }

    public class Router : IRouter
    {
        private readonly IList<IRoute> routes;

        public Router()
        {
            routes = new List<IRoute>();
        }

        public void Register(IController controller)
        {
            var routeMethods = controller.GetType()
                .GetMethods()
                .Where(mi => mi.GetCustomAttributes(true).Any(attr => attr is RouteAttribute));
            if (!routeMethods.Any())
                return;

            foreach (var method in routeMethods)
            {
                var route = new Route(controller, method);
                if (routes.All(r => !route.Equals(r)))
                    routes.Add(route);
            }
        }

        public int Route(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            var httpMethod = request.HttpMethod;
            var url = request.RawUrl;

            int count = 0;
            foreach (var route in routes.Where(route => route.IsMatch(httpMethod, url)))
            {
                var response = route.Invoke(request, context.Response);
                if (response == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.Close();
                }

                response?.Close();
                count++;
            }

            return count;
        }
    }
}
