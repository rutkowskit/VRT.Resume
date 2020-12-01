using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Resumes.Queries.GetResumeSkillList
{
    public sealed class GetResumeSkillListQuery : IRequest<Result<ResumeSkillListVM>>
    {
        public int ResumeId { get; set; }
        internal sealed class GetResumeSkillListQueryHandler : HandlerBase, 
            IRequestHandler<GetResumeSkillListQuery, Result<ResumeSkillListVM>>
        {
            public GetResumeSkillListQueryHandler(AppDbContext context, ICurrentUserService service)
                : base(context, service)
            {
            }
            public async Task<Result<ResumeSkillListVM>> Handle(GetResumeSkillListQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Bind(p=>GetResumes(p, request.ResumeId))
                    .Map(p =>
                    {
                        return new ResumeSkillListVM()
                        {
                            ResumeId = request.ResumeId,
                            ResumeSkills = p
                        };
                    });
            }

            private Result<ResumeSkillInListDto[]> GetResumes(int personId, int resumeId)
            {
                var query = from rd in Context.PersonSkill
                            join rs in Context.ResumePersonSkill
                                .Where(r=>r.ResumeId==resumeId) on rd.SkillId equals rs.SkillId into grs
                            from rs in grs.DefaultIfEmpty()
                            where rd.PersonId == personId
                            orderby rd.SkillTypeId, rd.Name                            
                            select new ResumeSkillInListDto()
                            {
                                SkillId = rd.SkillId,
                                Name = rd.Name,
                                Type = rd.SkillType.Name,
                                IsRelevent = rs.IsRelevent,
                                IsHidden = rs.IsHidden
                            };

                return query.ToArray();
            }
        }
    }
}
