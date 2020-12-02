using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetPersonExperienceDuty
{
    public sealed class GetPersonExperienceDutyQuery : IRequest<Result<PersonExperienceDutyVM>>
    {
        public int DutyId { get; set; }
        internal sealed class GetPersonExperienceQueryHandler : HandlerBase, 
            IRequestHandler<GetPersonExperienceDutyQuery, Result<PersonExperienceDutyVM>>
        { 
            public GetPersonExperienceQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {
            }
            public async Task<Result<PersonExperienceDutyVM>> Handle(GetPersonExperienceDutyQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Map(p =>
                    {
                        var query = from per in Context.PersonExperienceDuty
                                    where per.Experience.PersonId == p
                                    where per.DutyId== request.DutyId
                                    select new PersonExperienceDutyVM()
                                    {
                                        DutyId = per.DutyId,
                                        Name = per.Name,
                                        ExperienceId = per.ExperienceId
                                    };
                        return query.FirstOrDefault();
                    });                
            }
        }
    }
}
