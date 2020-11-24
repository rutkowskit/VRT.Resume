using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetPersonSkills
{

    public sealed class GetPersonSkillListQuery : IRequest<Result<PersonSkillInListVM[]>>
    {
        public sealed class GetPersonSkillListQueryHandler : HandlerBase, 
            IRequestHandler<GetPersonSkillListQuery, Result<PersonSkillInListVM[]>>
        { 
            public GetPersonSkillListQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            public async Task<Result<PersonSkillInListVM[]>> Handle(GetPersonSkillListQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Map(p =>
                    {
                        var query = from per in Context.PersonSkill
                                    where per.PersonId == p
                                    select new PersonSkillInListVM()
                                    {
                                        SkillId = per.SkillId,
                                        Type = per.SkillType.Name,
                                        Name = per.Name,
                                        Level = per.Level                                        
                                    };
                        return query.ToArray();
                    });                
            }
        }
    }
}
