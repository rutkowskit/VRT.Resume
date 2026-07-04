using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetPersonExperienceDutySkillList
{
    public sealed class GetPersonExperienceDutySkillListQuery : IRequest<Result<PersonExpDutySkillListVM>>
    {
        public int DutyId { get; set; }
        internal sealed class GetPersonExperienceDutySkillListQueryHandler : HandlerBase, 
            IRequestHandler<GetPersonExperienceDutySkillListQuery, Result<PersonExpDutySkillListVM>>
        {
            public GetPersonExperienceDutySkillListQueryHandler(AppDbContext context, ICurrentUserService service)
                : base(context, service)
            {
            }
            public async Task<Result<PersonExpDutySkillListVM>> Handle(GetPersonExperienceDutySkillListQuery request, CancellationToken cancellationToken)
            {
                var personIdResult = await GetCurrentUserPersonIdAsync(cancellationToken);
                if (personIdResult.IsFailure)
                    return Result.Failure<PersonExpDutySkillListVM>(personIdResult.Error);

                var skills = await GetPersonDutySkillsAsync(personIdResult.Value, request.DutyId, cancellationToken);
                return new PersonExpDutySkillListVM()
                {
                    DutyId = request.DutyId,
                    DutySkills = skills
                };
            }

            private async Task<PersonExpDutySkillInListDto[]> GetPersonDutySkillsAsync(int personId, int dutyId, CancellationToken cancellationToken)
            {
                var query = from rd in Context.PersonSkill.AsNoTracking()
                            join rs in Context.PersonExperienceDutySkill.AsNoTracking()
                                .Where(r=>r.DutyId==dutyId) on rd.SkillId equals rs.SkillId into grs
                            from rs in grs.DefaultIfEmpty()
                            where rd.PersonId == personId
                            orderby rd.SkillTypeId, rd.Name                            
                            select new PersonExpDutySkillInListDto()
                            {
                                SkillId = rd.SkillId,
                                Name = rd.Name,
                                Type = rd.SkillType.Name,
                                IsRelevant = rs!=null
                            };

                return await query.ToArrayAsync(cancellationToken);
            }
        }
    }
}