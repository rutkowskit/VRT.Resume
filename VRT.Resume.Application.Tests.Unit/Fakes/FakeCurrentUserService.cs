using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Application.Fakes
{
    internal sealed class FakeCurrentUserService : ICurrentUserService
    {        
        public string UserId => Defaults.UserId;
    }
}
