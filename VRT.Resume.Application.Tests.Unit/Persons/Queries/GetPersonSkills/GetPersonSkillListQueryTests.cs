using System.Threading.Tasks;
using Xunit;

namespace VRT.Resume.Application.Persons.Queries.GetPersonSkills
{
    public sealed class GetPersonSkillListQueryTests
        : QueryTestBase<GetPersonSkillListQuery, PersonSkillInListVM[]>
    {
        [Fact]
        public async Task Send_QueryWhenSkillsNotExists_ShouldReturnEmptyArray()
        {
            var sut = CreateSut();

            var result = await sut.Send();

            result.AssertSuccess();
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task Send_QueryWhenSkillsExists_ShouldReturnSkillsList()
        {
            var sut = CreateSut();

            var result = await sut.Send(async scope=> await scope.SeedSkill());

            result.AssertSuccess();
            var skill = Assert.Single(result.Value);
            Assert.Equal(1, skill.SkillId);
            Assert.Equal("Skill", skill.Name);
            Assert.Equal("10", skill.Level);
            Assert.Equal(SkillTypes.Technical.ToString(), skill.Type);
        }
        protected override GetPersonSkillListQuery CreateSut()
        {
            return new GetPersonSkillListQuery();
        }
    }
}
