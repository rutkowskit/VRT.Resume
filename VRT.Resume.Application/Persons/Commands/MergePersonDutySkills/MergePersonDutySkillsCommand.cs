using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.MergePersonDutySkills
{
    public sealed class MergePersonDutySkillsCommand : IRequest<Result>
    {
        public int DutyId { get; set; }
        public PersonExpDutySkillDto[] DutySkills { get; set; }
        internal  sealed class MergeResumeSkillsCommandHandler : HandlerBase, 
            IRequestHandler<MergePersonDutySkillsCommand,Result>
        {
            public MergeResumeSkillsCommandHandler(AppDbContext context, 
                ICurrentUserService userService) : base(context, userService)
            {                
            }

            public async Task<Result> Handle(MergePersonDutySkillsCommand request, CancellationToken cancellationToken)
            {
                return await GetCurrentUserPersonId()
                    .Bind(m => GetDutyWithSkills(m, request.DutyId))
                    .Tap(t => MergeDutySkills(t, request.DutySkills))
                    .Tap(t => Context.SaveChangesAsync());                   
            }

            private static void MergeDutySkills(PersonExperienceDuty duty, PersonExpDutySkillDto[] skills)
            {
                var toAdd = from r in skills ?? System.Array.Empty<PersonExpDutySkillDto>()
                            join e in duty.PersonExperienceDutySkill on r.SkillId equals e.SkillId into ge
                            from e in ge.DefaultIfEmpty()
                            where e == null || !r.IsRelevent
                            select (r, e);

                foreach (var (skill, existing) in toAdd)
                {
                    if (existing == null)
                    {
                        if(skill.IsRelevent)
                            duty.PersonExperienceDutySkill.Add(new PersonExperienceDutySkill() { SkillId =skill.SkillId });
                    }                        
                    else
                        duty.PersonExperienceDutySkill.Remove(existing);
                }
            }
            
            private Result<PersonExperienceDuty> GetDutyWithSkills(int personId, int dutyId)
            {
                var result = Context.PersonExperienceDuty                    
                    .Where(w => w.DutyId == dutyId)
                    .Where(w => w.Experience.PersonId == personId)
                    .Include(i => i.PersonExperienceDutySkill)
                    .FirstOrDefault();

                return result ?? Result.Failure<PersonExperienceDuty>(Errors.ResumeNotFound);                    
            }
        }
    }
}
