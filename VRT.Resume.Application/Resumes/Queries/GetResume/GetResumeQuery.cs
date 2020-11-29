using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class GetResumeQuery : IRequest<Result<ResumeVM>>
    {
        public int ResumeId { get; set; }
        internal sealed class GetResumeQueryHandler : HandlerBase,
            IRequestHandler<GetResumeQuery, Result<ResumeVM>>
        {
            public GetResumeQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            public async Task<Result<ResumeVM>> Handle(GetResumeQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Map(p =>
                    {
                        var query = from rd in Context.PersonResume
                                    where rd.PersonId==p
                                    where rd.ResumeId == request.ResumeId
                                    select new ResumeVM()
                                    {
                                        ResumeId = rd.ResumeId,
                                        Position = rd.Position,
                                        Summary = rd.Summary,
                                        ShowProfilePhoto = rd.ShowProfilePhoto ?? false,
                                        DataProcessingPermission = rd.Permission,
                                        Description = rd.Description
                                    };
                        return query.FirstOrDefault();
                    });                
            }
        }
    }
}
