using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetPersonContacts
{

    public sealed class GetPersonContactQuery : IRequest<Result<PersonContactVM>>
    {
        public int ContactId { get; set; }
        internal sealed class GetPersonContactQueryHandler : HandlerBase, 
            IRequestHandler<GetPersonContactQuery, Result<PersonContactVM>>
        { 
            public GetPersonContactQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            public async Task<Result<PersonContactVM>> Handle(GetPersonContactQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Map(p =>
                    {
                        var query = from per in Context.PersonContact
                                    where per.PersonId == p
                                    where per.ContactId == request.ContactId
                                    select new PersonContactVM()
                                    {
                                        Name = per.Name,
                                        ContactId = per.ContactId,
                                        Icon = per.Icon,
                                        Url = per.Url,
                                        Value = per.Value
                                    };
                        return query.FirstOrDefault();
                    });                
            }
        }
    }
}
