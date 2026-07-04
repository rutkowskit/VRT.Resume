namespace VRT.Resume.Application.Resumes.Commands.DeletePersonResume
{
    public sealed class DeletePersonResumeCommand : IRequest<Result>
    {
        public int ResumeId { get; }

        public DeletePersonResumeCommand(int resumeId)
        {
            ResumeId = resumeId;
        }
        internal sealed class DeletePersonResumeCommandHandler
            : DeleteHandlerBase<DeletePersonResumeCommand, PersonResume>
        {
            public DeletePersonResumeCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {
            }

            protected override Task<Result<PersonResume>> GetExistingData(DeletePersonResumeCommand request)
            {
                return GetCurrentUserPersonId()
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
