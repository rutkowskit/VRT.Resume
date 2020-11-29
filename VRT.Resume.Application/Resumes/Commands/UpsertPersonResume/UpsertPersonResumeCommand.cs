using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Resumes.Commands.UpsertPersonResume
{
    public sealed class UpsertPersonResumeCommand : IRequest<Result>
    {
        public int ResumeId { get; set; }
        public string Position { get; set; }
        public string Summary { get; set; }
        public bool ShowProfilePhoto { get; set; }
        public string DataProcessingPermission { get; set; }
        public string Description { get; set; }

        internal  sealed class UpsertPersonResumeCommandHandler : UpsertHandlerBase<UpsertPersonResumeCommand, PersonResume>            
        {
            public UpsertPersonResumeCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }

            protected override Result<PersonResume> UpdateData(PersonResume current, UpsertPersonResumeCommand request)
            {
                current.Permission = request.DataProcessingPermission;
                current.Summary = request.Summary;
                current.ShowProfilePhoto = request.ShowProfilePhoto;
                current.Position = request.Position;
                current.Description = request.Description;
                if(current.HasChanges(Context))
                {
                    current.ModifiedDate = GetCurrentDate();
                }
                return current;
            }
                        
            protected override Result<PersonResume> GetExistingData(UpsertPersonResumeCommand request)
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
