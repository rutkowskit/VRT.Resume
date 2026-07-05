namespace VRT.Resume.Application.Persons.Commands.DeletePersonEducation
{
    public sealed class DeletePersonEducationCommand : IRequest<Result>
    {
        public int EducationId { get; }

        public DeletePersonEducationCommand(int educationId)
        {
            EducationId = educationId;
        }
        internal sealed class DeletePersonEducationCommandHandler
            : DeleteHandlerBase<DeletePersonEducationCommand, PersonEducation>
        {
            public DeletePersonEducationCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {
            }

            protected override Task<Result<PersonEducation>> GetExistingData(DeletePersonEducationCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(async m =>
                    {
                        var query = from p in Context.PersonEducation
                                    where p.PersonId == m
                                    where p.EducationId == request.EducationId
                                    select p;
                        var result = await query.FirstOrDefaultAsync();
                        return result ?? Result.Failure<PersonEducation>(Errors.RecordNotFound);
                    });
            }
        }
    }
}
