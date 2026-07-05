namespace VRT.Resume.Application.Persons.Commands.UpsertPersonContact
{
    public sealed class UpsertPersonContactCommand : IRequest<Result>
    {
        public int ContactId { get; set; }
        public required string Name { get; set; }
        public required string Value { get; set; }
        public string? Icon { get; set; }
        public string? Url { get; set; }

        internal sealed class UpsertPersonDataCommandHandler : UpsertHandlerBase<UpsertPersonContactCommand, PersonContact>
        {
            public UpsertPersonDataCommandHandler(AppDbContext context,
                ICurrentUserService userService, IDateTimeService dateTimeService)
                : base(context, userService, dateTimeService)
            {
            }
            protected override Task<Result<PersonContact>> UpdateData(PersonContact current, UpsertPersonContactCommand request)
            {
                current.Name = request.Name;
                current.Icon = request.Icon.ToSafeImage();
                current.Value = request.Value;
                current.Url = request.Url;
                if (current.HasChanges(Context))
                {
                    current.ModifiedDate = GetCurrentDate();
                }
                return Task.FromResult(Result.Success(current));
            }

            protected override Task<Result<PersonContact>> GetExistingData(UpsertPersonContactCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(async m =>
                    {
                        var query = from p in Context.PersonContact
                                    where p.PersonId == m
                                    where p.ContactId == request.ContactId
                                    select p;
                        var result = await query.FirstOrDefaultAsync();
                        return result ?? Result.Failure<PersonContact>(Errors.RecordNotFound);
                    });
            }
        }
    }
}