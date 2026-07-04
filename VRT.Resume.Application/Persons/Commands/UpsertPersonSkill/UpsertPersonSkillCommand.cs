using VRT.Resume.Domain.Common;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonSkill
{
    public sealed class UpsertPersonSkillCommand : IRequest<Result>
    {
        public int SkillId { get; set; }
        public SkillTypes SkillType { get; set; }
        public string SkillName { get; set; }
        public string SkillLevel { get; set; }

        internal sealed class UpsertPersonDataCommandHandler : UpsertHandlerBase<UpsertPersonSkillCommand, PersonSkill>
        {
            public UpsertPersonDataCommandHandler(AppDbContext context,
                ICurrentUserService userService, IDateTimeService dateTimeService)
                : base(context, userService, dateTimeService)
            {
            }

            protected override Task<Result<PersonSkill>> UpdateData(PersonSkill current, UpsertPersonSkillCommand request)
            {
                current.Level = request.SkillLevel;
                current.Name = request.SkillName;
                current.SkillTypeId = (byte)request.SkillType;
                return Task.FromResult(Result.Success(current));
            }

            protected override Task<Result<PersonSkill>> GetExistingData(UpsertPersonSkillCommand request)
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
