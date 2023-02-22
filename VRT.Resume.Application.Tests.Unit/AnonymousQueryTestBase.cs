using CSharpFunctionalExtensions;
using MediatR;
using VRT.Resume.Application.Fixtures;

namespace VRT.Resume.Application;

[Collection(CollectionNames.Application)]
public abstract class AnonymousQueryTestBase<TRequest, TResultModel> : BaseRequestTest
    where TRequest : IRequest<Result<TResultModel>>
{
    protected AnonymousQueryTestBase(ApplicationFixture fixture) : base(fixture)
    {
        _ = fixture.Services;
        ResetState(fixture);
    }
    protected void ResetState(ApplicationFixture fixture)
    {
        fixture.ResetState().ConfigureAwait(false).GetAwaiter().GetResult();
        GetDbContext().SeedDbContext();
    }

    protected abstract TRequest CreateSut();
    
}
