using System;
using System.Text.RegularExpressions;

namespace ServerAgent.Web
{
    public class RouteAttribute : Attribute
    {
        public string Method { get; private set; } = "";
        public string Url { get; private set; } = "";

        public RouteAttribute(string method, string url)
        {
            Method = method;
            Url = url;
        }

        public bool IsMatch(string method, string url)
        {
            if (Method != method)
                return false;

            return Url == url || new Regex(Url).IsMatch(url);
        }
    }
}
