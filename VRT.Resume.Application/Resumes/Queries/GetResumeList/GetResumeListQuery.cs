using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Resumes.Queries.GetResumeList
{
    public sealed class GetResumeListQuery : IRequest<IEnumerable<ResumeInListVM>>
    {
        public int ResumeId { get; set; }
        internal sealed class GetResumeListQueryHandler : HandlerBase, IRequestHandler<GetResumeListQuery, IEnumerable<ResumeInListVM>>
        {
            public GetResumeListQueryHandler(AppDbContext context, ICurrentUserService service)
                : base(context, service)
            {
            }
            public async Task<IEnumerable<ResumeInListVM>> Handle(GetResumeListQuery request, CancellationToken cancellationToken)
            {
                var personIdResult = await GetCurrentUserPersonIdAsync(cancellationToken);
                if (personIdResult.IsFailure)
                    return new ResumeInListVM[0];

                return await GetResumesAsync(personIdResult.Value, cancellationToken);
            }

            private async Task<IEnumerable<ResumeInListVM>> GetResumesAsync(int personId, CancellationToken cancellationToken)
            {
                var query = from rd in Context.PersonResume.AsNoTracking()
                            where rd.PersonId == personId
                            select new ResumeInListVM()
                            {
                                ResumeId = rd.ResumeId,
                                Position = rd.Position,
                                ModifiedDate = rd.ModifiedDate,
                                Description = rd.Description
                            };

                return await query.ToListAsync(cancellationToken);
            }
        }
    }
}