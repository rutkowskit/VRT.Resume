using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
                var personIdResult = await GetCurrentUserPersonIdAsync(cancellationToken);
                if (personIdResult.IsFailure)
                    return Result.Failure<ResumeSkillListVM>(personIdResult.Error);

                var skills = await GetResumesAsync(personIdResult.Value, request.ResumeId, cancellationToken);
                return new ResumeSkillListVM()
                {
                    ResumeId = request.ResumeId,
                    ResumeSkills = skills
                };
            }

            private async Task<ResumeSkillInListDto[]> GetResumesAsync(int personId, int resumeId, CancellationToken cancellationToken)
            {
                var query = from rd in Context.PersonSkill.AsNoTracking()
                            join rs in Context.ResumePersonSkill.AsNoTracking()
                                .Where(r => r.ResumeId == resumeId) on rd.SkillId equals rs.SkillId into grs
                            from rs in grs.DefaultIfEmpty()
                            where rd.PersonId == personId
                            orderby rd.SkillTypeId, rd.Name
                            select new ResumeSkillInListDto()
                            {
                                SkillId = rd.SkillId,
                                Name = rd.Name,
                                Type = rd.SkillType.Name,
                                IsRelevant = rs == null ? false : rs.IsRelevant,
                                IsHidden = rs == null ? false : rs.IsHidden,
                                Position = rs == null ? 0 : rs.Position
                            };

                return await query.ToArrayAsync(cancellationToken);
            }
        }
    }
}