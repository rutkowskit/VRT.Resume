using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonExperience
{
    public sealed class DeletePersonExperienceCommand : IRequest<Result>
    {
        public int ExperienceId { get; }

        public DeletePersonExperienceCommand(int experienceId)
        {
            ExperienceId = experienceId;
        }
        internal sealed class DeletePersonEducationCommandHandler
            : DeleteHandlerBase<DeletePersonExperienceCommand, PersonExperience>            
        {
            public DeletePersonEducationCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }

            protected override Result<PersonExperience> GetExistingData(DeletePersonExperienceCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(m =>
                    {
                        var query = from p in Context.PersonExperience
                                    where p.PersonId == m
                                    where p.ExperienceId == request.ExperienceId
                                    select p;
                        var result = query.FirstOrDefault();
                        return result ?? Result.Failure<PersonExperience>(Errors.RecordNotFound);
                    });
            }
        }
    }
}
