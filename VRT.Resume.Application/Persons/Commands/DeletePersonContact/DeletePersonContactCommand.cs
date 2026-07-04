namespace VRT.Resume.Application.Persons.Commands.DeletePersonContact
{
    public sealed class DeletePersonContactCommand : IRequest<Result>
    {
        public DeletePersonContactCommand(int contactId)
        {
            ContactId = contactId;
        }
        public int ContactId { get; }
        internal sealed class DeletePersonContactCommandHandler : DeleteHandlerBase<DeletePersonContactCommand, PersonContact>

        {
            public DeletePersonContactCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {
            }

            protected override Task<Result<PersonContact>> GetExistingData(DeletePersonContactCommand request)
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
