using MediatR;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.UpsertProfileImage;

public sealed class UpsertProfileImageCommand : IRequest<Result>
{
    required public byte[] ImageData { get; set; }
    required public string ImageType { get; set; }

    internal sealed class Handler : UpsertHandlerBase<UpsertProfileImageCommand, PersonImage>
    {
        private readonly IProfileImageService _profileImageService;
        public Handler(
            AppDbContext context,
            ICurrentUserService userService,
            IProfileImageService profileImageService)
            : base(context, userService)
        {
            _profileImageService = profileImageService;
        }

        protected override Result<PersonImage> UpdateData(PersonImage current, UpsertProfileImageCommand request)
        {
            var scaledImg = _profileImageService
                .CreateProfileImage(request.ImageData)
                .Tap(img =>
                {
                    current.ImageData = img.ImageBytes;
                    current.ImageType = img.ImageMimeType;
                })
                .TapError(_ =>
                {
                    current.ImageData = request.ImageData;
                    current.ImageType = request.ImageType;
                });


            return current;
        }

        protected override Result<PersonImage> GetExistingData(UpsertProfileImageCommand request)
        {
            return GetCurrentUserPersonId()
               .Bind(m =>
               {
                   var query = from img in Context.PersonImage
                               where img.PersonId == m
                               select img;
                   var result = query.FirstOrDefault();
                   return result ?? Result.Failure<PersonImage>(Errors.ImageNotFound);
               });
        }
    }
}
