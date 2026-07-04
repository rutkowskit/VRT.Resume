namespace VRT.Resume.Application.Resumes.Commands.ClonePersonResume;

public sealed class ClonePersonResumeCommand : IRequest<Result>
{
    public int ResumeId { get; set; }

    internal sealed class ClonePersonResumeCommandHandler : HandlerBase,
        IRequestHandler<ClonePersonResumeCommand, Result>
    {
        private readonly IDateTimeService _dateTimeService;

        public ClonePersonResumeCommandHandler(AppDbContext context,
            ICurrentUserService userService, IDateTimeService dateTimeService)
            : base(context, userService)
        {
            _dateTimeService = dateTimeService;
        }

        public async Task<Result> Handle(ClonePersonResumeCommand request, CancellationToken cancellationToken)
        {
            return await GetExistingData(request)
                .Bind(CloneResume)
                .Map(Context.PersonResume.Add)
                .Tap(() => Context.SaveChangesAsync());
        }

        private Result<PersonResume> CloneResume(PersonResume resumeToClone)
        {
            var r = resumeToClone;

            var toAdd = new PersonResume()
            {
                Description = r.Description,
                ModifiedDate = _dateTimeService.Now,
                Permission = r.Permission,
                PersonId = r.PersonId,
                Position = r.Position,
                ShowProfilePhoto = r.ShowProfilePhoto,
                Summary = r.Summary
            };
            resumeToClone
                .ResumePersonSkill
                .Select(s => new ResumePersonSkill()
                {
                    IsHidden = s.IsHidden,
                    IsRelevant = s.IsRelevant,
                    Position = s.Position,
                    SkillId = s.SkillId
                })
                .ToList()
                .ForEach(toAdd.ResumePersonSkill.Add);
            return toAdd;
        }

        private Task<Result<PersonResume>> GetExistingData(ClonePersonResumeCommand request)
        {
            return GetCurrentUserPersonId()
                .Bind(async m =>
                {
                    var query = from p in Context.PersonResume
                                where p.PersonId == m
                                where p.ResumeId == request.ResumeId
                                select p;
                    var result = await query
                        .Include(f => f.ResumePersonSkill)
                        .FirstOrDefaultAsync();
                    return result ?? Result.Failure<PersonResume>(Errors.RecordNotFound);
                });
        }
    }
}
