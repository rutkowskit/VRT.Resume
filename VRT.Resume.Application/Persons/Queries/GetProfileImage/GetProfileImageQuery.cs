using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetProfileImage
{
    public sealed class GetProfileImageQuery : IRequest<ProfileImageVM>
    {
        public sealed class GetProfileImageQueryHandler : HandlerBase, IRequestHandler<GetProfileImageQuery, ProfileImageVM>
        {
            public GetProfileImageQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            public async Task<ProfileImageVM> Handle(GetProfileImageQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                var personId = GetCurrentUserPersonId();
                if (personId.IsFailure) return null;

                var query = from img in Context.PersonImage
                            where img.PersonId == personId.Value
                            select new ProfileImageVM()
                            {
                                ImageData = img.ImageData,
                                ImageType = img.ImageType
                            };
                return query.FirstOrDefault();
            }
        }
    }
}
