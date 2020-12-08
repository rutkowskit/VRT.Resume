using Autofac;
using MediatR;
using System.Threading.Tasks;
using VRT.Resume.Persistence.Data;
using System;
using Xunit;
using CSharpFunctionalExtensions;

namespace VRT.Resume.Application
{
    public abstract class ApplicationTestBase<TRequest>
        where TRequest : IRequest<Result>
    {        

        protected abstract TRequest CreateSut();

        [Fact]
        public async virtual Task Send_CommandWhenUserDoesNotExistsInDb_ShouldFailWithUnauthorizedMessage()
        {         
            var sut = CreateSut();
            var result = await Send(sut, onBeforeSend: scope =>
            {
                var db = scope.Resolve<AppDbContext>();
                db.UserPerson.RemoveRange(db.UserPerson);
                db.Person.RemoveRange(db.Person);
                db.SaveChanges();
            });            
            Assert.True(result.IsFailure, nameof(result.IsFailure));
            Assert.Equal(Errors.UserUnauthorized, result.Error);
        }

        protected async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
            Action<ILifetimeScope> onBeforeSend = null,
            Action <ILifetimeScope> onAfterSend=null)
        {
            return await request.Send(onBeforeSend, onAfterSend);            
        }        
    }
}
