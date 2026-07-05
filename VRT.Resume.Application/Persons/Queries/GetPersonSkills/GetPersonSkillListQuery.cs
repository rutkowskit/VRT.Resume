using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetPersonSkills
{

    public sealed class GetPersonSkillListQuery : IRequest<Result<PersonSkillInListVM[]>>
    {
        internal sealed class GetPersonSkillListQueryHandler : HandlerBase, 
            IRequestHandler<GetPersonSkillListQuery, Result<PersonSkillInListVM[]>>
        { 
            public GetPersonSkillListQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            public async Task<Result<PersonSkillInListVM[]>> Handle(GetPersonSkillListQuery request, CancellationToken cancellationToken)
            {
                var personIdResult = await GetCurrentUserPersonIdAsync(cancellationToken);
                if (personIdResult.IsFailure)
                    return Result.Failure<PersonSkillInListVM[]>(personIdResult.Error);

                var query = from per in Context.PersonSkill.AsNoTracking()
                            where per.PersonId == personIdResult.Value
                            select new PersonSkillInListVM()
                            {
                                SkillId = per.SkillId,
                                Type = per.SkillType.Name,
                                Name = per.Name,
                                Level = per.Level                                        
                            };
                return await query.ToArrayAsync(cancellationToken);
            }
        }
    }
}