using Xunit;
using System.Threading.Tasks;
using FluentValidation;
using Autofac;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Resumes.Commands.UpsertPersonResume
{
    public class UpsertPersonResumeCommandTests : CommandTestBase<UpsertPersonResumeCommand>
    {      
        [Fact()]
        public async Task Send_CommandWithoutDescription_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.Description = null;
            
            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));            
        }
        [Fact()]
        public async Task Send_CommandWithoutPosition_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.Position = null;
            
            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithRequiredParametersWrongId_ShouldAddNewRecordToDbContext()
        {
            var sut = CreateSut();

            var result = await Send(sut,onAfterSend: scope=>
            {                
                var resume = Assert.Single(scope.Resolve<AppDbContext>().PersonResume);
                Assert.Equal(sut.Description, resume.Description);
                Assert.Equal(sut.Position, resume.Position);
                Assert.Equal(1, resume.PersonId);
            });         

            Assert.True(result.IsSuccess, result.GetErrorSafe());
        }

        [Fact()]
        public async Task Send_CommandWithProvidedIdOfExistingResume_ShouldUpdateExistingRecord()
        {
            var sut = CreateSut();
            
            var result = await Send(sut, 
                async scope => await scope.SeedPersonResume(),
                onAfterSend: scope =>
            {
                var resume = Assert.Single(scope.Resolve<AppDbContext>().PersonResume);
                Assert.Equal(sut.Description, resume.Description);
                Assert.Equal(sut.Position, resume.Position);
                Assert.Equal(Defaults.PersonId, resume.PersonId);
                Assert.Equal(sut.ResumeId, resume.ResumeId);
                Assert.Equal(sut.Summary, resume.Summary);
                Assert.Equal(sut.DataProcessingPermission, resume.Permission);
                Assert.Equal(sut.ShowProfilePhoto, resume.ShowProfilePhoto);
            });

            Assert.True(result.IsSuccess, result.GetErrorSafe());
        }

        protected override UpsertPersonResumeCommand CreateSut()
        {
            return new UpsertPersonResumeCommand()
            {
                ResumeId = 1,
                Description = "Description",
                Position = "Position",
                DataProcessingPermission = "DataProcessingPermission",
                ShowProfilePhoto = true,
                Summary = "Summary"
            };
        }        
    }
}