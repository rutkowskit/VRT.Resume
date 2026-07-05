namespace VRT.Resume.Application.Persons.Queries.GetPersonContacts;

public sealed class GetPersonContactQuery : IRequest<Result<PersonContactVM>>
{
    public int ContactId { get; set; }
    internal sealed class GetPersonContactQueryHandler : HandlerBase,
        IRequestHandler<GetPersonContactQuery, Result<PersonContactVM>>
    {
        public GetPersonContactQueryHandler(AppDbContext context, ICurrentUserService userService)
            : base(context, userService)
        {
        }
        public async Task<Result<PersonContactVM>> Handle(GetPersonContactQuery request, CancellationToken cancellationToken)
        {
            var personIdResult = await GetCurrentUserPersonIdAsync(cancellationToken);
            if (personIdResult.IsFailure)
                return Result.Failure<PersonContactVM>(personIdResult.Error);

            var query = from per in Context.PersonContact.AsNoTracking()
                        where per.PersonId == personIdResult.Value
                        where per.ContactId == request.ContactId
                        select new PersonContactVM()
                        {
                            Name = per.Name,
                            ContactId = per.ContactId,
                            Icon = per.Icon,
                            Url = per.Url,
                            Value = per.Value
                        };
            var result = await query.FirstOrDefaultAsync(cancellationToken);
            return result ?? Result.Failure<PersonContactVM>(Errors.RecordNotFound);
        }
    }
}