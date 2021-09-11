using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.UpdatePersonData
{
    public sealed class UpdatePersonDataCommand : IRequest<Result>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        internal sealed class UpdatePersonDataCommandHandler 
            : HandlerBase, IRequestHandler<UpdatePersonDataCommand, Result>

        {
            private readonly IDateTimeService _dateTimeService;

            public UpdatePersonDataCommandHandler(AppDbContext context, 
                ICurrentUserService userService, IDateTimeService dateTimeService)
                : base(context, userService)
            {
                _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            }

            public async Task<Result> Handle(UpdatePersonDataCommand request, CancellationToken cancellationToken)
            {
                return await GetExistingData(request)                
                    .Bind(i => UpdateData(i, request))
                    .Map(i => Context.SaveChangesAsync());
            }
            private Result<Person> UpdateData(Person current, UpdatePersonDataCommand request)
            {
                current.FirstName = request.FirstName;
                current.LastName = request.LastName;
                current.DateOfBirth = request.DateOfBirth;                
                if (current.HasChanges(Context))
                {
                    current.ModifiedDate = GetCurrentDate();
                }
                return current;
            }

            private Result<Person> GetExistingData(UpdatePersonDataCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(m =>
                    {
                        var query = from p in Context.Person
                                    where p.PersonId == m
                                    select p;
                        var result = query.FirstOrDefault();
                        return result ?? Result.Failure<Person>(Errors.PersonNotExists);
                    });
            }
            private DateTime GetCurrentDate() => _dateTimeService.Now;            
        }
    }
}
