using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonExperienceDuty
{
    public sealed class UpsertPersonExperienceDutyCommand : IRequest<Result>
    {
        public int ExperienceId { get; set; }
        public int DutyId { get; set; }
        public string Name { get; set; }

        internal sealed class UpsertPersonExperienceDutyCommandHandler : UpsertHandlerBase<UpsertPersonExperienceDutyCommand, PersonExperienceDuty>

        {
            public UpsertPersonExperienceDutyCommandHandler(AppDbContext context,
                ICurrentUserService userService)
                : base(context, userService)
            {
            }

            protected override Result<PersonExperienceDuty> CreateNewData(UpsertPersonExperienceDutyCommand request)
            {
                return GetPersonExperience(request.ExperienceId)
                    .Map(s => new PersonExperienceDuty()
                    {
                        ExperienceId = s.ExperienceId,
                        Name = request.Name
                    });
            }

            private Result<PersonExperience> GetPersonExperience(int experienceId)
            {
                return GetCurrentUserPersonId()
                    .Bind(s =>
                    {
                        var result = Context.PersonExperience
                            .Where(w => w.PersonId == s)
                            .Where(w => w.ExperienceId == experienceId)
                            .FirstOrDefault();
                        return result ?? Result.Failure<PersonExperience>(Errors.PersonExperienceNotExists);
                    });
            }

            protected override Result<PersonExperienceDuty> UpdateData(PersonExperienceDuty current, UpsertPersonExperienceDutyCommand request)
            {
                current.Name = request.Name;                
                return current;
            }
            protected override Result<PersonExperienceDuty> GetExistingData(UpsertPersonExperienceDutyCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(m =>
                    {
                        var query = from p in Context.PersonExperienceDuty
                                    where p.ExperienceId == request.ExperienceId
                                    where p.DutyId == request.DutyId
                                    select p;
                        var result = query.FirstOrDefault();
                        return result ?? Result.Failure<PersonExperienceDuty>(Errors.RecordNotFound);
                    });
            }
        }
    }
}
