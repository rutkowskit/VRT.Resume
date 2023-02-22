using CSharpFunctionalExtensions;
using MediatR;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.CreatePersonAccount;

public sealed class CreatePersonAccountCommand : IRequest<Result<int>>
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    internal sealed class CreatePersonAccountCommandHandler : IRequestHandler<CreatePersonAccountCommand, Result<int>>
    {
        private readonly AppDbContext _context;
        private readonly IDateTimeService _dateTime;

        public CreatePersonAccountCommandHandler(AppDbContext context, IDateTimeService dateTime)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime)); ;
        }

        public async Task<Result<int>> Handle(CreatePersonAccountCommand request, CancellationToken cancellationToken)
        {
            await Task.Yield();
            return GetExistingPersonId(request.UserId)
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

        private Result<int> GetExistingPersonId(string userId)
        {
            var query = from p in _context.UserPerson
                        where p.UserId == userId
                        select p.PersonId;
            var id = query.FirstOrDefault();
            return id <= 0
                ? Result.Failure<int>(Errors.PersonNotExists)
                : id;
        }

        private Result<UserPerson> InitiateAccout(CreatePersonAccountCommand request)
        {
            var curDate = _dateTime.Now;
            var result = new UserPerson()
            {
                UserId = request.Email ?? request.UserId,
                Person = new Person()
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    ModifiedDate = curDate,
                }
            };
            if (request.Email != null)
            {
                result.Person.PersonContact.Add(
                    new PersonContact()
                    {
                        Name = "Email",
                        Value = request.Email,
                        Url = $"mailto:{request.Email}",
                        ModifiedDate = curDate
                    });
            }
            return result;
        }
    }
}
