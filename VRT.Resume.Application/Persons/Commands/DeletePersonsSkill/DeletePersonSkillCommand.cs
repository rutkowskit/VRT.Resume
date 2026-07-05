namespace VRT.Resume.Application.Persons.Commands.DeletePersonsSkill
{
    public sealed class DeletePersonSkillCommand : IRequest<Result>
    {
        public DeletePersonSkillCommand(int skillId)
        {
            SkillId = skillId;
        }
        public int SkillId { get; }
        internal sealed class DeletePersonSkillCommandHandler : DeleteHandlerBase<DeletePersonSkillCommand, PersonSkill>

        {
            public DeletePersonSkillCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {
            }

            protected override Task<Result<PersonSkill>> GetExistingData(DeletePersonSkillCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(async m =>
                    {
                        var query = from p in Context.PersonSkill
                                    where p.PersonId == m
                                    where p.SkillId == request.SkillId
                                    select p;
                        var result = await query.FirstOrDefaultAsync();
                        return result ?? Result.Failure<PersonSkill>(Errors.RecordNotFound);
                    });
            }
        }
    }
}
