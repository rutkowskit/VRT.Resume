using Autofac;
using FluentValidation;
using System.Linq;
using System.Threading.Tasks;
using VRT.Resume.Domain.Common;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;
using Xunit;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonSkill
{
    public class UpsertPersonSkillCommandTests : CommandTestBase<UpsertPersonSkillCommand>
    {
        [Fact()]
        public async Task Send_CommandWithTooShortSkillName_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.SkillName = "";

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithTooShortSkillLevel_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.SkillLevel = "";

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }
       
        [Fact()]
        public async Task Send_CommandWithWrongId_ShouldAddNewRecordToDbContext()
        {
            var sut = CreateSut();            
            var result = await Send(sut,
                onBeforeSend: scope =>
                {
                    var entity = scope.Resolve<AppDbContext>().PersonSkill.FirstOrDefault();
                    Assert.Null(entity);
                },
                onAfterSend: scope =>
            {
                var entity = Assert.Single(scope.Resolve<AppDbContext>().PersonSkill);
                AssertPersonSkill(entity, sut);                
            });
            
            result.AssertSuccess();            
        }

        [Fact()]
        public async Task Send_CommandWithIdOfExistingSkill_ShouldUpdateExistingRecord()
        {
            var sut = CreateSut();

            var result = await Send(sut,
                async scope => await scope.SeedSkill(),
                onAfterSend: scope =>
                {
                    var exp = Assert.Single(scope.Resolve<AppDbContext>().PersonSkill);
                    AssertPersonSkill(exp, sut);
                });

            result.AssertSuccess();            
        }

        private void AssertPersonSkill(PersonSkill skill,
               UpsertPersonSkillCommand sut)
        {
            Assert.NotNull(skill);
            Assert.Equal(1, skill.SkillId);
            Assert.Equal(sut.SkillLevel,skill.Level);
            Assert.Equal(sut.SkillName, skill.Name);
            Assert.Equal(sut.SkillType, (SkillTypes)skill.SkillTypeId);
            Assert.Equal(Defaults.PersonId, skill.PersonId);
        }

        protected override UpsertPersonSkillCommand CreateSut()
        {
            return new UpsertPersonSkillCommand()
            {
                SkillId = 1,
                SkillLevel = "0",
                SkillName = "screwdriver",
                SkillType = SkillTypes.Tool
            };
        }
    }
}