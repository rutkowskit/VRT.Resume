using MediatR;
using CSharpFunctionalExtensions;
using Xunit;
using System.Threading.Tasks;
using FluentValidation;
using Autofac;
using VRT.Resume.Persistence.Data;
using System.Linq;

namespace VRT.Resume.Application
{
    public abstract class DeleteCommandTestBase<TRequest,TDomainModel> : CommandTestBase<TRequest>
        where TRequest : IRequest<Result>
        where TDomainModel : class
    {        

        [Fact()]
        public async Task Send_CommandWithInvalidEntityId_ShouldThrowValidationError()
        {
            var sut = CreateSut(0);
            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }
        [Fact()]
        public async Task Send_CommandWithValidEntityId_ShouldDeleteEntity()
        {
            var sut = CreateSut();

            var result = await sut.Send(
                onBeforeSend: async scope =>
                {
                    await SeedEntity(scope);                    
                    Assert.Single(scope.Resolve<AppDbContext>()
                        .Set<TDomainModel>());
                },
                onAfterSend: scope =>
                {
                    var entities = scope.Resolve<AppDbContext>()
                        .Set<TDomainModel>().Count();
                    Assert.Equal(0, entities);
                });
            Assert.True(result.IsSuccess, result.GetErrorSafe());
        }

        [Fact()]
        public async Task Send_CommandWithValidNonExistingEntityId_ShouldFailWithMessage()
        {
            var sut = CreateSut(646464);

            var result = await sut.Send();

            Assert.True(result.IsFailure, "It should fail");
            Assert.Equal(Errors.RecordNotFound, result.Error);
        }

        protected override TRequest CreateSut() => CreateSut(1);
        protected abstract TRequest CreateSut(int entityId);
        protected abstract Task SeedEntity(ILifetimeScope scope);        
    }
}
