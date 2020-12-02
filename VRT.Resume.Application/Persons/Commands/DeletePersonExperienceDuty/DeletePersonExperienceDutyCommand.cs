using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
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

            protected override Result<PersonExperienceDuty> GetExistingData(DeletePersonExperienceDutyCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(m =>
                    {
                        var query = from p in Context.PersonExperienceDuty
                                    where p.Experience.PersonId == m
                                    where p.DutyId == request.DutyId
                                    select p;
                        var result = query.FirstOrDefault();
                        return result ?? Result.Failure<PersonExperienceDuty>(Errors.RecordNotFound);
                    });
            }
        }
    }
}
