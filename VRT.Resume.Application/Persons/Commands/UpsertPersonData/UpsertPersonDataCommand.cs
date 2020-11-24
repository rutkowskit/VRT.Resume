using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public sealed class UpsertPersonDataCommandHandler : HandlerBase, IRequestHandler<UpsertPersonDataCommand, Result>
        {
            public UpsertPersonDataCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            
            public async Task<Result> Handle(UpsertPersonDataCommand request, CancellationToken cancellationToken)
            {
                return await GetPersonData()
                    .OnFailureCompensate(() => CreatePersonData(request).Tap(i => Context.Add(i)))
                    .Bind(i => UpdatePersonData(i, request))
                    .Map(i => Context.SaveChangesAsync());                
            }

            private Result<Person> UpdatePersonData(Person person, UpsertPersonDataCommand request)
            {                   
                person.FirstName = request.FirstName;                                
                person.LastName = request.LastName;
                person.DateOfBirth = request.DateOfBirth;                
                if(person.HasChanges(Context))
                {
                    person.ModifiedDate = DateTime.Now; //TODO: create date service
                }
                return person;
            }
            
            private Result<Person> CreatePersonData(UpsertPersonDataCommand request)
            {
                return GetCurrentUserPersonId()
                    .Map(s => new Person());                
            }            
            private Result<Person> GetPersonData()
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
