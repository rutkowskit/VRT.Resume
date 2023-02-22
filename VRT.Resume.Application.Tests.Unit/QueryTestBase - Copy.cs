using CSharpFunctionalExtensions;
using MediatR;
using VRT.Resume.Application.Fixtures;

namespace VRT.Resume.Application;

[Collection(CollectionNames.Application)]
public abstract class QueryTestBase<TRequest, TResultModel> : BaseRequestTest
    where TRequest : IRequest<Result<TResultModel>>
{
    protected QueryTestBase(ApplicationFixture fixture) : base(fixture)
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

    [Fact]
    public async virtual Task Send_CommandWhenUserDoesNotExistsInDb_ShouldFailWithUnauthorizedMessage()
    {
        var db = GetDbContext();
        db.UserPerson.RemoveRange(db.UserPerson);
        db.Person.RemoveRange(db.Person);
        db.SaveChanges();

        var sut = CreateSut();

        var result = await Send(sut);

        Assert.True(result.IsFailure, nameof(result.IsFailure));
        Assert.Equal(Errors.UserUnauthorized, result.Error);
    }
}
