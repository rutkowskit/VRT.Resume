using Microsoft.AspNetCore.Http;
using System;

namespace VRT.Resume.Mvc
{
    public static class HttpRequestExtensions
    {
        public static Uri GetReferer(this HttpRequest request)
            => new Uri(request.Headers["Referer"].ToString());

        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// https://stackoverflow.com/questions/29282190/where-is-request-isajaxrequest-in-asp-net-core-mvc
        /// </summary>        
        /// <returns>
        /// true if the specified HTTP request is an AJAX request; otherwise, false.
        /// </returns>
        /// <param name="request">The HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }


    }
}
