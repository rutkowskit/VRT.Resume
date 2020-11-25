using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Linq;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonData
{
    public sealed class UpsertPersonDataCommand : IRequest<Result>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        internal sealed class UpsertPersonDataCommandHandler : UpsertHandlerBase<UpsertPersonDataCommand, Person>
            
        {
            public UpsertPersonDataCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }

            protected override Result<Person> UpdateData(Person current, UpsertPersonDataCommand request)
            {
                current.FirstName = request.FirstName;
                current.LastName = request.LastName;
                current.DateOfBirth = request.DateOfBirth;
                if (current.HasChanges(Context))
                {
                    current.ModifiedDate = DateTime.Now; //TODO: create date service
                }
                return current;
            }

            protected override Result<Person> GetExistingData(UpsertPersonDataCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(m =>
                    {
                        var query = from p in Context.Person
                                    where p.PersonId == m
                                    select p;
                        var result = query.FirstOrDefault();
                        return result ?? Result.Failure<Person>("Person not found");
                    });
            }
        }
    }
}
