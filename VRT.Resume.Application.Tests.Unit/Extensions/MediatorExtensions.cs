using Autofac;
using MediatR;
using System;
using System.Threading.Tasks;
using VRT.Resume.Application.Fakes;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application
{
    internal static class MediatorRequestExtensions
    {
        public static async Task<TResponse> Send<TResponse>(this IRequest<TResponse> request,
            Action<ILifetimeScope> onBeforeSend = null,
            Action<ILifetimeScope> onAfterSend = null,
            bool seedDbWithDefaults=true)
        {
            using (var scope = FakeIocContainer.Instance.Container.BeginLifetimeScope())
            {
                var mediator = scope.Resolve<IMediator>();
                if(seedDbWithDefaults)
                    await SeedDbContext(scope.Resolve<AppDbContext>());
                onBeforeSend?.Invoke(scope);
                var result = await mediator.Send(request);
                onAfterSend?.Invoke(scope);
                return result;
            }
        }

        private static async Task SeedDbContext(AppDbContext context)
        {
            context.UserPerson.Add(new Domain.Entities.UserPerson()
            {
                UserId = Defaults.UserId,
                PersonId = Defaults.PersonId,
                Person = new Domain.Entities.Person()
                {
                    PersonId = Defaults.PersonId,
                    FirstName = "Tom",
                    LastName = "Tester",
                    ModifiedDate = new DateTime(2020, 11, 20)
                }
            });
            await context.SaveChangesAsync();
        }
    }
}
