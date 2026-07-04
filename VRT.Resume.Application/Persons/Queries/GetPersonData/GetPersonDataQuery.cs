using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetPersonData
{
    public sealed class GetPersonDataQuery : IRequest<Result<PersonDataVM>>
    {
        internal sealed class GetPersonDataQueryHandler : HandlerBase, IRequestHandler<GetPersonDataQuery, Result<PersonDataVM>>
        { 
            public GetPersonDataQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            public async Task<Result<PersonDataVM>> Handle(GetPersonDataQuery request, CancellationToken cancellationToken)
            {
                var personIdResult = await GetCurrentUserPersonIdAsync(cancellationToken);
                if (personIdResult.IsFailure)
                    return Result.Failure<PersonDataVM>(personIdResult.Error);

                var query = from per in Context.Person.AsNoTracking()
                            where per.PersonId == personIdResult.Value
                            select new PersonDataVM()
                            {
                               PersonId = per.PersonId,
                               FirstName = per.FirstName,
                               LastName = per.LastName,
                               DateOfBirth = per.DateOfBirth
                            };
                return await query.FirstOrDefaultAsync(cancellationToken);
            }
        }
    }
}