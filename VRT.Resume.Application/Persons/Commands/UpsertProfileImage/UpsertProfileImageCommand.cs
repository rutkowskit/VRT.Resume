using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.UpsertProfileImage
{
    public sealed class UpsertProfileImageCommand : IRequest<Result>
    {
        public byte[] ImageData { get; set; }
        public string ImageType { get; set; }

        internal sealed class UpsertProfileImageCommandHandler : UpsertHandlerBase<UpsertProfileImageCommand, PersonImage>        
        {
            public UpsertProfileImageCommandHandler(AppDbContext context, 
                ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            
            protected override Result<PersonImage> UpdateData(PersonImage current, UpsertProfileImageCommand request)
            {
                var scaledImg = request.ImageData.ScaleImage(300);
                if(request.ImageData != null && scaledImg.Length>0 && scaledImg.Length < request.ImageData.Length)
                {
                    current.ImageData = scaledImg;
                    current.ImageType = "image/jpeg";
                }
                else
                {
                    current.ImageData = request.ImageData;
                    current.ImageType = request.ImageType;
                }                
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
}
