using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonContact
{
    public sealed class DeletePersonContactCommand : IRequest<Result>
    {
        public DeletePersonContactCommand(int contactId)
        {
            ContactId = contactId;
        }
        public int ContactId { get;}
        internal sealed class DeletePersonContactCommandHandler : DeleteHandlerBase<DeletePersonContactCommand, PersonContact>
            
        {
            public DeletePersonContactCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }

            protected override Result<PersonContact> GetExistingData(DeletePersonContactCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(m =>
                    {
                        var query = from p in Context.PersonContact
                                    where p.PersonId == m
                                    where p.ContactId == request.ContactId
                                    select p;
                        var result = query.FirstOrDefault();
                        return result ?? Result.Failure<PersonContact>(Errors.RecordNotFound);
                    });
            }
        }
    }
}
