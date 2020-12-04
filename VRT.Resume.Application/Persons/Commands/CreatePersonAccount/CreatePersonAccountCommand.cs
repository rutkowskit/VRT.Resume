using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.CreatePersonAccount
{
    public sealed class CreatePersonAccountCommand : IRequest<Result<int>>
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }       

        public sealed class CreatePersonAccountCommandHandler : IRequestHandler<CreatePersonAccountCommand, Result<int>>
        {
            private readonly AppDbContext _context;

            public CreatePersonAccountCommandHandler(AppDbContext context)                
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }
            
            public async Task<Result<int>> Handle(CreatePersonAccountCommand request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return GetExistingPersonId(request.Email)
                    .OnFailureCompensate(() =>
                    {
                        return InitiateAccout(request)
                            .Tap(i => _context.Add(i))
                            .Map(p =>
                            {
                                 _context.SaveChanges();
                                return p.PersonId;
                            });
                    });
                    
            }

            private Result<int> GetExistingPersonId(string email)
            {
                var query = from p in _context.UserPerson
                            where p.UserId == email
                            select p.PersonId;
                var id = query.FirstOrDefault();
                return id <= 0
                    ? Result.Failure<int>(Errors.PersonNotExists)
                    : id;
            }
            
            private static Result<UserPerson> InitiateAccout(CreatePersonAccountCommand request)
            {
                var curDate = DateTime.UtcNow;
                var result = new UserPerson()
                {
                    UserId = request.Email,
                    Person = new Person()
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        ModifiedDate = curDate,
                        PersonContact = new[]
                        {
                            new PersonContact()
                            {
                                Name = "Email",
                                Value = request.Email,
                                Url = $"mailto:{request.Email}",                                
                                ModifiedDate = curDate
                            }
                        }                        
                    }
                };
                return result;
            }            
        }
    }
}
