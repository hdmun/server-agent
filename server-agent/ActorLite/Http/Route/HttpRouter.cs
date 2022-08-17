using ServerAgent.Actor.Message;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ServerAgent.ActorLite.Http.Route
{
    public class HttpRouter
    {
        private readonly List<HttpRoute> _routes = new List<HttpRoute>();

        public HttpRouter()
        {
        }

        public void Register(object controller)
        {
            var routeMethods = controller.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(mi => mi.GetCustomAttributes(true)
                    .Any(attr => attr.GetType().IsSubclassOf(typeof(HttpRouteAttribute))));
            if (!routeMethods.Any())
                return;

            foreach (var method in routeMethods)
            {
                var route = new HttpRoute(controller, method);
                if (_routes.All(r => !route.Equals(r)))
                    _routes.Add(route);
            }
        }

        public bool Route(HttpContextMessage message)
        {
            var httpMethod = message.Request.HttpMethod;
            var url = message.Request.RawUrl;

            foreach (var route in _routes)
            {
                if (!route.Matches(httpMethod, url))
                    continue;

                var matches = route.ParseEndpoint(url);
                if (matches == null)
                    continue;

                if (!route.Invoke(new object[] { message }.Concat(matches.Values.ToArray()).ToArray()))
                    continue;

                return true;
            }

            return false;
        }
    }
}
