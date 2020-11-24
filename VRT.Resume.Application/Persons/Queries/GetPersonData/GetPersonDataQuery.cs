using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetPersonData
{
    public sealed class GetPersonDataQuery : IRequest<Result<PersonDataVM>>
    {
        public sealed class GetPersonDataQueryHandler : HandlerBase, IRequestHandler<GetPersonDataQuery, Result<PersonDataVM>>
        { 
            public GetPersonDataQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            public async Task<Result<PersonDataVM>> Handle(GetPersonDataQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Map(p =>
                    {
                        var query = from per in Context.Person
                                    where per.PersonId == p
                                    select new PersonDataVM()
                                    {
                                       PersonId = per.PersonId,
                                       FirstName = per.FirstName,
                                       LastName = per.LastName,
                                       DateOfBirth = per.DateOfBirth
                                    };
                        return query.FirstOrDefault();
                    });                
            }
        }
    }
}
