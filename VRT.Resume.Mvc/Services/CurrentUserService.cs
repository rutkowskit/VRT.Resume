using Microsoft.AspNetCore.Http;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Mvc.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContext;

        public CurrentUserService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        public string UserId
            => _httpContext?.HttpContext?.User.AsUserLoginViewModel()?.UserId;
    }
}