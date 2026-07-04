using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetPersonExperience
{

    public sealed class GetPersonExperienceListQuery : IRequest<Result<PersonExperienceInListVM[]>>
    {
        internal sealed class GetPersonSkillListQueryHandler : HandlerBase, 
            IRequestHandler<GetPersonExperienceListQuery, Result<PersonExperienceInListVM[]>>
        { 
            public GetPersonSkillListQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            public async Task<Result<PersonExperienceInListVM[]>> Handle(GetPersonExperienceListQuery request, CancellationToken cancellationToken)
            {
                var personIdResult = await GetCurrentUserPersonIdAsync(cancellationToken);
                if (personIdResult.IsFailure)
                    return Result.Failure<PersonExperienceInListVM[]>(personIdResult.Error);

                var query = from per in Context.PersonExperience.AsNoTracking()
                            where per.PersonId == personIdResult.Value
                            select new PersonExperienceInListVM()
                            {
                                ExperienceId = per.ExperienceId,
                                CompanyName = per.CompanyName,
                                FromDate = per.FromDate,
                                ToDate = per.ToDate,
                                Location = per.Location,
                                Position = per.Position,
                                Duties = per.PersonExperienceDuty
                                    .Select(s => new PersonExperienceDutyInListDto()
                                    {
                                        DutyId = s.DutyId,
                                        Name = s.Name,
                                        Skills = s.PersonExperienceDutySkill.Select(d=>d.Skill.Name)
                                    })
                            };
                return await query.ToArrayAsync(cancellationToken);
            }
        }
    }
}