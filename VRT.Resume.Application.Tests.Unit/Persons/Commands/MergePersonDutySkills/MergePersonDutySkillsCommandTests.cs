using Autofac;
using FluentValidation;
using System.Linq;
using System.Threading.Tasks;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;
using Xunit;

namespace VRT.Resume.Application.Persons.Commands.MergePersonDutySkills
{
    public sealed class MergePersonDutySkillsCommandTests : CommandTestBase<MergePersonDutySkillsCommand>
    {
        [Fact()]
        public async Task Send_CommandWithInvalidEntityId_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.DutyId = 0;
            await Assert.ThrowsAsync<ValidationException>(() => sut.Send());
        }

        [Fact()]
        public async Task Send_CommandWithEntityIdThatIsNotInDb_ShouldFail()
        {
            var sut = CreateSut();
            sut.DutyId = 434343434;
            await sut.Send().AssertFail();            
        }

        [Fact()]
        public async Task Send_CommandWithNullDutySkillList_ShouldDoNoChanges()
        {
            var sut = CreateSut();
            sut.DutySkills = null;
            await sut.Send(
                onBeforeSend: async scope =>
                {
                    await PrepareSkillsInDb(scope);
                },
                onAfterSend: scope =>
                {
                    Assert.Single(scope.Resolve<AppDbContext>().Set<PersonExperienceDutySkill>());
                }).AssertSuccess();
        }

        [Fact()]
        public async Task Send_CommandWithRelevantDutySkills_AddedPersonExpDutySkill()
        {
            var sut = CreateSut();
            var expectedSkillId = 2;
            sut.DutySkills = new[]
            {
                new PersonExpDutySkillDto()
                {
                    IsRelevant = true,
                    SkillId = expectedSkillId //should be added 
                }
            };
            await sut.Send(
                onBeforeSend: async scope=>await PrepareSkillsInDb(scope, expectedSkillId),
                onAfterSend: scope =>
                {
                    var skillSet = scope.Resolve<AppDbContext>()
                        .Set<PersonExperienceDutySkill>().ToList();
                    Assert.Equal(2, skillSet.Count); //one default + one added
                    var added = skillSet
                        .FirstOrDefault(s => s.SkillId == expectedSkillId && s.DutyId == 1);
                    Assert.NotNull(added);
                }).AssertSuccess();            
        }

        [Fact()]
        public async Task Send_CommandWithIrrelevantDutySkills_UnchangedDutySkills()
        {
            var sut = CreateSut();
            var expectedSkillId = 2;
            sut.DutySkills = new[]
            {
                new PersonExpDutySkillDto()
                {
                    IsRelevant = false,
                    SkillId = expectedSkillId //should be added 
                }
            };
            await sut.Send(
                onBeforeSend: async scope => await PrepareSkillsInDb(scope, expectedSkillId),
                onAfterSend: scope =>
                {                  
                    Assert.Single(scope.Resolve<AppDbContext>().Set<PersonExperienceDutySkill>());
                }).AssertSuccess();
        }

        [Fact()]
        public async Task Send_CommandWithIrrelevantExistingDutySkills_RemovedDutySkill()
        {
            var sut = CreateSut();
            sut.DutySkills = new[]
            {
                new PersonExpDutySkillDto()
                {
                    IsRelevant = false,
                    SkillId = 1 //this one exists after default seeding
                }
            };
            await sut.Send(
                onBeforeSend: async scope => await PrepareSkillsInDb(scope),
                onAfterSend: scope =>
                {
                    var count = scope.Resolve<AppDbContext>()
                        .Set<PersonExperienceDutySkill>()                        
                        .Count();
                    Assert.Equal(0, count);
                }).AssertSuccess();
        }

        private async Task PrepareSkillsInDb(ILifetimeScope scope, params int[] expectedSkillIds)
        {
            var skillSet = scope.Resolve<AppDbContext>().Set<PersonSkill>();
            skillSet.AddRange(expectedSkillIds.Select(s=>SkillHelper.CreateSkill(s)));
            await scope.SeedExperience();
        }
        protected override MergePersonDutySkillsCommand CreateSut()
        {
            return new MergePersonDutySkillsCommand()
            {
                DutyId = 1
            };
        }
    }
}
