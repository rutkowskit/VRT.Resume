using Microsoft.AspNetCore.Http;
using System;

namespace VRT.Resume.Mvc
{
    public static class HttpRequestExtensions
    {
        public static Uri GetReferer(this HttpRequest request)
            => new Uri(request.Headers["Referer"].ToString());

    }
}
