using Autofac;
using FluentValidation;
using System.Linq;
using System.Threading.Tasks;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;
using Xunit;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonExperienceDuty
{
    public class UpsertPersonExperienceDutyCommandTests : ApplicationTestBase<UpsertPersonExperienceDutyCommand>
    {
        [Fact()]
        public async Task Send_CommandWithoutExperienceId_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.ExperienceId = 0;

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithTooShortDutyName_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.Name = "";

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithExperienceThatDoesNotExists_ShouldFail()
        {
            var sut = CreateSut();
            sut.ExperienceId = 63533535;

            var result = await sut.Send();

            result.AssertFail();
            Assert.Equal(Errors.PersonExperienceNotExists, result.Error);
        }

        [Fact()]
        public async Task Send_CommandWithWrongDutyId_ShouldAddNewRecordToDbContext()
        {
            var sut = CreateSut();            
            var result = await Send(sut,
                onBeforeSend: async scope =>
                {
                    await scope.SeedExperience(seedDuty:false);
                    var duty = scope.Resolve<AppDbContext>()
                        .PersonExperienceDuty.FirstOrDefault();
                    Assert.Null(duty);
                },
                onAfterSend: scope =>
            {
                var duty = Assert.Single(scope.Resolve<AppDbContext>().PersonExperienceDuty);
                AssertPersonExperienceDuty(duty, sut);                
            });
            
            result.AssertSuccess();            
        }

        [Fact()]
        public async Task Send_CommandWithIdOfExistingExpDuty_ShouldUpdateExistingRecord()
        {
            var sut = CreateSut();
            sut.DutyId = 1;
            var result = await Send(sut,
                async scope => await scope.SeedExperience(),
                onAfterSend: scope =>
                {
                    var duty = Assert.Single(scope.Resolve<AppDbContext>().PersonExperienceDuty);
                    AssertPersonExperienceDuty(duty, sut);
                    Assert.Equal(sut.DutyId, duty.DutyId);
                });

            result.AssertSuccess();            
        }

        private void AssertPersonExperienceDuty(PersonExperienceDuty exp,
               UpsertPersonExperienceDutyCommand sut)
        {
            Assert.NotNull(exp);
            Assert.Equal(1, sut.ExperienceId);            
            Assert.Equal(sut.Name, exp.Name);
        }

        protected override UpsertPersonExperienceDutyCommand CreateSut()
        {
            return new UpsertPersonExperienceDutyCommand()
            {
                ExperienceId = 1,                
                Name = "Creating Web Services"                
            };
        }
    }
}