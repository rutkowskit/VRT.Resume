using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetPersonExperience
{
    public sealed class PersonExperienceVM
    {
        public int ExperienceId { get; internal set; }
        public string Position { get; internal set; }
        public string CompanyName { get; internal set; }
        public string Location { get; internal set; }
        public DateTime FromDate { get; internal set; }
        public DateTime? ToDate { get; internal set; }
        public IEnumerable<PersonExperienceDutyInListDto> Duties { get; set; }
    }

    public sealed class GetPersonExperienceQuery : IRequest<Result<PersonExperienceVM>>
    {
        
        internal sealed class GetPersonExperienceQueryHandler : HandlerBase, 
            IRequestHandler<GetPersonExperienceQuery, Result<PersonExperienceVM>>
        { 
            public GetPersonExperienceQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {
            }
            public async Task<Result<PersonExperienceVM>> Handle(GetPersonExperienceQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Map(p =>
                    {
                        var query = from per in Context.PersonExperience
                                    where per.PersonId == p
                                    select new PersonExperienceVM()
                                    {
                                        ExperienceId = per.ExperienceId,
                                        CompanyName = per.CompanyName,
                                        FromDate = per.FromDate,
                                        ToDate = per.ToDate,
                                        Location = per.Location,
                                        Position = per.Position,
                                        Duties = per.PersonExperienceDuty
                                            .Select(s => new PersonExperienceDutyInListDto()
                                            {
                                                DutyId = s.DutyId,
                                                Name = s.Name
                                            })
                                    };
                        return query.FirstOrDefault();
                    });                
            }
        }
    }
}
