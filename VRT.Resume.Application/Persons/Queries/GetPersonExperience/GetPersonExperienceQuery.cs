using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetPersonExperience
{
    public sealed class GetPersonExperienceQuery : IRequest<Result<PersonExperienceVM>>
    {
        public int ExperienceId { get; set; }
        internal sealed class GetPersonExperienceQueryHandler : HandlerBase, 
            IRequestHandler<GetPersonExperienceQuery, Result<PersonExperienceVM>>
        { 
            public GetPersonExperienceQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {
            }
            public async Task<Result<PersonExperienceVM>> Handle(GetPersonExperienceQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Map(p =>
                    {
                        var query = from per in Context.PersonExperience
                                    where per.PersonId == p
                                    where per.ExperienceId == request.ExperienceId
                                    select new PersonExperienceVM()
                                    {
                                        ExperienceId = per.ExperienceId,
                                        CompanyName = per.CompanyName,
                                        FromDate = per.FromDate,
                                        ToDate = per.ToDate,
                                        Location = per.Location,
                                        Position = per.Position                                        
                                    };
                        return query.FirstOrDefault();
                    });                
            }
        }
    }
}
