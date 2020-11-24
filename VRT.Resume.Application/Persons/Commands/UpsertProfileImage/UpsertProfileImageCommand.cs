using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.UpsertProfileImage
{
    public sealed class UpsertProfileImageCommand : IRequest
    {
        public byte[] ImageData { get; set; }
        public string ImageType { get; set; }

        public sealed class UpsertProfileImageCommandHandler : HandlerBase, IRequestHandler<UpsertProfileImageCommand>
        {
            public UpsertProfileImageCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            
            public async Task<Unit> Handle(UpsertProfileImageCommand request, CancellationToken cancellationToken)
            {
                await GetCurrentProfileImage()
                    .OnFailureCompensate(() => CreateImage(request).Tap(i => Context.Add(i)))
                    .Bind(i => UpdateImage(i, request))
                    .Map(i => Context.SaveChangesAsync());
                
                return Unit.Value;
            }

            private Result<PersonImage> UpdateImage(PersonImage image, UpsertProfileImageCommand request)
            {
                image.ImageData = request.ImageData;
                image.ImageType = request.ImageType;
                return image;
            }

            private Result<PersonImage> CreateImage(UpsertProfileImageCommand request)
            {
                return GetCurrentUserPersonId()
                    .Map(s => new PersonImage() { PersonId = s});                
            }            
            private Result<PersonImage> GetCurrentProfileImage()
            {
                return GetCurrentUserPersonId()
                    .Bind(m =>
                    {
                        var query = from img in Context.PersonImage
                                    where img.PersonId == m
                                    select img;
                        var result = query.FirstOrDefault();
                        return result ?? Result.Failure<PersonImage>("Image not found");                        
                    });                
            }
        }
    }
}
