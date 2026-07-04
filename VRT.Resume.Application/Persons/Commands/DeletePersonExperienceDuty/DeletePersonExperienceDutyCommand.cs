using MediatR;
using Microsoft.EntityFrameworkCore;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonExperienceDuty
{
    public sealed class DeletePersonExperienceDutyCommand : IRequest<Result>
    {
        public int DutyId { get; }

        public DeletePersonExperienceDutyCommand(int dutyId)
        {
            DutyId = dutyId;
        }
        internal sealed class DeletePersonExperienceDutyCommandHandler
            : DeleteHandlerBase<DeletePersonExperienceDutyCommand, PersonExperienceDuty>
        {
            public DeletePersonExperienceDutyCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {
            }

            protected override async Task<Result<PersonExperienceDuty>> GetExistingData(DeletePersonExperienceDutyCommand request)
            {
                return await GetCurrentUserPersonId()
                    .Bind(async m =>
                    {
                        var query = from p in Context.PersonExperienceDuty
                                    where p.Experience.PersonId == m
                                    where p.DutyId == request.DutyId
                                    select p;
                        var result = await query.FirstOrDefaultAsync();
                        return result ?? Result.Failure<PersonExperienceDuty>(Errors.RecordNotFound);
                    });
            }
        }
    }
}
