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

    public sealed class GetPersonSkillQuery : IRequest<Result<PersonSkillVM>>
    {
        public int SkillId { get; set; }
        internal sealed class GetPersonSkillListQueryHandler : HandlerBase, 
            IRequestHandler<GetPersonSkillQuery, Result<PersonSkillVM>>
        { 
            public GetPersonSkillListQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            public async Task<Result<PersonSkillVM>> Handle(GetPersonSkillQuery request, CancellationToken cancellationToken)
            {
                var personIdResult = await GetCurrentUserPersonIdAsync(cancellationToken);
                if (personIdResult.IsFailure)
                    return Result.Failure<PersonSkillVM>(personIdResult.Error);

                var query = from per in Context.PersonSkill.AsNoTracking()
                            where per.PersonId == personIdResult.Value
                            where per.SkillId == request.SkillId
                            select new PersonSkillVM()
                            {
                                SkillId = per.SkillId,
                                SkillTypeId = per.SkillTypeId,
                                Name = per.Name,
                                Level = per.Level                                        
                            };
                return await query.FirstOrDefaultAsync(cancellationToken);
            }
        }
    }
}