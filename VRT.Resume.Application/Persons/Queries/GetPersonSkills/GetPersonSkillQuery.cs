using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Map(p =>
                    {
                        var query = from per in Context.PersonSkill
                                    where per.PersonId == p
                                    where per.SkillId == request.SkillId
                                    select new PersonSkillVM()
                                    {
                                        SkillId = per.SkillId,
                                        SkillTypeId = per.SkillTypeId,
                                        Name = per.Name,
                                        Level = per.Level                                        
                                    };
                        return query.FirstOrDefault();
                    });                
            }
        }
    }
}
