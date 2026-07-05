namespace VRT.Resume.Application.Persons.Queries.GetPersonExperience
{
    public sealed class GetPersonExperienceQuery : IRequest<Result<PersonExperienceVM>>
    {
        public int ExperienceId { get; set; }
        internal sealed class GetPersonExperienceQueryHandler : HandlerBase,
            IRequestHandler<GetPersonExperienceQuery, Result<PersonExperienceVM>>
        {
            public GetPersonExperienceQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {
            }
            public async Task<Result<PersonExperienceVM>> Handle(GetPersonExperienceQuery request, CancellationToken cancellationToken)
            {
                var personIdResult = await GetCurrentUserPersonIdAsync(cancellationToken);
                if (personIdResult.IsFailure)
                    return Result.Failure<PersonExperienceVM>(personIdResult.Error);

                var query = from per in Context.PersonExperience.AsNoTracking()
                            where per.PersonId == personIdResult.Value
                            where per.ExperienceId == request.ExperienceId
                            select new PersonExperienceVM()
                            {
                                ExperienceId = per.ExperienceId,
                                CompanyName = per.CompanyName,
                                FromDate = per.FromDate,
                                ToDate = per.ToDate,
                                Location = per.Location,
                                Position = per.Position
                            };
                return await query.FirstOrDefaultAsync(cancellationToken)
                    ?? Result.Failure<PersonExperienceVM>(Errors.RecordNotFound);
            }
        }
    }
}