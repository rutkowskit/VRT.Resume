using Xunit;
using System.Threading.Tasks;
using FluentValidation;
using Autofac;
using VRT.Resume.Persistence.Data;
using System.Linq;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Resumes.Commands.ClonePersonResume
{
    public class ClonePersonResumeCommandTests : CommandTestBase<ClonePersonResumeCommand>
    {      
        [Fact()]
        public async Task Send_CommandWithoutResumeId_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.ResumeId = 0;
            
            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));            
        }

        [Fact()]
        public async Task Send_CommandWithResumeIdThatDoesNotExists_ShouldFail()
        {
            var sut = CreateSut();
            sut.ResumeId = 43434343;

            var result = await sut.Send();

            result.AssertFail();
            Assert.Equal(Errors.RecordNotFound, result.Error);            
        }

        [Fact()]
        public async Task Send_CommandWithProvidedIdOfExistingResume_ShouldCloneResume()
        {
            var sut = CreateSut();

            var result = await Send(sut,
                async scope => await scope.SeedPersonResume(),                
                onAfterSend: scope =>
                {                    
                    var clonedResume = scope.Resolve<AppDbContext>().PersonResume
                        .FirstOrDefault(r => r.ResumeId != sut.ResumeId);
                    Assert.NotNull(clonedResume);
                    var baseResume = scope.Resolve<AppDbContext>().PersonResume
                        .FirstOrDefault(r => r.ResumeId == sut.ResumeId);

                    AssertClonedResume(baseResume, clonedResume);

                });

            Assert.True(result.IsSuccess, result.GetErrorSafe());
        }

        private void AssertClonedResume(PersonResume template, PersonResume clone)
        {
            var t = template;
            var c = clone;
            Assert.Equal(t.Description, c.Description);
            Assert.Equal(t.Permission, c.Permission);
            Assert.Equal(t.PersonId, c.PersonId);
            Assert.Equal(t.Position, c.Position);
            Assert.Equal(t.ShowProfilePhoto, c.ShowProfilePhoto);
            Assert.Equal(t.Summary, c.Summary);
            Assert.Equal(t.ResumePersonSkill.Count, c.ResumePersonSkill.Count);

            foreach(var s in t.ResumePersonSkill)
            {
                var cSkill = c.ResumePersonSkill
                       .FirstOrDefault(i => i.SkillId == s.SkillId);
                Assert.NotNull(cSkill);
                Assert.Equal(s.IsHidden, cSkill.IsHidden);
                Assert.Equal(s.IsRelevant, cSkill.IsRelevant);
                Assert.Equal(s.Position, cSkill.Position);               
            }
        }

        protected override ClonePersonResumeCommand CreateSut()
        {
            return new ClonePersonResumeCommand()
            {
                ResumeId = 1                
            };
        }
    }
}