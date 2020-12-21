using Autofac;
using FluentValidation;
using System.Linq;
using System.Threading.Tasks;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;
using Xunit;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonExperience
{
    public class UpsertPersonExperienceCommandTests : CommandTestBase<UpsertPersonExperienceCommand>
    {
        [Fact()]
        public async Task Send_CommandWithTooShortCompanyName_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.CompanyName = "";

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithTooShortPosition_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.Position = "";

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithTooShortLocation_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.Location = "";

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithEmptyFromDate_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.FromDate = default;

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithFromDateGreaterThanToDate_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.ToDate = sut.FromDate.AddDays(-1);

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }
        
        [Fact()]
        public async Task Send_CommandWithWrongId_ShouldAddNewRecordToDbContext()
        {
            var sut = CreateSut();            
            var result = await Send(sut,
                onBeforeSend: scope =>
                {
                    var edu = scope.Resolve<AppDbContext>().PersonExperience.FirstOrDefault();
                    Assert.Null(edu);
                },
                onAfterSend: scope =>
            {
                var edu = Assert.Single(scope.Resolve<AppDbContext>().PersonExperience);
                AssertPersonExperience(edu, sut);                
            });
            
            result.AssertSuccess();            
        }

        [Fact()]
        public async Task Send_CommandWithIdOfExistingEducation_ShouldUpdateExistingRecord()
        {
            var sut = CreateSut();

            var result = await Send(sut,
                async scope => await scope.SeedExperience(),
                onAfterSend: scope =>
                {
                    var exp = Assert.Single(scope.Resolve<AppDbContext>().PersonExperience);
                    AssertPersonExperience(exp, sut);
                });

            result.AssertSuccess();            
        }

        private void AssertPersonExperience(PersonExperience exp,
               UpsertPersonExperienceCommand sut)
        {
            Assert.NotNull(exp);
            Assert.Equal(1, exp.ExperienceId);
            Assert.Equal(sut.CompanyName, exp.CompanyName);
            Assert.Equal(sut.Location,exp.Location);
            Assert.Equal(sut.Position, exp.Position);
            Assert.Equal(sut.FromDate, exp.FromDate);
            Assert.Equal(sut.ToDate, exp.ToDate);
            Assert.Equal(Defaults.PersonId, exp.PersonId);
        }

        protected override UpsertPersonExperienceCommand CreateSut()
        {
            return new UpsertPersonExperienceCommand()
            {
                ExperienceId = 1,
                CompanyName = "Some nice company",
                FromDate = Defaults.Today.AddYears(-10),
                ToDate = Defaults.Today.AddMonths(-5),
                Location = "Universe",
                Position = "Soft Coder"
            };
        }
    }
}