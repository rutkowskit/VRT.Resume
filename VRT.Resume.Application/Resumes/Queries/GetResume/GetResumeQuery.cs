using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class GetResumeQuery : IRequest<ResumeVM>
    {
        public int ResumeId { get; set; }
        public sealed class GetResumeQueryHandler : IRequestHandler<GetResumeQuery, ResumeVM>
        {
            private readonly AppDbContext _context;

            public GetResumeQueryHandler(AppDbContext context)
            {
                _context = context;
            }
            public async Task<ResumeVM> Handle(GetResumeQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();                
                var query = from rd in _context.PersonResume
                            where rd.ResumeId==request.ResumeId
                            select new ResumeVM()
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
                if (null == result) return result;
                result.Contact = GetContactItems(request.ResumeId).ToArray();
                result.Education = GetEducationItems(request.ResumeId).ToArray();
                result.Skills = GetSkills(request.ResumeId).ToArray();
                result.WorkExperience = GetWorkExperience(request.ResumeId).ToArray();
                return result;                
            }

            private IQueryable<WorkExperienceDto> GetWorkExperience(int resumeId)
            {
                return _context.PersonResume
                    .Where(c => c.ResumeId == resumeId)
                    .SelectMany(s => s.Person.PersonExperience)
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
                                Skills = d.PersonExperienceDutySkill
                                .Select(ds => new SkillDto()
                                {
                                    Name = ds.Skill.Name,
                                    Type = (SkillTypes)ds.Skill.SkillTypeId,
                                    Level = ds.Skill.Level,
                                    IsRelevant = true
                                }).ToArray()
                            }).ToArray()
                    });
            }

            private IQueryable<SkillDto> GetSkills(int resumeId)
            {
                return _context.ResumePersonSkill
                    .Where(c => c.ResumeId == resumeId)                    
                    .Select(s => new SkillDto()
                    {
                        IsRelevant = s.IsRelevant,
                        Name = s.Skill.Name,
                        Level = s.Skill.Level,
                        Type = (SkillTypes)s.Skill.SkillTypeId
                    });
            }

            private IQueryable<EducationDto> GetEducationItems(int resumeId)
            {
                return _context.PersonResume
                    .Where(c => c.ResumeId == resumeId)
                    .SelectMany(p=>p.Person.PersonEducation)
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

            private IQueryable<ContactItemDto> GetContactItems(int resumeId)
            {
                return _context.ResumeContact
                    .Where(c => c.ResumeId == resumeId)
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
