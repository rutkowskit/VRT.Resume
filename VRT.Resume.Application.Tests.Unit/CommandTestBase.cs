using CSharpFunctionalExtensions;
using FluentAssertions;
using MediatR;
using VRT.Resume.Application.Fixtures;

namespace VRT.Resume.Application;

[Collection(CollectionNames.Application)]
public abstract class CommandTestBase<TRequest> : BaseRequestTest
    where TRequest : IRequest<Result>
{
    protected private CommandTestBase(ApplicationFixture fixture) : base(fixture)
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
        var sut = CreateSut();
     
        var users = db.UserPerson.ToArray();
        db.UserPerson.RemoveRange(users);
        db.SaveChanges();

        var result = await Send(sut);
        result.AssertFailure();
        result.Error.Should().Be(Errors.UserUnauthorized);
    }
}
