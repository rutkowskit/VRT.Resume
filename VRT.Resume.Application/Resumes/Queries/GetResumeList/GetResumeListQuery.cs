using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
                await Task.Yield();
                var result = GetCurrentUserPersonId()
                    .Bind(GetResumes);

                return result.IsSuccess
                    ? result.Value
                    : new ResumeInListVM[0];
            }

            private Result<IEnumerable<ResumeInListVM>> GetResumes(int personId)
            {
                var query = from rd in Context.PersonResume
                            where rd.PersonId == personId
                            select new ResumeInListVM()
                            {
                                ResumeId = rd.ResumeId,
                                Position = rd.Position,
                                ModifiedDate = rd.ModifiedDate
                            };

                return query.ToList();
            }
        }
    }
}
