using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Common;

namespace VRT.Resume.Application.Persons.Queries.GetPersonSkills
{
    public sealed class GetPersonSkillListQueryTests
        : QueryTestBase<GetPersonSkillListQuery, PersonSkillInListVM[]>
    {
        public GetPersonSkillListQueryTests(ApplicationFixture fixture) : base(fixture)
        {

        }

        [Fact]
        public async Task Send_QueryWhenSkillsNotExists_ShouldReturnEmptyArray()
        {
            var sut = CreateSut();

            var result = await Send(sut);

            result.AssertSuccess();
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task Send_QueryWhenSkillsExists_ShouldReturnSkillsList()
        {
            var sut = CreateSut();
            var skillseed = await GetDbContext().SeedSkill();

            var result = await Send(sut);

            result.AssertSuccess();
            var skill = Assert.Single(result.Value);
            Assert.Equal(skillseed.SkillId, skill.SkillId);
            Assert.Equal(skillseed.Name, skill.Name);
            Assert.Equal(skillseed.Level, skill.Level);
            Assert.Equal(SkillTypes.Technical.ToString(), skill.Type);
        }
        protected override GetPersonSkillListQuery CreateSut()
        {
            return new GetPersonSkillListQuery();
        }
    }
}
