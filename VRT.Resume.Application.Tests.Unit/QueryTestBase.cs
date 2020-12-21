using Autofac;
using MediatR;
using System.Threading.Tasks;
using VRT.Resume.Persistence.Data;
using Xunit;
using CSharpFunctionalExtensions;

namespace VRT.Resume.Application
{
    public abstract class QueryTestBase<TRequest, TResultModel>
        where TRequest : IRequest<Result<TResultModel>>        
    {        
        protected abstract TRequest CreateSut();

        [Fact]
        public async virtual Task Send_CommandWhenUserDoesNotExistsInDb_ShouldFailWithUnauthorizedMessage()
        {         
            var sut = CreateSut();
            var result = await sut.Send(onBeforeSend: scope =>
            {
                var db = scope.Resolve<AppDbContext>();
                db.UserPerson.RemoveRange(db.UserPerson);
                db.Person.RemoveRange(db.Person);
                db.SaveChanges();
            });                        
            Assert.True(result.IsSuccess, nameof(result.IsFailure));
            Assert.Equal(Errors.UserUnauthorized, result.Error);
        }                
    }
}
