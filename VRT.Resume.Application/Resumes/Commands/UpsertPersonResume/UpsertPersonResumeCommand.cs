namespace VRT.Resume.Application.Resumes.Commands.UpsertPersonResume
{
    public sealed class UpsertPersonResumeCommand : IRequest<Result>
    {
        public int ResumeId { get; set; }
        public required string Position { get; set; }
        public string? Summary { get; set; }
        public bool ShowProfilePhoto { get; set; }
        public string? DataProcessingPermission { get; set; }
        public required string Description { get; set; }

        internal sealed class UpsertPersonResumeCommandHandler : UpsertHandlerBase<UpsertPersonResumeCommand, PersonResume>
        {
            public UpsertPersonResumeCommandHandler(AppDbContext context,
                ICurrentUserService userService, IDateTimeService dateTimeService)
                : base(context, userService, dateTimeService)
            {
            }

            protected override async Task<Result<PersonResume>> UpdateData(PersonResume current, UpsertPersonResumeCommand request)
            {
                await Task.Yield();
                current.Permission = request.DataProcessingPermission;
                current.Summary = request.Summary;
                current.ShowProfilePhoto = request.ShowProfilePhoto;
                current.Position = request.Position;
                current.Description = request.Description;
                if (current.HasChanges(Context))
                {
                    current.ModifiedDate = GetCurrentDate();
                }
                return current;
            }

            protected override async Task<Result<PersonResume>> GetExistingData(UpsertPersonResumeCommand request)
            {
                return await GetCurrentUserPersonId()
                    .Bind(async m =>
                    {
                        var query = from p in Context.PersonResume
                                    where p.PersonId == m
                                    where p.ResumeId == request.ResumeId
                                    select p;
                        var result = await query.FirstOrDefaultAsync();
                        return result ?? Result.Failure<PersonResume>(Errors.RecordNotFound);
                    });
            }
        }
    }
}