using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonContact
{
    public sealed class UpsertPersonContactCommand : IRequest<Result>
    {
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }

        internal  sealed class UpsertPersonDataCommandHandler : UpsertHandlerBase<UpsertPersonContactCommand, PersonContact>            
        {
            public UpsertPersonDataCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }

            protected override Result<PersonContact> UpdateData(PersonContact current, UpsertPersonContactCommand request)
            {                
                current.Name = request.Name;
                current.Icon = request.Icon;
                current.Value = request.Value;
                current.Url = request.Url;
                if(current.HasChanges(Context))
                {
                    current.ModifiedDate = GetCurrentDate();
                }
                return current;
            }
                        
            protected override Result<PersonContact> GetExistingData(UpsertPersonContactCommand request)
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
