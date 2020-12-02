using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Bind(p=> GetPersonDutySkills(p, request.DutyId))
                    .Map(p =>
                    {
                        return new PersonExpDutySkillListVM()
                        {
                            DutyId = request.DutyId,
                            DutySkills = p
                        };
                    });
            }

            private Result<PersonExpDutySkillInListDto[]> GetPersonDutySkills(int personId, int dutyId)
            {
                var query = from rd in Context.PersonSkill
                            join rs in Context.PersonExperienceDutySkill
                                .Where(r=>r.DutyId==dutyId) on rd.SkillId equals rs.SkillId into grs
                            from rs in grs.DefaultIfEmpty()
                            where rd.PersonId == personId
                            orderby rd.SkillTypeId, rd.Name                            
                            select new PersonExpDutySkillInListDto()
                            {
                                SkillId = rd.SkillId,
                                Name = rd.Name,
                                Type = rd.SkillType.Name,
                                IsRelevent = rs!=null
                            };

                return query.ToArray();
            }
        }
    }
}
