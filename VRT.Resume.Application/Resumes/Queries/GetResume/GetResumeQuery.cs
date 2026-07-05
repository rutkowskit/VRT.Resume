namespace VRT.Resume.Application.Resumes.Queries.GetResume;

public sealed class GetResumeQuery : IRequest<Result<ResumeVM>>
{
    public int ResumeId { get; set; }
    internal sealed class GetResumeQueryHandler : HandlerBase,
        IRequestHandler<GetResumeQuery, Result<ResumeVM>>
    {
        public GetResumeQueryHandler(AppDbContext context, ICurrentUserService userService)
            : base(context, userService)
        {
        }
        public async Task<Result<ResumeVM>> Handle(GetResumeQuery request, CancellationToken cancellationToken)
        {
            var personIdResult = await GetCurrentUserPersonIdAsync(cancellationToken);
            if (personIdResult.IsFailure)
                return Result.Failure<ResumeVM>(personIdResult.Error);

            var query = from rd in Context.PersonResume.AsNoTracking()
                        where rd.PersonId == personIdResult.Value
                        where rd.ResumeId == request.ResumeId
                        select new ResumeVM()
                        {
                            ResumeId = rd.ResumeId,
                            Position = rd.Position,
                            Summary = rd.Summary,
                            ShowProfilePhoto = rd.ShowProfilePhoto,
                            DataProcessingPermission = rd.Permission,
                            Description = rd.Description
                        };
            var result = await query.FirstOrDefaultAsync(cancellationToken);
            return result ?? Result.Failure<ResumeVM>(Errors.ResumeNotFound);
        }
    }
}