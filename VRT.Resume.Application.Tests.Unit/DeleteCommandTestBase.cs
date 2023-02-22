using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentValidation;
using MediatR;
using VRT.Resume.Application.Fixtures;

namespace VRT.Resume.Application;

public abstract class DeleteCommandTestBase<TRequest, TDomainModel> : CommandTestBase<TRequest>
    where TRequest : IRequest<Result>
    where TDomainModel : class
{
    private protected DeleteCommandTestBase(ApplicationFixture fixture) : base(fixture)
    {
    }

    [Fact()]
    public async Task Send_CommandWithInvalidEntityId_ShouldThrowValidationError()
    {
        var sut = CreateSut(0);
        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }
    [Fact()]
    public async Task Send_CommandWithValidEntityId_ShouldDeleteEntity()
    {
        var entity = await SeedEntity();
        GetDbContext().Set<TDomainModel>().Count()
            .Should().Be(1);

        var sut = CreateSut(entity);

        var result = await Send(sut);

        Assert.True(result.IsSuccess, result.GetErrorSafe());
        GetDbContext().Set<TDomainModel>().Count()
            .Should().Be(0);
    }

    [Fact()]
    public async Task Send_CommandWithValidNonExistingEntityId_ShouldFailWithMessage()
    {
        var sut = CreateSut();

        var result = await Send(sut);

        Assert.True(result.IsFailure, "It should fail");
        Assert.Equal(Errors.RecordNotFound, result.Error);
    }
    sealed protected override TRequest CreateSut() => CreateSut(int.MaxValue); //invalid entity;
    protected abstract TRequest CreateSut(int id);
    protected abstract TRequest CreateSut(TDomainModel entity);
    protected abstract Task<TDomainModel> SeedEntity();        
}
