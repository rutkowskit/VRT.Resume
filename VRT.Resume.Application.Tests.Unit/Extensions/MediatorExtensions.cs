using Autofac;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;
using VRT.Resume.Application.Fakes;
using VRT.Resume.Domain.Entities;
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
            context.UserPerson.Add(new UserPerson()
            {
                UserId = Defaults.UserId,
                PersonId = Defaults.PersonId,
                Person = new Person()
                {
                    PersonId = Defaults.PersonId,
                    FirstName = "Tom",
                    LastName = "Tester",
                    ModifiedDate = new DateTime(2020, 11, 20)
                }
            });

            var skillTypes = Enum.GetNames(typeof(SkillTypes))
                .Select(t => new SkillType()
                {
                    SkillTypeId = (byte)Enum.Parse<SkillTypes>(t),
                    Name = t
                }).ToArray();

            context.SkillType.AddRange(skillTypes);
            await context.SaveChangesAsync();
        }
    }
}
