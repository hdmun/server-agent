using System;

namespace ServerAgent.ActorLite.Http.Route
{
    public class HttpRouteAttribute : Attribute
    {
        public string Method { get; private set; }
        public string Path { get; private set; }

        public HttpRouteAttribute(string method, string path)
        {
            Method = method;
            Path = path;
        }
    }

    public class HttpGetAttribute : HttpRouteAttribute
    {
        public HttpGetAttribute(string url)
            : base("GET", url)
        {
        }
    }

    public class HttpPatchAttribute : HttpRouteAttribute
    {
        public HttpPatchAttribute(string url)
            : base("PATCH", url)
        {
        }
    }

    public class HttpPutAttribute : HttpRouteAttribute
    {
        public HttpPutAttribute(string url)
            : base("PUT", url)
        {
        }
    }

    public class HttpDeleteAttribute : HttpRouteAttribute
    {
        public HttpDeleteAttribute(string url)
            : base("DELETE", url)
        {
        }
    }
}
