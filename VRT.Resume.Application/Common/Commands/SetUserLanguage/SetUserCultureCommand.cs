using CSharpFunctionalExtensions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Application.Common.Commands.SetUserLanguage
{
    public sealed class SetUserCultureCommand : IRequest<Result>
    {
        public SetUserCultureCommand(string newCulture)
        {
            NewCulture = newCulture;
        }
        public string NewCulture { get; }

        internal sealed class SetUserCultureCommandHandler : IRequestHandler<SetUserCultureCommand, Result>
        {
            private readonly ICultureService _culture;

            public SetUserCultureCommandHandler(ICultureService culture)
            {
                _culture = culture;
            }
            public Task<Result> Handle(SetUserCultureCommand request, CancellationToken cancellationToken)
            {
                _culture.SetCurrentCulture(request.NewCulture);
                return Task.FromResult(Result.Success());
            }
        }
    }
}
