using VRT.Resume.Domain.Common;

namespace VRT.Resume.Application.Resumes.Queries.GetResume;

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
            var personIdResult = await GetCurrentUserPersonIdAsync(cancellationToken);
            if (personIdResult.IsFailure)
                return Result.Failure<ResumeFullVM>(personIdResult.Error);

            var resumeResult = await GetResumeAsync(request.ResumeId, personIdResult.Value, cancellationToken);
            if (resumeResult.IsFailure)
                return resumeResult;

            var r = resumeResult.Value;
            r.Education = await GetEducationItems(r.PersonId).ToArrayAsync(cancellationToken);
            r.Skills = await GetSkills(r.ResumeId, r.PersonId).ToArrayAsync(cancellationToken);
            r.Contact = await GetContactItems(r.PersonId).ToArrayAsync(cancellationToken);
            r.WorkExperience = await GetWorkExperience(r.ResumeId, r.PersonId).ToArrayAsync(cancellationToken);
            return r;
        }

        private async Task<Result<ResumeFullVM>> GetResumeAsync(int resumeId, int personId, CancellationToken cancellationToken)
        {
            var query = from rd in Context.PersonResume.AsNoTracking()
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
                            ShowProfilePhoto = rd.ShowProfilePhoto ?? true,
                            Contact = Array.Empty<ContactItemDto>(),
                            WorkExperience = Array.Empty<WorkExperienceDto>(),
                            Education = Array.Empty<EducationDto>(),
                            Skills = Array.Empty<SkillDto>()
                        };
            var result = await query.FirstOrDefaultAsync(cancellationToken);
            return result == null
                ? Result.Failure<ResumeFullVM>(Errors.RecordNotFound)
                : result;
        }

        private IQueryable<WorkExperienceDto> GetWorkExperience(int resumeId, int personId)
        {
            var skillsQuery = GetSkills(resumeId, personId);

            return Context.PersonExperience.AsNoTracking()
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
            var query = from pk in Context.PersonSkill.AsNoTracking()
                        join rpk in Context.ResumePersonSkill.AsNoTracking().Where(x => x.ResumeId == resumeId)
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
            return Context.PersonEducation.AsNoTracking()
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
            return Context.PersonContact.AsNoTracking()
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