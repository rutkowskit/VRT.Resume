using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Application.Common.Queries.GetSupportedLanguages
{
    public sealed class GetSupportedLanguagesQuery : IRequest<Result<LanguageVM[]>>
    {
        internal sealed class GetSupportedLanguagesQueryHandler : IRequestHandler<GetSupportedLanguagesQuery, Result<LanguageVM[]>>
        {
            private readonly ICultureService _cultureService;

            public GetSupportedLanguagesQueryHandler(ICultureService cultureService)
            {
                _cultureService = cultureService ?? throw new ArgumentNullException(nameof(cultureService));
            }
            public async Task<Result<LanguageVM[]>> Handle(GetSupportedLanguagesQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return _cultureService.GetSupportedLanguages().Values
                    .Select(s => new LanguageVM(s.key, s.caption))
                    .ToArray();                    
            }
        }
    }
}
