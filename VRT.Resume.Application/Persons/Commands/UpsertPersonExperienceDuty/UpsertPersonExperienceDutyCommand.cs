namespace VRT.Resume.Application.Persons.Commands.UpsertPersonExperienceDuty;

public sealed class UpsertPersonExperienceDutyCommand : IRequest<Result>
{
    public int ExperienceId { get; set; }
    public int DutyId { get; set; }
    public string? Name { get; set; }

    internal sealed class UpsertPersonExperienceDutyCommandHandler : UpsertHandlerBase<UpsertPersonExperienceDutyCommand, PersonExperienceDuty>

    {
        public UpsertPersonExperienceDutyCommandHandler(AppDbContext context,
            ICurrentUserService userService)
            : base(context, userService)
        {
        }

        protected override Task<Result<PersonExperienceDuty>> CreateNewData(UpsertPersonExperienceDutyCommand request)
        {
            return GetPersonExperience(request.ExperienceId)
                .Map(s => new PersonExperienceDuty()
                {
                    ExperienceId = s.ExperienceId,
                    Name = request.Name
                });
        }

        private Task<Result<PersonExperience>> GetPersonExperience(int experienceId)
        {
            return GetCurrentUserPersonId()
                .Bind(async s =>
                {
                    var result = await Context.PersonExperience
                        .Where(w => w.PersonId == s)
                        .Where(w => w.ExperienceId == experienceId)
                        .FirstOrDefaultAsync();
                    return result ?? Result.Failure<PersonExperience>(Errors.PersonExperienceNotExists);
                });
        }

        protected override Task<Result<PersonExperienceDuty>> UpdateData(PersonExperienceDuty current, UpsertPersonExperienceDutyCommand request)
        {
            current.Name = request.Name;
            return Task.FromResult(Result.Success(current));
        }
        protected override Task<Result<PersonExperienceDuty>> GetExistingData(UpsertPersonExperienceDutyCommand request)
        {
            return GetCurrentUserPersonId()
                .Bind(async m =>
                {
                    var query = from p in Context.PersonExperienceDuty
                                where p.ExperienceId == request.ExperienceId
                                where p.DutyId == request.DutyId
                                select p;
                    var result = await query.FirstOrDefaultAsync();
                    return result ?? Result.Failure<PersonExperienceDuty>(Errors.RecordNotFound);
                });
        }
    }
}
