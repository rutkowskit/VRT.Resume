namespace VRT.Resume.Application.Persons.Queries.GetProfileImage;

public sealed class GetProfileImageQuery : IRequest<ProfileImageVM?>
{
    internal sealed class GetProfileImageQueryHandler : HandlerBase, IRequestHandler<GetProfileImageQuery, ProfileImageVM?>
    {
        public GetProfileImageQueryHandler(AppDbContext context, ICurrentUserService userService)
            : base(context, userService)
        {
        }
        public async Task<ProfileImageVM?> Handle(GetProfileImageQuery request, CancellationToken cancellationToken)
        {
            var personId = await GetCurrentUserPersonIdAsync(cancellationToken);
            if (personId.IsFailure) return null;

            var query = from img in Context.PersonImage.AsNoTracking()
                        where img.PersonId == personId.Value
                        select new ProfileImageVM()
                        {
                            ImageData = img.ImageData,
                            ImageType = img.ImageType
                        };
            return await query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}