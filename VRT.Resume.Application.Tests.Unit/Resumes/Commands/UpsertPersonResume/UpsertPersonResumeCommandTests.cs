using Xunit;
using System.Threading.Tasks;
using FluentValidation;
using Autofac;
using VRT.Resume.Persistence.Data;
using VRT.Resume.Domain.Entities;
using System.Linq;

namespace VRT.Resume.Application.Resumes.Commands.UpsertPersonResume.Tests
{
    public class UpsertPersonResumeCommandTests : ApplicationTestBase
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
        public async Task Send_CommandWhenUserDoesNotExistsInDb_ShouldFailWithUnauthorizedMessage()
        {
            var sut = CreateSut();

            var result = await Send(sut, onBeforeSend: scope=> 
            {
                var db = scope.Resolve<AppDbContext>();
                db.UserPerson.RemoveRange(db.UserPerson);
                db.Person.RemoveRange(db.Person);                
                db.SaveChanges();                
            });

            Assert.True(result.IsFailure, nameof(result.IsFailure));
            Assert.Equal(Errors.UserUnauthorized, result.Error);
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
                async scope => await SeedResume(scope),
                onAfterSend: scope =>
            {
                var resume = Assert.Single(scope.Resolve<AppDbContext>().PersonResume);
                Assert.Equal(sut.Description, resume.Description);
                Assert.Equal(sut.Position, resume.Position);
                Assert.Equal(DefaultPersonId, resume.PersonId);
                Assert.Equal(sut.ResumeId, resume.ResumeId);
                Assert.Equal(sut.Summary, resume.Summary);
                Assert.Equal(sut.DataProcessingPermission, resume.Permission);
                Assert.Equal(sut.ShowProfilePhoto, resume.ShowProfilePhoto);
            });

            Assert.True(result.IsSuccess, result.GetErrorSafe());
        }

        private UpsertPersonResumeCommand CreateSut(int resumeId=1)
        {
            return new UpsertPersonResumeCommand()
            {
                ResumeId = resumeId,
                Description = "Description",
                Position = "Position",
                DataProcessingPermission = "DataProcessingPermission",
                ShowProfilePhoto = true,
                Summary = "Summary"
            };
        }

        private async Task SeedResume(ILifetimeScope scope)
        {
            var db = scope.Resolve<AppDbContext>();
            
            var toAdd = new PersonResume()
            {
                ResumeId = 1,   
                PersonId = DefaultPersonId,
                Description = "Default",
                Permission = "Default",
                ShowProfilePhoto = false,
                Summary = "Default",
                Position = "Default",
                ModifiedDate = new System.DateTime(2020, 2, 3)
            };
            db.PersonResume.Add(toAdd);            
            await db.SaveChangesAsync();
        }
    }
}