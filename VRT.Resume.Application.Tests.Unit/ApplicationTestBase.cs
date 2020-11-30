using VRT.Resume.Application.Fakes;
using Autofac;
using MediatR;
using System.Threading.Tasks;
using VRT.Resume.Persistence.Data;
using System;

namespace VRT.Resume.Application
{
    public class ApplicationTestBase
    {
        protected const int DefaultPersonId = 1;
        
        protected async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
            Action<ILifetimeScope> onBeforeSend = null,
            Action <ILifetimeScope> onAfterSend=null)
        {
            using (var scope = FakeIocContainer.Instance.Container.BeginLifetimeScope())
            {
                var mediator = scope.Resolve<IMediator>();
                await SeedDbContext(scope.Resolve<AppDbContext>());
                onBeforeSend?.Invoke(scope);
                var result = await mediator.Send(request);
                onAfterSend?.Invoke(scope);
                return result;
            }
        }
        protected virtual async Task SeedDbContext(AppDbContext context)
        {
            context.UserPerson.Add(new Domain.Entities.UserPerson()
            {
                UserId = FakeCurrentUserService.DefaultUserId,
                PersonId = DefaultPersonId,
                Person = new Domain.Entities.Person()
                {
                    PersonId = DefaultPersonId,
                    FirstName = "Tom",
                    LastName = "Tester",
                    ModifiedDate = new DateTime(2020,11,20)                    
                }
            });
            await context.SaveChangesAsync();
        }
    }
}
