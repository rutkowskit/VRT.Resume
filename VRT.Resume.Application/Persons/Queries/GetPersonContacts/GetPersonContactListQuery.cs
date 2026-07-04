using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
                var personIdResult = await GetCurrentUserPersonIdAsync(cancellationToken);
                if (personIdResult.IsFailure)
                    return Result.Failure<PersonContactInListVM[]>(personIdResult.Error);

                var query = from per in Context.PersonContact.AsNoTracking()
                            where per.PersonId == personIdResult.Value
                            select new PersonContactInListVM()
                            {
                                Name = per.Name,
                                ContactId = per.ContactId,                                        
                                Value = per.Value
                            };
                return await query.ToArrayAsync(cancellationToken);
            }
        }
    }
}