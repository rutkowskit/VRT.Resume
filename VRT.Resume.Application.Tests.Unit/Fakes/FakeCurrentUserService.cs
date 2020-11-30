using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Application.Fakes
{
    internal sealed class FakeCurrentUserService : ICurrentUserService
    {
        internal const string DefaultUserId = "tester@testing.me";
        public string UserId => DefaultUserId;
    }
}
