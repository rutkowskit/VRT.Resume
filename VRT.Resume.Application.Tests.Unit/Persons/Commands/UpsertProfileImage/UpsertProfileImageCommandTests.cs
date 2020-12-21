using Autofac;
using FluentValidation;
using System;
using System.Linq;
using System.Threading.Tasks;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;
using Xunit;

namespace VRT.Resume.Application.Persons.Commands.UpsertProfileImage
{
    public class UpsertProfileImageCommandTests : CommandTestBase<UpsertProfileImageCommand>
    {
        [Fact()]
        public async Task Send_CommandWithEmptyImageData_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.ImageData = Array.Empty<byte>();

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }
        
        [Fact()]
        public async Task Send_WhenImageNotExistsIndDb_ShouldAddNewRecordToDbContext()
        {
            var sut = CreateSut();            
            var result = await Send(sut,
                onBeforeSend: scope =>
                {
                    var entity = scope.Resolve<AppDbContext>().PersonImage.FirstOrDefault();
                    Assert.Null(entity);
                },
                onAfterSend: scope =>
            {
                var entity = Assert.Single(scope.Resolve<AppDbContext>().PersonImage);
                AssertPersonImage(entity, sut);                
            });
            
            result.AssertSuccess();            
        }

        [Fact()]
        public async Task Send_CommandWithIdOfExistingSkill_ShouldUpdateExistingRecord()
        {
            var sut = CreateSut();
            
            var result = await Send(sut,
                async scope => await scope.SeedImage(),
                onAfterSend: scope =>
                {
                    var img = Assert.Single(scope.Resolve<AppDbContext>().PersonImage);
                    AssertPersonImage(img, sut);
                });

            result.AssertSuccess();            
        }

        private void AssertPersonImage(PersonImage img,
               UpsertProfileImageCommand sut)
        {
            Assert.NotNull(img);
            Assert.Equal(1, img.ImageId);
            Assert.Equal(sut.ImageType, img.ImageType);
            Assert.Equal(sut.ImageData, img.ImageData);            
            Assert.Equal(Defaults.PersonId, img.PersonId);
        }

        protected override UpsertProfileImageCommand CreateSut()
        {
            return new UpsertProfileImageCommand()
            {
                ImageData = new byte[] {0x1, 0x2, 0x3},
                ImageType = "image/png"
            };
        }
    }
}