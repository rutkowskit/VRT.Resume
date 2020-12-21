using Autofac;
using FluentValidation;
using System.Threading.Tasks;
using VRT.Resume.Persistence.Data;
using Xunit;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonContact
{
    public class UpsertPersonContactCommandTests : ApplicationTestBase<UpsertPersonContactCommand>
    {
        [Fact()]
        public async Task Send_CommandWithoutName_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.Name = null;

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }
        [Fact()]
        public async Task Send_CommandWithoutValue_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.Value = "";

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithRequiredParametersWrongId_ShouldAddNewRecordToDbContext()
        {
            var sut = CreateSut();

            var result = await Send(sut, onAfterSend: scope =>
            {
                var resume = Assert.Single(scope.Resolve<AppDbContext>().PersonContact);
                Assert.Equal(sut.Name, resume.Name);
                Assert.Equal(sut.Value, resume.Value);
                Assert.Equal(Defaults.PersonId, resume.PersonId);
            });
            Assert.True(result.IsSuccess, result.GetErrorSafe());
        }

        [Fact()]
        public async Task Send_CommandWithProvidedIdOfExistingResume_ShouldUpdateExistingRecord()
        {
            var sut = CreateSut();

            var result = await Send(sut,
                async scope => await scope.SeedContact(),
                onAfterSend: scope =>
                {
                    var resume = Assert.Single(scope.Resolve<AppDbContext>().PersonContact);
                    Assert.Equal(sut.Url, resume.Url);
                    Assert.Equal(sut.Value, resume.Value);
                    Assert.Equal(Defaults.PersonId, resume.PersonId);
                    Assert.Equal(sut.Name, resume.Name);
                    Assert.Equal(sut.Icon, resume.Icon);
                    Assert.Equal(sut.ContactId, resume.ContactId);                    
                });

            Assert.True(result.IsSuccess, result.GetErrorSafe());
        }

        [Fact()]
        public async Task Send_CommandWithUnsuportedIcon_ShouldThrowValidationException()
        {
            var sut = CreateSut();
            sut.Icon = "<script>doSomethingNasty();</script>";
            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));            
        }

        [Fact()]
        public async Task Send_CommandWithCorrectImgIcon_ShouldSetImage()
        {
            const string expectedImg = "<img src=\"test\">";
            var sut = CreateSut();
            sut.Icon = expectedImg;

            var result = await Send(sut,                
                onAfterSend: scope =>
                {
                    var resume = Assert.Single(scope.Resolve<AppDbContext>().PersonContact);                    
                    Assert.Equal(expectedImg, resume.Icon);                    
                });            
        }

        [Fact]
        public async Task Send_CommandWithCorrectSvgIcon_ShouldSetImage()
        {
            const string expectedImg = "<svg>Test</svg>";
            var sut = CreateSut();
            sut.Icon = expectedImg;

            var result = await Send(sut,
                onAfterSend: scope =>
                {
                    var resume = Assert.Single(scope.Resolve<AppDbContext>().PersonContact);
                    Assert.Equal(expectedImg, resume.Icon);
                });
        }

        protected override UpsertPersonContactCommand CreateSut()
        {
            return new UpsertPersonContactCommand()
            {
                ContactId = 1,
                Name = "Name",
                Value = "Value",
                Url = "",
                Icon = null
            };
        }
    }
}