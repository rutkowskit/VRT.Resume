using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetPersonContacts
{

    public sealed class GetPersonContactListQuery : IRequest<Result<PersonContactInListVM[]>>
    {
        internal sealed class GetPersonContactListQueryHandler : HandlerBase, 
            IRequestHandler<GetPersonContactListQuery, Result<PersonContactInListVM[]>>
        { 
            public GetPersonContactListQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            public async Task<Result<PersonContactInListVM[]>> Handle(GetPersonContactListQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Map(p =>
                    {
                        var query = from per in Context.PersonContact
                                    where per.PersonId == p
                                    select new PersonContactInListVM()
                                    {
                                        Name = per.Name,
                                        ContactId = per.ContactId,                                        
                                        Value = per.Value
                                    };
                        return query.ToArray();
                    });
            }
        }
    }
}
