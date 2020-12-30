using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Common;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class GetFullResumeQuery : IRequest<Result<ResumeFullVM>>
    {
        public int ResumeId { get; set; }
        internal sealed class GetFullResumeQueryHandler : HandlerBase,
            IRequestHandler<GetFullResumeQuery, Result<ResumeFullVM>>
        {
            public GetFullResumeQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            public async Task<Result<ResumeFullVM>> Handle(GetFullResumeQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Bind(m => GetResume(request.ResumeId, m))                    
                    .Tap(r =>
                    {
                        r.Education = GetEducationItems(r.PersonId).ToArray();
                        r.Skills = GetSkills(r.ResumeId, r.PersonId).ToArray();
                        r.Contact = GetContactItems(r.PersonId).ToArray();
                        r.WorkExperience = GetWorkExperience(r.ResumeId, r.PersonId).ToArray();
                    });                    
            }

            private Result<ResumeFullVM> GetResume(int resumeId, int personId)
            {
                var query = from rd in Context.PersonResume
                            where rd.PersonId == personId
                            where rd.ResumeId == resumeId
                            select new ResumeFullVM()
                            {
                                ResumeId = rd.ResumeId,
                                PersonId = rd.PersonId,
                                FirstName = rd.Person.FirstName,
                                LastName = rd.Person.LastName,
                                Permission = rd.Permission,
                                Position = rd.Position,
                                Summary = rd.Summary,
                                ShowProfilePhoto = rd.ShowProfilePhoto ?? false
                            };
                var result = query.FirstOrDefault();
                return result == null
                    ? Result.Failure<ResumeFullVM>(Errors.RecordNotFound)
                    : result;
            }

            private IQueryable<WorkExperienceDto> GetWorkExperience(int resumeId, int personId)
            {                
                var skillsQuery = GetSkills(resumeId, personId);

                return Context.PersonExperience
                    .Where(c => c.PersonId == personId)                    
                    .Select(s => new WorkExperienceDto()
                    {
                        CompanyName = s.CompanyName,
                        Location = s.Location,
                        Position = s.Position,
                        FromDate = s.FromDate,
                        ToDate = s.ToDate,
                        WorkActivities = s.PersonExperienceDuty
                            .Select(d => new WorkActivityDto()
                            {
                                Description = d.Name,
                                Skills = (from pds in d.PersonExperienceDutySkill
                                         join sq in skillsQuery on pds.SkillId equals sq.SkillId
                                         select sq).ToArray()                                
                            }).ToArray()
                    });
            }

            private IQueryable<SkillDto> GetSkills(int resumeId, int personId)
            {
                var query = from pk in Context.PersonSkill
                            join rpk in Context.ResumePersonSkill.Where(x => x.ResumeId == resumeId)
                                     on pk.SkillId equals rpk.SkillId into grpk
                            from x in grpk.DefaultIfEmpty()
                            where pk.PersonId == personId
                            select new SkillDto()
                            {
                                SkillId = pk.SkillId,
                                IsRelevant = x == null ? false : x.IsRelevant,
                                IsHidden = x == null ? false : x.IsHidden,
                                Name = pk.Name,
                                Level = pk.Level,
                                Type = (SkillTypes)pk.SkillTypeId,  
                                Position = x == null ? 0 : x.Position
                            };
                return query;
            }

            private IQueryable<EducationDto> GetEducationItems(int personId)
            {
                return Context.PersonEducation
                    .Where(c => c.PersonId == personId)                    
                    .Select(s => new EducationDto()
                    {
                        Degree = s.Degree.Name,
                        Field = s.EducationField.Name,
                        FinalGrade = s.Grade,
                        FromDate = s.FromDate,
                        ToDate = s.ToDate,
                        SchoolName = s.School.Name,
                        Specjalization = s.Specialization,
                        ThesisTitle = s.ThesisTitle
                    });
            }

            private IQueryable<ContactItemDto> GetContactItems(int personId)
            {
                return Context.PersonContact
                    .Where(c => c.PersonId == personId)                    
                    .Select(s => new ContactItemDto()
                    {
                        Icon = s.Icon,
                        Name = s.Name,
                        Url = s.Url,
                        Value = s.Value
                    });
            }
        }
    }
}
