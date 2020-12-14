using Autofac;
using FluentValidation;
using System.Linq;
using System.Threading.Tasks;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;
using Xunit;

namespace VRT.Resume.Application.Resumes.Commands.MergeResumeSkills
{
    public sealed class MergeResumeSkillsCommandTests : ApplicationTestBase<MergeResumeSkillsCommand>
    {
        [Fact()]
        public async Task Send_CommandWithInvalidResumeId_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.ResumeId = 0;
            await Assert.ThrowsAsync<ValidationException>(() => sut.Send());
        }

        [Fact()]
        public async Task Send_CommandWithResumeIdThatIsNotInDb_ShouldFail()
        {
            var sut = CreateSut();
            sut.ResumeId = 434343434;
            await sut.Send().AssertFail();            
        }

        [Fact()]
        public async Task Send_CommandWithNullResumeSkillList_ShouldDoNoChanges()
        {
            var sut = CreateSut();
            sut.ResumeSkills = null;
            await sut.Send(
                onBeforeSend: async scope =>
                {
                    await PrepareSkillsInDb(scope);
                },
                onAfterSend: scope =>
                {
                    Assert.Single(scope.Resolve<AppDbContext>().Set<ResumePersonSkill>());
                }).AssertSuccess();
        }

        [Fact()]
        public async Task Send_CommandWithRelevantDutySkills_AddedResumePersonSkill()
        {
            var sut = CreateSut();
            var expectedSkillId = 2;
            sut.ResumeSkills = new[]
            {
                new ResumePersonSkillDto()
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
                        .Set<ResumePersonSkill>().ToList();
                    Assert.Equal(2, skillSet.Count); //one default + one added
                    var added = skillSet
                        .FirstOrDefault(s => s.SkillId == expectedSkillId && s.ResumeId == 1);
                    Assert.NotNull(added);
                }).AssertSuccess();            
        }

        [Fact()]
        public async Task Send_CommandWithIrrelevantDutySkills_UnchangedDutySkills()
        {
            var sut = CreateSut();
            var expectedSkillId = 2;
            sut.ResumeSkills = new[]
            {
                new ResumePersonSkillDto()
                {
                    IsRelevant = false,
                    SkillId = expectedSkillId //should be added 
                }
            };
            await sut.Send(
                onBeforeSend: async scope => await PrepareSkillsInDb(scope, expectedSkillId),
                onAfterSend: scope =>
                {                  
                    Assert.Single(scope.Resolve<AppDbContext>().Set<ResumePersonSkill>());
                }).AssertSuccess();
        }

        [Fact()]
        public async Task Send_CommandWithRelevantExistingResumePersonSkill_UpdatedResumePersonSkill()
        {
            var sut = CreateSut();
            sut.ResumeSkills = new[]
            {
                new ResumePersonSkillDto()
                {
                    IsRelevant = true,
                    IsHidden = true,
                    SkillId = 1 //this one exists after default seeding
                }
            };
            await sut.Send(
                onBeforeSend: async scope => await PrepareSkillsInDb(scope),
                onAfterSend: scope =>
                {
                    var rpSkill = scope.Resolve<AppDbContext>()
                        .Set<ResumePersonSkill>()                        
                        .FirstOrDefault(s=>s.SkillId==1);
                    Assert.NotNull(rpSkill);
                    Assert.True(rpSkill.IsRelevant);
                    Assert.True(rpSkill.IsHidden);
                }).AssertSuccess();
        }

        private async Task PrepareSkillsInDb(ILifetimeScope scope, params int[] expectedSkillIds)
        {
            var skillSet = scope.Resolve<AppDbContext>().Set<PersonSkill>();
            skillSet.AddRange(expectedSkillIds.Select(s=>SkillHelper.CreateSkill(s)));
            await scope.SeedPersonResume();
        }
        protected override MergeResumeSkillsCommand CreateSut()
        {
            return new MergeResumeSkillsCommand()
            {
                ResumeId = 1
            };
        }
    }
}
