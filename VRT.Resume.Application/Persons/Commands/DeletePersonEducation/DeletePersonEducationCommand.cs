using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonEducation
{
    public sealed class DeletePersonEducationCommand : IRequest<Result>
    {
        public int EducationId { get; }

        public DeletePersonEducationCommand(int educationId)
        {
            EducationId = educationId;
        }
        internal sealed class DeletePersonEducationCommandHandler
            : DeleteHandlerBase<DeletePersonEducationCommand, PersonEducation>            
        {
            public DeletePersonEducationCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }

            protected override Result<PersonEducation> GetExistingData(DeletePersonEducationCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(m =>
                    {
                        var query = from p in Context.PersonEducation
                                    where p.PersonId == m
                                    where p.EducationId == request.EducationId
                                    select p;
                        var result = query.FirstOrDefault();
                        return result ?? Result.Failure<PersonEducation>(Errors.RecordNotFound);
                    });
            }
        }
    }
}
