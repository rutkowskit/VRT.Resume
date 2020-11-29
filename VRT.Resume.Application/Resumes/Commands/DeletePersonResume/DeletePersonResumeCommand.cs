using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Resumes.Commands.DeletePersonResume
{
    public sealed class DeletePersonResumeCommand : IRequest<Result>
    {
        public int ResumeId { get; }

        public DeletePersonResumeCommand(int resumeId)
        {
            ResumeId = resumeId;
        }
        internal sealed class DeletePersonResumeCommandHandler
            : DeleteHandlerBase<DeletePersonResumeCommand, PersonResume>            
        {
            public DeletePersonResumeCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }

            protected override Result<PersonResume> GetExistingData(DeletePersonResumeCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(m =>
                    {
                        var query = from p in Context.PersonResume
                                    where p.PersonId == m
                                    where p.ResumeId == request.ResumeId
                                    select p;
                        var result = query.FirstOrDefault();
                        return result ?? Result.Failure<PersonResume>(Errors.RecordNotFound);
                    });
            }
        }
    }
}
