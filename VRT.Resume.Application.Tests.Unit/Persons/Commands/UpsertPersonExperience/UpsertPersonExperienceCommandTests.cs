using FluentAssertions;
using FluentValidation;
using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonExperience
{
    public class UpsertPersonExperienceCommandTests : CommandTestBase<UpsertPersonExperienceCommand>
    {
        public UpsertPersonExperienceCommandTests(ApplicationFixture fixture) : base(fixture)
        {

        }

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
        public async Task Send_WhenWrongExperienceId_ShouldAddNewRecordToDbContext()
        {
            var sut = CreateSut();
            sut.ExperienceId = int.MaxValue;

            GetDbContext().PersonExperience.FirstOrDefault().Should().BeNull();

            var result = await Send(sut);

            result.AssertSuccess();

            var edu = GetDbContext().PersonExperience.Single();
            edu.Should().NotBeNull();
            AssertPersonExperience(edu!, sut);
        }

        [Fact()]
        public async Task Send_CommandWithIdOfExistingEducation_ShouldUpdateExistingRecord()
        {
            var expSeed = await GetDbContext().SeedExperience();

            var sut = CreateSut();
            sut.ExperienceId = expSeed.ExperienceId;

            var result = await Send(sut);

            result.AssertSuccess();
            var exp = GetDbContext().PersonExperience.ToArray();
            exp.Should().HaveCount(1);
            AssertPersonExperience(exp[0], sut);
        }

        private static void AssertPersonExperience(
            PersonExperience exp,
               UpsertPersonExperienceCommand sut)
        {
            Assert.NotNull(exp);            
            Assert.Equal(sut.CompanyName, exp.CompanyName);
            Assert.Equal(sut.Location, exp.Location);
            Assert.Equal(sut.Position, exp.Position);
            Assert.Equal(sut.FromDate, exp.FromDate);
            Assert.Equal(sut.ToDate, exp.ToDate);            
        }

        protected override UpsertPersonExperienceCommand CreateSut()
        {
            return new UpsertPersonExperienceCommand()
            {                
                CompanyName = "Some nice company",
                FromDate = Defaults.Today.AddYears(-10),
                ToDate = Defaults.Today.AddMonths(-5),
                Location = "Universe",
                Position = "Soft Coder"
            };
        }
    }
}