using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Resumes.Commands.MergeResumeSkills
{
    public sealed class MergeResumeSkillsCommand : IRequest<Result>
    {
        public int ResumeId { get; set; }
        public ResumePersonSkillDto[] ResumeSkills { get; set; }
        internal  sealed class MergeResumeSkillsCommandHandler : HandlerBase, 
            IRequestHandler<MergeResumeSkillsCommand,Result>
        {
            public MergeResumeSkillsCommandHandler(AppDbContext context, 
                ICurrentUserService userService) : base(context, userService)
            {                
            }

            public async Task<Result> Handle(MergeResumeSkillsCommand request, CancellationToken cancellationToken)
            {
                return await GetCurrentUserPersonId()
                    .Bind(m => GetResumeWithSkills(m, request.ResumeId))
                    .Tap(t => MergeResumeSkills(t, request.ResumeSkills))
                    .Tap(t => Context.SaveChangesAsync());                   
            }

            private static void MergeResumeSkills(PersonResume resume, ResumePersonSkillDto[] skills)
            {
                var toAdd = from r in skills ?? System.Array.Empty<ResumePersonSkillDto>()
                            join e in resume.ResumePersonSkill on r.SkillId equals e.SkillId into ge
                            from e in ge.DefaultIfEmpty()                            
                            select (r, e);

                foreach (var (skill, existing) in toAdd)
                {
                    if (existing == null)
                    {
                        if(skill.IsHidden || skill.IsRelevent || skill.Position>1)
                            resume.ResumePersonSkill.Add(Update(new ResumePersonSkill(), skill));
                    }                        
                    else
                        Update(existing, skill);
                }
            }
            private static ResumePersonSkill Update(ResumePersonSkill skill, ResumePersonSkillDto newValue)
            {
                skill.IsHidden = newValue.IsHidden;
                skill.IsRelevent = newValue.IsRelevent;                
                skill.SkillId = newValue.SkillId;
                skill.Position = newValue.Position;
                return skill;
            }
            private Result<PersonResume> GetResumeWithSkills(int personId, int resumeId)
            {
                var result = Context.PersonResume                    
                    .Where(w => w.ResumeId == resumeId)
                    .Where(w => w.PersonId == personId)
                    .Include(i => i.ResumePersonSkill)
                    .FirstOrDefault();

                return result ?? Result.Failure<PersonResume>(Errors.ResumeNotFound);                    
            }
        }
    }
}
